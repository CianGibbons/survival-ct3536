using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardLine
{
    public string name;
    public int score;

    public LeaderboardLine(string playerName, int playerScore)
    {
        name = playerName;
        score = playerScore;
    }

    public override string ToString()
    {
        return name+","+score;
    }
}
