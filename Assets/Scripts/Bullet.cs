using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    //public GameObject explosion;

    public float bulletStrength;

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody of the bullet and use it to add force to the bullet (to get it to move)
        // add force to the using the up vector with the bullet speed, use Impulse force to apply instant force impluse to the rigidbody
        GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed, ForceMode2D.Impulse);
        //Destroy(gameObject,10f); // destroy bullet after 10seconds if it hasnt collided with anything
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // instantiate an explosion at the bullets current position, with the natural rotation of the explosion (it doesnt really matter what rotation the explosion has)
        // i had explosion working - took it out as didnt want to use external explosion asset
        
        //GameObject explosion1 = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject); // destroy the bullet
        //Destroy(explosion1, 0.7f);// destroy the explosion object after 1 seconda


    }

    public float GetBulletStrength() // getter method for the strength of the bullet
    {
        return bulletStrength;
    }

    private void OnBecameInvisible() // when the bullets go off the screen
    {
        Destroy(gameObject); // destroy the bullet
    }
}
