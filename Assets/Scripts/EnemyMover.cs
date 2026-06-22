using System.Collections.Generic;
using UnityEngine;

// Gắn vào prefab Enemy (cùng với Health).
// Đi sang trái. Gặp Unit cùng ô -> dừng và đập (gây damage). Vượt mép trái -> thua hàng.
public class EnemyMover : MonoBehaviour
{
    public GridManager grid;
    public int row;
    public float speed = 0.5f;
    public float attackDamage = 30f;   // damage mỗi giây lên Unit

    // Trạng thái bị làm chậm (do súng băng).
    private float slowFactor = 1f;     // 1 = bình thường, <1 = chậm
    private float slowTimer = 0f;
    public float attackSfxInterval = 0.65f;
    private float attackSfxTimer = 0f;

    private bool caught = false;       // true khi đã bị lawnmower "nhận" — đứng im chờ bị huỷ
    private CharacterAnimator anim;    // null trên bản xám -> mọi lệnh anim tự bỏ qua

    // Danh sách mọi địch đang sống, để Shooter và Projectile tra cứu (khỏi cần collider).
    public static readonly List<EnemyMover> All = new List<EnemyMover>();

    void Awake()
    {
        anim = GetComponent<CharacterAnimator>();
        // Art của địch đã vẽ sẵn quay mặt sang trái (đúng hướng di chuyển) nên KHÔNG lật.
        // Nếu sau này dùng art vẽ quay phải, gọi anim.FaceLeft(true) ở đây.
    }

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }

    // Projectile gọi khi trúng đạn băng. Lấy hệ số chậm mạnh nhất, gia hạn thời gian.
    public void ApplySlow(float factor, float duration)
    {
        if (factor < slowFactor) slowFactor = factor;
        slowTimer = Mathf.Max(slowTimer, duration);
    }

    void Update()
    {
        if (grid == null) return;
        if (caught) return;   // đã giao cho lawnmower, đứng im chờ bị huỷ

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) slowFactor = 1f;
        }

        int col = grid.ColFromWorldX(transform.position.x);
        GameObject unit = grid.GetUnitAt(col, row);

        if (unit != null)
        {
            var hp = unit.GetComponent<Health>();
            if (hp != null && hp.IsAlive)
            {
                if (anim != null) { anim.SetWalking(false); anim.TriggerAttack(); }
                attackSfxTimer -= Time.deltaTime;
                if (attackSfxTimer <= 0f)
                {
                    AudioManager.PlaySfx(SfxType.Hit);
                    attackSfxTimer = attackSfxInterval;
                }
                hp.TakeDamage(attackDamage * Time.deltaTime);
                return; // đang đập Unit thì đứng yên
            }
        }

        attackSfxTimer = 0f;
        if (anim != null) anim.SetWalking(true);
        transform.position += Vector3.left * speed * slowFactor * Time.deltaTime;

        if (transform.position.x < grid.origin.x - 0.5f)
        {
            // Hàng còn lawnmower -> nó lo; địch đứng im chờ bị huỷ.
            if (GameManager.Instance != null && GameManager.Instance.TryLawnmower(row))
            {
                caught = true;
                return;
            }

            // Hết đường cứu -> thua.
            if (GameManager.Instance != null) GameManager.Instance.GameOver(row);
            Debug.Log($"Địch vượt tuyến ở hàng {row} -> THUA HÀNG");
            Destroy(gameObject);
        }
    }

    public float CurrentX => transform.position.x;
}
