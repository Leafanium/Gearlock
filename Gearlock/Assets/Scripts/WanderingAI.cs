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

    [Header("References")]
    private Transform player;
    private Vector3 currentDestination;
    private Animator animator;

    private float refreshTimer;
    private bool isAlive = true;

    void Start()
    {
        Debug.Log("WanderingAI: Start() called on " + name);

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("WanderingAI: Could not find an object named 'Player'. Ensure you have a GameObject in the scene named exactly 'Player'.");
            return;
        }

        player = playerObj.transform;
        animator = GetComponentInChildren<Animator>(); // Ensure Animator is on the Mesh child

        if (animator == null)
        {
            Debug.LogError("WanderingAI: No Animator component found in child objects of " + name);
        }

        PickRandomDestination();
        animator.SetBool("isRunning", true); // Start running animation
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("WanderingAI: 'player' is null in Update(). Stopping movement.");
            return;
        }

        MoveTowardDestination();

        float distToDest = Vector3.Distance(transform.position, currentDestination);
        refreshTimer += Time.deltaTime;

        if (distToDest < stopDistance || refreshTimer >= targetRefreshTime)
        {
            Debug.Log("WanderingAI: Refreshing destination...");
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

        Debug.Log("WanderingAI: New random destination is " + currentDestination);
    }

    public void SetAlive(bool alive)
    {
        isAlive = alive;

        if (!isAlive)
        {
            Debug.Log(name + " has died.");
            animator.SetBool("isRunning", false); // Stop running
            animator.SetTrigger("Die"); // Play death animation
        }
    }
}
