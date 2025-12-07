using UnityEngine;
using System; // Needed for Events

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] public int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    // Event to update the UI: (CurrentHP, MaxHP)
    public event Action<int, int> OnHealthChanged;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private MonoBehaviour playerController;

    [Header("Audio")]
    public AudioClip hurtSound;
    
    private bool isDead = false;
    private float minimumY = 0f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Initialize the UI at start
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Update()
    {
        // Safety Check: Did we fall off the world?
        if (transform.position.y < minimumY)
        {
            // Instant Death (Game Over)
            ApplyDamage(maxHealth); 
            Debug.Log("Player fell out of the world!");
        }
    }

    // This allows enemies to damage the player
    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;

        Debug.Log($"took {amount} damage. Source: {new System.Diagnostics.StackTrace()}");

        CurrentHealth -= amount;
        AudioManager.Instance.PlaySFX(hurtSound);
        Debug.Log($"Player took {amount} damage! HP: {CurrentHealth}");

        // Update UI
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void HealFull()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void UpgradeMaxHealth(int amount)
    {
        maxHealth += amount;
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player Died!");

        // Play Animation
        if (animator != null) animator.SetTrigger("Die");

        // Disable Movement Script immediately
        if (playerController != null) playerController.enabled = false;
        
        // Disable Physics (so enemies stop pushing you)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        CallGameOver();
    }

    private void CallGameOver()
    {
        GameManager.Instance.TriggerGameOver();
    }

    public void Revive()
    {
        isDead = false;
        CurrentHealth = maxHealth; 
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (playerController != null) playerController.enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
}