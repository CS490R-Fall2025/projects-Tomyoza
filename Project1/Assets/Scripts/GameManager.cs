using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    public TextMeshProUGUI healthText; // A reference to the UI text for health
    public int playerHealth = 3;
    public bool isGameOver = false;
    public TextMeshProUGUI scoreText;
    public int scorePerSecond = 10; // Score increment per second
    public GameObject deathEffectPrefab;
    public Transform playerTransform;
    public GameObject healthTextBackground;
    public GameObject scoreTextBackground;
    public GameObject gameOverPanel;

    private float score = 0f; // Score by the number of obstacles destroyed
    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText.text = "HP: " + playerHealth;
        scoreText.text = "Score: 0";
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        healthText.text = "HP: " + playerHealth; // Update the display

        if (playerHealth <= 0)
        {
            isGameOver = true;
            Debug.Log("Game Over!");

            Time.timeScale = 0f;

            // Spawn the explosion at the player's position
            Instantiate(deathEffectPrefab, playerTransform.position, Quaternion.identity);

            // Hide the player model
            playerTransform.gameObject.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        // High contrast toggle
        healthTextBackground.SetActive(false);
        scoreTextBackground.SetActive(false);
    }
    
    public void AddScore(int amount)
    {
        // Only add score if the game isn't over
        if (isGameOver)
        {
            return;
        }
        score += amount;
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is over, stop counting score
        if (isGameOver)
        {
            return;
        }

        // Add score just for surviving
        score += scorePerSecond * Time.deltaTime;

        // Update the score text on screen
        scoreText.text = "Score: " + (int)score;
    }

    public void ToggleHighContrast(bool isToggled)
    {
        healthTextBackground.SetActive(isToggled);
        scoreTextBackground.SetActive(isToggled);
    }

    public void RestartGame()
    {
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
