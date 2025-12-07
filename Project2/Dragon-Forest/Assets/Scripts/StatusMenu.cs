using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatusMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Player Data")]
    private PlayerHealth playerHealth;
    private PlayerWallet playerWallet;
    private SwordAttack swordAttack; 
    private PlayerController playerController;

    private bool isMenuOpen = false;

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        
        FindPlayerComponents();
    }

    void Update()
    {
        // Toggle with "TAB" key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerComponents();
    }

    private void FindPlayerComponents()
    {
        // Reset to prevent holding old data
        playerHealth = null;
        playerWallet = null;
        swordAttack = null;
        playerController = null;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            if (playerHealth == null) playerHealth = player.GetComponent<PlayerHealth>();
            if (playerWallet == null) playerWallet = player.GetComponent<PlayerWallet>();
            
            if (swordAttack == null) swordAttack = player.GetComponentInChildren<SwordAttack>();
            if (playerController == null) playerController = player.GetComponent<PlayerController>();
        }
        // Force an update right now to see if values change
        UpdateStats();
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        
        if (uiPanel != null) uiPanel.SetActive(isMenuOpen);

        if (isMenuOpen)
        {
            UpdateStats();
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void UpdateStats()
    {
        if (nameText != null)
        {
            if (GameManager.Instance != null)
                nameText.text = "Name: " + GameManager.Instance.playerName;
            else
                nameText.text = "Name: Unknown";
        }
        if (playerHealth != null)
            hpText.text = $"HP: {playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";

        if (playerWallet != null)
            moneyText.text = $"Money: $ {playerWallet.currentMoney}";

        if (swordAttack != null)
            attackText.text = $"Attack Power: {swordAttack.damageAmount}";
            
        if (playerController != null)
            speedText.text = $"Speed: {playerController.MoveSpeed}";
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}