using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static AStar pathfinder;
    public Player player;
    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        

        TestSceneMethod();

    }

    private void TestSceneMethod()
    {
        // Ensure the camera starts in the position we want it
        Camera.main.transform.position = new Vector3(30, 12, -10);

        // Create our AStar Pathfinder object with a Grid Map that is 57 wide, 32 high and is located at the origin
        pathfinder = new AStar(57, 31, new Vector3(0, 0, 0));

        // Get the Grid
        MyGrid grid = AStar.instance.GetGrid();

        //Make Some Areas not walkable (These are the Squares in which the objects are in the Test Scene)
        
        //SMALL SQUARE OBJECT
        grid.GetGridSquare(30, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(29, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(31, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(30, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(30, 12).SetIfSquareIsWalkable(false);
        // 1 is unwalkable, 0 is walkable
        // 
        // 0 1 0
        // 1 1 1
        // 0 1 0
        //
        //Note we are making the edges of the objects unwalkable too, leaving the corners walkable.



        //BIG SQUARE OBJECT

        grid.GetGridSquare(34, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(34, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(34, 15).SetIfSquareIsWalkable(false);


        grid.GetGridSquare(35, 12).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(35, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(35, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(35, 15).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(35, 16).SetIfSquareIsWalkable(false);

        grid.GetGridSquare(36, 12).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(36, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(36, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(36, 15).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(36, 16).SetIfSquareIsWalkable(false);

        grid.GetGridSquare(37, 12).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(37, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(37, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(37, 15).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(37, 16).SetIfSquareIsWalkable(false);

        grid.GetGridSquare(38, 12).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(38, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(38, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(38, 15).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(38, 16).SetIfSquareIsWalkable(false);

        grid.GetGridSquare(39, 13).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(39, 14).SetIfSquareIsWalkable(false);
        grid.GetGridSquare(39, 15).SetIfSquareIsWalkable(false);

        // 1 is unwalkable, 0 is walkable
        // 
        // 0 1 1 1 1 0
        // 1 1 1 1 1 1
        // 1 1 1 1 1 1
        // 1 1 1 1 1 1
        // 0 1 1 1 1 0
        //
        //Note we are making the edges of the objects unwalkable too, leaving the corners walkable.
    }


}
