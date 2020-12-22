using UnityEngine;
using System.Collections;
public class ArmourPack : MonoBehaviour
{
    //inspector settings
    [SerializeField] private int armourValue = 10;

    // class level private variables
    private bool StartedExpiring = false;
    private float timeToLive = 10f;
    private float timeToFadeOut = 5f;
    private float timeToScaleDone = 6f;
    private float timeSpawned;

    private void Start()
    {
        // initialized the time spawned
        timeSpawned = Time.time;
        //Debug.Log(timeSpawned);
    }
    private void Update()
    {
        if (Time.time > timeSpawned + timeToLive && !StartedExpiring)// ensures than after timeToLive seconds (10) the armour pack will start to expire and that this method will only be called once
        {
            StartedExpiring = true;
            StartCoroutine(StartExpiring(transform.GetComponent<SpriteRenderer>())); // passed in the sprite renderer so i can adjust the alpha value
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // ensuring the player can pickup the armour boost
        if (collider.gameObject.tag == "Player")
        {
            // healing the player's armour by the value of the armour pack
            collider.gameObject.GetComponent<Player>().HealArmour(armourValue);
            Destroy(this.gameObject); // destroying the object as it has now been used
        }
    }


    IEnumerator StartExpiring(SpriteRenderer sprite)
    {
        //Debug.Log("Starting to fade out");
        Color SpriteColor = sprite.color; // storing the sprite's color so that we can manipulate it
        //initializing the change
        float change = 0;
        // initializing the rate of scaling
        float scalingRate = 1 / timeToScaleDone;
        while (SpriteColor.a > 0f) // while the object isnt yet transparent
        {
            //Debug.Log("Fading");
            SpriteColor.a -= Time.deltaTime / timeToFadeOut; // decreasing the alpha value at a rate in which it will reach 0 after timeToFadeOut seconds
            change += Time.deltaTime * scalingRate; // making the change take the scaling rate and the time into accounnt

            // changing the scale of the armour pack from transform.localScale, to Vector3.zero ie. {0,0,0}
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, change);
            if (SpriteColor.a < 0) // if the last subtraction made the a value less than 0 set it to 0
            {
                SpriteColor.a = 0;
            }
            // set the color to the transparent version
            sprite.color = SpriteColor;
            yield return null;
        }
        // ensuring the color is set to the transparent version
        sprite.color = SpriteColor;
        // destroying the gameobject as it has expired
        Destroy(this.gameObject);
        yield break;
    }

    
}
