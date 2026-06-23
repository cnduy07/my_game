using System.Collections.Generic;
using UnityEngine;

// Gắn vào prefab "EnergyOrb" (nhân bản từ Bullet, bỏ Projectile).
// Là "mặt trời": rơi xuống tới targetY rồi đứng yên chờ click. Click vào -> cộng năng lượng.
// Không dùng collider — PlacementController gọi TryCollectAt khi có click.
public class EnergyOrb : MonoBehaviour
{
    public int value = 25;
    public float fallSpeed = 1.5f;
    public float targetY = 0f;
    public float lifetime = 8f;     // sau khi tới đáy, không nhặt thì tự biến mất
    public float clickRadius = 0.6f;

    public static readonly List<EnergyOrb> All = new List<EnergyOrb>();
    private float initialLifetime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetSceneState()
    {
        All.Clear();
    }

    void Awake()
    {
        initialLifetime = lifetime;
    }

    void OnEnable()
    {
        lifetime = initialLifetime;
        All.Add(this);
    }

    void OnDisable() { All.Remove(this); }

    void Update()
    {
        if (transform.position.y > targetY)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            if (transform.position.y < targetY)
            {
                var p = transform.position; p.y = targetY; transform.position = p;
            }
            return;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f) ObjectPooler.Despawn(gameObject);
    }

    void Collect()
    {
        if (EnergySystem.Instance != null) EnergySystem.Instance.Add(value);
        AudioManager.PlaySfx(SfxType.EnergyCollect);
        ObjectPooler.Despawn(gameObject);
    }

    // Nhặt orb gần điểm click nhất trong bán kính (chỉ xét x,y — bỏ qua z). Trả true nếu nhặt được.
    public static bool TryCollectAt(Vector3 world)
    {
        EnergyOrb best = null;
        float bestSq = float.MaxValue;
        Vector2 w = world;
        foreach (var o in All)
        {
            Vector2 p = o.transform.position;
            float sq = (p - w).sqrMagnitude;
            if (sq <= o.clickRadius * o.clickRadius && sq < bestSq) { bestSq = sq; best = o; }
        }
        if (best == null) return false;
        best.Collect();
        return true;
    }
}
