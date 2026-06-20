using UnityEngine;

// Một "seed packet": loại unit đặt được, kèm giá và thời gian hồi (cooldown).
[System.Serializable]
public class UnitType
{
    public string label = "Unit";
    public GameObject prefab;
    public int cost = 50;
    public float cooldown = 5f;
}

// Gắn vào object "GameSystems".
// Thanh chọn loại unit (seed packet). Mỗi loại có giá + cooldown riêng.
// Vẽ nút bằng IMGUI. PlacementController hỏi SeedBar.Instance để biết đang chọn gì.
public class SeedBar : MonoBehaviour
{
    public static SeedBar Instance { get; private set; }

    public UnitType[] seeds;
    public int selectedIndex = -1;

    private float[] cdTimer;

    void Awake()
    {
        Instance = this;
        cdTimer = new float[seeds != null ? seeds.Length : 0];
    }

    void Update()
    {
        for (int i = 0; i < cdTimer.Length; i++)
            if (cdTimer[i] > 0f) cdTimer[i] -= Time.deltaTime;
    }

    public UnitType Selected =>
        (seeds != null && selectedIndex >= 0 && selectedIndex < seeds.Length) ? seeds[selectedIndex] : null;

    bool IsReady(int i) => i >= 0 && i < cdTimer.Length && cdTimer[i] <= 0f;

    // PlacementController gọi trước khi đặt: phải có chọn seed, hết cooldown, đủ năng lượng.
    public bool CanPlaceSelected()
    {
        var t = Selected;
        if (t == null || t.prefab == null || !IsReady(selectedIndex)) return false;
        return EnergySystem.Instance != null && EnergySystem.Instance.CanAfford(t.cost);
    }

    // PlacementController gọi sau khi đặt thành công: trừ tiền + bật cooldown.
    public void OnPlacedSelected()
    {
        var t = Selected;
        if (t == null || EnergySystem.Instance == null) return;
        EnergySystem.Instance.TrySpend(t.cost);
        cdTimer[selectedIndex] = t.cooldown;
    }

    // Chặn click "đặt unit" khi con trỏ đang nằm trên thanh seed (tránh vừa bấm nút vừa đặt).
    public bool PointerOverBar(float guiX, float guiY)
    {
        if (seeds == null) return false;
        return new Rect(0, 44, 10 + seeds.Length * 130, 62).Contains(new Vector2(guiX, guiY));
    }

    void OnGUI()
    {
        if (seeds == null) return;
        for (int i = 0; i < seeds.Length; i++)
        {
            var s = seeds[i];
            Rect r = new Rect(10 + i * 130, 48, 120, 52);
            bool afford = EnergySystem.Instance != null && EnergySystem.Instance.CanAfford(s.cost);

            GUI.enabled = IsReady(i) && afford;
            string cd = IsReady(i) ? "" : $"  ({cdTimer[i]:0.0}s)";
            string label = $"{s.label}\n{s.cost}{cd}";

            GUI.color = (i == selectedIndex) ? Color.cyan : Color.white;
            if (GUI.Button(r, label)) selectedIndex = i;
            GUI.color = Color.white;
            GUI.enabled = true;
        }
    }
}
