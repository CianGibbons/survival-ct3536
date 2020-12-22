using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    // inspector settings
    [SerializeField] private GameObject LinesInTheLeaderboard;
    [SerializeField] private GameObject LineInLeaderboard;
    
    // declaring the list of line objects
    private List<LeaderboardLine> leaderboardLines;

    //place every line 30 below the one before

    private static Vector3 nextPosition; // declaring the position on the canvas for the next line in the leaderboard
    private static int nextIntPositionOnLeaderboard = 1;
    // store the position of the next entry 
    
    public void Start()
    {
        LineInLeaderboard.SetActive(false); // this will be used as a template to instantiate other gameObjects/lines in the leaderboard
        nextPosition = LineInLeaderboard.transform.position; // set the next position for a new line in the leaderboard

        leaderboardLines = new List<LeaderboardLine>(); // create a blank list
        LoadList(); // load the list from PlayerPrefs into leaderboadLines list - this gets any existing lines from the playerprefs and adds them to the list
        leaderboardLines = SortList(leaderboardLines); // sort the list by score to ensure it is in order 
        
        foreach(LeaderboardLine l in leaderboardLines) // loop through each line in the list
        {
            CreateLineInLeaderboard(l); // create the canvas object lines to display the lines in the list
        }

        // Save the PlayerPrefs so we can load them later
        PlayerPrefs.Save();
    }

    private void UpdateLeaderboard()
    {
        //Update the entries on the leaderboard
        nextPosition = LineInLeaderboard.transform.position; // reset the next position to be the position of the template 
        nextIntPositionOnLeaderboard = 1; // next place available on the leaderboard is 1st place as there is nothing on the leaderboard
        int numberOfChildren = LinesInTheLeaderboard.transform.childCount; // count the children in the LinesInTheLeaderboard object
        if (numberOfChildren > 1) // if there is more than just the template, delete them - keep the template - we dont want duplication of lines
        {
            for (int i = 1; i < numberOfChildren; i++) // loop through the children - skipping the template one that is at index 0
            {
                Destroy(LinesInTheLeaderboard.transform.GetChild(i).gameObject); // destroy the gameobjects one by one
            }
        }
        leaderboardLines = SortList(leaderboardLines);// sort the stored list of lines on the leaderboard
        foreach (LeaderboardLine l in leaderboardLines)// loop through each line and create a gameobject for it
        {
            CreateLineInLeaderboard(l);// create a game object with the values taken from the current leaderboard line
        }
        PlayerPrefs.Save();//save the player prefs sso we can load them later
    }

    private void CreateLineInLeaderboard(LeaderboardLine lineObject)
    {
        GameObject line = Instantiate(LineInLeaderboard) as GameObject; // create the line
        line.transform.SetParent(LinesInTheLeaderboard.transform); // set the line as the child of lines
        line.transform.position = nextPosition; // set the position of the line
        line.SetActive(true); // ensure the line is shown and is not hiding
        nextPosition.y = nextPosition.y - 30; // set the position of the next line 30 below the line just made

        // change the text of each column in the line
        // Child 0 - Position
        // Child 1 - Name
        // Child 2 - Score
        // get the text component for each column in this line
        TMP_Text position = line.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text name = line.transform.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text score = line.transform.GetChild(2).GetComponent<TMP_Text>();

        // get the position of this object on the leaderboard - keep in mind this list should be sorted when this is called
        int pos = nextIntPositionOnLeaderboard;
        // increment the next position so that the next line will be a position below the current
        nextIntPositionOnLeaderboard++;
        string positionString = ""; // initialize a string to set the position as
        switch (pos) // use a switch case to determine whether the current position needs a 'st','nd','rd' or 'th' at the end of its position number
        {
            case 1: positionString = "1st"; break;
            case 2: positionString = "2nd"; break;
            case 3: positionString = "3rd"; break;
            case 4: positionString = "4th"; break;
            case 5: positionString = "5th"; break;
            case 6: positionString = "6th"; break;
            case 7: positionString = "7th"; break;
            case 8: positionString = "8th"; break;
            case 9: positionString = "9th"; break;
            case 10: positionString = "10th"; break;
        }
        position.text = positionString; // set the position text from the positionString
        name.text = lineObject.name; // set the name text from the name stored in the line object
        int scoreInt = lineObject.score; // set the score text from the score in the line object
        score.text = ""+scoreInt; // set the score text as the score - i added a blank string as a quick way in making a string
        // store the current line in the playerprefs 
        // eg for the first item called in this function ie. nextIntPositionOnLeaderboard = 1 at the beginning of the functio
        // "leaderboardItem"+pos = "leaderboardItem1"
        // lets say lineObject.name = "Cian" and the score achieved was 100
        // Under the key "leaderboadItem1" the value "Cian,100" was set, this will be saved when PlayerPrefs.Save() is called
        PlayerPrefs.SetString("leaderboardItem"+pos, lineObject.name + ","+scoreInt);
        
    }

    public void CheckScore(string name, int score) 
    {
        // create new object
        LeaderboardLine newLine = new LeaderboardLine(name, score);
        //Debug.Log(newLine);
        // create blank list
        List<LeaderboardLine> lines = new List<LeaderboardLine>();
        // Debug.Log(lines.Count);

        bool toBeAddedToList = false; // new name and score not to be added to the list by default
        for (int i = 1; i < 11; i++) // loop from 1 to 10
        {
            string temp = "leaderboardItem" + i; // string to load PlayerPref with
            //Debug.Log(temp);
            string str = PlayerPrefs.GetString(temp); // load leaderboard item 1 to 10
            //Debug.Log(str);
            if (str.Length > 0) // if the value that gets taken from the player prefs is not a blank string
            {
                string[] attributes = str.Split(','); // split using the ',' delimiter
                LeaderboardLine l = new LeaderboardLine(attributes[0], Int32.Parse(attributes[1])); // use the two attributes - name and score - to create a new LeaderboardLine object
                lines.Add(l); // add the line to the list
                if (Int32.Parse(attributes[1]) < score) // if the new objects score is greater
                { // it is to be added to the list
                    toBeAddedToList = true;
                }
                //Debug.Log(lines.Count);
            } else
            {
                // string with length 0 found - therefore a blank line in the list - the new line can be placed here
                toBeAddedToList = true;
            }
        }
        //Debug.Log(toBeAddedToList);
        if (toBeAddedToList) // if the line is to be added to the list
        {
            lines = SortList(lines); // sort the list of lines
            if(lines.Count < 10)// if there is space on the list of 10 lines
            {
                lines.Add(newLine); // add the newLine to the list
                
            } else if (lines.Count == 10)
            {
                // if there is no space on the list of Lines, remove the last line 
                lines.Remove(lines[9]);
                // add new line
                lines.Add(newLine);
            }
        }
        //Debug.Log(lines.Count);
        leaderboardLines = lines;
        //Debug.Log(leaderboardLines.Count);
        PlayerPrefs.DeleteAll();
        LoadList();
        PlayerPrefs.Save();
        UpdateLeaderboard();
    }

    private List<LeaderboardLine> SortList(List<LeaderboardLine> list)
    {
        //sorting the list by its score from best to worse ie. largest to smallest
        for (int i = 0; i < list.Count; i++)
        { // loop from starting line
            for (int j = i + 1; j < list.Count; j++) // loop from the line after the starting line
            {
                // if the current j score is greater that the current i score then we have to swap i and j to move j up the list because it has a better score
                if (list[j].score > list[i].score)
                {
                    // we have to swap i and j
                    LeaderboardLine l = list[j]; // temporarily store j
                    list[j] = list[i]; // put i in the position of j
                    list[i] = l; // put the stored j in the position of i
                }
            }
        }
        return list;
    }

    private void LoadList()
    {
        
        for(int i = 1; i < 11; i++)
        {
            string str = PlayerPrefs.GetString("leaderboardItem" + i); // returns name,score
            if (str.Length > 0)
            {
                string[] attributes = str.Split(',');
                LeaderboardLine l = new LeaderboardLine(attributes[0], Int32.Parse(attributes[1]));
                leaderboardLines.Add(l);
            }

        }

    }

    private void ClearLeaderboards()
    {
        // clears the leaderboard by clearing the playerprefs and the leaderboard lines
        PlayerPrefs.DeleteAll();
        leaderboardLines.Clear();
    }

    public bool CompareScore(int score)
    {
        leaderboardLines = SortList(leaderboardLines); // ensure list is sorted by score
        int numberOfLinesInTheLeaderboard = leaderboardLines.Count; // get the length of the list
        if (score != 0)
        {
            if (numberOfLinesInTheLeaderboard == 10) // if the number on the list equal to 10
            {
                //Compare the item at pos 10
                if (leaderboardLines[9].score < score) // if the score is bigger than the last item on the leaderboard
                {
                    // the score is to be added
                    return true;
                }
                return false; // the score is not to be added as it places in a position less than 10
            }
            else if (numberOfLinesInTheLeaderboard == 0)
            {
                // nothing on the leaderboard yet, the score is to be added
                return true;
            }
            else if (numberOfLinesInTheLeaderboard < 10)
            {
                // there are between 1 and 9 items on the leaderboard.
                return true; // score is to be added
            }
        }
        
        return false;
    }
}
