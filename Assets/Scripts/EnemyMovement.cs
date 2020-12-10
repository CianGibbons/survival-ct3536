using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float viewDistance;
    private List<Vector3> pathToFollow;
    private int indexOfCurrentSquareOnList;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackingRange;
    [SerializeField] private bool CloseRangeEnemy;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player"); // get the player gameobject for ease of access
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
        // note to self: later i might implement long range players, here i would set the path to the player to be null so that my enemy will stop moving towards the player
        if (DistanceToPlayer() <= attackingRange) return true; 

        return false;
    }

    private void GetTarget()
    {

        
        
        //Debug.Log("MoveToPlayer() Called");
        indexOfCurrentSquareOnList = 0; // start at the first square in the list
        pathToFollow = AStar.instance.FindPath(transform.position, player.transform.position); // get the list of positional vectors to move to to get to the player

        if(pathToFollow != null && pathToFollow.Count > 1) // no need to move from where the enemy is starting to the first square as the first square is where the enemy is standing!!
        {
            pathToFollow.RemoveAt(0);
        }

        //THIS IS FOR TESTING PURPOSES AND IS TO BE COMMENTED OUT WHEN TESTING A* IS FINISHED
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


            //?? MIGHT NEED TO ASK A QUESTION HERE??//
            //if (Vector3.Distance(transform.position, target) > 1f)
            if (DistanceToPlayer() > 2.5f) // if the distance to the player is greater than 2.5f 
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
        }
    }
}
