using UnityEngine;

public class EnemyTraits : MonoBehaviour
{
    [Header("Damage")]
    public float projectileDamageMultiplier = 1f;
    public float empDamageMultiplier = 1f;

    [Header("Control Effects")]
    public float slowEffectMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public float stunDurationMultiplier = 1f;

    public float ModifyProjectileDamage(float damage)
    {
        return damage * Mathf.Max(0f, projectileDamageMultiplier);
    }

    public float ModifyEmpDamage(float damage)
    {
        return damage * Mathf.Max(0f, empDamageMultiplier);
    }

    public float ModifySlowFactor(float factor)
    {
        float clampedEffect = Mathf.Max(0f, slowEffectMultiplier);
        return Mathf.Clamp(Mathf.Lerp(1f, factor, clampedEffect), 0.01f, 1f);
    }

    public float ModifySlowDuration(float duration)
    {
        return duration * Mathf.Max(0f, slowEffectMultiplier);
    }

    public float ModifyKnockback(float distance)
    {
        return distance * Mathf.Max(0f, knockbackMultiplier);
    }

    public float ModifyStunDuration(float duration)
    {
        return duration * Mathf.Max(0f, stunDurationMultiplier);
    }
}
