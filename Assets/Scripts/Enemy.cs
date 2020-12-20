﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Allow these private fields to be seen in the Inspector
    [SerializeField] private float maxHealth;
    [SerializeField] private BarManager HealthBar;
    [SerializeField] private float enemyStrength;
    [SerializeField] private GameObject armourBoost;
    [SerializeField] private GameObject healthBoost;
    
    private float lastAttackingTime = 0f;
    
    GameObject player;
    Rigidbody2D rb;
    Rigidbody2D targetRB;
    EnemyMovement MovementSystem;
    float viewDistance;

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

            
            
            // Can attack every half of a second
            if (lastAttackingTime + 0.5f <= Time.time)
            {
                lastAttackingTime = Time.time; // set the last time that the enemy has attacked to be the time now
                Attack(); // attack the player
            }

            
            

        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet") // if the game object that was collided with has a tag called "Bullet"
        {
            // get the mass of the asteroid that collided with the bullet.
            // Lets call this asteroid the Master Asteroid.
            float bulletStrength = collision.gameObject.GetComponent<Bullet>().GetBulletStrength(); // get the strength of the bullet shot by the player
            TakeDamage(bulletStrength); // upon the collision of the bullet, take damage from the bullet. 
            //Debug.Log(bulletStrength);

            if (HealthBar.GetHealth() <= 0)
            {
                GameManager.SetScore(GameManager.score + 100);
                enemies.Remove(this.transform.parent.gameObject);
                Debug.Log("Enemies Present: " + enemies.Count);
                Destroy(this.gameObject.transform.parent.gameObject); // if the enemy's health runs out, destroy the enemy

                System.Random randomizer = new System.Random();
                int chanceOfBoost = randomizer.Next(100); // get random number between 0 and 99;
                if(chanceOfBoost >= 75) // if the number is greater or equal to 75, spawn a boost
                {
                    SpawnBoost(); // spawn a boost - be it health or armour
                }
                
            }
            //Destroy(collision.gameObject); // destroy bullet - this was moved to bullet script
        }
    }

    private void SpawnBoost()
    {
        System.Random randomizer = new System.Random();
        int option = randomizer.Next(2); // get random number either 0 or 1
        //1 - Armour
        //0 - Health
        GameObject boost; // declare the game object
        switch (option) // switch case where case 0 is health boost and case 1 is armour boost
        {
            case 0:
                boost = Instantiate(healthBoost) as GameObject; // instantiate gameobject
                boost.transform.position = transform.position; // set position
                break;
            case 1:
                boost = Instantiate(armourBoost) as GameObject; // instantiate gameobject
                boost.transform.position = transform.position; // set positon
                break;
        }
        
    }

    private void Attack()
    {
        // Make enemy lunge towards player and lunge back
        //TODO

        //Make player take damage
        Player p = player.GetComponent<Player>(); // get the Player component off the player GameObject so that we can access methods in the Player Script.
        if(p != null) p.TakeDamage(enemyStrength); // Damage the player with the current enemy's strength

      
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


}
