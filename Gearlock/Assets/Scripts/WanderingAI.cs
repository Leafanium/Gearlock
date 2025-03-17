using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;
    public float wanderRadius = 3f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f; // Distance at which the AI will start attacking
    public int damage = 10; // Amount of damage per attack
    public float attackCooldown = 1.5f; // Time between attacks

    [Header("Randomization Timing")]
    public float targetRefreshTime = 2f;

    [Header("References")]
    private Transform player;
    private PlayerCharacter playerCharacter;
    private Vector3 currentDestination;

    private float refreshTimer;
    private bool isAlive = true;
    private bool canAttack = true; // Attack cooldown control

    void Start()
    {
        Debug.Log("WanderingAI: Start() called on " + name);

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("WanderingAI: Could not find an object named 'Player'. " +
                             "Ensure you have a GameObject in the scene named exactly 'Player'.");
            return;
        }

        player = playerObj.transform;
        playerCharacter = player.GetComponent<PlayerCharacter>();

        Debug.Log("WanderingAI: Found the player object: " + player.name);

        PickRandomDestination();
    }

    void Update()
    {
        if (!isAlive) return;

        if (player == null)
        {
            Debug.LogWarning("WanderingAI: 'player' is null in Update(). Stopping movement.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (canAttack)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            MoveTowardDestination();
        }

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
        if (!isAlive) return;

        refreshTimer = 0f;

        if (player == null)
        {
            Debug.LogWarning("WanderingAI: Attempted to pick a destination, but 'player' is null!");
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);

        currentDestination = player.position + randomOffset;
    }

    IEnumerator AttackPlayer()
    {
        canAttack = false;

        if (playerCharacter != null)
        {
            playerCharacter.TakeDamage(damage);
            Debug.Log("Zombie attacked! Player health is now: " + playerCharacter.GetCurrentHealth());
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void SetAlive(bool alive)
    {
        isAlive = alive;

        if (!isAlive)
        {
            Debug.Log(name + " has died.");
        }
    }
}
