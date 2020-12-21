using UnityEngine;
using System.Collections;
public class ArmourPack : MonoBehaviour
{

    public int armourValue = 10;
    private bool StartedExpiring = false;
    private float timeToLive = 10f;
    private float timeToFadeOut = 5f;
    private float timeToScaleDone = 6f;

    private float timeSpawned;

    private void Start()
    {
        timeSpawned = Time.time;
        //Debug.Log(timeSpawned);
    }
    private void Update()
    {
        if (Time.time > timeSpawned + timeToLive && !StartedExpiring)
        {
            StartedExpiring = true;
            StartCoroutine(StartExpiring(transform.GetComponent<SpriteRenderer>()));
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            
            collider.gameObject.GetComponent<Player>().HealArmour(armourValue);
            Destroy(this.gameObject);
        }
    }


    IEnumerator StartExpiring(SpriteRenderer sprite)
    {

        
        //Debug.Log("Starting to fade out");
        Color SpriteColor = sprite.color;
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
        sprite.color = SpriteColor;
        Destroy(this.gameObject);
        yield break;
    }

    
}
