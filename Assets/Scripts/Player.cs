using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float maxHealth;
    public float maxArmor;
    public Rigidbody2D rb;
    public Camera gameCamera;
    private Vector2 move;
    private Vector2 mousePosition;
    public Transform firingPoint;
    public GameObject bulletPrefab;
    public BarManager HealthBar;
    public BarManager ArmorBar;
    

    private void Start()
    {
        //initialize the health and armor bars
        HealthBar.SetMaxHealth(maxHealth);
        ArmorBar.SetMaxHealth(maxArmor);
    }

    // Update is called once per frame
    private void Update()
    {
        // Using the predefined Axis for movement
        move.y = Input.GetAxisRaw("Vertical");
        move.x = Input.GetAxisRaw("Horizontal");

        //getting the position of the mouse in relation to the world rather than the screen
        mousePosition = gameCamera.ScreenToWorldPoint(Input.mousePosition);

        // if mouse1 is clicked down 
        if(Input.GetButtonDown("Fire1")) // Fire1 is a preset made by Unity for mouse1
        {
            FireBullet(); // fire the bullet
        }

        // Used for testing

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    HealthBar.Damage();
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    HealthBar.Heal(10);
        //}
    }

    private void FixedUpdate()
    {
        // move object to current position plus move Vector at moveSpeed speed
        // multiply by Time to ensure that the move speed wont be changed by the amount of times FixedUpdate is called
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime * moveSpeed);
        
        //rigidbody.MovePosition(rigidbody.position + move * moveSpeed); - this was causing player to Jump about the place due to the amount of times fixed update was called.

        //Subtracting the Vector2's of the mouse position and the player position to get the target direction
        Vector2 targetDirection = mousePosition - rb.position;
        //Calculating the angle for the rotation using atan method, this returns in radians so multiplied it by Rad2Deg which is a constant
        // had to subtract 90 degrees as the Player sprite needed to be rotated.
        float angleToTarget = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        //Apply the rotation
        rb.rotation = angleToTarget;
    }

    private void FireBullet()
    {
        // instantiating a bullet at the firing point position (top of gun) with the same rotation as the firing point
        GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
    }

    public void TakeDamage(float enemyStrength)
    {
        float damageToBeTaken = enemyStrength; // set the damage to be taken by the player to be the strength of the enemy
        if (ArmorBar.hasRemainingHealth()) // if there is still health left in the armor bar (i.e. there is still armor left on the player)
        {
            float ArmorLeft = ArmorBar.GetHealth(); // get the amount of armor that is left 
            if(enemyStrength >= ArmorLeft) // if the enemy strength is big enough to destroy the armor
            {
                damageToBeTaken = damageToBeTaken - ArmorLeft; // take the amount of damage that the armor sustains off the damage to be taken
                ArmorBar.Damage(ArmorLeft); // take the last sustainable amount of damage off the armor bar
            } else // if the armor can take all of the damage given by the enemy 
            {
                ArmorBar.Damage(damageToBeTaken); // take the damage from the enemy and decrement the armor bar
                damageToBeTaken = 0; // set the damage left to be taken to be 0 to ensure no more damage is taken
            }
        }
        HealthBar.Damage(damageToBeTaken); // take any remaining damage on the health bar (at this stage the armor has been depleted)
    }
}
