using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{
    //Declarations
    private int width, height;
    private float size;
    private Vector3 location;
    private Square[,] grid;

    public MyGrid(int width, int height, float size, Vector3 location)
    {
        // initialisation
        this.width = width;
        this.height = height;
        this.size = size;
        this.location = location;
        grid = new Square[width, height]; 

        //Populate the grid with the squares
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new Square(x, y); // create a new square in the grid at x, y
                //Draw the bottom and left lines for the square in the grid - can be viewed using Gizmos
                Debug.DrawLine(GetSquareWorldPosition(x, y), GetSquareWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetSquareWorldPosition(x, y), GetSquareWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        //Draw the Horizontal and Vertical outside lines for the top and right of the grid - can be viewed using Gizmos
        Debug.DrawLine(GetSquareWorldPosition(0, height), GetSquareWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetSquareWorldPosition(width, 0), GetSquareWorldPosition(width, height), Color.white, 100f);
    }

    public int GetHeight() { return height; } // getter for the height of the grid in squares
    public int GetWidth() { return width; } // getter for the width of the grid in squares
    public float GetSize() { return size; } // getter for the size of each square - i.e. the length of each side (Everry square is 1x1, therefore size = 1)

    public Vector2 GetXAndY(Vector3 worldPos)
    {
        // get the x and y indexes for a square in the grid based on its location in the world.
        // take into account where the grid begins and what size each square is
        int x = Mathf.FloorToInt((worldPos - location).x / size);
        int y = Mathf.FloorToInt((worldPos - location).y / size);
        return new Vector2(x, y);
    }

    public Vector3 GetSquareWorldPosition(int x, int y)
    {
        // get the location of a square at x, y in relation to the world, taking into account the size of each cell and the starting location of the grid (i.e. the bottom left of the grid)
        return new Vector3(x, y) * size + location;
    }

    public Square GetGridSquare(int x, int y)
    {
        //get the square using the x and y indexes in the grid
        if (x >= 0 && y >= 0 && x < width && y < height)// check x and y are not out of bounds
        {
            return grid[x, y];
        }
        return null;
    }
    // get a square on the grid using the world position
    public Square GetGridSquare(Vector3 worldPos)
    {
        Vector2 v = GetXAndY(worldPos); // get the x and y indexes for a square in the grid based on its location in the world.
        return GetGridSquare((int)v.x,(int) v.y); // //get the square using the x and y indexes in the grid
    }

    public void SetGridSquare(int x, int y, Square sqaure) //setter method for the squares
    {
        if (x >= 0 && y >= 0 && x < width && y < height) // check x and y are not out of bounds
        {
            grid[x, y] = sqaure; // set the square in the grid 
        }
    }

    public void SetGridSquare(Vector3 worldPos, Square square) //setter method for setting a square at the world position specified - i used this when testing my grid where by i clicked a square to set and get values
    {
        Vector2 v = GetXAndY(worldPos);
        SetGridSquare((int) v.x, (int) v.y, square);
    }

    public override string ToString() {
        // this is a simple toString method I used to Debug 
        return "Grid Width: "+ width +"\nGrid Height: "+height;
    }
}
