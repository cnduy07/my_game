using UnityEngine;

// Gắn vào prefab "Lawnmower" (tuyến cứu cuối mỗi hàng — reskin laser cuối hàng).
// Nằm im bên trái lưới. Khi bị kích hoạt -> phóng sang phải, huỷ mọi địch cùng hàng nó chạm.
// Ra khỏi mép phải -> tự biến mất (hàng đó hết đường cứu).
public class Lawnmower : MonoBehaviour
{
    public GridManager grid;
    public int row;
    public float speed = 8f;
    public float killDistance = 0.6f;
    private bool running;

    public void Activate() { running = true; }

    void Update()
    {
        if (!running) return;

        transform.position += Vector3.right * speed * Time.deltaTime;

        // Duyệt trên bản sao: huỷ địch có thể đổi danh sách All ngay trong lúc lặp.
        var list = EnemyMover.All.ToArray();
        foreach (var e in list)
        {
            if (e == null || e.row != row) continue;
            if (Mathf.Abs(e.CurrentX - transform.position.x) <= killDistance)
            {
                var hp = e.GetComponent<Health>();
                if (hp != null) hp.TakeDamage(99999f);
            }
        }

        if (grid != null && transform.position.x > grid.origin.x + grid.cols * grid.cellSize)
        {
            if (GameManager.Instance != null) GameManager.Instance.ClearLawnmower(row);
            Destroy(gameObject);
        }
    }
}
