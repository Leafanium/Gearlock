using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float downThreshold = 30f;

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int enemyCount = 15;

    [Header("Spawn Points")]
    public Transform[] wallSpots;
    public Transform[] fallbackGroundSpots;

    [Header("Climb Targets")]
    public Transform[] buildingTopCorners;

    [Header("Player Reference")]
    public Transform player;

    void Start()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.player = player;
                ai.climbTargets = buildingTopCorners;
            }
        }
    }

    Transform GetRandomSpawnPoint()
    {
        if (wallSpots.Length > 0 && Random.value > 0.3f)
            return wallSpots[Random.Range(0, wallSpots.Length)];
        else
            return fallbackGroundSpots[Random.Range(0, fallbackGroundSpots.Length)];
    }
}
