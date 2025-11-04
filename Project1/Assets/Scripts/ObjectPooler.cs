using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public GameObject projectilePrefab;
    public int poolSize = 20; // How many projectiles to create at the start

    private Queue<GameObject> projectilePool;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        projectilePool = new Queue<GameObject>();

        // Create all the projectiles at the start and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Enqueue(obj);
        }
    }

    // Call this to get a projectile from the pool
    public GameObject GetProjectile()
    {
        // If the pool is empty, create a new one
        if (projectilePool.Count == 0)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Enqueue(obj);
        }

        GameObject projectileToSpawn = projectilePool.Dequeue();
        projectileToSpawn.SetActive(true);
        return projectileToSpawn;
    }

    // Call this to return a projectile to the pool
    public void ReturnProjectile(GameObject projectileToReturn)
    {
        projectileToReturn.SetActive(false);
        projectilePool.Enqueue(projectileToReturn);
    }
}