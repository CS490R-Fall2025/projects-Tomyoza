using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 1;

    // Only damage enemies when the attack animation is playing
    public bool isAttacking = false; 

    private void OnTriggerEnter(Collider other)
    {
        // If we aren't swinging, the sword is just a safe object
        if (!isAttacking) return;

        // Check if the object we hit has the IDamageable interface
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null)
        {
            target.ApplyDamage(damageAmount);
        }
    }
}