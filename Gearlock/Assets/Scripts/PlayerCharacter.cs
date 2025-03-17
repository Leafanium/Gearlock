using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isInvincible = false; // Invincibility flag after taking damage
    public float invincibilityDuration = 1.5f; // Time before player can be damaged again

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.3f;
    private Rigidbody rb;

    [Header("References")]
    private GameOverManager gameOverManager;
    private Animator animator; // Optional animator reference

    void Start()
    {
        currentHealth = maxHealth;
        gameOverManager = FindObjectOfType<GameOverManager>(); // Find GameOverManager in the scene
        rb = GetComponent<Rigidbody>(); // Ensure Rigidbody is attached
        animator = GetComponent<Animator>(); // Ensure Animator is attached if available
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Healed! Health is: " + currentHealth);
        UpdateHealthUI();
    }

    // Method: Takes Damage from Enemy Attacks
    public void TakeDamage(int amount, Vector3 damageSource)
    {
        if (isInvincible) return; // If invincible, ignore damage

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevents health from going below zero
        Debug.Log("Player took damage! Health is: " + currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hurt"); // Trigger damage animation if available
        }

        StartCoroutine(ApplyKnockback(damageSource));
        StartCoroutine(InvincibilityFrames());

        UpdateHealthUI();

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

        if (animator != null)
        {
            animator.SetTrigger("Die"); // Trigger death animation if available
        }

        if (gameOverManager != null)
        {
            gameOverManager.TriggerGameOver(); // Calls Game Over screen
        }
    }

    // Method: Apply Knockback Effect
    private IEnumerator ApplyKnockback(Vector3 damageSource)
    {
        if (rb == null) yield break;

        Vector3 knockbackDirection = (transform.position - damageSource).normalized;
        knockbackDirection.y = 0f; // Keep knockback horizontal
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector3.zero; // Stop movement after knockback duration
    }

    // Method: Enable Invincibility for a brief duration
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    // Optional: Update Health UI (If using UI elements)
    private void UpdateHealthUI()
    {
        // Call a UI manager method here if you have one
        // Example: UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }
}
