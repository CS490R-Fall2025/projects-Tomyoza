using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    private PlayerHealth playerHealth;

    private void Start()
    {
        FindPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void FindPlayer()
    {
        // Reset old reference
        playerHealth = null;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            
            // Unsubscribe to avoid double events
            playerHealth.OnHealthChanged -= UpdateBar; 
            playerHealth.OnHealthChanged += UpdateBar;
            
            UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
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