using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // This script is attached to the rectanglular objects that I created as walls/obstacles for the A* path to go around.
    // It is meant to set each square that the object covers to be not walkable.


    private void Start()
    {
        // getting the scale and position of the obstacle
        Vector3 position = transform.position;
        Vector3 localScale = transform.localScale;

        // Used for Debugging
        //Debug.Log("Square Position: " + position);
        //Debug.Log("Square Scale: " + localScale);


        // there is no point changing the z value from 0 as this is a 2D game
        localScale.z = 0; // setting the z of the local scale to be 0 as we do not want to change any values using this
        
        // This is a vector that I made to correct the calculation of the top right corner - without this the method I created would calculate the square diagonally up and to the right from the square that i need
        Vector3 correctionVector = new Vector3(1, 1);

        Vector3 bottomLeftCorner = position - (localScale / 2); // this gets the bottom left corner of the square
        Vector3 topRightCorner = (position + (localScale / 2)) - correctionVector; // this gets the top right corner exactly which would be a square up and to the right too many so we have to subtract 1
        Vector3 topLeftCorner = new Vector3(bottomLeftCorner.x, topRightCorner.y); // getting the top left corner using the previously calculated corners
        Vector3 bottomRightCorner = new Vector3(topRightCorner.x, bottomLeftCorner.y); // getting the bottom right corner using the previously calculated corners

        //Used for Debugging to see if my calculations were correct
        //Debug.Log("Square BL: " + bottomLeftCorner); 
        //Debug.Log("Square TR: " + topRightCorner);
        //Debug.Log("Square TL: " + topLeftCorner);
        //Debug.Log("Square BR: " + bottomRightCorner);

        //Declaring x and y -  these are to be used in the for loops and would be wasteful to keep declaring over and over.
        int x;
        int y;
        MyGrid grid = GameManager.grid; // getting access to the grid. This is needed to be able to set certain squares on the grid to be not walkable.
        //Debug.Log(grid);
        
        // loops through each square that the rectangular object covers
        for (int i = 0; i<=(topRightCorner.x - topLeftCorner.x); i++) // loops through horizontally
        {
            for(int j = 0; j<=(topLeftCorner.y - bottomLeftCorner.y); j++) // loops through vertically
            {
                x = (int) Mathf.Floor(topLeftCorner.x) + i; // getting the current squares x value by adding on the current value of i
                y = (int) Mathf.Floor(topLeftCorner.y) - j; // getting the current squares y value by subtracting the current value of j
                //Debug.Log(x + ", " + y); // used for testing purposed

                // set this square on the grid to be unwalkable
                grid.GetGridSquare(x, y).SetIfSquareIsWalkable(false);
            }
        }
    }

}
