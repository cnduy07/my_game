using UnityEngine;

// Gắn vào prefab "DroneEMP" (nhân bản Unit, bỏ Shooter, thêm script này).
// Đặt xuống -> đếm fuse giây -> nổ gây damage mọi địch trong bán kính -> tự huỷ (ô tự trống lại).
// Reskin của Cherry Bomb. Không dùng collider — quét EnemyMover.All theo khoảng cách.
public class BombUnit : MonoBehaviour
{
    public float fuse = 1.2f;      // chờ bao lâu rồi nổ
    public float radius = 1.5f;    // bán kính nổ (world unit; 1 ô = 1 unit)
    public float damage = 1000f;   // đủ lớn để one-shot phần lớn địch
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < fuse) return;

        Explode();
        Destroy(gameObject);
    }

    void Explode()
    {
        Vector2 center = transform.position;
        // Lặp bản sao vì TakeDamage có thể Destroy -> đổi danh sách All.
        var list = EnemyMover.All.ToArray();
        foreach (var e in list)
        {
            if (e == null) continue;
            Vector2 p = e.transform.position;
            if ((p - center).sqrMagnitude <= radius * radius)
            {
                var hp = e.GetComponent<Health>();
                if (hp != null) hp.TakeDamage(damage);
            }
        }
    }
}
