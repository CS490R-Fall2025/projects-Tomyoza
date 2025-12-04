using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepOnLoad : MonoBehaviour
{
    private static KeepOnLoad playerInstance;
    private static KeepOnLoad cameraInstance;
    private static KeepOnLoad canvasInstance;
    private static KeepOnLoad gameManagerInstance;
    private static KeepOnLoad audioManagerInstance;

    private void Awake()
    {
        // If this script is on the PLAYER...
        if (CompareTag("Player"))
        {
            if (playerInstance == null)
            {
                playerInstance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // A player already exists, destroy this new duplicate
                Destroy(gameObject);
            }
        }
        // If this script is on the CAMERA...
        else if (CompareTag("MainCamera"))
        {
            if (cameraInstance == null)
            {
                cameraInstance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // If this script is on the HUD / UI...
        else if (CompareTag("Canvas"))
        {
            if (canvasInstance == null)
            {
                canvasInstance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (CompareTag("GameManager"))
        {
            if (gameManagerInstance == null)
            {
                gameManagerInstance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (CompareTag("AudioManager"))
        {
            if (audioManagerInstance == null)
            {
                audioManagerInstance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only run this logic if THIS object is the Canvas/HUD
        if (CompareTag("Canvas"))
        {
            Canvas myCanvas = GetComponent<Canvas>();
            if (myCanvas != null)
            {
                if (scene.buildIndex == 0)
                {
                    myCanvas.enabled = false;
                }
                else
                {
                    myCanvas.enabled = true;
                }
            }
        }
    }

    public static void ResetAll()
    {
        if (playerInstance != null) { Destroy(playerInstance.gameObject); playerInstance = null; }
        if (cameraInstance != null) { Destroy(cameraInstance.gameObject); cameraInstance = null; }
        if (canvasInstance != null) { Destroy(canvasInstance.gameObject); canvasInstance = null; }
        if (gameManagerInstance != null) { Destroy(gameManagerInstance.gameObject); gameManagerInstance = null; }
        
        // Optional: Reset Audio? Usually we keep music playing.
        // if (audioManagerInstance != null) { Destroy(audioManagerInstance.gameObject); audioManagerInstance = null; }
    }
}