using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts;

public class GameManager : MonoBehaviour
{

    //--------------------------------------------------------
    // Game variables

    public int Level = 0;
    public int lives = 3;

    public enum GameState
    {
        Init,
        Game,
        Dead,
        Scores
    }

    public static GameState gameState;

    private GameObject pacman;
    private GameObject blinky;
    private GameObject pinky;
    private GameObject inky;
    private GameObject clyde;
    private GameGUINavigation gui;
    public RuntimeAnimatorController PacmanAnimatorController;
    public RuntimeAnimatorController PinkyAnimatorController;
    public RuntimeAnimatorController InkyAnimatorController;
    public RuntimeAnimatorController BlinkyAnimatorController;
    public RuntimeAnimatorController ClydeAnimatorController;

    public GameObject Powerup;
    public SpawnPoint[] PowerupSpawnPoints;
    private float PowerupSpawnTime = 3f;
    private float PowerupTimer;
    private float PowerupMax = 3;

    public GameObject MoveableWall;
    private Vector3 MoveableWallBeginPosition;

    private Vector3 MoveableWallEndPosition => new Vector3(MoveableWallBeginPosition.x - 8f, MoveableWallBeginPosition.y, 0f);
    private bool shouldMoveWallTowardBegin;
    private bool shouldMoveWallTowardEnd;

    public static bool scared;
    public bool isAnimatorFlipped = false;
    public bool isCameraFollowingPacman = false;
	public bool isCameraBackgroundGoingFlashy = false;
    public Camera camera;
    static public int score;

	public float scareLength;
	private float _timeToCalm;
    private float _timeToFlipBack;
	private float _timeToCameraBackgroundFlash;

	public float SpeedPerLevel;

	public string NextLevel;
    
    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
				_instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    //-------------------------------------------------------------------
    // function definitions

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            if(this != _instance)   
                Destroy(this.gameObject);
        }

        AssignGhosts();
    }

	void Start () 
	{
		gameState = GameState.Init;

	    PowerupTimer = Time.time + PowerupSpawnTime;
        if (isAnimatorFlipped)
            FlipAnimator();

	    MoveableWallBeginPosition = MoveableWall.transform.position;
	}

    void OnLevelWasLoaded()
    {
        if (Level == 0) lives = 3;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts();
        ResetVariables();

        // Adjust Ghost variables!
        //clyde.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //blinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //pinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //inky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

    // Update is called once per frame
	void Update () 
	{
		if(scared && _timeToCalm <= Time.time)
			CalmGhosts();

        if (isAnimatorFlipped && _timeToFlipBack <= Time.time)
            FlipAnimatorBack();

        if (isCameraFollowingPacman)
            camera.transform.position = new Vector3(pacman.transform.position.x, pacman.transform.position.y, camera.transform.position.z);

		if (isCameraBackgroundGoingFlashy && _timeToCameraBackgroundFlash <= Time.time) {
			Color[] colors = { Color.cyan, Color.green, Color.blue, Color.yellow, Color.red, Color.magenta };
			camera.backgroundColor = colors[new System.Random().Next(colors.Length)];
			_timeToCameraBackgroundFlash = Time.time + 1;
		}

        if (Level >= 2 && PowerupTimer <= Time.time && PowerupSpawnPoints.Count(x => x.SpawnedObject != null) < PowerupMax)
	    {
	        PowerupTimer = Time.time + PowerupSpawnTime;
            SpawnPowerup();
	    }

	    if (Level == 2) {
	        if (shouldMoveWallTowardEnd)
	        {
	            MoveableWall.transform.position = Vector3.MoveTowards(MoveableWall.transform.position, MoveableWallEndPosition, 0.3f);
	            if (Vector3.Distance(MoveableWall.transform.position, MoveableWallEndPosition) < 0.001f)
	                shouldMoveWallTowardEnd = false;
	        }

	        if (shouldMoveWallTowardBegin)
	        {
	            MoveableWall.transform.position = Vector3.MoveTowards(MoveableWall.transform.position, MoveableWallBeginPosition, 0.3f);
	            if (Vector3.Distance(MoveableWall.transform.position, MoveableWallBeginPosition) < 0.001f)
	                shouldMoveWallTowardBegin = false;
	        }
	    }
    }

	public void ResetScene() {
		if (lives == 0) {
			LoadNextLevel();
			return;
		}
		CalmGhosts();

		pacman.transform.position = new Vector3(15f, 11f, 0f);
		blinky.transform.position = new Vector3(15f, 20f, 0f);
		pinky.transform.position = new Vector3(14.5f, 17f, 0f);
		inky.transform.position = new Vector3(16.5f, 17f, 0f);
		clyde.transform.position = new Vector3(12.5f, 17f, 0f);

		pacman.GetComponent<PlayerController>().ResetDestination();
		blinky.GetComponent<GhostMove>().InitializeGhost();
		pinky.GetComponent<GhostMove>().InitializeGhost();
		inky.GetComponent<GhostMove>().InitializeGhost();
		clyde.GetComponent<GhostMove>().InitializeGhost();

        // Ruim gespawnde powerups op.
	    foreach (var spawnedPowerup in PowerupSpawnPoints.Where(x => x != null && x.SpawnedObject != null).Select(x => x.SpawnedObject).ToArray())
	        Destroy(spawnedPowerup.gameObject);

        InverseControls(false);
	    FlipAnimatorBack();

        gameState = GameState.Init;  
        gui.H_ShowReadyScreen();
	}

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevel);
    }

	public void ToggleScare()
	{
		if(!scared)	ScareGhosts();
		else 		CalmGhosts();
	}

	public void SetCameraBackgroundFlashy() {

	}

    public void FlipAnimator()
    {
        isAnimatorFlipped = true;
        RuntimeAnimatorController[] animators = { BlinkyAnimatorController, PinkyAnimatorController, InkyAnimatorController, ClydeAnimatorController };

        pacman.GetComponent<Animator>().runtimeAnimatorController = animators[new System.Random().Next(0, 4)];
        blinky.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        pinky.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        inky.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        clyde.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        _timeToFlipBack = Time.time + scareLength;

        Debug.Log("Flip Animators");
    }

    public void FlipAnimatorBack()
    {
        isAnimatorFlipped = false;
        pacman.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        blinky.GetComponent<Animator>().runtimeAnimatorController = BlinkyAnimatorController;
        pinky.GetComponent<Animator>().runtimeAnimatorController = PinkyAnimatorController;
        inky.GetComponent<Animator>().runtimeAnimatorController = InkyAnimatorController;
        clyde.GetComponent<Animator>().runtimeAnimatorController = ClydeAnimatorController;

        Debug.Log("Flip Animators back");
    }

    public void ScareGhosts()
	{
		scared = true;
		blinky.GetComponent<GhostMove>().Frighten();
		pinky.GetComponent<GhostMove>().Frighten();
		inky.GetComponent<GhostMove>().Frighten();
		clyde.GetComponent<GhostMove>().Frighten();
		_timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
	}

	public void CalmGhosts()
	{
		scared = false;
		blinky.GetComponent<GhostMove>().Calm();
		pinky.GetComponent<GhostMove>().Calm();
		inky.GetComponent<GhostMove>().Calm();
		clyde.GetComponent<GhostMove>().Calm();
	    PlayerController.killstreak = 0;
    }

    void AssignGhosts()
    {
        // find and assign ghosts
        clyde = GameObject.Find("clyde");
        pinky = GameObject.Find("pinky");
        inky = GameObject.Find("inky");
        blinky = GameObject.Find("blinky");
        pacman = GameObject.Find("pacman");

        if (clyde == null || pinky == null || inky == null || blinky == null) Debug.Log("One of ghosts are NULL");
        if (pacman == null) Debug.Log("Pacman is NULL");

        gui = GameObject.FindObjectOfType<GameGUINavigation>();

        if(gui == null) Debug.Log("GUI Handle Null!");

    }

    public void LoseLife()
    {
        lives--;
        gameState = GameState.Dead;
    
        // update UI too
        UIScript ui = GameObject.FindObjectOfType<UIScript>();
        Destroy(ui.lives[ui.lives.Count - 1]);
        ui.lives.RemoveAt(ui.lives.Count - 1);
    }

    public static void DestroySelf()
    {
        score = 0;
        //Level = 0;
        //lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }

    public void SpawnPowerup()
    {
        var emptySpawnPoints = PowerupSpawnPoints.Where(x => x.SpawnedObject == null).ToArray();
        var spawnPoint = emptySpawnPoints[new System.Random().Next(0, emptySpawnPoints.Length)];

        var powerup = Instantiate(Powerup, spawnPoint.transform.position, spawnPoint.transform.rotation);
        spawnPoint.SpawnedObject = powerup;
    }

    public void InverseControls(bool? inversionEnabled = null)
    {
        var playerController = pacman.GetComponent<PlayerController>();
        if (inversionEnabled.HasValue)
            playerController._isInversed = inversionEnabled.Value;
        else
            playerController._isInversed = !playerController._isInversed;
    }

    public void ToggleMoveableWall()
    {
        if (MoveableWall.transform.position == MoveableWallBeginPosition)
            shouldMoveWallTowardEnd = true;
        else
            shouldMoveWallTowardBegin = true;
    }
}
