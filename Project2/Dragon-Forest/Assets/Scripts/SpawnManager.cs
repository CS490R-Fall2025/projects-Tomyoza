using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [Header("Settings")]
    public GameObject enemyPrefab; 
    public Transform centerPoint;
    public int numberOfEnemies = 5;
    public float spawnRadius = 10f;
    public float respawnDelay = 2.0f;

    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Please assign an Enemy Prefab in the Inspector!");
            return;
        }

        int currentSpawnedCount = 0;
        int attempts = 0;
        int maxAttempts = numberOfEnemies * 10; 

        // Keep looping until we have enough enemies OR we tried too many times
        while (currentSpawnedCount < numberOfEnemies && attempts < maxAttempts)
        {
            bool success = SpawnOneEnemy();
            if (success)
            {
                currentSpawnedCount++;
            }
            attempts++;
        }

        // Log result
        if (currentSpawnedCount < numberOfEnemies)
        {
            Debug.LogWarning($"Only spawned {currentSpawnedCount} enemies after {attempts} attempts. Try increasing Spawn Radius or checking NavMesh.");
        }
        else
        {
            Debug.Log($"Successfully spawned all {currentSpawnedCount} enemies.");
        }
    }

    public void RequestRespawn()
    {
        // Wait a few seconds, then spawn the replacement
        Invoke("SpawnOneEnemy", respawnDelay);
    }

    bool SpawnOneEnemy()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPos = centerPoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        NavMeshHit hit;
        // If we find a valid spot
        if (NavMesh.SamplePosition(randomPos, out hit, 10.0f, NavMesh.AllAreas))
        {
            Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            return true; // Success!
        }
        
        return false; // Failed, try again
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = centerPoint != null ? centerPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, spawnRadius);
    }
}