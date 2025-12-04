using UnityEngine;

public class BossSceneManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform playerStartPoint;
    public Transform dragonSpawnPoint;
    public GameObject dragonPrefab;

    void Start()
    {
        Invoke("SetupPlayer", 0.1f);
        SpawnDragon();
    }

    void SetupPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        
        if (player != null)
        {
            // DISABLE EVERYTHING
            // If these are on, they overwrite your position change!
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            UnityEngine.AI.NavMeshAgent agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Stop gravity temporarily
                rb.linearVelocity = Vector3.zero; // Stop falling momentum
            }

            // TELEPORT
            player.transform.position = playerStartPoint.position;
            player.transform.rotation = playerStartPoint.rotation;

            // RE-ENABLE EVERYTHING
            if (cc != null) cc.enabled = true;
            if (agent != null) agent.enabled = true;
            if (rb != null) rb.isKinematic = false; // Turn gravity back on

            Debug.Log("Player Teleported Successfully!");
        }
    }

    void SpawnDragon()
    {
        if (dragonPrefab != null && dragonSpawnPoint != null)
        {
            Instantiate(dragonPrefab, dragonSpawnPoint.position, dragonSpawnPoint.rotation);
        }
    }
}