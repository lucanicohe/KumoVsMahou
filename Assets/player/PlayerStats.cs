using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Limits")]
    public int maxHealth = 320;
    public int maxMana = 320;

    [Header("Current Stats")]
    public int currentHealth;
    public int currentMana;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    public bool HasEnoughMana(int amount) => currentMana >= amount;

    public void ConsumeMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        Debug.Log($"Mana consumed: {amount} | Remaining: {currentMana}");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Took damage: {amount} | Remaining: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}