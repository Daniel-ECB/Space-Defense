using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // For getting access to UI functions
using UnityEngine.Analytics; // And for the analytics

public class GameController : MonoBehaviour 
{
	#region Fields

	// Some general game variables
	public static GameController instance = null; // By making instance a static variable we can access it from the class
	public GameObject playerCharacter; // Reference to the player object
	int playerHealth; 
	public float resetDelay = 0.5f;
	public GameObject pickups;

	private bool gameStarted = false; // Indicates if the game has already started

	float version = 0.1f; // First version uploaded to Kongregate
	
	// Robot Spawning variables
	public GameObject prefabRobot;
	
	GameObject newRobot;
	public GameObject[] robotsInGame;
	int robotCountdown = GameConstants.TOTAL_ROBOTS;
	
	int randomSpawnPoint;
	public GameObject[] spawnPoints;
	
	// GUI support
	int score = 0;
	public Text scoreText;

	int robotCountdownTotal = GameConstants.TOTAL_ROBOTS;
	public Text robotCountdownText;
	public Slider robotCountdownSlider;
	
	public GameObject gameOver;
	public GameObject youWon;

	public GameObject splashScreen;
	public GameObject gameOverMessage;
	public GameObject youWonMessage;
    //GUIText scoreText = null;

    // Music Support
    AudioSource backgroundMusic;

	#endregion
	
	#region Private Methods
	
	// Use this for initialization, awake is called before start
	void Awake()
	{
		// If we don't have a game manager, create one, if we already have one, destroy it
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}
	
	// Use this for initialization
	void Start()
	{
		Time.timeScale = 0f;

		playerCharacter = GameObject.Find("Player");

        // Spawning initial max amount of robots on screen
        //		for (int i = robotsInGame.Length; i < GameConstants.MAX_ROBOTS; i = robotsInGame.Length)
        //		{
        //			SpawnRobot();
        //			robotCountdown--;
        //			robotsInGame = GameObject.FindGameObjectsWithTag("Enemy");
        //		}

        backgroundMusic = GetComponent<AudioSource>();
	}
	
	// We check if the game has ended, either if the player has lost (we're due on this) or won
	void CheckGameOver()
	{
		// We win if all robots are eliminated
		robotsInGame = GameObject.FindGameObjectsWithTag("Enemy");
//		Debug.Log("robotsInGame.Length: " + robotsInGame.Length);
//		Debug.Log("robotCountdown: " + robotCountdown);
		
		if (robotCountdownTotal < 1 /*&& robotsInGame.Length < 1*/)
		{
			RecordGameOver ();
			//youWon.SetActive(true); Old, ugly default font
			youWonMessage.SetActive(true);
			Time.timeScale = .25f;  // This makes us enter into slow motion, just for the heck of it
			Invoke("Reset", resetDelay); // Invoke lets us call a function with some delay
		}

		Player playerScript = playerCharacter.GetComponent<Player>();
		//Debug.Log ("Player Health: " + playerScript.Health);
		if (playerScript.Health <= 0)
		{
			RecordGameOver ();
			//Debug.Log ("Game Over");
			//gameOver.SetActive(true); Old, ugly default font
			gameOverMessage.SetActive(true);
			Time.timeScale = .25f;  // This makes us enter into slow motion, just for the heck of it
			Invoke("Reset", resetDelay);
		}

		//playerHealth = playerCharacter.AccessHealth();
		//if (playerCharacter < 1)
		//{
		
		//}
	}
	
	// Update is called once per frame
	void Update()
	{
		// Check for input to start game
		if(gameStarted == false)
		{
			if(Input.GetKeyDown("return"))
			{
				StartGame();
			}
		}

		// Always keeping the same number of robots on screen until the player kills them all
		if(robotCountdown > 0 && gameStarted == true)
		{
			robotsInGame = GameObject.FindGameObjectsWithTag("Enemy");
			for (int i = robotsInGame.Length; i < GameConstants.MAX_ROBOTS && robotCountdown > 0;
			     i = robotsInGame.Length)
			{
				SpawnRobot();
				robotCountdown--;
				robotsInGame = GameObject.FindGameObjectsWithTag("Enemy");
			}
		}
	}
	
	/// <summary>
	/// Spawns a robot at a prefixed location
	/// </summary>
	void SpawnRobot()
	{
		// create new robot
		Quaternion spawnRotation = Quaternion.identity;
		randomSpawnPoint = Random.Range(0, spawnPoints.Length);
		Instantiate(prefabRobot, spawnPoints[randomSpawnPoint].transform.position, spawnRotation);        
		
		//newRobot = Instantiate(prefabRobot) as GameObject;
		//randomSpawnPoint = Random.Range(0, spawnPoints.Length);
		//newRobot.transform.position = spawnPoints[randomSpawnPoint].transform.position; //Camera.main.ScreenToWorldPoint(bearSpawnPosition);        
	}    
	
	/// <summary>
	/// Resets the current level
	/// </summary>
	private void Reset()
	{
		Time.timeScale = 1f;
		Application.LoadLevel(Application.loadedLevel);
	}

	/// <summary>
	/// Starts the game.
	/// </summary>
	private void StartGame ()
	{
		RecordStart ();

		splashScreen.SetActive(false);
		gameStarted = true;
		pickups.SetActive (true);

        backgroundMusic.Play();

		// Spawning initial max amount of robots on screen
		for (int i = robotsInGame.Length; i < GameConstants.MAX_ROBOTS; i = robotsInGame.Length)
		{
			SpawnRobot();
			robotCountdown--;
			robotsInGame = GameObject.FindGameObjectsWithTag("Enemy");
		}
		
		Time.timeScale = 1f;
	}

	#endregion
	
	#region Public Methods

	/// <summary>
	/// Records analytics when the game starts playing.
	/// </summary>
	public void RecordStart()
	{
		Debug.Log ("The game has started");

		Analytics.CustomEvent ("gameStart", new Dictionary<string, object> {
				
		});
	}

	/// <summary>
	/// Records game analytics information when the game is over.
	/// </summary>
	public void RecordGameOver()
	{
		Debug.Log ("The game has ended");

		float gameDurationSecs = Time.time;
		int playerHealth = playerCharacter.GetComponent<Player> ().Health;
		//robotCountdownTotal

		Analytics.CustomEvent ("gameOver", new Dictionary<string, object>
			{
				{"time", gameDurationSecs},
				{"playerHealth", playerHealth},
				{"totalRobots", robotCountdownTotal},
				{"version", version},
			});
	}

	/// <summary>
	/// Updates the value of score
	/// </summary>
	/// <param name="value">Value to be added to the score</param>
	public void AddPoints(int value)
	{
		score += value;
		scoreText.text = "Score: " + score;
		//scoreText = GameObject.Find("score").GetComponent<GUIText>();
		//scoreText.text = GameConstants.SCORE_PREFIX + score;
	}

	/// <summary>
	/// Updates the total remaining robots GUIText
	/// </summary>
	public void UpdateRemainingRobots()
	{
		robotCountdownTotal--;
		robotCountdownText.text = "" + robotCountdownTotal;
		robotCountdownSlider.value = robotCountdownTotal;
		//Debug.Log ("One dies, remaining: " + robotCountdownTotal);
	}

	/// <summary>
	/// Determines whether this game has started.
	/// </summary>
	/// <returns><c>true</c> if this instance has game started; otherwise, <c>false</c>.</returns>
	public bool HasGameStarted()
	{
		return gameStarted;
	}
	
	#endregion
}
