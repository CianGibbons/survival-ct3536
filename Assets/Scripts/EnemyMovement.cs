﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // inspector settings
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackingRange;
    [SerializeField] private Transform firingPointTransform;
    [SerializeField] private Rigidbody2D enemyRB;

    //class level private variables
    private List<Vector3> pathToFollow;
    private int indexOfCurrentSquareOnList;
    private GameObject player;

    //class level public variables
    public float viewDistance;
    public bool CloseRangeEnemy;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player"); // get the player gameobject for ease of access
    }

    
    //Using Raycasting i check if the enemy has the player in sight
    private bool CheckIfPlayerIsInSight()
    {
        Vector3 directionToPlayer = player.transform.position - firingPointTransform.position; // getting the directional vector to the player
        RaycastHit2D rayCastHit = Physics2D.Raycast(firingPointTransform.position, directionToPlayer); // casting the ray from the ray from the firing points position to the player
        Debug.DrawRay(firingPointTransform.position, directionToPlayer); // drawing a gizmos ray for debugging reasons
        if (rayCastHit.collider != null) // if the ray collides with something
        {
            if(rayCastHit.collider.gameObject.tag == "Player")// if the player is in sight, return true
            {
                return true;
            }
           // bullets are ignored using the ignore raycast layer
        }
        return false; // the collision is not the player, return false
    }

    // Update is called once per frame
    void Update()
    {
        if (DistanceToPlayer() <= viewDistance) // once the enemy can see the player, it will give chase
        {
            //print(gameObject.name + " will chase the player");
            GetTarget(); // calculate a path to the player if the player is within the view distance of the enemy
        }
        MoveToPlayer(); // follow the calculated path to the player
    }

    public float DistanceToPlayer() // get the distance to the player
    {
        return Vector3.Distance(player.transform.position, transform.position); // using the players position and the enemy's position we can calculate the distance between the two using Vector3.Distance
    }

    public bool CloseEnoughToAttack() // this is to test that the enemy is close enough to attack the player
    {
        if (DistanceToPlayer() <= attackingRange) return true; // if the distance from the current enemy to the player is less than or equal to the attacking range return true as the enemy is close enough to attack 
        return false;// the enemy is not close enough to attack, return false
    }

    private void GetTarget() // uses A* to get the path to the player
    {   
        //Debug.Log("MoveToPlayer() Called");
        indexOfCurrentSquareOnList = 0; // start at the first square in the list
        pathToFollow = AStar.instance.FindPath(transform.position, player.transform.position); // get the list of positional vectors to move to, to get to the player

        if(pathToFollow != null && pathToFollow.Count > 1) // no need to move from where the enemy is starting to the first square as the first square is where the enemy is standing!!
        {
            pathToFollow.RemoveAt(0); // removing the first square
        }

        //THIS IS FOR TESTING PURPOSES
        //Used for Drawing Gizmos to help debug the A*
        for (int i = 0; i < pathToFollow.Count - 1; i++) // loop through each square on the path that the enemy is to follow
        {
            //Draw a gizmos line from the current location to the next location -  this is why the loop ends at Count -1 because otherwise we would have a null pointer
                Debug.DrawLine((new Vector3(pathToFollow[i].x, pathToFollow[i].y) + Vector3.one), (new Vector3(pathToFollow[i + 1].x, pathToFollow[i + 1].y) + Vector3.one));
        }


    }

    private void MoveToPlayer() // this handles the movement of the enemy to the player by following the path that has been calculated
    {
        if (pathToFollow != null) // if there exists a path to follow
        {
            Vector3 target = pathToFollow[indexOfCurrentSquareOnList]; // set the current target for the enemy to move towards
            //Debug.Log(DistanceToPlayer());
            if (CloseRangeEnemy) // if the enemy is a close range enemy - they have to travel close to the enemy to attack
            {
                if (DistanceToPlayer() > 2f) // if the distance to the player is greater than 2f 
                {
                    Vector3 direction = (target - transform.position).normalized; // get the normalized directional vector to the target
                    transform.position = transform.position + direction * moveSpeed * Time.deltaTime; // move towards the target at moveSpeed speed
                }
                else
                {
                    indexOfCurrentSquareOnList++; // increment 
                    if (indexOfCurrentSquareOnList >= pathToFollow.Count) // if the index is out of range
                    {
                        // set the path to follow to null
                        pathToFollow = null;
                    }
                }
            } else
            {
                
                if (DistanceToPlayer() > 15f || !CheckIfPlayerIsInSight()) // if the distance to the player is greater than 15f or the player isnt in direct line of sight to shoot at
                {// keep chasing the player
                    Vector3 direction = (target - transform.position).normalized; // get the normalized directional vector to the target
                    transform.position = transform.position + direction * moveSpeed * Time.deltaTime; // move towards the target at moveSpeed speed
                }
                else
                {
                    indexOfCurrentSquareOnList++; // increment 
                    if (indexOfCurrentSquareOnList >= pathToFollow.Count) // if the index is out of range
                    {
                        // set the path to follow to null
                        pathToFollow = null;
                    }
                }
            }
            
        }
    }
}
