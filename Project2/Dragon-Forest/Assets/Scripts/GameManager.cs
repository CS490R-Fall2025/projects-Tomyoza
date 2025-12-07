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
    public GameObject victoryCanvas;
    [Header("Quest Settings")]
    public int totalCrystals = 4;
    public int TotalCrystals => totalCrystals;
    public int bossSceneIndex = 2;
    public string playerName = "Adventurer";
    
    private int crystalsDestroyed = 0; 
    private bool gameHasEnded = false;
    [SerializeField] private TextMeshProUGUI crystalWarningText;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This runs automatically every time a scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if we just loaded the Forest (Index 1)
        if (scene.buildIndex == 1)
        {
            ApplyCloudData();
        }
    }
    public void ApplyCloudData()
    {
        // Safety Check: Do we have a Cloud Manager? Do we have data?
        if (CloudSaveManager.Instance == null || CloudSaveManager.Instance.loadedData == null) 
        {
            Debug.Log("No cloud save data found (or Manager missing). Using default stats.");
            return;
        }

        // Get the data packet
        var data = CloudSaveManager.Instance.loadedData;
        Debug.Log($"Applying Cloud Data: Gold {data.gold}, HP {data.currentHealth}/{data.maxHealth}");

        // Find the Player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Apply Gold
            PlayerWallet wallet = player.GetComponent<PlayerWallet>();
            if (wallet != null) 
            {
                wallet.currentMoney = data.gold;
            }

            // Apply Health
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null) 
            {
                // Set Max Capacity first
                if (data.maxHealth > 0) health.maxHealth = data.maxHealth; 

                // Set Current Health
                if (data.currentHealth > 0)
                {
                    health.SetHealth(data.currentHealth);
                }
                else
                {
                    health.HealFull();
                }
            }

            // Apply Speed
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null && data.speed > 0)
            {
                // Note: You might need to change 'moveSpeed' to public in PlayerController
                // or add a method: public void SetSpeed(float s) { moveSpeed = s; }
                controller.UpgradeSpeed(data.speed - controller.MoveSpeed); 
            }

            // Apply Attack Damage
            SwordAttack sword = player.GetComponentInChildren<SwordAttack>();
            if (sword != null && data.attackDamage > 0)
            {
                sword.damageAmount = data.attackDamage;
            }
        }
    }
    private void Start()
    {
        if (crystalWarningText != null) crystalWarningText.gameObject.SetActive(false);
        if (victoryCanvas != null) victoryCanvas.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        gameHasEnded = false;
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
        if (gameHasEnded) return;
        
        // Lock the game state
        gameHasEnded = true;

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

        BossController boss = FindFirstObjectByType<BossController>();
        if (boss != null) boss.enabled = false;

        // Show Cursor (so you can click the button)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TriggerVictory()
    {
        if (gameHasEnded) return;

        // Lock the game state
        gameHasEnded = true;

        // Show UI
        if (victoryCanvas != null) victoryCanvas.SetActive(true);

        // Play Music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.victoryMusic);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) 
        {   
            CharacterController cc = player.GetComponent<CharacterController>();
            if(cc != null) cc.enabled = false;

            PlayerController pc = player.GetComponent<PlayerController>();

            player.transform.position = pc.initialSpawnPosition;
            player.transform.rotation = Quaternion.identity;

            if(cc != null) cc.enabled = true;

            PlayerHealth healthScript = player.GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                healthScript.enabled = true;
                healthScript.Revive();
            }
        }

        crystalsDestroyed = 0;
        gameHasEnded = false;

        if (victoryCanvas != null) victoryCanvas.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        
        Time.timeScale = 1f;

        SceneManager.LoadScene(1);
    }
    public void GoToMainMenu()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) Destroy(player);

        GameObject cam = GameObject.FindWithTag("MainCamera");
        if (cam != null) Destroy(cam);

        GameObject canvas = GameObject.FindWithTag("Canvas");
        if (canvas != null) Destroy(canvas);

        GameObject audio = GameObject.FindWithTag("AudioManager");
        if (audio != null) Destroy(audio);

        Destroy(gameObject);

        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}