using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 1;
    [Header("Audio")]
    public AudioClip swingSound;

    // Only damage enemies when the attack animation is playing
    public bool isAttacking = false; 

    private void OnTriggerEnter(Collider other)
    {
        // If we aren't swinging, the sword is just a safe object
        if (!isAttacking) return;
        if (other.CompareTag("Player")) return;
        if (other.isTrigger) return;

        // Check if the object we hit has the IDamageable interface
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null)
        {
            target.ApplyDamage(damageAmount);
            AudioManager.Instance.PlaySFX(swingSound);
        }
    }

    public void UpgradeAttack(int amount)
    {
        damageAmount += amount;
    }
}