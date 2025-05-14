using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;      // Seconds between spawn attempts

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Raycast Settings")]
    public float maxRaycastDistance = 100f;

    [Header("Concurrent-Limit")]
    public int maxConcurrent = 10;        // Max on the field at once

    private readonly List<GameObject> activeEnemies = new();  // tracks living enemies
    private float timer = 0f;

    void Update()
    {
        // Clean up any entries that were destroyed
        activeEnemies.RemoveAll(e => e == null);

        // If we already have the maximum number alive, skip this frame
        if (activeEnemies.Count >= maxConcurrent) return;

        // Otherwise count up and try to spawn when ready
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No spawn points assigned!");
            return;
        }

        // Pick a random spawn point
        int idx = Random.Range(0, spawnPoints.Length);
        Transform sp = spawnPoints[idx];

        // Find ground below the spawn point
        Vector3 pos = sp.position;
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, maxRaycastDistance))
            pos.y = hit.point.y + 1f;   // 1-unit offset for capsule pivot
        else
            Debug.LogWarning($"EnemySpawner: No ground detected below {sp.name}!");

        // Instantiate and record the new enemy
        GameObject newEnemy = Instantiate(enemyPrefab, pos, sp.rotation);
        activeEnemies.Add(newEnemy);
    }
}
