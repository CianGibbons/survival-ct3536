using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{
    //Declarations
    private MyGrid grid;
    public int x, y, g, f , h;
    public bool canWalkOnSquare;
    public Square previousSquare;
    // F is the estimated cost from start to the destination node  via this node
    // G is the known cost from the start to the current node
    // H is the estimated cost from this node to the destination node

    public Square(MyGrid grid, int x, int y)
    {
        //Initializations
        this.grid = grid;
        this.x = x;
        this.y = y;
        canWalkOnSquare = true;
    }

    public void SetIfSquareIsWalkable(bool canWalkOnSquare)
    {
        // Set whether the A* algorithm will search this square or not
        // if the square cannot be walked on then the square will be immediately added to the closed list
        this.canWalkOnSquare = canWalkOnSquare;
    }

    public void CalculateF() // calculate the f of this square
    {
        f = g + h;
    }

    public override string ToString() // to string method i used to print out the x and y values when debugging
    {
        return x + "," + y;
    }
}
