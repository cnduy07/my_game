using UnityEngine;

// Gắn vào prefab Unit (cùng với Health).
// Chỉ bắn khi có địch cùng hàng và đang ở bên phải. Đạn bay sang phải.
public class Shooter : MonoBehaviour
{
    public GridManager grid;          // được PlacementController gán khi đặt
    public int row;                   // được PlacementController gán khi đặt
    public GameObject bulletPrefab;   // kéo prefab Bullet vào (trên prefab Unit)
    public Transform muzzlePoint;     // empty child đặt ở đúng đầu nòng; trống -> bắn từ root như cũ
    public float fireInterval = 1.2f;
    public float bulletSpeed = 6f;
    public float bulletDamage = 25f;

    // Để mặc định (factor=1, duration=0) cho turret thường.
    // Súng băng: đặt factor < 1 (vd 0.5) và duration > 0 (vd 3) trên Inspector.
    public float bulletSlowFactor = 1f;
    public float bulletSlowDuration = 0f;

    private float timer;
    private CharacterAnimator anim;   // null trên bản xám -> bỏ qua

    void Awake() { anim = GetComponent<CharacterAnimator>(); }

    void Update()
    {
        if (!EnemyInRowAhead())
        {
            timer = 0f;
            return;
        }

        float rateMultiplier = OverchargeSystem.FireRateMultiplierForRow(row);
        float currentInterval = fireInterval / Mathf.Max(0.01f, rateMultiplier);

        timer += Time.deltaTime;
        if (timer >= currentInterval)
        {
            timer = 0f;
            Fire();
        }
    }

    bool EnemyInRowAhead()
    {
        foreach (var e in EnemyMover.All)
            if (e.row == row && e.CurrentX > transform.position.x)
                return true;
        return false;
    }

    void Fire()
    {
        if (anim != null) anim.TriggerAttack();

        Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position;
        CombatVfx.PlayMuzzleFlash(spawnPos);

        GameObject b = ObjectPooler.Spawn(bulletPrefab, spawnPos, Quaternion.identity);
        if (b == null) return;

        var proj = b.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.row = row;
            proj.speed = bulletSpeed;
            proj.damage = bulletDamage * OverchargeSystem.DamageMultiplierForRow(row);
            proj.slowFactor = bulletSlowFactor;
            proj.slowDuration = bulletSlowDuration;
        }

        AudioManager.PlaySfx(SfxType.Shoot);
    }

    void OnDrawGizmosSelected()
    {
        Transform point = muzzlePoint != null ? muzzlePoint : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(point.position, 0.08f);
        Gizmos.DrawLine(point.position, point.position + Vector3.right * 0.35f);
    }
}
