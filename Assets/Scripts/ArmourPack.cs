using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPack : MonoBehaviour
{
    public int armourValue = 10;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collided");
            collision.gameObject.GetComponent<Player>().HealArmour(armourValue);
        }
    }
}
