using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private int maxHealth = 100;
    private int currentHealth;
    private GameOverManager gameOverManager; // Reference to GameOverManager

    void Start()
    {
        currentHealth = maxHealth;
        gameOverManager = FindObjectOfType<GameOverManager>(); // Find GameOverManager in the scene
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Healed! Health is: " + currentHealth);
    }

    // Method: Takes Damage from Enemy Attacks
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevents health from going below zero
        Debug.Log("Player took damage! Health is: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method: Get Current Health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Method: Handle Player Death
    private void Die()
    {
        Debug.Log("Player has died!");

        if (gameOverManager != null)
        {
            gameOverManager.TriggerGameOver(); // Calls Game Over screen
        }
    }
}
