using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void OnHit(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} has died.");

        // Optional: Trigger animation, particle, sound
        // Animator anim = GetComponent<Animator>();
        // if (anim != null) anim.SetTrigger("Die");

        Destroy(gameObject);
    }
}
