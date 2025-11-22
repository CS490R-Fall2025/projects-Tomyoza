using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class EnemyManager : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private bool destroyOnDeath = true;
    
    // Public read-only property for the bar
    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    // Event the UI Bar listens to: (Current, Max)
    public event Action<int, int> OnHealthChanged;

    [Header("Attack Settings")]
    [SerializeField] private int damageAmount = 10; 
    [SerializeField] private float attackCooldown = 2.0f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseDistance = 10f;
    [SerializeField] private float stopDistance = 11f;

    [Header("Move Settings")]
    [SerializeField] private float agentSpeed = 3.5f;
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private float acceleration = 8f;

    [Header("Animation (Optional)")]
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Transform target;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isChasing = false;
    private Coroutine pathCoroutine;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        animator = GetComponent<Animator>();

        // Auto-find player if not assigned
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            enabled = false;
            return;
        }

        agent.speed = agentSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
    }

    private void Start()
    {
        // Update UI immediately
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void Update()
    {
        if (isDead || target == null) return;
        if (animator != null && agent != null)
        {
            // Get the actual speed the agent is moving
            float currentSpeed = agent.velocity.magnitude;
            
            // Send it to the Animator "Speed" parameter
            animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
        }

        if (target == null) return;

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= chaseDistance)
        {
            if (!isChasing) StartChase();
        }
        else if (distance > stopDistance)
        {
            if (isChasing) StopChase();
        }
    }

    private void StartChase()
    {
        isChasing = true;
        if (agent.isOnNavMesh) agent.isStopped = false; 
        
        if (pathCoroutine == null)
        {
            pathCoroutine = StartCoroutine(UpdatePathRoutine());
        }
    }

    private void StopChase()
    {
        isChasing = false;
        if (agent.isOnNavMesh) agent.isStopped = true;
        if (agent.isOnNavMesh) agent.ResetPath();

        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
            pathCoroutine = null;
        }
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (isChasing)
        {
            if (target != null && agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;

        CurrentHealth -= amount;
        
        // Notify UI
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        // Visual Feedback (Optional)
        Debug.Log($"{gameObject.name} took {amount} damage! HP: {CurrentHealth}");

        // Death Check
        if (CurrentHealth <= 0 && destroyOnDeath)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death animation or sound here later
        Destroy(gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isDead) return;

        // Check if the thing we are touching is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Stop pushing the player (freeze movement)
            if (agent.isOnNavMesh) agent.isStopped = true;

            // Check if cooldown is ready
            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time; // Reset timer

                // Deal Damage
                IDamageable playerHealth = collision.gameObject.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.ApplyDamage(damageAmount);
                    Debug.Log("Enemy hit the Player!");
                }
            }
        }
    }

    // RESUME MOVING WHEN PLAYER RUNS AWAY
    private void OnCollisionExit(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Resume chasing if they leave our body contact range
            if (agent.isOnNavMesh) agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}