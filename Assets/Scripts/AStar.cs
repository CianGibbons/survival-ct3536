using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{

    //class level private variables

    // The cost of moving straight and the cost of moving diagonally
    private const int StraightMoveCost = 10; // this is the cost of moving up, down, left or right
    
    // this is calculated by getting the square root of 200
    private const int DiagonalMoveCost = 14; // This is the cost of moving diagonally to the Top Left/Top Right/Bottom Left/Bottom Right from the Center

    // The open list consists of nodes to which the algorithm has already found a route, but which have not themselves been expanded
    // the closed list consists of nodes that have been expanded and as a result do not need to be revisited
    private List<Square> openList;
    private List<Square> closedList;

    private MyGrid grid; //Declaring the grid

    // Class level statics
    // Declaring an instance of the AStar Pathfinder
    public static AStar instance { get; private set; } // this makes it publically readonly but within this class it can be set
    // so other classes can get the instance but cannot edit the instance

    

    public AStar(int width, int height, Vector3 location)
    {
        instance = this; // setting the instance
        grid = new MyGrid(width, height, 1f, location); // initializing the grid with the given width and height to declare the amount of cells, the size of the cell and the location of the origin of the grid
    }

    public MyGrid GetGrid() { return grid; } // getter method for the grid

    // method to get a list of locational vector3s to be used as the path for the enemy to follow 
    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        Vector2 vStart = grid.GetXAndY(startWorldPos); // get the x and y (grid coords) of the starting position of the algorithm
        Vector2 vDestination = grid.GetXAndY(endWorldPos); // get the x and y (grid coords) of the destination position of the algorithm
        // Call the Find Path method to start the algorithm which returns a list of Squares for the enemy to travel on to get to the destination
        List<Square> path = FindPath(Mathf.FloorToInt(vStart.x), Mathf.FloorToInt(vStart.y), Mathf.FloorToInt(vDestination.x), Mathf.FloorToInt(vDestination.y));
        // if the path is not null - Avoiding null pointers
        if (path != null)
        {
            // create a list of vector3s which we can return with our results
            List<Vector3> vPath = new List<Vector3>();
            foreach(Square current in path) // for each square in the path
            {
                // Convert the Square to a Vector3 by using the Squares x and y values to create a new vector3 and add this vector3 to the vectorList
                vPath.Add(new Vector3(current.x, current.y) * grid.GetSize() + Vector3.one * grid.GetSize() * 0.5f);
            }
            return vPath; // return the vector list containing the path when whole path has been calculated and converted to Vector3s
        }
        else // else if the path is null
        {
            return null; // return null because there is no path to follow
        }
    }
    public List<Square> FindPath(int startX, int startY, int destX, int destY)
    { 
        // this method is used to run the A* algorithm. 
        // It finds a path of squares that the enemy can travel on to get to the player
        
        //Initialise the open and closed lists
        openList = new List<Square>();
        closedList = new List<Square>();

        // initialise the starting location square
        Square startingSqaure = grid.GetGridSquare(startX, startY);
        
        //Debug.Log(startingSqaure);
        openList.Add(startingSqaure); // add the starting location square to the open list as this needs to be searched and expanded.

        //Debug.Log("Destination: "+ destX + "," +destY);
        
        // initialise the destination location square
        Square destinationSqaure = grid.GetGridSquare(destX, destY);
        
        //Debug.Log(destinationSqaure);
        //Debug.Log("Starting Square: " + startingSqaure);
        //Debug.Log("Destination Square: " + destinationSqaure);

        // F is the estimated cost from start to the destination node  via this node
        // G is the known cost from the start to the current node
        // H is the estimated cost from this node to the destination node

        //Loop through all of the squares in the 2D Grid
        for (int x=0;x<grid.GetWidth();x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                // get the current square
                Square current = grid.GetGridSquare(x, y);
                // initialize
                current.g = int.MaxValue; //initialize G to be infinity  (Max storable value) - this may be overwritten later in calculations
                current.CalculateF(); // calculate f = g + h for this node
                current.previousSquare = null; // all squares initially have no previous square
            }
        }

        startingSqaure.g = 0; // set g to be 0 as we have not moved yet
        startingSqaure.h = CalculateDistToDestCost(startingSqaure, destinationSqaure); // calculating the h value by calculating the distance from the starting square to the destination square with the Costs of moving straight and diagonally in mind
        startingSqaure.CalculateF(); // calculate the f cost of the journey for the starting square


        while(openList.Count > 0)// while there are items in the open list, keep searching
        {
            Square current = GetSquareWithLowestF(openList); // get the square in the openlist with the lowest f value
            if(current == destinationSqaure) // if the current square is the destination
            {
                //Destination has been reached
                return PathToDest(destinationSqaure);// traverse bacakwards through the linked list to create the path
            }
            // Destination has not been reached, this square is NOT the destination
            openList.Remove(current); // remove from open list as this square has been searched
            closedList.Add(current);  // current has been expanded and searched, we are done with this square now.

            foreach(Square neighbour in GetListOfNeighbours(current)) // get the list of the current squares neighbouts and loop through them
            {
                if (closedList.Contains(neighbour)) continue; // This node has been searched previously, so we can ignore it and continue
                if(!neighbour.canWalkOnSquare) // if the neighbour square is an object that cannot be walked over 
                {
                    closedList.Add(neighbour); // add the neighbour to the closed list as we do not need to expand
                    continue; // go to the next neighbour
                }
                
                int PossibleNeighbourG = current.g + CalculateDistToDestCost(current, neighbour); // g cost so far plus the cost of moving from the previous (current) node to this node (neighbour)
                if(PossibleNeighbourG < neighbour.g) // only overwrite the neighbours information if the path we are taking now is shorter than the path that was calculated previously
                { // cost of the journey is less, hence update the neighbours data
                    neighbour.g = PossibleNeighbourG; // set the new G cost
                    neighbour.previousSquare = current; // set the previous square of this neighbour to be the current square
                    neighbour.h = CalculateDistToDestCost(neighbour, destinationSqaure); // calculate the H value
                    neighbour.CalculateF(); // calculate F using the new G and H
                }
                if (!openList.Contains(neighbour)) openList.Add(neighbour); // if the neighbour isnt on the open list add it to the open list as it is also not on the closed list
            }
        }

        // No more nodes on the open list once the while loop is exited, No path was found
        return null;

    }

    private Square GetSquareWithLowestF(List<Square> list)
    { // get the square with the lowest f value as this is the square to be expanded next
        Square WithLowestF = list[0]; //start setting the first node to be the lowest and compare the rest to it after
        for(int i=1; i< list.Count;i++)// start at 1 as the first square at index 0 is already selected
        {
            if (list[i].f < WithLowestF.f) WithLowestF = list[i]; // check if the current has a lower f, if so set the current as the lowest
        }
        return WithLowestF; // return the square with the lowest F value
    }

    private List<Square> GetListOfNeighbours(Square current) {
        List<Square> neighbouringSquares = new List<Square>();

        // Imagine a 3 by 3 square grid
        // 0 0 0
        // 0 C 0
        // 0 0 0
        // Where 0 stands for squares not checked, C stands for the current node
        // Every other number stands for the order in which the squares are checked and added to the neighbouring list
        if(current.x - 1 >= 0) // all if statements are to ensure we dont go out of bounds - eg look for a 4th 0 in the first row as this doesnt exist
        {
            neighbouringSquares.Add(grid.GetGridSquare(current.x - 1, current.y));
            //Left
            // 0 0 0
            // 1 C 0
            // 0 0 0
            if (current.y - 1 >= 0) neighbouringSquares.Add(grid.GetGridSquare(current.x - 1, current.y - 1));
            //Left Down
            // 0 0 0
            // 1 C 0
            // 2 0 0
            if (current.y + 1 > grid.GetHeight()) neighbouringSquares.Add(grid.GetGridSquare(current.x - 1, current.y + 1));
            //Left Up
            // 3 0 0
            // 1 C 0
            // 2 0 0
        }
        if (current.x + 1 < grid.GetWidth())
        {
            neighbouringSquares.Add(grid.GetGridSquare(current.x + 1, current.y));
            //Right
            // 3 0 0
            // 1 C 4
            // 2 0 0
            if (current.y - 1 >= 0) neighbouringSquares.Add(grid.GetGridSquare(current.x + 1, current.y - 1));
            //Right Down
            // 3 0 0
            // 1 C 4
            // 2 0 5
            if (current.y + 1 < grid.GetHeight()) neighbouringSquares.Add(grid.GetGridSquare(current.x + 1, current.y + 1));
            //Right Up
            // 3 0 6
            // 1 C 4
            // 2 0 5
        }
        if (current.y - 1 >= 0) neighbouringSquares.Add(grid.GetGridSquare(current.x, current.y - 1));
        // Down
        // 3 0 6
        // 1 C 4
        // 2 7 5
        if (current.y + 1 > grid.GetHeight()) neighbouringSquares.Add(grid.GetGridSquare(current.x, current.y + 1));
        //Up
        // 3 8 6
        // 1 C 4
        // 2 7 5

        // All Squares that could be neighbouring the current have been checked.
        // Return the list

        return neighbouringSquares;
    }

    private List<Square> PathToDest(Square destination)
    {
        // Each Square has stored the square that it came from so if we start from the destination square we can work backwards to create our path
        List<Square> path = new List<Square>(); // initialise a list of squares
        path.Add(destination); // add the destination to the list -  keep in mind we are working backwards from the destination to the start as in this linked list we stored the previous square in each square
        Square current = destination; // start with the current square being the destination node
        while (current.previousSquare != null) // the starting square doesnt have a previous square, we can use this to identify when we have reached the starting square
        {
            path.Add(current.previousSquare); // add the square that came before the current to the path
            current = current.previousSquare; // reset the current to move back in the list

        }
        path.Reverse(); // our path is currently backwards, so we have to reverse it so that it is working from start to destination, not destination to start
        return path; // return the path in the correct order.
    }

    private int CalculateDistToDestCost(Square start, Square dest) // the distance to the destination square
    {
        
        int distX = Mathf.Abs(start.x - dest.x); // distance across
        int distY = Mathf.Abs(start.y - dest.y); // distance up
        
        
        //int distToGo = Mathf.Abs(distX - distY); // distance that's left
        //return StraightMoveCost * distToGo + DiagonalMoveCost * Mathf.Min(distX, distY); // return the calculation of the distance keeping in mind the cost of the move

        return StraightMoveCost * (distX + distY) + (DiagonalMoveCost - 2 * StraightMoveCost) * Mathf.Min(distX, distY);


    }

    
}
