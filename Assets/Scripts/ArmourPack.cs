using UnityEngine;

public class ArmourPack : MonoBehaviour
{

    public int armourValue = 10;


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            
            collider.gameObject.GetComponent<Player>().HealArmour(armourValue);
            Destroy(this.gameObject);
        }
    }
}
