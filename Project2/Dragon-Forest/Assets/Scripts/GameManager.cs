using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverCanvas;
    [Header("Quest Settings")]
    public int totalCrystals = 4;
    public int bossSceneIndex = 1;
    
    private int crystalsDestroyed = 0; 
    [SerializeField] private TextMeshProUGUI crystalWarningText;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (crystalWarningText != null) crystalWarningText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (crystalWarningText != null)
        {
            // Stop any old message so the new one shows immediately
            StopAllCoroutines(); 
            StartCoroutine(MessageCoroutine(message));
        }
    }

    private IEnumerator MessageCoroutine(string message)
    {
        crystalWarningText.text = message;
        crystalWarningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        crystalWarningText.gameObject.SetActive(false);
    }

    public void OnCrystalDestroyed()
    {
        crystalsDestroyed++;
        Debug.Log($"Crystals Destroyed: {crystalsDestroyed} / {totalCrystals}");

        if (crystalsDestroyed >= totalCrystals)
        {
            LoadBossScene();
        }
    }

    private void LoadBossScene()
    {
        Debug.Log("ALL CRYSTALS DESTROYED! SUMMONING DRAGON...");
        // Ensure you have added the Boss Scene to 'File > Build Settings'
        SceneManager.LoadScene(bossSceneIndex);
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