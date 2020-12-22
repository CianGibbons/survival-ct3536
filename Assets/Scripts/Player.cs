using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // inspector level settings
    [SerializeField] private GameObject InjuredBloodSplatter;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject level1BulletPrefab;
    [SerializeField] private GameObject level2BulletPrefab;
    [SerializeField] private GameObject level3BulletPrefab;
    [SerializeField] private GameObject PlayerBarsPrefab;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioClip shootLaserSFX;
    [SerializeField] [Range(0, 1)] private float shootLaserSFXVolume = 1f;

    // class level public variables
    public float moveSpeed;
    public float maxHealth;
    public float maxArmour;

    // class level public statics
    public static GameObject PlayerBars;

    // class level private variables 
    private BarManager HealthBar;
    private BarManager ArmourBar;
    private GameObject bulletPrefab;
    private int currentWeaponLevel;
    private Vector2 move;
    private Vector2 mousePosition;

    private void Start()
    {
        //initialize the health and armor bars
        PlayerBars = Instantiate(this.PlayerBarsPrefab) as GameObject;
        HealthBar = PlayerBars.transform.GetChild(0).gameObject.GetComponent<BarManager>();
        ArmourBar = PlayerBars.transform.GetChild(2).gameObject.GetComponent<BarManager>();
        currentWeaponLevel = 1;
        HealthBar.SetMaxHealth(maxHealth);
        ArmourBar.SetMaxHealth(maxArmour);
        bulletPrefab = level1BulletPrefab;
    }

    // Update is called once per frame
    private void Update()
    {
        // Using the predefined Axis for movement
        move.y = Input.GetAxisRaw("Vertical");
        move.x = Input.GetAxisRaw("Horizontal");

        //getting the position of the mouse in relation to the world rather than the screen
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
        AudioSource.PlayClipAtPoint(shootLaserSFX, firingPoint.position, shootLaserSFXVolume); // volume is from 0 to 1
    }

    public void TakeDamage(float enemyStrength)
    {
        float damageToBeTaken = enemyStrength; // set the damage to be taken by the player to be the strength of the enemy
        if (ArmourBar.hasRemainingHealth()) // if there is still health left in the armor bar (i.e. there is still armor left on the player)
        {
            float ArmourLeft = ArmourBar.GetHealth(); // get the amount of armor that is left 
            if(enemyStrength >= ArmourLeft) // if the enemy strength is big enough to destroy the armor
            {
                damageToBeTaken = damageToBeTaken - ArmourLeft; // take the amount of damage that the armor sustains off the damage to be taken
                ArmourBar.Damage(ArmourLeft); // take the last sustainable amount of damage off the armor bar
            } else // if the armor can take all of the damage given by the enemy 
            {
                ArmourBar.Damage(damageToBeTaken); // take the damage from the enemy and decrement the armor bar
                damageToBeTaken = 0; // set the damage left to be taken to be 0 to ensure no more damage is taken
            }
        }
        HealthBar.Damage(damageToBeTaken); // take any remaining damage on the health bar (at this stage the armor has been depleted)
       
        //Play the Visual Damage Taken Particle System
        GameObject blood1 = Instantiate(InjuredBloodSplatter, transform.position, transform.rotation) as GameObject;
        Destroy(blood1, 1f);


        if (HealthBar.GetHealth() <= 0)
        {
            //Dead - call GameOver()
            GameManager.GameOver();
        }
    }

    public void HealArmour(int armour)
    {
        ArmourBar.Heal(armour);
    }

    public void Heal(int health)
    {
        HealthBar.Heal(health);
    }

    public void UpgradeWeapon()
    {
        currentWeaponLevel++;
        switch(currentWeaponLevel)
        {
            case 1: bulletPrefab = level1BulletPrefab; break;
            case 2: bulletPrefab = level2BulletPrefab; break;
            case 3: bulletPrefab = level3BulletPrefab; break;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EnemyBullet") // if the game object that was collided with has a tag called "EnemyBullet"
        {
            float bulletStrength = collision.gameObject.GetComponent<Bullet>().GetBulletStrength(); // get the strength of the bullet shot by the enemy
            TakeDamage(bulletStrength);
        }
    }
}
