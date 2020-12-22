using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
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
        instance = this;

        leaderboard = this.gameObject.GetComponent<Leaderboard>();
        
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
            // once the score is greater than 1500, upgrade the users bullet type from level 1 to level 2.
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
        score = 0; // initialize the score to 0
        weaponLevel = 1; // initialize the weapon level to 1
        SetGameState(GAMESTATES.PLAYING);
        SpawnPlayer();
        NextWave();
  
        //used for testing purposes
        //GameObject go = Instantiate(longRangeEnemyGameObject) as GameObject;
        //go.transform.position = new Vector3(70,70);
    }

    private void NextWave()
    {
       
        txtWaveNumber.text = "Wave Number: " + waveNumber;
        StartCoroutine(SpawnEnemy(2));
    }


    private void ShowLeaderBoard() {
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
        }
        else { instance.LeaderboardCanvas.transform.GetChild(4).gameObject.SetActive(false); }
    }
    private void HideLeaderBoard() {
        SetGameState(GAMESTATES.MENU);
        LeaderboardCanvas.SetActive(false);
        instance.GameOverCanvas.SetActive(false);
    }

    public static void GameOver()
    {

        // Destroy all Enemies
        Enemy.DestroyAllEnemies();

        //Destroy Player
        Destroy(Player.PlayerBars);
        Destroy(player.gameObject);

        instance.isPlaying = false;
        instance.gameCamera.orthographicSize = 50f;
        // Set the Camera back to its position
        CameraController.SetCameraPosition(new Vector3(88.7f, 54.6f, -131.8168f));

        // Update Leaderboard
        bool isScoreToGoOnLeaderboard = leaderboard.CompareScore(score);

        // show game over canvas
        instance.GameOverCanvas.SetActive(true);
        instance.GameOverCanvas.transform.GetChild(5).gameObject.SetActive(false);
        if (isScoreToGoOnLeaderboard)
        {
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
       
       

        // comment leaderboard code

    }


    IEnumerator SpawnEnemy(int numberOfEnemies)
    {
        waveNumber += 1;
        numberOfEnemies = numberOfEnemies * waveNumber + 3;
        System.Random randomizer = new System.Random();
        GameObject enemy;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            int num = randomizer.Next(4); // creates a random number between 0 and 3 - because there are 4 enemy spawn points
            //Debug.Log(num);
            Vector3 spawnPoint = enemySpawnPoints.transform.GetChild(num).gameObject.transform.position;
            if (numberOfEnemiesSpawned % 3 == 0) { enemy = Instantiate(longRangeEnemyGameObject) as GameObject; }
            else { enemy = Instantiate(enemyGameObject) as GameObject; }
            numberOfEnemiesSpawned++;
            if(!instance.isPlaying) instance.isPlaying = true;
            //Debug.Log("Enemy");
            enemy.transform.position = spawnPoint;
            
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
        player = p.GetComponent<Player>(); // set the public Player script component
    }


    public static void SetGameState(GAMESTATES state)
    {
        gameState = state;
        switch(state)
        {
            case GAMESTATES.MENU:
                instance.TitlePageCanvas.SetActive(true);
                instance.PlayingCanvas.SetActive(false);
                CursorController.instance.SetToNavigationCursor();
                break;
            case GAMESTATES.PLAYING:
                instance.TitlePageCanvas.SetActive(false);
                instance.PlayingCanvas.SetActive(true);
                instance.txtWeaponLevel.text = "Weapon Level: " + weaponLevel;
                CursorController.instance.SetToCrossHairCursor();
                break;
        }
    }

    public static void SetScore(int newScore)
    {
        score = newScore;
        instance.txtScore.text = score.ToString();
    }

    private void GetNameFromField()
    {
        string name = inputNameField.text;
        if (name == "")
        {
            instance.GameOverCanvas.transform.GetChild(5).gameObject.SetActive(true);
        }
        else {
            inputNameField.text = ""; // resetting the text field for the next player
                                      // send name and score to the CheckScore method
            leaderboard.CheckScore(name, score);
            // hide input name object
            instance.GameOverCanvas.transform.GetChild(2).gameObject.SetActive(false);

            // show leaderboard
            instance.ShowLeaderBoard(); // show leaderboard
                                        //show game over text and better luck next time
            instance.LeaderboardCanvas.transform.GetChild(1).gameObject.SetActive(true); // show game over
            instance.LeaderboardCanvas.transform.GetChild(3).gameObject.SetActive(true); // show congratulations
            instance.LeaderboardCanvas.transform.GetChild(2).gameObject.SetActive(false); // hide better luck next time
        }
        


    }

    public void MuteAudio()
    {
        AudioListener.pause = !AudioListener.pause;
        if(activeAudioSprite == instance.audioOn)
        {
            activeAudioSprite = instance.audioOff;
            
        } else
        {
            activeAudioSprite = instance.audioOn;
        }
        audioButtonImage.sprite = activeAudioSprite;
    }
    
}
