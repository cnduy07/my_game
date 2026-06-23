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
// Runtime HUD đọc trạng thái qua API public bên dưới.
public class SeedBar : MonoBehaviour
{
    public static SeedBar Instance { get; private set; }

    public UnitType[] seeds;
    public int selectedIndex = -1;

    private float[] cdTimer;
    private UnitType[] allSeeds;

    void Awake()
    {
        Instance = this;
        EnsureAllSeedsSnapshot();
        ResetCooldowns();
    }

    void Start()
    {
        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplySeedList(allSeeds);

        ApplyLevelUnlocks(LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null);
    }

    void Update()
    {
        for (int i = 0; i < cdTimer.Length; i++)
            if (cdTimer[i] > 0f) cdTimer[i] -= Time.deltaTime;
    }

    public UnitType Selected =>
        (seeds != null && selectedIndex >= 0 && selectedIndex < seeds.Length) ? seeds[selectedIndex] : null;

    public int SeedCount => seeds != null ? seeds.Length : 0;

    public UnitType GetSeed(int i)
    {
        return seeds != null && i >= 0 && i < seeds.Length ? seeds[i] : null;
    }

    public bool IsReady(int i) => i >= 0 && i < cdTimer.Length && cdTimer[i] <= 0f;

    public float GetCooldownRemaining(int i)
    {
        return i >= 0 && i < cdTimer.Length ? Mathf.Max(0f, cdTimer[i]) : 0f;
    }

    public float GetCooldownNormalized(int i)
    {
        UnitType seed = GetSeed(i);
        if (seed == null || seed.cooldown <= 0f) return 0f;
        return Mathf.Clamp01(GetCooldownRemaining(i) / seed.cooldown);
    }

    public void SelectSeed(int i)
    {
        if (seeds == null || i < 0 || i >= seeds.Length) return;
        selectedIndex = i;
        AudioManager.PlaySfx(SfxType.UiClick);
    }

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

    public void ApplyLevelUnlocks(LevelDefinition level)
    {
        EnsureAllSeedsSnapshot();

        if (level == null || !level.HasUnitRestrictions)
        {
            seeds = allSeeds;
        }
        else
        {
            int count = 0;
            foreach (var seed in allSeeds)
                if (seed != null && level.AllowsUnit(seed.label))
                    count++;

            UnitType[] filtered = new UnitType[count];
            int write = 0;
            foreach (var seed in allSeeds)
                if (seed != null && level.AllowsUnit(seed.label))
                    filtered[write++] = seed;

            seeds = filtered;
        }

        if (selectedIndex >= SeedCount)
            selectedIndex = SeedCount > 0 ? 0 : -1;
        else if (selectedIndex < 0 && SeedCount > 0)
            selectedIndex = 0;

        ResetCooldowns();
    }

    // Chặn click "đặt unit" khi con trỏ đang nằm trên thanh seed (tránh vừa bấm nút vừa đặt).
    public bool PointerOverBar(float guiX, float guiY)
    {
        return GameUiController.Instance != null && GameUiController.Instance.PointerOverPanel(guiX, guiY);
    }

    void EnsureAllSeedsSnapshot()
    {
        if (allSeeds != null && allSeeds.Length > 0) return;
        allSeeds = seeds != null ? (UnitType[])seeds.Clone() : new UnitType[0];
    }

    void ResetCooldowns()
    {
        cdTimer = new float[seeds != null ? seeds.Length : 0];
    }
}
