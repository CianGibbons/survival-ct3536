using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Allow these private fields to be seen in the Inspector
    //inspector settings
    [SerializeField] private float maxHealth;
    [SerializeField] private BarManager HealthBar;
    [SerializeField] private float enemyStrength;
    [SerializeField] private GameObject armourBoost;
    [SerializeField] private GameObject healthBoost;
    [SerializeField] private Transform firingPointTransform;
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] private GameObject DeadBloodSplatter;
    [SerializeField] private GameObject InjuredBloodSplatter;
    [SerializeField] private bool CloseRangeEnemy;
    [SerializeField] private AudioClip shootLaserSFX;
    [SerializeField] [Range(0, 1)] private float shootLaserSFXVolume = 1f;
    [SerializeField] private AudioClip enemySplatSFX;
    [SerializeField] [Range(0,1)] private float enemySplatSFXVolume = 1f;
    [SerializeField] private AudioClip meleeAttackSFX;
    [SerializeField] [Range(0, 1)] private float meleeAttackSFXVolume = 1f;

    //class level private variables
    private float lastAttackingTime = 0f;
    private GameObject player;
    private Rigidbody2D rb;
    private Rigidbody2D targetRB;
    private EnemyMovement MovementSystem;
    private float viewDistance;

    
    // private statics
    // keeping a list of all the current enemies alive
    private static List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        HealthBar.SetMaxHealth(maxHealth); // initialise the health of the enemy
        player = GameObject.FindWithTag("Player"); // find the player object using the tag that has been given to this object only
        rb = GetComponent<Rigidbody2D>(); // get the rigidbody of the current enemy 
        targetRB = player.GetComponent<Rigidbody2D>(); // get the rigidbody of the player so we can use it for the rotation
        MovementSystem = transform.parent.gameObject.GetComponent<EnemyMovement>(); // get the movement system so that we can access it in this script
        viewDistance = MovementSystem.viewDistance; // get the view distance from the movement system
        
        enemies.Add(this.transform.parent.gameObject);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (MovementSystem.DistanceToPlayer() <= viewDistance){ // if the Enemy can see the player, rotate the enemy to face the player
            Vector3 targetVector = targetRB.transform.position - transform.position; // get the directional target vector
            float angleToRotate = (Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg) - 90; // get the angle of the rotation in degrees, not radians
            Quaternion q = Quaternion.AngleAxis(angleToRotate, Vector3.forward); //get the quaternion for the rotation using the angle
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5); // rotate using the quaternion. I used the Slerp rotation so that there would be a uniform angular velocity around a fixed rotation axis
        }

        if(MovementSystem.CloseEnoughToAttack())
        {
            // I was implementing where the enemy would lunge at the player and then lunge back to the original position
            // I decided to leave this implementation for now as i might create an animation instead.

            //transform.position + direction * moveSpeed * Time.deltaTime;
            //Vector3 direction = (player.transform.position - transform.position).normalized;
            //Vector3 OriginalPosition = transform.position;
            //transform.position = player.transform.position + direction * 10f * Time.deltaTime; // 10 is the attackSpeed



            //transform.position = OriginalPosition + direction*-1f * 10f * Time.deltaTime;

            if (CloseRangeEnemy)
            {
                // Can attack every half of a second
                if (lastAttackingTime + 0.5f <= Time.time)
                {
                    lastAttackingTime = Time.time; // set the last time that the enemy has attacked to be the time now
                    Attack(); // attack the player
                }
            } else
            {
                // Can attack every  second
                if (lastAttackingTime + 1f <= Time.time)
                {
                    lastAttackingTime = Time.time; // set the last time that the enemy has attacked to be the time now
                    //Attack(); // attack the player

                    ShootAtPlayer();
                }
            }

        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet") // if the game object that was collided with has a tag called "Bullet"
        {
            
            float bulletStrength = collision.gameObject.GetComponent<Bullet>().GetBulletStrength(); // get the strength of the bullet shot by the player
            TakeDamage(bulletStrength); // upon the collision of the bullet, take damage from the bullet. 
            //Debug.Log(bulletStrength);
            GameObject blood1 = Instantiate(InjuredBloodSplatter, transform.position, transform.rotation) as GameObject;
            Destroy(blood1, 1f);

            if (HealthBar.GetHealth() <= 0)
            {
                if (CloseRangeEnemy) { GameManager.SetScore(GameManager.score + 100); }
                else { GameManager.SetScore(GameManager.score + 250); }
                
                enemies.Remove(this.transform.parent.gameObject);
                //Debug.Log("Enemies Present: " + enemies.Count);
                Destroy(this.gameObject.transform.parent.gameObject); // if the enemy's health runs out, destroy the enemy
                // Play the splat sound effect - playing it close to the camera as the audio listener is on the camera and we want this louder
                AudioSource.PlayClipAtPoint(enemySplatSFX, Camera.main.transform.position, enemySplatSFXVolume); // volume goes from 0 to 1
                // play the splat particle effect
                GameObject blood = Instantiate(DeadBloodSplatter) as GameObject;
                blood.transform.position = this.transform.position;
                Destroy(blood,1f);

                System.Random randomizer = new System.Random();
                int chanceOfBoost = randomizer.Next(101); // get random number between 0 and 100;
                if(chanceOfBoost >= 75) // if the number is greater or equal to 75, spawn a boost
                {
                    SpawnBoost(chanceOfBoost); // spawn a boost - be it health or armour
                }
                
            }
        }
    }

    private void SpawnBoost(int chanceOfBoost)
    {
        //chanceOfBoost is always between 75 and 100 inclusive
        
        GameObject boost; // declare the game object
        if(chanceOfBoost <= 88) // less than or equal to 88 is a health boost
        {
            //Debug.Log(chanceOfBoost + "Health Boost Spawned");
            boost = Instantiate(healthBoost) as GameObject; // instantiate gameobject
            boost.transform.position = transform.position; // set position
        } else // greater than 88 is an armour boost
        {
            //Debug.Log(chanceOfBoost + "Armour Boost Spawned");
            boost = Instantiate(armourBoost) as GameObject; // instantiate gameobject
            boost.transform.position = transform.position; // set positon
        }
        
        
    }

    private void Attack()
    {
        // Make enemy lunge towards player and lunge back
        //TODO

        //Make player take damage
        Player p = player.GetComponent<Player>(); // get the Player component off the player GameObject so that we can access methods in the Player Script.
        if(p != null) p.TakeDamage(enemyStrength); // Damage the player with the current enemy's strength
        AudioSource.PlayClipAtPoint(meleeAttackSFX, firingPointTransform.position, meleeAttackSFXVolume); // volume is from 0 to 1
        GameObject blood1 = Instantiate(InjuredBloodSplatter, transform.position, transform.rotation) as GameObject;
        Destroy(blood1, 1f);

    }

    public void TakeDamage(float damage)
    {
        HealthBar.Damage(damage); // take damage to the current enemy's health bar
    }

    
    public static void DestroyAllEnemies()
    {
        //destroying all gameobjects within enemies
        foreach(GameObject go in enemies)
        {
            Destroy(go);
        }
        // clearing the list of gameobjects as all of these gameobjects have been destroyed
        enemies.Clear();
    }

    public static List<GameObject> GetList() {
        return enemies;
    }

    public void ShootAtPlayer()
    {
        GameObject bullet = Instantiate(enemyBullet, firingPointTransform.position, firingPointTransform.rotation);
        // this method receives the AudioClip, the Vector3 position of where to play audio and finally the volume of the sound effect.
        AudioSource.PlayClipAtPoint(shootLaserSFX, firingPointTransform.position, shootLaserSFXVolume); // volume is from 0 to 1
    }

}
