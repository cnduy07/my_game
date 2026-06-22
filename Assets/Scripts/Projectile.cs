using UnityEngine;

// Gắn vào prefab Bullet.
// Bay sang phải, trúng địch cùng hàng (theo khoảng cách) -> gây damage rồi tự huỷ.
public class Projectile : MonoBehaviour
{
    public int row;
    public float speed = 6f;
    public float damage = 25f;
    public float hitDistance = 0.4f;
    public float maxX = 20f;

    // Đạn băng: slowFactor < 1 thì làm chậm địch trúng đạn trong slowDuration giây.
    public float slowFactor = 1f;
    public float slowDuration = 0f;
    public ProjectileHitEffect[] hitEffects;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        foreach (var e in EnemyMover.All)
        {
            if (e.row != row) continue;
            if (Mathf.Abs(e.CurrentX - transform.position.x) <= hitDistance)
            {
                CombatVfx.PlayHitSpark(transform.position);
                var hp = e.GetComponent<Health>();
                if (hp != null) hp.TakeDamage(damage);
                ApplyHitEffects(e);
                AudioManager.PlaySfx(SfxType.Hit);
                Destroy(gameObject);
                return;
            }
        }

        if (transform.position.x > maxX)
            Destroy(gameObject);
    }

    void ApplyHitEffects(EnemyMover enemy)
    {
        if (slowDuration > 0f)
            enemy.ApplySlow(slowFactor, slowDuration);

        if (hitEffects == null) return;

        foreach (var effect in hitEffects)
        {
            if (effect == null) continue;

            switch (effect.type)
            {
                case ProjectileEffectType.Slow:
                    enemy.ApplySlow(effect.value, effect.duration);
                    break;
                case ProjectileEffectType.Knockback:
                    enemy.ApplyKnockback(effect.value);
                    break;
                case ProjectileEffectType.Stun:
                    enemy.ApplyStun(effect.duration);
                    break;
            }
        }
    }
}

public enum ProjectileEffectType
{
    Slow,
    Knockback,
    Stun
}

[System.Serializable]
public class ProjectileHitEffect
{
    public ProjectileEffectType type;
    public float value = 1f;
    public float duration = 0f;
}
