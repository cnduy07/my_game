using UnityEngine;

// Gắn vào prefab "ArcReactor" (unit sản năng lượng — thay cho Shooter).
// Định kỳ cộng năng lượng vào EnergySystem. Đây là bản reskin của Sunflower.
public class EnergyProducer : MonoBehaviour
{
    public int amount = 25;
    public float interval = 5f;
    private float timer;

    void Update()
    {
        if (EnergySystem.Instance == null) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            // Nở ra "mặt trời" bật lên trên rồi rơi về chân reactor, chờ click nhặt.
            Vector3 spawn = transform.position + Vector3.up * 0.6f;
            spawn.z = -2f;   // z=-2: mặt trời luôn nằm trước unit/enemy
            EnergySystem.Instance.SpawnOrb(spawn, amount, transform.position.y);
        }
    }
}
