using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int healthValue = 10;



    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            
            collider.gameObject.GetComponent<Player>().Heal(healthValue);
            Destroy(this.gameObject);
        }
    }
}
