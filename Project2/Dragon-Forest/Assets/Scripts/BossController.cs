using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossController : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int damageAmount = 50;

    [Header("Combat Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform mouthPos;
    [SerializeField] private float attackCooldown = 3.0f;
    [SerializeField] private float flyDuration = 10.0f;

    [Header("AI")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [Header("Audio")]
    public AudioClip screamSound;
    public AudioClip fireballSound;
    public AudioClip flyingSound;
    public AudioClip deathSound;

    // Internal State
    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public event System.Action<int, int> OnHealthChanged;

    private Transform target;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isFlying = false;
    private bool isIntroDone = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) target = player.transform;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        
        // Start with a scary roar!
        StartCoroutine(IntroRoar());
    }

    private IEnumerator IntroRoar()
    {
        // Wait a moment, then SCREAM
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("Scream");
        AudioManager.Instance.PlaySFX(screamSound);
        yield return new WaitForSeconds(2f); // Wait for scream to finish
        isIntroDone = true;
    }

    private void Update()
    {
        if (isDead || target == null || !isIntroDone) return;

        float distance = Vector3.Distance(transform.position, target.position);
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // --- FLYING PHASE ---
        if (isFlying)
        {
            // While flying, look at player and shoot fire
            FaceTarget();
            if (Time.time - lastAttackTime > 2.0f) // Shoot every 2 seconds
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("FlyAttack"); // Trigger animation
                Invoke("ShootFireball", 0.5f); // Launch projectile delay
            }
            return; // Skip ground logic
        }

        // --- GROUND PHASE ---
        if (distance <= 30.0f) // Close range
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            FaceTarget();

            if (Time.time - lastAttackTime > attackCooldown)
            {
                DoRandomGroundAttack();
            }
        }
        else
        {
            // Chase player
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void DoRandomGroundAttack()
    {
        lastAttackTime = Time.time;

        // Pick a number: 0, 1, or 2
        int roll = Random.Range(0, 3);
        animator.SetInteger("AttackType", roll);
        animator.SetTrigger("Attack");

        // Logic for hits
        if (roll == 2) 
        {
            // AttackType 2 is FLAME BREATH (Ranged)
            Invoke("ShootFireball", 0.5f);
            AudioManager.Instance.PlaySFX(fireballSound);
        }
        else 
        {
            // 0 and 1 are Melee (Bite/Claw)
            Invoke("DealMeleeDamage", 0.5f);
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab != null && mouthPos != null)
        {
            GameObject ball = Instantiate(fireballPrefab, mouthPos.position, Quaternion.identity);
            
            Vector3 aimPoint = target.position + (Vector3.up * 1.5f);
            Vector3 direction = (aimPoint - mouthPos.position).normalized;

            ball.transform.forward = direction;

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.linearVelocity = direction * 50f;
            }
        }
    }

    private void DealMeleeDamage()
    {
        if (target == null) return;
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= 30.0f)
        {
            IDamageable playerHp = target.GetComponent<IDamageable>();
            if (playerHp != null) playerHp.ApplyDamage(damageAmount);
        }
    }

    // Logic to switch phases (Called when health drops, or on a timer)
    public void ApplyDamage(int amount)
    {
        if (isDead) return;
        CurrentHealth -= amount;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        // FUN MECHANIC: If HP drops below 50%, start flying!
        if (CurrentHealth < maxHealth / 2 && !isFlying)
        {
            StartCoroutine(FlyPhase());
        }

        if (CurrentHealth <= 0) Die();
    }

    private IEnumerator FlyPhase()
    {
        isFlying = true;
        animator.SetBool("IsFlying", true);
        agent.baseOffset = 5.0f; // Lift dragon up on NavMesh
        AudioManager.Instance.PlaySFX(flyingSound);
        
        yield return new WaitForSeconds(flyDuration); // Fly for 10 seconds
        
        // Land
        isFlying = false;
        animator.SetBool("IsFlying", false);
        agent.baseOffset = 0f; // Back to ground
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines(); 
        isFlying = false;
        animator.SetBool("IsFlying", false);

        // Force the agent back to the ground level
        if (agent != null)
        {
            agent.isStopped = true;
            agent.baseOffset = 0f;
        }

        // Disable the collider so the player can walk through the dead body
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        animator.SetTrigger("Die");
        
        // Trigger Victory
        GameManager.Instance.TriggerVictory();

        Destroy(gameObject, 5.0f);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}