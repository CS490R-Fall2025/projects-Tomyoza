using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private PlayerHealth playerHealth; 

    private void Start()
    {
        if (playerHealth == null)
        {
            // Auto-find player if you forgot to drag it
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            // Subscribe to the event
            playerHealth.OnHealthChanged += UpdateBar;
            
            // Force update immediately
            UpdateBar(playerHealth.CurrentHealth, playerHealth.maxHealth);
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent errors
        if (playerHealth != null) playerHealth.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        if (fillImage != null)
        {
            float pct = (float)current / max;
            fillImage.fillAmount = pct;
        }
    }
}