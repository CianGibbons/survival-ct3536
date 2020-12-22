using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //inspector settings
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletStrength;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip explosionSFX;
    [SerializeField] [Range(0, 1)] private float explosionSFXVolume = 1f;

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

        
        if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "EnemyBullet") // if the game object that was collided with was a bullet
        {
            // play the explosion sound effect that i created
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position, explosionSFXVolume); // volume is from 0 to 1
            // play the explosion particle effect i created
            GameObject explosion1 = Instantiate(explosion, transform.position, Quaternion.identity); // instantiate an explosion at the position of the bullet -  "no rotation" - the object is perfectly aligned with the world ie its natural rotation
            Destroy(explosion1, 0.5f);// destroy the explosion object after 0.5 seconds
        }
        Destroy(gameObject); // destroy the bullet

    }

    public float GetBulletStrength() // getter method for the strength of the bullet
    {
        return bulletStrength;
    }

    public float GetBulletSpeed() // getter method for the speed of the bullet
    {
        return bulletStrength;
    }

    private void OnBecameInvisible() // when the bullets go off the screen
    {
        Destroy(gameObject); // destroy the bullet
    }
}
