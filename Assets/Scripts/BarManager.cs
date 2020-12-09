using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{

    public Slider slider;


    public void SetHealth(float health)
    {
        slider.value = health;
    }
    public float GetHealth()
    {
        return slider.value;
    }
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void Heal(float extraHealth)
    {
        float currentHealth = slider.value + extraHealth;

        if (currentHealth > slider.maxValue)
        {
            SetHealth(slider.maxValue);
        }
        else
        {
            SetHealth(currentHealth);
        }
        
    }

    public void Damage(float damage)
    {
        float currentHealth = slider.value - damage;

        if (currentHealth < 0)
        {
            SetHealth(0);
        } else
        {
            SetHealth(currentHealth);
        }  

    }

}
