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

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        foreach (var e in EnemyMover.All)
        {
            if (e.row != row) continue;
            if (Mathf.Abs(e.CurrentX - transform.position.x) <= hitDistance)
            {
                var hp = e.GetComponent<Health>();
                if (hp != null) hp.TakeDamage(damage);
                if (slowDuration > 0f) e.ApplySlow(slowFactor, slowDuration);
                Destroy(gameObject);
                return;
            }
        }

        if (transform.position.x > maxX)
            Destroy(gameObject);
    }
}
