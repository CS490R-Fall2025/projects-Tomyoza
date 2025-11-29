using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    [SerializeField] private PlayerWallet playerWallet;

    private void Start()
    {
        if (playerWallet == null) 
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerWallet = player.GetComponent<PlayerWallet>();
        }

        if (playerWallet != null)
        {
            // Subscribe to the event
            playerWallet.OnMoneyChanged += UpdateText;
            
            // Initial update
            UpdateText(playerWallet.currentMoney);
        }
    }

    private void OnDestroy()
    {
        if (playerWallet != null) playerWallet.OnMoneyChanged -= UpdateText;
    }

    private void UpdateText(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + amount.ToString();
        }
    }
}