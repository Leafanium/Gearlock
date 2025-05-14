using System.Collections;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.3f;
    private Rigidbody rb;

    [Header("References")]
    private UIManager uiManager;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        UpdateHealthUI();
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    public void TakeDamage(int amount, Vector3 damageSource)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        StartCoroutine(ApplyKnockback(damageSource));
        StartCoroutine(InvincibilityFrames());
        UpdateHealthUI();

    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private IEnumerator ApplyKnockback(Vector3 damageSource) //We wanted bouncy physics
    {
        if (rb == null) yield break;

        Vector3 knockbackDirection = (transform.position - damageSource).normalized;
        knockbackDirection.y = 0f;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector3.zero;
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void UpdateHealthUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth);
        }
    }
}
