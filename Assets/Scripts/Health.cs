using UnityEngine;

// Gắn vào prefab Unit và prefab Enemy.
// Máu chung cho cả hai. Hết máu -> phát anim chết rồi mới huỷ.
// Nếu chưa có CharacterAnimator (bản xám) thì huỷ ngay như cũ.
public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float deathAnimTime = 0.8f;   // khớp độ dài clip Death; nếu không có anim thì bỏ qua
    private float current;
    private bool dead;

    private CharacterAnimator anim;

    void Awake()
    {
        current = maxHealth;
        anim = GetComponent<CharacterAnimator>();
    }

    public void TakeDamage(float amount)
    {
        if (dead) return;
        current -= amount;
        if (current <= 0f) Die();
    }

    void Die()
    {
        dead = true;

        // Ngừng hành vi để khi đang chết không còn di chuyển/bắn, và không bị nhắm bắn nữa.
        var mover = GetComponent<EnemyMover>();
        if (mover != null) mover.enabled = false;     // OnDisable tự gỡ khỏi EnemyMover.All
        var shooter = GetComponent<Shooter>();
        if (shooter != null) shooter.enabled = false;

        if (anim != null && deathAnimTime > 0f)
        {
            anim.TriggerDie();
            Destroy(gameObject, deathAnimTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsAlive => !dead;

    // Tỉ lệ máu còn lại 0..1 (cho DamageStages đổi sprite theo máu).
    public float Normalized => maxHealth > 0f ? Mathf.Clamp01(current / maxHealth) : 0f;
}
