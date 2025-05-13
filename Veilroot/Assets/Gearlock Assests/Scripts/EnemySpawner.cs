using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;     // Their spawnrate

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Raycast Settings")]
    public float maxRaycastDistance = 100f;

    private float timer = 0f;

    void Update()
    {
        
        timer += Time.deltaTime;

      
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Lil Safety check
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to EnemySpawner!");
            return;
        }

        
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawn = spawnPoints[randomIndex];

        
        Vector3 spawnPos = chosenSpawn.position;

        
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, maxRaycastDistance))
        {
            
            spawnPos.y = hit.point.y + 1f;
        }
        else
        {
            // Helped us debug
            Debug.LogWarning($"No ground detected below {chosenSpawn.name} within {maxRaycastDistance} units!");
        }

        // Spawn the robot
        Instantiate(enemyPrefab, spawnPos, chosenSpawn.rotation);
    }
}



