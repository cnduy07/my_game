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
    public bool showFinalStageBeforeDestroy = true;

    private int currentStage = -1;

    void Awake()
    {
        ResolveReferences();

        if (health != null && showFinalStageBeforeDestroy)
            health.delayDestroyWithoutAnimator = true;
    }

    void OnEnable()
    {
        ResolveReferences();

        if (health == null) return;
        health.Changed += UpdateStage;
        health.Died += HandleDied;
    }

    void Start()
    {
        UpdateStage(health);
    }

    void OnDisable()
    {
        if (health == null) return;
        health.Changed -= UpdateStage;
        health.Died -= HandleDied;
    }

    void ResolveReferences()
    {
        if (health == null) health = GetComponent<Health>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void UpdateStage(Health source)
    {
        if (source == null || spriteRenderer == null || stages == null || stages.Length == 0) return;

        // Máu 1..0  ->  chỉ số 0..len-1. Chia đều ngưỡng theo số stage.
        float f = source.Normalized;                       // 1 = đầy, 0 = chết
        int idx = Mathf.Clamp(Mathf.FloorToInt((1f - f) * stages.Length), 0, stages.Length - 1);
        SetStage(idx);
    }

    void HandleDied(Health source)
    {
        if (stages == null || stages.Length == 0) return;
        SetStage(stages.Length - 1);
    }

    void SetStage(int idx)
    {
        if (spriteRenderer == null || stages == null || stages.Length == 0) return;
        idx = Mathf.Clamp(idx, 0, stages.Length - 1);

        if (idx != currentStage)
        {
            currentStage = idx;
            spriteRenderer.sprite = stages[idx];
        }
    }
}
