using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{

    [SerializeField] private Slider slider; // The slider for the current bar


    public void SetHealth(float health) // set the slider value for the current bar
    {
        slider.value = health;
    }
    public float GetHealth() // get the slider value for the current bar
    {
        return slider.value;
    }
    public void SetMaxHealth(float health) // set the max health for the slider and initialise the bar to be at maximum health
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void Heal(float extraHealth) // heal the bar by a certain amount of Health Points
    {
        float currentHealth = slider.value + extraHealth;

        if (currentHealth > slider.maxValue) // ensure that the heal doesnt make the value exceed the maximum value
        {
            SetHealth(slider.maxValue);
        }
        else
        {
            SetHealth(currentHealth);
        }
        
    }

    public void Damage(float damage) // damage the health bar by decreasing the slider by the damage amount
    {
        float currentHealth = slider.value - damage;

        if (currentHealth < 0) // ensure that the bar doesnt go below the minimum
        {
            SetHealth(0);
        } else
        {
            SetHealth(currentHealth); // set the health of the bar after receiving the damage
        }  

    }

    public bool hasRemainingHealth() // check if the bar still has health remaining
    {
        if(slider.value > 0) // if there is still health in the bar
        {
            return true; // TRUE means there is health in the bar
        }
        return false; // FALSE means there is no more health in the bar
    }

  

}
