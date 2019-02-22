using System;
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
    public GameObject[] ghosts = {};
    private GameGUINavigation gui;
    public RuntimeAnimatorController PacmanAnimatorController;
    public RuntimeAnimatorController PinkyAnimatorController;
    public RuntimeAnimatorController InkyAnimatorController;
    public RuntimeAnimatorController BlinkyAnimatorController;
    public RuntimeAnimatorController ClydeAnimatorController;

    public AudioSource BackgroundSound;
    public AudioSource ChompSound;
    public AudioSource DeathSound;
    public AudioSource EatFruitSound;
    public AudioSource EatGhostSound;
    public AudioSource EnergizeSound;

    public GameObject Powerup;
    public SpawnPoint[] PowerupSpawnPoints;
    private float PowerupSpawnTime = 3f;
    private float PowerupTimer;
    private float PowerupMax = 3;

    public GameObject MoveableWall;
    private Vector3 MoveableWallBeginPosition;

    private Vector3 MoveableWallEndPosition => new Vector3(MoveableWallBeginPosition.x - 9f, MoveableWallBeginPosition.y, 0f);
    private bool shouldMoveWallTowardBegin;
    private bool shouldMoveWallTowardEnd;

    public int NumberOfGhostKilledInLevel5 = 0;
    public bool PacmanHasMoved = false;
    public bool pacmanHasSpeedBoost = false;

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

        if (MoveableWall != null)
	        MoveableWallBeginPosition = MoveableWall.transform.position;

        PlaySound(Sound.Background);
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
    void Update()
    {
        if (scared && _timeToCalm <= Time.time)
            CalmGhosts();

        if (isAnimatorFlipped && _timeToFlipBack <= Time.time)
            FlipAnimatorBack();

        if (isCameraFollowingPacman)
            camera.transform.position = new Vector3(pacman.transform.position.x, pacman.transform.position.y,
                camera.transform.position.z);

        if (isCameraBackgroundGoingFlashy && _timeToCameraBackgroundFlash <= Time.time)
        {
            Color[] colors = {Color.cyan, Color.green, Color.blue, Color.yellow, Color.red, Color.magenta};
            camera.backgroundColor = colors[new System.Random().Next(colors.Length)];
            _timeToCameraBackgroundFlash = Time.time + 1;
        }

        // Powerups vanaf level 2.
        if ((Level == 2 || Level == 3) && PowerupTimer <= Time.time &&
            PowerupSpawnPoints.Count(x => x.SpawnedObject != null) < PowerupMax)
        {
            PowerupTimer = Time.time + PowerupSpawnTime;
            SpawnPowerup();
        }

        if (Level == 2 && MoveableWall != null)
        {
            if (shouldMoveWallTowardEnd)
            {
                MoveableWall.transform.position =
                    Vector3.MoveTowards(MoveableWall.transform.position, MoveableWallEndPosition, 0.3f);
                if (Vector3.Distance(MoveableWall.transform.position, MoveableWallEndPosition) < 0.001f)
                    shouldMoveWallTowardEnd = false;
            }

            if (shouldMoveWallTowardBegin)
            {
                MoveableWall.transform.position =
                    Vector3.MoveTowards(MoveableWall.transform.position, MoveableWallBeginPosition, 0.3f);
                if (Vector3.Distance(MoveableWall.transform.position, MoveableWallBeginPosition) < 0.001f)
                    shouldMoveWallTowardBegin = false;
            }
        }

        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.P))
            LoadNextLevel();
    }

    public void ResetScene()
	{
	    PacmanHasMoved = false;

		if (lives == 0 && Level == 1) {
			LoadNextLevel();
			return;
		}

	    if (lives == 0 && Level == 5)
	    {
	        NextLevel = "level_5_text";
            LoadNextLevel();
	        return;
	    }
		CalmGhosts();

	    if (pacman != null)
	    {
	        if (Level == 4)
	            pacman.transform.position = new Vector3(13f, 30f, 0f);
	        else
	            pacman.transform.position = new Vector3(15f, 11f, 0f);
	    }

	    if (blinky != null)
            blinky.transform.position = new Vector3(15f, 20f, 0f);
        if (pinky != null)
            pinky.transform.position = new Vector3(14.5f, 17f, 0f);
        if (inky != null)
	        inky.transform.position = new Vector3(16.5f, 17f, 0f);
		if (clyde != null)
		    clyde.transform.position = new Vector3(12.5f, 17f, 0f);

        if (pacman != null)
		    pacman.GetComponent<PlayerController>().ResetDestination();
        if (blinky != null)
		    blinky.GetComponent<GhostMove>().InitializeGhost();
        if (pinky != null)
		    pinky.GetComponent<GhostMove>().InitializeGhost();
        if (inky != null)
		    inky.GetComponent<GhostMove>().InitializeGhost();
        if (clyde != null)
		    clyde.GetComponent<GhostMove>().InitializeGhost();

	    if (ghosts != null)
	        foreach (var ghost in ghosts)
	        {
	            if (ghost != null)
	            {
	                ghost.GetComponent<GhostMove>().MoveToStartPosition();
	                ghost.GetComponent<GhostMove>().InitializeGhost();
	            }
	        }

	    // Ruim gespawnde powerups op.
	    foreach (var spawnedPowerup in PowerupSpawnPoints.Where(x => x != null && x.SpawnedObject != null).Select(x => x.SpawnedObject).ToArray())
	        Destroy(spawnedPowerup.gameObject);

        // Zet de muur terug op z'n plek.
        if (MoveableWall != null)
	        MoveableWall.transform.position = MoveableWallBeginPosition;

        InverseControls(false);
	    ToggleSpeed(false);
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

        if (pacman != null)
            pacman.GetComponent<Animator>().runtimeAnimatorController = animators[new System.Random().Next(0, 4)];
        if (blinky != null)
            blinky.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        if (pinky != null)
            pinky.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        if (inky != null)
            inky.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        if (clyde != null)
            clyde.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        _timeToFlipBack = Time.time + scareLength;

        Debug.Log("Flip Animators");
    }

    public void FlipAnimatorBack()
    {
        isAnimatorFlipped = false;
        if (pacman != null)
            pacman.GetComponent<Animator>().runtimeAnimatorController = PacmanAnimatorController;
        if (blinky != null)
            blinky.GetComponent<Animator>().runtimeAnimatorController = BlinkyAnimatorController;
        if (pinky != null)
            pinky.GetComponent<Animator>().runtimeAnimatorController = PinkyAnimatorController;
        if (inky != null)
            inky.GetComponent<Animator>().runtimeAnimatorController = InkyAnimatorController;
        if (clyde != null)
            clyde.GetComponent<Animator>().runtimeAnimatorController = ClydeAnimatorController;

        Debug.Log("Flip Animators back");
    }

    public void ScareGhosts()
	{
		scared = true;
        if (blinky != null)
		    blinky.GetComponent<GhostMove>().Frighten();
        if (pinky != null)
	    pinky.GetComponent<GhostMove>().Frighten();
		if (inky != null)
		    inky.GetComponent<GhostMove>().Frighten();
        if (clyde != null)
	        clyde.GetComponent<GhostMove>().Frighten();
		_timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
	}

	public void CalmGhosts()
	{
		scared = false;
        if (blinky != null)
		    blinky.GetComponent<GhostMove>().Calm();
        if (pinky != null)
		    pinky.GetComponent<GhostMove>().Calm();
        if (inky != null)
	        inky.GetComponent<GhostMove>().Calm();
        if (clyde != null)
	        clyde.GetComponent<GhostMove>().Calm();
	    PlayerController.killstreak = 0;
    }

    void AssignGhosts()
    {
        // find and assign ghosts
        clyde = GameObject.Find("clyde") ?? GameObject.Find("clyde_black");
        pinky = GameObject.Find("pinky") ?? GameObject.Find("pinky_black");
        inky = GameObject.Find("inky") ?? GameObject.Find("inky_black");
        blinky = GameObject.Find("blinky") ?? GameObject.Find("blinky_black");
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
        if (ui != null && ui.lives != null && ui.lives.ElementAtOrDefault(ui.lives.Count - 1) != null)
        {
            Destroy(ui.lives[ui.lives.Count - 1]);
            ui.lives.RemoveAt(ui.lives.Count - 1);
        }
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
		DizzyEffect.StartDoingTheDizzy();
        var playerController = pacman.GetComponent<PlayerController>();
        if (inversionEnabled.HasValue)
            playerController._isInversed = inversionEnabled.Value;
        else
            playerController._isInversed = !playerController._isInversed;
		if (playerController._isInversed) {
			MoveInvertEffect.StartDoingTheEffect();
		}
		else {
			MoveInvertEffect.StopDoingTheEffect();
		}
    }

    public bool IsInversed()
    {
        return pacman.GetComponent<PlayerController>()._isInversed;
    }

    public void ToggleSpeed(bool? enableSpeedBoost = null)
    {
        if (Level != 2)
            return;

        const int speedBoost = 1;
        DizzyEffect.StartDoingTheDizzy();
        var playerController = pacman.GetComponent<PlayerController>();
        if ((enableSpeedBoost.HasValue && enableSpeedBoost.Value == false) || !enableSpeedBoost.HasValue && pacmanHasSpeedBoost)
        {
            if (pacmanHasSpeedBoost)
                playerController.speed = playerController.speed -= speedBoost;
            pacmanHasSpeedBoost = false;
            MoveInvertEffect.StopDoingTheEffect();
        }
        else
        {
            if (!pacmanHasSpeedBoost)
                playerController.speed = playerController.speed += speedBoost;
            pacmanHasSpeedBoost = true;
            MoveInvertEffect.StartDoingTheEffect();
        }
    }

    public void ToggleMoveableWall()
    {
        if (MoveableWall.transform.position == MoveableWallBeginPosition)
            shouldMoveWallTowardEnd = true;
        else
            shouldMoveWallTowardBegin = true;
    }

    public void PlaySound(Sound sound)
    {
        var source = GetAudioSource(sound);
        if (source != null && !source.isPlaying)
            source.Play();
    }

    private AudioSource GetAudioSource(Sound sound)
    {
        AudioSource source;
        switch (sound)
        {
            case Sound.Background:
                source = BackgroundSound;
                break;
            case Sound.Chomp:
                source = ChompSound;
                break;
            case Sound.Death:
                source = DeathSound;
                break;
            case Sound.EatFruit:
                source = EatFruitSound;
                break;
            case Sound.EatGhost:
                source = EatGhostSound;
                break;
            case Sound.Energize:
                source = EnergizeSound;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
        }

        return source;
    }
}

public enum Sound {
    Background,
    Chomp,
    Death,
    EatFruit,
    EatGhost,
    Energize
}
