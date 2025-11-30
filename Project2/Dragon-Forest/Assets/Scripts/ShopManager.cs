using UnityEngine;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI shopTitle;      
    [SerializeField] private TextMeshProUGUI playerMoneyText; 

    [Header("Buttons")]
    [SerializeField] private GameObject healButton;
    [SerializeField] private GameObject hpUpgradeButton;
    [SerializeField] private GameObject speedButton;
    [SerializeField] private GameObject attackButton;

    [Header("Button Text (Price Labels)")]
    [SerializeField] private TextMeshProUGUI healPriceText;   
    [SerializeField] private TextMeshProUGUI hpPriceText;     
    [SerializeField] private TextMeshProUGUI speedPriceText;  
    [SerializeField] private TextMeshProUGUI attackPriceText; 

    [Header("Pricing Settings")]
    [SerializeField] private int healCost = 50;
    
    // Base Costs
    [SerializeField] private int hpBaseCost = 100;
    [SerializeField] private int speedBaseCost = 150;
    [SerializeField] private int attackBaseCost = 200;

    [SerializeField] private TextMeshProUGUI warningText;

    // Multipliers (How much price goes up per level)
    private float priceMultiplier = 1.5f; 

    // Track Player Levels
    private int hpLevel = 1;
    private int speedLevel = 1;
    private int attackLevel = 1;

    // References to Player
    private PlayerHealth pHealth;
    private PlayerWallet pWallet;
    private PlayerController pController;
    private SwordAttack pSword;

    private void Awake()
    {
        Instance = this;
        // Don't disable panel here yet, or you can't reference it in Start()
    }

    private void Start()
    {
        // Hide shop at start
        if (shopPanel != null) shopPanel.SetActive(false);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            pHealth = player.GetComponent<PlayerHealth>();
            pWallet = player.GetComponent<PlayerWallet>();
            pController = player.GetComponent<PlayerController>();
            pSword = player.GetComponentInChildren<SwordAttack>();
        }
    }

    public void OpenShop(string shopType)
    {
        shopPanel.SetActive(true);
        shopTitle.text = shopType + " SHOP"; // TMP uses .text just like standard UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateUI();

        // Reset Buttons
        healButton.SetActive(false);
        hpUpgradeButton.SetActive(false);
        speedButton.SetActive(false);
        attackButton.SetActive(false);

        // Logic
        if (shopType == "Meat")
        {
            hpUpgradeButton.SetActive(true);
        }
        else if (shopType == "Vegetable")
        {
            healButton.SetActive(true);
        }
        else if (shopType == "Fish")
        {
            speedButton.SetActive(true);
        }
        else if (shopType == "Weapon")
        {
            attackButton.SetActive(true);
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void BuyHeal()
    {
        if (TrySpendMoney(healCost))
        {
            pHealth.HealFull();
        }
    }

    public void BuyMaxHP()
    {
        int cost = CalculateCost(hpBaseCost, hpLevel);
        if (TrySpendMoney(cost))
        {
            pHealth.UpgradeMaxHealth(10);
            hpLevel++;
            UpdateUI();
        }
    }

    public void BuySpeed()
    {
        int cost = CalculateCost(speedBaseCost, speedLevel);
        if (TrySpendMoney(cost))
        {
            pController.UpgradeSpeed(1.0f);
            speedLevel++;
            UpdateUI();
        }
    }

    public void BuyAttack()
    {
        int cost = CalculateCost(attackBaseCost, attackLevel);
        if (TrySpendMoney(cost))
        {
            pSword.UpgradeAttack(2);
            attackLevel++;
            UpdateUI();
        }
    }

    private bool TrySpendMoney(int cost)
    {
        if (pWallet.currentMoney >= cost)
        {
            pWallet.AddMoney(-cost);
            return true;
        }
        else
        {
            StartCoroutine(ShowWarning());
            return false;
        }
    }

    private IEnumerator ShowWarning()
    {
        if (warningText != null)
        {
            warningText.text = "NOT ENOUGH MONEY!";
            warningText.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1.5f);
            
            warningText.gameObject.SetActive(false);
        }
    }

    private int CalculateCost(int baseCost, int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(priceMultiplier, level - 1));
    }

    private void UpdateUI()
    {
        playerMoneyText.text = "Money: $" + pWallet.currentMoney;

        healPriceText.text = $"Heal Full (${healCost})";
        hpPriceText.text = $"Max HP +20 (${CalculateCost(hpBaseCost, hpLevel)})";
        speedPriceText.text = $"Speed +1 (${CalculateCost(speedBaseCost, speedLevel)})";
        attackPriceText.text = $"Damage +5 (${CalculateCost(attackBaseCost, attackLevel)})";
    }
}