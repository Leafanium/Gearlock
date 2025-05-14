using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;
    public float wanderRadius = 3f;

    [Header("Randomization Timing")]
    public float targetRefreshTime = 2f;

    [Header("Combat Settings")]
    public int damageAmount = 10;
    public float damageCooldown = 1.5f;

    [Header("References")]
    private Transform player;
    private Vector3 currentDestination;
    private Animator animator;
    private float refreshTimer;
    private bool isAlive = true;
    private float lastDamageTime;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player"); // Ensure the player is tagged correctly
        if (playerObj == null)
        {
            Debug.LogWarning("WanderingAI: Could not find 'Player'. Ensure the player GameObject has the 'Player' tag.");
            return;
        }

        player = playerObj.transform;
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("WanderingAI: No Animator found in child objects of " + name);
        }

        PickRandomDestination();
        animator.SetBool("isRunning", true);
    }

    void Update()
    {
        // ---------- DEBUG CHECK ----------
        if (player == null)
        {
            Debug.LogWarning($"{name}: no player – AI halted");
            return;                      // we’re done if player is missing
        }
        Debug.Log($"{name}: moving toward {currentDestination}");
        // ----------------------------------
        if (!isAlive || player == null)
            return;

        MoveTowardDestination();

        float distToDest = Vector3.Distance(transform.position, currentDestination);
        refreshTimer += Time.deltaTime;

        if (distToDest < stopDistance || refreshTimer >= targetRefreshTime)
        {
            PickRandomDestination();
        }
    }

    private void MoveTowardDestination()
    {
        if (!isAlive) return;

        Vector3 direction = (currentDestination - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void PickRandomDestination()
    {
        if (!isAlive || player == null)
            return;

        refreshTimer = 0f;

        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);

        currentDestination = player.position + randomOffset;
    }

    public void SetAlive(bool alive)
    {
        isAlive = alive;

        if (!isAlive)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Die");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DamagePlayer(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                DamagePlayer(other.gameObject);
                lastDamageTime = Time.time;
            }
        }
    }

    private void DamagePlayer(GameObject playerObj)
    {
        PlayerCharacter playerCharacter = playerObj.GetComponent<PlayerCharacter>();

        if (playerCharacter != null)
        {
            playerCharacter.TakeDamage(damageAmount, transform.position);
            Debug.Log("WanderingAI: Damaged player for " + damageAmount);
        }
        else
        {
            Debug.LogError("WanderingAI: No PlayerCharacter component found on Player!");
        }
    }
}
