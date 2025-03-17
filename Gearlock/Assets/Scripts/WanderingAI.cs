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
    private Transform player;      // We'll try to find this by name
    private Vector3 currentDestination;

    private float refreshTimer;

    void Start()
    {
        Debug.Log("WanderingAI: Start() called on " + name);

        // Try to find a GameObject named "Player"
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("WanderingAI: Could not find an object named 'Player'. " +
                             "Ensure you have a GameObject in the scene named exactly 'Player'.");
            return; // no player found; script won't do anything
        }

        // Store the player's Transform
        player = playerObj.transform;
        Debug.Log("WanderingAI: Found the player object: " + player.name);

        // Immediately pick a random destination if the player was found
        PickRandomDestination();
    }

    void Update()
    {
        // If no player assigned, exit early
        if (player == null)
        {
            Debug.LogWarning("WanderingAI: 'player' is null in Update(). Stopping movement.");
            return;
        }

        // Move toward the current destination
        MoveTowardDestination();

        // Check distance to current destination
        float distToDest = Vector3.Distance(transform.position, currentDestination);
        refreshTimer += Time.deltaTime;

        // If we’re close or time is up, choose a new destination
        if (distToDest < stopDistance || refreshTimer >= targetRefreshTime)
        {
            Debug.Log("WanderingAI: Refreshing destination...");
            PickRandomDestination();
        }


    private void MoveTowardDestination()
    {
        // Calculate direction
        Vector3 direction = (currentDestination - transform.position).normalized;
        direction.y = 0f; // keep upright if desired

        // Rotate to face the direction (smoothly)
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
        refreshTimer = 0f;

        // If the player is somehow null, bail out
        if (player == null)
        {
            Debug.LogWarning("WanderingAI: Attempted to pick a destination, but 'player' is null!");
            return;
        }

        // Randomly pick a point around the player within wanderRadius
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);

        currentDestination = player.position + randomOffset;

        Debug.Log("WanderingAI: New random destination is " + currentDestination);
    }
}
