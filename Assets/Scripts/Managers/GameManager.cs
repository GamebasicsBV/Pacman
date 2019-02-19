﻿using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;
    public static int lives = 3;

	public enum GameState { Init, Game, Dead, Scores }
	public static GameState gameState;

    private GameObject pacman;
    private GameObject blinky;
    private GameObject pinky;
    private GameObject inky;
    private GameObject clyde;
    private GameGUINavigation gui;
    public AnimatorController PacmanAnimatorController;
    public AnimatorController PinkyAnimatorController;
    public AnimatorController InkyAnimatorController;
    public AnimatorController BlinkyAnimatorController;
    public AnimatorController ClydeAnimatorController;

    public GameObject Powerup;
    public Transform[] PowerupSpawnPoints;

    public static bool scared;
    public static bool isAnimatorFlipped = false;
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
    }

    void OnLevelWasLoaded()
    {
        if (Level == 0) lives = 3;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts();
        ResetVariables();


        // Adjust Ghost variables!
        clyde.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        blinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        pinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        inky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;
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
    }

	public void ResetScene() {
		if (lives == 0) {
			SceneManager.LoadScene(NextLevel);
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

        gameState = GameState.Init;  
        gui.H_ShowReadyScreen();

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
        AnimatorController[] animators = { BlinkyAnimatorController, PinkyAnimatorController, InkyAnimatorController, ClydeAnimatorController };

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
        Level = 0;
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }

    public void SpawnPowerup()
    {
        var spawnPoint = PowerupSpawnPoints[new System.Random().Next(0, PowerupSpawnPoints.Length)];

        Instantiate(Powerup, spawnPoint.position, spawnPoint.rotation);
    }

    public void InverseControls()
    {
        var playerController = pacman.GetComponent<PlayerController>();
        playerController._isInversed = !playerController._isInversed;
    }
}
