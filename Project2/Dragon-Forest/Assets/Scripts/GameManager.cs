using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverCanvas; 

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerGameOver()
    {
        // Show the "Game Over" screen
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);

        // Stop all Enemies
        EnemyManager[] enemies = FindObjectsByType<EnemyManager>(FindObjectsSortMode.None);
        foreach (EnemyManager enemy in enemies)
        {
            enemy.enabled = false; // Disable script
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null) agent.isStopped = true; // Stop moving
        }

        // Show Cursor (so you can click the button)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}