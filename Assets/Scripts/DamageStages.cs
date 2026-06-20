using UnityEngine;

// Đổi sprite theo % MÁU (như Wall-nut nứt dần) — không phụ thuộc thời gian,
// nên địch đập nhanh hay chậm đều hiển thị đúng mức hư hại.
//
// Dùng cho object KHÔNG rig (bunker, arc reactor...) có Health + SpriteRenderer.
// KHÔNG dùng cho object có Sprite Skin (đang rig) vì đổi sprite sẽ xung đột với skin.
//
// stages xếp từ NGUYÊN VẸN (máu đầy) -> HƯ NẶNG NHẤT (máu thấp).
public class DamageStages : MonoBehaviour
{
    public Health health;
    public SpriteRenderer spriteRenderer;
    public Sprite[] stages;   // [0] = nguyên vẹn, phần tử cuối = hư nặng nhất

    private int currentStage = -1;

    void Awake()
    {
        if (health == null) health = GetComponent<Health>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (health == null || spriteRenderer == null || stages == null || stages.Length == 0) return;

        // Máu 1..0  ->  chỉ số 0..len-1. Chia đều ngưỡng theo số stage.
        float f = health.Normalized;                       // 1 = đầy, 0 = chết
        int idx = Mathf.Clamp(Mathf.FloorToInt((1f - f) * stages.Length), 0, stages.Length - 1);

        if (idx != currentStage)
        {
            currentStage = idx;
            spriteRenderer.sprite = stages[idx];
        }
    }
}
