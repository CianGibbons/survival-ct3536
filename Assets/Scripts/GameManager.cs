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
    public GameObject TitlePageCanvas, PlayingCanvas, LeaderboardCanvas, GameOverCanvas;
    public GameObject playerPrefab;
    public GameObject enemyGameObject;
    public GameObject playerSpawnPosition;
    public GameObject enemySpawnPoints;
    public TMP_Text txtScore;
    public Button playButton;
    public Button leaderboardButton;
    public Button leaderboardReturnButton;
    public Button submitNameButton;
    public Camera gameCamera;
    public TMP_InputField inputNameField;


    // class level statics
    public static GAMESTATES gameState = GAMESTATES.MENU;
    public static GameManager instance;
    public static AStar pathfinder;
    public static Player player;
    public static MyGrid grid;
    public static int score = 0;
    private static int wave = 1;
    private static Leaderboard leaderboard;


    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        leaderboard = this.gameObject.GetComponent<Leaderboard>();
        LeaderboardCanvas.SetActive(false);
        // Create our AStar Pathfinder object with a Grid Map that is 180 wide, 180 high and is located at the origin
        pathfinder = new AStar(180, 180, new Vector3(0, 0, 0));
        // Get the Grid
        grid = AStar.instance.GetGrid();

        playButton.onClick.AddListener(StartGame);
        leaderboardButton.onClick.AddListener(ShowLeaderBoard);
        leaderboardReturnButton.onClick.AddListener(HideLeaderBoard);
        submitNameButton.onClick.AddListener(GetNameFromField);
        GameOverCanvas.SetActive(false);
        SetGameState(GAMESTATES.MENU);


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
    }

    private void StartGame()
    {
        wave = 0;
        SetGameState(GAMESTATES.PLAYING);
        score = 0;
        SpawnPlayer();
        NextWave();
    }

    private void NextWave()
    {
        wave++;
        StartCoroutine(SpawnEnemy(4 * wave));

    }


    private void ShowLeaderBoard() {
        LeaderboardCanvas.SetActive(true);
        instance.LeaderboardCanvas.transform.GetChild(1).gameObject.SetActive(false); // hide game over
        instance.LeaderboardCanvas.transform.GetChild(2).gameObject.SetActive(false); // hide better luck next time
        instance.LeaderboardCanvas.transform.GetChild(3).gameObject.SetActive(false); // hide congratulations
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
        
       

        // Set the Camera back to its position
        CameraController.SetCameraPosition(new Vector3(88.7f, 54.6f, -131.8168f));

        // Update Leaderboard
        bool isScoreToGoOnLeaderboard = leaderboard.CompareScore(score);

        // show game over canvas
        instance.GameOverCanvas.SetActive(true);
        if (isScoreToGoOnLeaderboard)
        {
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
        }
       
       

        // comment leaderboard code

    }


    IEnumerator SpawnEnemy(int numberOfEnemies)
    {
        System.Random randomizer = new System.Random();

        for(int i = 0; i < numberOfEnemies; i++)
        {
            int num = randomizer.Next(4); // creates a random number between 0 and 3 - because there are 4 enemy spawn points
            //Debug.Log(num);
            Vector3 spawnPoint = enemySpawnPoints.transform.GetChild(num).gameObject.transform.position; 
            GameObject enemy = Instantiate(enemyGameObject) as GameObject;
            enemy.transform.position = spawnPoint;
            
            yield return new WaitForSeconds(1f);
            Debug.Log("Enemies Present: " + Enemy.GetList().Count);
        }
        
        yield break;
    }

    private void SpawnPlayer()
    {
        // Spawn Player
        GameObject p = Instantiate(playerPrefab) as GameObject; // instantiate player object
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
                break;
            case GAMESTATES.PLAYING:
                instance.TitlePageCanvas.SetActive(false);
                instance.PlayingCanvas.SetActive(true);
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
        inputNameField.text = ""; // resetting the text field for the next player
        // send name and score to the CheckScore method
        leaderboard.CheckScore(name,score);
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
