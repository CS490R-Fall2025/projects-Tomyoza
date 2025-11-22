using UnityEngine;

public interface IDamageable
{
    // Any script that implements this MUST have an ApplyDamage function
    void ApplyDamage(int amount);
}
