using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public float maxHealth;
    public Rigidbody2D rb;
    public Rigidbody2D targetRB;
    public BarManager HealthBar;
    public float enemyStrength;
    

   
    // Start is called before the first frame update
    void Start()
    {
        HealthBar.SetMaxHealth(maxHealth);
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetVector = targetRB.transform.position - transform.position;
        float angleToRotate = (Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg)-90;
        Quaternion q = Quaternion.AngleAxis(angleToRotate, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet") // if the game object that was collided with has a tag called "Bullet"
        {
            // get the mass of the asteroid that collided with the bullet.
            // Lets call this asteroid the Master Asteroid.
            float bulletStrength = collision.gameObject.GetComponent<Bullet>().GetBulletStrength();
            HealthBar.Damage(bulletStrength);
            Debug.Log(bulletStrength);
            if(HealthBar.GetHealth() <= 0) Destroy(this.gameObject.transform.parent.gameObject);



            //Destroy(collision.gameObject); // destroy bullet
        }

        
    }
}
