using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;       // The enemy prefab to spawn
    public float spawnInterval = 3f;     // How often to spawn (in seconds)

    [Header("Spawn Points")]
    public Transform[] spawnPoints;      // Assign your 6 spawn points in the Inspector

    [Header("Raycast Settings")]
    public float maxRaycastDistance = 100f; // How far downward to look for ground

    private float timer = 0f;

    void Update()
    {
        // Count up each frame
        timer += Time.deltaTime;

        // If time >= interval, spawn an enemy and reset timer
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Safety check: If no spawn points were assigned, log warning and exit
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to EnemySpawner!");
            return;
        }

        // Pick one of the spawn points at random
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawn = spawnPoints[randomIndex];

        // Start spawn position at the chosen spawn point's position
        Vector3 spawnPos = chosenSpawn.position;

        // Raycast straight down to find the floor
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, maxRaycastDistance))
        {
            // If we hit the ground, place the enemy 1 unit above that y-position
            spawnPos.y = hit.point.y + 1f;
        }
        else
        {
            // If no ground was found within maxRaycastDistance, just spawn at the original position
            Debug.LogWarning($"No ground detected below {chosenSpawn.name} within {maxRaycastDistance} units!");
        }

        // Spawn the enemy at the adjusted position (on the ground + offset), with the spawn point's rotation
        Instantiate(enemyPrefab, spawnPos, chosenSpawn.rotation);
    }
}



