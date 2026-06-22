using UnityEngine;

// Gắn vào prefab Unit và prefab Enemy.
// Máu chung cho cả hai. Hết máu -> phát anim chết rồi mới huỷ.
// Nếu chưa có CharacterAnimator thì huỷ ngay, trừ object tĩnh bật delayDestroyWithoutAnimator.
public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float deathAnimTime = 0.8f;   // khớp độ dài clip Death; nếu không có anim thì bỏ qua
    public bool delayDestroyWithoutAnimator = false; // bật cho object tĩnh muốn hiện frame hư cuối trước khi biến mất

    private float current;
    private bool dead;

    private CharacterAnimator anim;

    public event System.Action<Health> Changed;
    public event System.Action<Health> Died;

    void Awake()
    {
        current = maxHealth;
        anim = GetComponent<CharacterAnimator>();
    }

    public void TakeDamage(float amount)
    {
        if (dead) return;
        current = Mathf.Max(0f, current - amount);
        Changed?.Invoke(this);

        if (current <= 0f) Die();
    }

    void Die()
    {
        dead = true;
        Died?.Invoke(this);

        // Ngừng hành vi để khi đang chết không còn di chuyển/bắn, và không bị nhắm bắn nữa.
        var mover = GetComponent<EnemyMover>();
        bool wasEnemy = mover != null;
        if (mover != null) mover.enabled = false;     // OnDisable tự gỡ khỏi EnemyMover.All
        var shooter = GetComponent<Shooter>();
        if (shooter != null) shooter.enabled = false;

        AudioManager.PlaySfx(wasEnemy ? SfxType.EnemyDeath : SfxType.UnitBreak);

        bool shouldDelayDestroy = deathAnimTime > 0f && (anim != null || delayDestroyWithoutAnimator);

        if (anim != null)
            anim.TriggerDie();

        if (shouldDelayDestroy)
        {
            Destroy(gameObject, deathAnimTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsAlive => !dead;

    public void SetMaxHealth(float value, bool refill)
    {
        maxHealth = Mathf.Max(1f, value);
        current = refill ? maxHealth : Mathf.Min(current, maxHealth);
        Changed?.Invoke(this);
    }

    public float Current => current;

    // Tỉ lệ máu còn lại 0..1 (cho DamageStages đổi sprite theo máu).
    public float Normalized => maxHealth > 0f ? Mathf.Clamp01(current / maxHealth) : 0f;
}
