using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Obstacle spawning variables
    public GameObject[] obstaclePrefabs;
    public float spawnInterval = 2f;
    public float spawnDistance = 10f;

    // Ground spawning variables
    public GameObject groundPrefab;
    public float groundLength = 1f; // Match the Z-scale of your ground prefab
    public int initialGroundPieces = 5;

    // Power-up spawning variables
    public GameObject[] powerUpPrefabs; // Array for our new power-ups
    public float powerUpSpawnInterval = 10f;

    // --- Private Helper Variables ---
    private Transform playerTransform;
    private float groundSpawnPositionZ = 0;
    private List<GameObject> activeGroundPieces = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Spawn initial ground pieces
        for (int i = 0; i < initialGroundPieces; i++)
        {
            SpawnGround();
        }

        StartCoroutine(SpawnObstacles());
        StartCoroutine(SpawnPowerUps());
    }

    // Update is called once per frame
    void Update()
    {
        // Stop all spawning logic in Update if the game is over
        if (GameManager.Instance.isGameOver)
        {
            return;
        }

        // --- Ground Spawner Logic ---
        if (playerTransform.position.z > groundSpawnPositionZ - (initialGroundPieces * groundLength) + (2 * groundLength))
        {
            SpawnGround();
            DeleteOldestGround();
        }
    }

    IEnumerator SpawnObstacles()
    {
        while (GameManager.Instance.isGameOver == false)
        {
            yield return new WaitForSeconds(spawnInterval);
            int randomIndex = Random.Range(0, obstaclePrefabs.Length);
            Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 1, playerTransform.position.z + spawnDistance);

            // Create the new obstacle
            Instantiate(obstaclePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator SpawnPowerUps()
    {
        while (GameManager.Instance.isGameOver == false)
        {
            // Wait for the power-up spawn interval (e.g., 10 seconds)
            yield return new WaitForSeconds(powerUpSpawnInterval);

            // Pick a random power-up from the array
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);

            // Spawn it at a random position in front of the player
            Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 1, playerTransform.position.z + spawnDistance);
            Instantiate(powerUpPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
    }

    void SpawnGround()
    {
        GameObject newGround = Instantiate(groundPrefab, new Vector3(0, 0, groundSpawnPositionZ), Quaternion.identity);
        activeGroundPieces.Add(newGround);
        groundSpawnPositionZ += groundLength;
    }

    void DeleteOldestGround()
    {
        Destroy(activeGroundPieces[0]);
        activeGroundPieces.RemoveAt(0);
    }

}
