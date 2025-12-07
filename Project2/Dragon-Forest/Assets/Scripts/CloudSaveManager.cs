using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance;

    [Header("Config")]
    public string apiUrl;      
    public string webApiKey;   

    private string _idToken;   
    private string _userId;    

    [Serializable]
    public class SaveData
    {
        public int gold;
        public int currentHealth;
        public int maxHealth;
        public float speed;
        public int attackDamage;
        public string playerName;
    }

    public SaveData loadedData; 

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        // Try to recover the old session first
        if (PlayerPrefs.HasKey("Cloud_Token") && PlayerPrefs.HasKey("Cloud_UserID"))
        {
            _idToken = PlayerPrefs.GetString("Cloud_Token");
            _userId = PlayerPrefs.GetString("Cloud_UserID");
            Debug.Log($"Welcome back! Reusing Session for: {_userId}");
            
            // Try loading immediately with the old token
            LoadGame();
        }
        else
        {
            // No old session found, create a new one
            StartCoroutine(SignInAnonymous());
        }
    }

    IEnumerator SignInAnonymous()
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={webApiKey}";
        string json = "{\"returnSecureToken\":true}";
        
        using (UnityWebRequest req = UnityWebRequest.Post(url, json, "application/json"))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                var data = JsonUtility.FromJson<AuthResponse>(req.downloadHandler.text);
                _idToken = data.idToken;
                _userId = data.localId;
                
                // [FIX] Save this identity so we remember it when we restart the game!
                PlayerPrefs.SetString("Cloud_Token", _idToken);
                PlayerPrefs.SetString("Cloud_UserID", _userId);
                PlayerPrefs.Save();

                Debug.Log("New Account Created: " + _userId);
                LoadGame();
            }
            else Debug.LogError("Login Failed: " + req.error);
        }
    }

    // --- SAVE ---
    public void SaveGame()
    {
        if (string.IsNullOrEmpty(_idToken)) return;
        StartCoroutine(SaveRoutine());
    }

    IEnumerator SaveRoutine()
    {
        SaveData data = new SaveData();
        if (GameManager.Instance != null) data.playerName = GameManager.Instance.playerName;
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var wallet = player.GetComponent<PlayerWallet>();
            if (wallet) data.gold = wallet.currentMoney;

            var health = player.GetComponent<PlayerHealth>();
            if (health) 
            {
                data.maxHealth = health.MaxHealth;
                data.currentHealth = health.CurrentHealth;
            }

            var controller = player.GetComponent<PlayerController>();
            if (controller) data.speed = controller.MoveSpeed;

            var sword = player.GetComponentInChildren<SwordAttack>();
            if (sword) data.attackDamage = sword.damageAmount;
        }

        string json = JsonUtility.ToJson(data);
        string url = $"{apiUrl}/saveGame"; 

        using (UnityWebRequest req = UnityWebRequest.Put(url, json))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + _idToken); 

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                Debug.Log("Game Saved Successfully!");
            else
            {
                // If we get "401 Unauthorized", it means our saved token expired.
                if (req.responseCode == 401)
                {
                    Debug.LogWarning("Token Expired! Creating new session...");
                    PlayerPrefs.DeleteKey("Cloud_Token"); // Clear bad token
                    StartCoroutine(SignInAnonymous()); // Log in again (New User)
                }
                else
                {
                    Debug.LogError("Save Failed: " + req.error);
                }
            }
        }
    }

    // --- LOAD ---
    public void LoadGame()
    {
        if (string.IsNullOrEmpty(_idToken)) return;
        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {
        string url = $"{apiUrl}/loadGame"; 

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Authorization", "Bearer " + _idToken);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                if (json != "{}" && json.Length > 2) 
                {
                    loadedData = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log($"Data Loaded! Gold: {loadedData.gold}");
                    
                    // [ADDED] Logic to Apply Data if we are already in-game
                    if (GameManager.Instance != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex > 0)
                    {
                        GameManager.Instance.ApplyCloudData();
                    }
                }
                else
                {
                    Debug.Log("No save found (New User).");
                }
            }
            else
            {
                // If Token Expired (401), clear prefs so we get a fresh one next time
                if (req.responseCode == 401)
                {
                    Debug.LogWarning("Token Expired during load.");
                    PlayerPrefs.DeleteKey("Cloud_Token");
                }
            }
        }
    }

    // Helper to clear save data for testing (Call this if you want to be a new user again)
    [ContextMenu("Reset ID")]
    public void ResetIdentity()
    {
        PlayerPrefs.DeleteKey("Cloud_Token");
        PlayerPrefs.DeleteKey("Cloud_UserID");
        Debug.Log("Identity Cleared. Next play will be New User.");
    }

    [Serializable] class AuthResponse { public string idToken; public string localId; }
}