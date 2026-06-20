using UnityEngine;

// Gắn vào prefab Unit (cùng với Health).
// Chỉ bắn khi có địch cùng hàng và đang ở bên phải. Đạn bay sang phải.
public class Shooter : MonoBehaviour
{
    public GridManager grid;          // được PlacementController gán khi đặt
    public int row;                   // được PlacementController gán khi đặt
    public GameObject bulletPrefab;   // kéo prefab Bullet vào (trên prefab Unit)
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

        timer += Time.deltaTime;
        if (timer >= fireInterval)
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
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        var proj = b.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.row = row;
            proj.speed = bulletSpeed;
            proj.damage = bulletDamage;
            proj.slowFactor = bulletSlowFactor;
            proj.slowDuration = bulletSlowDuration;
        }
    }
}
