using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;   // How close to get before picking new random offset
    public float wanderRadius = 3f;     // Random offset radius around player

    [Header("Randomization Timing")]
    public float targetRefreshTime = 2f; // How often to pick a new random offset

    [Header("Combat Settings")]
    public float attackRange = 2f;      // Distance at which AI can damage the player
    public float damageDelay = 1f;      // Time between each damage tick
    public int damageAmount = 1;

    private PlayerCharacter player;     // Reference to your PlayerCharacter
    private Vector3 currentDestination;
    private float refreshTimer;
    private float damageTimer;

    void Start()
    {
        // Locate the PlayerCharacter in the scene
        player = FindObjectOfType<PlayerCharacter>();

        // Immediately pick a random destination
        PickRandomDestination();
    }

    void Update()
    {
        // If no player found, do nothing
        if (player == null) return;

        // Move toward the current destination
        MoveTowardDestination();

        // Check distance to current destination; refresh if close or time is up
        float distToDest = Vector3.Distance(transform.position, currentDestination);
        refreshTimer += Time.deltaTime;
        if (distToDest < stopDistance || refreshTimer >= targetRefreshTime)
        {
            PickRandomDestination();
        }

        // Check if AI is close enough to deal damage
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distToPlayer <= attackRange)
        {
            // Accumulate timer for damage
            damageTimer += Time.deltaTime;

            // If it's been long enough, damage the player and reset timer
            if (damageTimer >= damageDelay)
            {
                player.Heal(damageAmount);
                damageTimer = 0f;
            }
        }
        else
        {
            // If out of range, reset the damage timer so we wait again
            damageTimer = 0f;
        }
    }

    private void MoveTowardDestination()
    {
        // Face the destination (only on Y-axis if you prefer upright rotation)
        Vector3 direction = (currentDestination - transform.position).normalized;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Move forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void PickRandomDestination()
    {
        // Reset refresh timer
        refreshTimer = 0f;

        // Pick a random point around the player's position
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        currentDestination = player.transform.position + randomOffset;
    }
}
