using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardLine
{
    // declaring public variables
    public string name;
    public int score;

    public LeaderboardLine(string playerName, int playerScore)
    {
        //initializing public variables
        name = playerName;
        score = playerScore;
    }
    // creating a toString method for a Line object
    public override string ToString()
    {
        return name+","+score;
    }
}
