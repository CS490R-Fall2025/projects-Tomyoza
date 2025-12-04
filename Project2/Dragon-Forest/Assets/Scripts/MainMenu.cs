using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField nameInput;
    public Slider volumeSlider;

    private void Start()
    {
        // Initialize Volume Slider
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            // Add listener to change volume in real-time
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Initialize Name Input
        if (nameInput != null)
        {
            nameInput.onValueChanged.AddListener(SetPlayerName);
        }
        
        // Play Menu Music (if you have a specific menu song)
        if (AudioManager.Instance != null) 
            AudioManager.Instance.PlayMusic(AudioManager.Instance.forestMusic);
    }

    public void StartGame()
    {
        // Load the Forest Scene (Index 1)
        SceneManager.LoadScene(1);
    }

    public void SetVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(value);
        }
    }

    public void SetPlayerName(string name)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerName = name;
        }
    }
}