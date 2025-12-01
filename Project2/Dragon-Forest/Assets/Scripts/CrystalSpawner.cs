using UnityEngine;
using UnityEngine.AI;

public class CrystalSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject crystalPrefab;
    public Transform centerPoint;
    public float spawnWidthX = 10f;
    public float spawnLengthZ = 50f;
    
    // Difficulty Settings:
    private int[] hpLevels = { 20, 40, 80, 150 };
    private int[] powerLevels = { 0, 10, 20, 30 };
    public Color[] levelColors = new Color[] { Color.cyan, Color.magenta, new Color(0.627451f, 0.1254902f, 0.9411765f, 1f), new Color(0.2f, 0.2f, 0.2f) };

    void Start()
    {
        if (centerPoint == null) centerPoint = transform;
        SpawnCrystals();
    }

    void SpawnCrystals()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool validPositionFound = false;
            int attempts = 0;

            // Retry until we find a valid spot on the NavMesh
            while (!validPositionFound && attempts < 30)
            {
                spawnPos = GetRandomSquarePoint(out validPositionFound);
                attempts++;
            }

            if (validPositionFound)
            {
                // Spawn slightly above ground (+0.5f) to look nice
                GameObject obj = Instantiate(crystalPrefab, spawnPos + Vector3.up * 1.5f, Quaternion.identity);
                
                Crystal crystalScript = obj.GetComponent<Crystal>();
                if (crystalScript != null)
                {
                    crystalScript.Initialize(hpLevels[i], powerLevels[i], levelColors[i]);
                }
            }
            else
            {
                Debug.LogWarning($"Could not find a spot for Crystal {i}. Try increasing Spawn Range.");
            }
        }
    }

    Vector3 GetRandomSquarePoint(out bool success)
    {
        // Random X and Z inside a square range
        float randomX = Random.Range(-spawnWidthX, spawnWidthX);
        float randomZ = Random.Range(-spawnLengthZ, spawnLengthZ);

        Vector3 randomPos = centerPoint.position + new Vector3(randomX, 0, randomZ);

        NavMeshHit hit;
        
        // Find the nearest ground on the NavMesh.
        if (NavMesh.SamplePosition(randomPos, out hit, 50.0f, NavMesh.AllAreas))
        {
            success = true;
            return hit.position;
        }

        success = false;
        return Vector3.zero;
    }

    // Visual helper to see the square in the Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = centerPoint != null ? centerPoint.position : transform.position;
        // Draw a wire cube representing the spawn zone
        Gizmos.DrawWireCube(center, new Vector3(spawnWidthX * 2, 10, spawnLengthZ * 2));
    }
}