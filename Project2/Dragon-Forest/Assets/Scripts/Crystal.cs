using UnityEngine;
using System.Collections;

public class Crystal : MonoBehaviour, IDamageable
{
    [Header("Stats (Set by Spawner)")]
    public int maxHealth = 50;
    public int requiredPower = 10;
    [Header("Settings")]
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float rotateSpeed = 50f;

    private int currentHealth;
    private Renderer myRenderer;
    private Color originalColor;
    private Vector3 startPos;
    private float timeOffset;
    private Light myLight;
    private ParticleSystem myParticles;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        myLight = GetComponentInChildren<Light>();
        myParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;
        timeOffset = Random.Range(0f, 2f);
    }

    private void Update()
    {
        // 1. ROTATE
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        // 2. FLOAT (Sine Wave)
        // Mathf.Sin creates a wave between -1 and 1 over time
        float newY = startPos.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    public void Initialize(int hp, int power, Color myColor)
    {
        maxHealth = hp;
        currentHealth = hp;
        requiredPower = power;
        originalColor = myColor;
        if (myRenderer != null)
        {
            myRenderer.material.color = myColor;
            myRenderer.material.SetColor("_EmissionColor", myColor * 2f);
            myRenderer.material.EnableKeyword("_EMISSION");
        }
        if (myLight != null) myLight.color = myColor;

        // Apply Color to Particles (Sparkles)
        if (myParticles != null)
        {
            var main = myParticles.main;
            main.startColor = myColor;
        }
    }

    public void ApplyDamage(int amount)
    {
        // Check if Player is strong enough
        if (amount < requiredPower)
        {
            // Visual feedback (Log or floating text)
            GameManager.Instance.ShowMessage($"Attack Weak! Need {requiredPower} Power");
            return;
        }

        // Take Damage
        currentHealth -= amount;
        Debug.Log($"Crystal Hit! HP: {currentHealth}/{maxHealth}");
        StartCoroutine(FlashColor());

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator FlashColor()
    {
        if (myRenderer != null)
        {
            // Flash White
            myRenderer.material.color = Color.white;
            myRenderer.material.SetColor("_EmissionColor", Color.white * 2f);
            
            // Wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);
            
            // Return to the correct level color (Green/Red/etc)
            myRenderer.material.color = originalColor;
            myRenderer.material.SetColor("_EmissionColor", originalColor * 2f);
        }
    }

    private void Die()
    {
        GameManager.Instance.OnCrystalDestroyed();
        
        Destroy(gameObject);
    }
}