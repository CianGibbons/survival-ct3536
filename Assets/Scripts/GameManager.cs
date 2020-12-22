using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Creating our Game State enums
    public enum GAMESTATES
    {
        MENU,
        PLAYING
    }

    //inspector settings
    [SerializeField] private GameObject TitlePageCanvas, PlayingCanvas, LeaderboardCanvas, GameOverCanvas;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyGameObject;
    [SerializeField] private GameObject longRangeEnemyGameObject;
    [SerializeField] private GameObject playerSpawnPosition;
    [SerializeField] private GameObject enemySpawnPoints;
    [SerializeField] private GameObject LinesInTheLeaderboard;
    [SerializeField] private TMP_Text txtScore;
    [SerializeField] private TMP_Text txtWeaponLevel;
    [SerializeField] private TMP_Text txtWaveNumber;
    [SerializeField] private Button playButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button leaderboardReturnButton;
    [SerializeField] private Button submitNameButton;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private TMP_InputField inputNameField;
    [SerializeField] private Image audioButtonImage;
    [SerializeField] private Sprite audioOn;
    [SerializeField] private Sprite audioOff;



    // class level statics
    public static GAMESTATES gameState = GAMESTATES.MENU;
    public static GameManager instance;
    public static AStar pathfinder;
    public static Player player;
    public static MyGrid grid;
    public static int score = 0;
    private static int waveNumber = 0;
    private static Leaderboard leaderboard; 
    private static int weaponLevel;
    private static int numberOfEnemiesSpawned = 0;

    //class level variable
    private bool isPlaying = false;
    private Sprite activeAudioSprite;
    


    // Start is called before the first frame update
    private void Start()
    {
        // setting the instance
        instance = this;
        // getting access to the leaderboard script
        leaderboard = this.gameObject.GetComponent<Leaderboard>();
        // setting the leaderboard to be not visible
        LeaderboardCanvas.SetActive(false);

        // Set the image that is currently active on the audio button to be the audio on by default
        activeAudioSprite = audioOn;
        
        // Create our AStar Pathfinder object with a Grid Map that is 180 wide, 180 high and is located at the origin
        pathfinder = new AStar(180, 180, new Vector3(0, 0, 0));
        // Get the Grid
        grid = AStar.instance.GetGrid();

        //Set up the Event Listeners for the Buttons
        playButton.onClick.AddListener(StartGame);
        leaderboardButton.onClick.AddListener(ShowLeaderBoard);
        leaderboardReturnButton.onClick.AddListener(HideLeaderBoard);
        submitNameButton.onClick.AddListener(GetNameFromField);
        
        //hide the game over canvas
        GameOverCanvas.SetActive(false);
        // set the first game state to be menu
        SetGameState(GAMESTATES.MENU);

        //Used for testing - resetting the playerprefs
        //PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        
        /*
        //used for testing purposes to get the x and y coorindates of a square that gets clicked on.
        if (Input.GetButtonDown("Fire1"))
        {
            //get the mouse position in relation the the world from the camera
            Vector2 mousePosition = gameCamera.ScreenToWorldPoint(Input.mousePosition);
            // get the square that we click on from the mouse position
            Square s = pathfinder.GetGrid().GetGridSquare((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
            // log the x and y to the console.
            Debug.Log(s.x + ", " + s.y + "   -   Walkable: " + s.canWalkOnSquare);
        }
        */

        // if the player is currently playing 
        if (instance.isPlaying) {
            // if the player has cleared the current wave
            if (Enemy.GetList().Count == 0)
            {
                // Call the next wave
                NextWave();
            }
            // once the score is greater than 2000, upgrade the users bullet type from level 1 to level 2.
            if (score > 2000 && weaponLevel == 1)
            {
                weaponLevel++; // increment the weapon level
                txtWeaponLevel.text = "Weapon Level: " + weaponLevel; // set the set on the canvas so the user can see the level weapon they have
                player.UpgradeWeapon(); // upgrade the enemies weapon
            }
            // once the score is greater than 5000, upgrade the users bullet type from level 2 to level 3.
            if (score > 5000 && weaponLevel == 2)
            {
                weaponLevel++; // increment the weapon level
                txtWeaponLevel.text = "Weapon Level: " + weaponLevel; // set the set on the canvas so the user can see the level weapon they have
                player.UpgradeWeapon(); // upgrade the enemies weapon
            }
        }
    }

    private void StartGame() // start the game
    {
        
        waveNumber = 0; // initialize the wave number to 0
        SetScore(0); // initialize the score to 0
        weaponLevel = 1; // initialize the weapon level to 1
        SetGameState(GAMESTATES.PLAYING); // set the game state to be playing
        SpawnPlayer(); // spawn the player
        NextWave(); // goto the first wave and spawn enemies
  
        //used for testing purposes
        //GameObject go = Instantiate(longRangeEnemyGameObject) as GameObject;
        //go.transform.position = new Vector3(70,70);
    }

    private void NextWave()
    {
       //set the wave number text for the user to see
        txtWaveNumber.text = "Wave Number: " + waveNumber;
        StartCoroutine(SpawnEnemy(2)); //start spawning enemies for this wave
    }


    private void ShowLeaderBoard() {
        // show the leaderboard canvas
        LeaderboardCanvas.SetActive(true);
        instance.LeaderboardCanvas.transform.GetChild(1).gameObject.SetActive(false); // hide game over
        instance.LeaderboardCanvas.transform.GetChild(2).gameObject.SetActive(false); // hide better luck next time
        instance.LeaderboardCanvas.transform.GetChild(3).gameObject.SetActive(false); // hide congratulations
        CursorController.instance.SetToNavigationCursor(); // set the mouse cursor to be the navigation cursor

        // If there exists only the template line as a child of lines in the leaderboard
        //Debug.Log(LinesInTheLeaderboard.transform.childCount);
        if (LinesInTheLeaderboard.transform.childCount == 1)
        {
            // show the "Be the first to make the leaderboard" text
            instance.LeaderboardCanvas.transform.GetChild(4).gameObject.SetActive(true);
        }// else hide the "Be the first to make the leaderboard" text
        else { instance.LeaderboardCanvas.transform.GetChild(4).gameObject.SetActive(false); }
    }
    private void HideLeaderBoard() {
        //set the game state to be menu as returning from the leaderboard always goes to the menu screen
        SetGameState(GAMESTATES.MENU);
        LeaderboardCanvas.SetActive(false);//hide the leaderboard canvas
        instance.GameOverCanvas.SetActive(false);// hide the game over canvas
    }

    public static void GameOver()
    {

        // Destroy all Enemies
        Enemy.DestroyAllEnemies();

        //Destroy Player health/armour bars and the Player object
        Destroy(Player.PlayerBars);
        Destroy(player.gameObject);
        
        //set the isPlaying bool to false as the player is no longer playing
        instance.isPlaying = false;
        // set the size (Field of View/Zoom) of the camera back to 50
        instance.gameCamera.orthographicSize = 50f;
        // Set the Camera back to its default position
        CameraController.SetCameraPosition(new Vector3(88.7f, 54.6f, -131.8168f));

        // Update Leaderboard
       
        //check if the score obtained by the user is worthy of the leaderboard
        bool isScoreToGoOnLeaderboard = leaderboard.CompareScore(score);

        // show game over canvas
        instance.GameOverCanvas.SetActive(true); // show the game over canvas
        instance.GameOverCanvas.transform.GetChild(5).gameObject.SetActive(false); // hide the error message that gets displayed to the user when they try to enter an empty string as a name
        if (isScoreToGoOnLeaderboard) // if the score is worthy of the leaderboard
        {
            // set the cursor to be the navigation cursor
            CursorController.instance.SetToNavigationCursor(); // set the mouse cursor to be the navigation cursor
            // show input Name object
            instance.GameOverCanvas.transform.GetChild(2).gameObject.SetActive(true);
            // hide better luck next time text
            instance.GameOverCanvas.transform.GetChild(3).gameObject.SetActive(false);

        } else
        {
            
            // hide input name object 
            instance.GameOverCanvas.transform.GetChild(2).gameObject.SetActive(false);
            instance.GameOverCanvas.transform.GetChild(4).gameObject.SetActive(false); // hide congratulations

            // show leaderboard
            instance.ShowLeaderBoard();
            //show game over text and better luck next time
            instance.LeaderboardCanvas.transform.GetChild(1).gameObject.SetActive(true); // show game over
            instance.LeaderboardCanvas.transform.GetChild(2).gameObject.SetActive(true); // show better luck next time
            instance.LeaderboardCanvas.transform.GetChild(3).gameObject.SetActive(false); // hide congratulations
            instance.LeaderboardCanvas.transform.GetChild(4).gameObject.SetActive(false); // hide "Be the first to make the leaderboard"
        }

    }


    IEnumerator SpawnEnemy(int numberOfEnemies) // used to spawn enemies 1 second apart to avoid them stacking immediately
    {
        waveNumber += 1; // increments the wave number as this method is only called once every wave
        numberOfEnemies = numberOfEnemies * waveNumber + 3; // calculating the number of enemies to be spawned
        System.Random randomizer = new System.Random(); // creating a randomizer to allow us to get random numbers
        GameObject enemy; // declaring an enemy object that will be used to instantiate enemies in the for loop
        for (int i = 0; i < numberOfEnemies; i++) // looping through as many times as enemies need spawning
        {
            int num = randomizer.Next(4); // creates a random number between 0 and 3 - because there are 4 enemy spawn points
            //Debug.Log(num);
            Vector3 spawnPoint = enemySpawnPoints.transform.GetChild(num).gameObject.transform.position; // getting a random spawn point of the 4 
            if (numberOfEnemiesSpawned % 3 == 0) { enemy = Instantiate(longRangeEnemyGameObject) as GameObject; } // if the number of enemies spawned is divisible by 3 we spawn a long-range enemy
            else { enemy = Instantiate(enemyGameObject) as GameObject; } // other wise we spawn a regular enemy
            numberOfEnemiesSpawned++; // increment the number of enemies spawned
            if(!instance.isPlaying) instance.isPlaying = true; // if the bool isPlaying is false, set it to true
            //Debug.Log("Enemy");
            enemy.transform.position = spawnPoint; // set the position of the new enemy to be at the spawn point
            // wait one second before spawning the next enemy so that they dont spawn on top of each other
            yield return new WaitForSeconds(1f);
            //Debug.Log("Enemies Present: " + Enemy.GetList().Count);
        }
        
        yield break;
    }

    private void SpawnPlayer()
    {
        // Spawn Player
        GameObject p = Instantiate(playerPrefab) as GameObject; // instantiate player object
        //Debug.Log("Player");
        p.transform.position = playerSpawnPosition.transform.position; // set the position of the object
        player = p.GetComponent<Player>(); // set the public static Player script component
    }


    public static void SetGameState(GAMESTATES state)
    {
        gameState = state; // set the gamestate
        switch(state)
        {
            case GAMESTATES.MENU: // if the game state is menu
                instance.TitlePageCanvas.SetActive(true);// show the title page
                instance.PlayingCanvas.SetActive(false);// hide the playing canvas
                CursorController.instance.SetToNavigationCursor();// use the navigation cursor
                break;
            case GAMESTATES.PLAYING: // if the game state is playing
                instance.TitlePageCanvas.SetActive(false); // hide the title page
                instance.PlayingCanvas.SetActive(true); // show the playing canvas
                instance.txtWeaponLevel.text = "Weapon Level: " + weaponLevel; // set the weapon level text
                CursorController.instance.SetToCrossHairCursor();// ensure the cursor being used is the crosshair
                break;
        }
    }

    public static void SetScore(int newScore)
    {
        score = newScore; // set the score 
        instance.txtScore.text = score.ToString();// update the text on the canvas with the new score
    }

    private void GetNameFromField() // get the players name from the input field before adding their score to the leaderboard
    { // note the input box is limited to 20 characters in the inspector
        string name = inputNameField.text; // getting the text from the input field
        if (name == "") // if the name is blank
        {
            //show the error message asking for a name to be entered
            instance.GameOverCanvas.transform.GetChild(5).gameObject.SetActive(true);
        }
        else { // if there exists a name in the text box
            
            inputNameField.text = ""; // resetting the text field for the next player
            // send name and score to the CheckScore method
            leaderboard.CheckScore(name, score); // which once again checks the score is worthy of the leaderboard and then if so adds it to the leaderboard
            // hide input name object
            instance.GameOverCanvas.transform.GetChild(2).gameObject.SetActive(false);

            // show leaderboard
            instance.ShowLeaderBoard(); // show leaderboard
            instance.LeaderboardCanvas.transform.GetChild(1).gameObject.SetActive(true); // show game over
            instance.LeaderboardCanvas.transform.GetChild(3).gameObject.SetActive(true); // show congratulations
            instance.LeaderboardCanvas.transform.GetChild(2).gameObject.SetActive(false); // hide better luck next time
        }
    }

    public void MuteAudio() // this is for the mute audio button on the title screen to allow the player to play without sounds if they so please
    {
        // set the bool for whether the audio listener is paused or not to be the opposite of its current state
        AudioListener.pause = !AudioListener.pause;
        // if the sprite for audio being turned on is active
        if(activeAudioSprite == instance.audioOn)
        {
            // change the active sprite to be the sprite for audio being turned off
            activeAudioSprite = instance.audioOff;
            
        } else
        {// if the spite for audio being turned off is active
            activeAudioSprite = instance.audioOn; // set the active sprite to be the sprite for audio being turned on
        }
        // set the sprite for the button
        audioButtonImage.sprite = activeAudioSprite;
    }
    
}
