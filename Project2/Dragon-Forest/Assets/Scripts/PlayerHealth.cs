using UnityEngine;
using System; // Needed for Events

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] public int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    // Event to update the UI: (CurrentHP, MaxHP)
    public event Action<int, int> OnHealthChanged;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private MonoBehaviour playerController;
    
    private bool isDead = false;

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

    // This allows enemies to damage the player
    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;

        CurrentHealth -= amount;
        Debug.Log($"Player took {amount} damage! HP: {CurrentHealth}");

        // Update UI
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
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
}