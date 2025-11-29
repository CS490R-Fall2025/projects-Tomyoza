using UnityEngine;
using TMPro;

public class StatusMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Player Data")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private SwordAttack swordAttack; 
    [SerializeField] private PlayerController playerController;

    private bool isMenuOpen = false;

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        
        // Auto-find components if not assigned
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            if (playerHealth == null) playerHealth = player.GetComponent<PlayerHealth>();
            if (playerWallet == null) playerWallet = player.GetComponent<PlayerWallet>();
            
            if (swordAttack == null) swordAttack = player.GetComponentInChildren<SwordAttack>();
            if (playerController == null) playerController = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        // Toggle with "TAB" key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
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