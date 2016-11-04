using UnityEngine;
using System.Collections;

public class Health
{
    private int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }

    public Health(int max)
    {
        maxHealth = max;
        currentHealth = maxHealth;
    }

    public bool TakeHit()
    {
        currentHealth -= 1;
        return (currentHealth <= 0);
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
