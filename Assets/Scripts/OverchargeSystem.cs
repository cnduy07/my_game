using UnityEngine;

// Gắn vào GameSystems.
// Overcharge là cơ chế chủ động: tốn energy để buff một hàng trong thời gian ngắn.
public class OverchargeSystem : MonoBehaviour
{
    public static OverchargeSystem Instance { get; private set; }

    public GridManager grid;
    public int energyCost = 50;
    public float duration = 6f;
    public float fireRateMultiplier = 1.6f;
    public float damageMultiplier = 1.25f;
    public bool unlocked = true;
    public bool showDebugImGui;
    public float feedbackDuration = 2.2f;

    private float[] timers;
    private GUIStyle activeStyle;
    private GUIStyle inactiveStyle;
    string feedbackText = "";
    float feedbackUntil;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplyOverchargeSystem(this);

        if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
            SetUnlocked(LevelManager.Instance.currentLevel.overchargeUnlocked);

        EnsureTimers();
    }

    void Update()
    {
        EnsureTimers();
        if (timers == null) return;

        for (int i = 0; i < timers.Length; i++)
            if (timers[i] > 0f) timers[i] -= Time.deltaTime;

        for (int key = 0; key < timers.Length && key < 9; key++)
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + key)))
                TryActivate(key);
    }

    public static float FireRateMultiplierForRow(int row)
    {
        var sys = Instance;
        if (sys == null || !sys.unlocked || !sys.IsActive(row)) return 1f;
        return sys.fireRateMultiplier;
    }

    public static float DamageMultiplierForRow(int row)
    {
        var sys = Instance;
        if (sys == null || !sys.unlocked || !sys.IsActive(row)) return 1f;
        return sys.damageMultiplier;
    }

    public int RowCount
    {
        get
        {
            if (!unlocked) return 0;
            EnsureTimers();
            return timers != null ? timers.Length : 0;
        }
    }

    public bool IsUnlocked => unlocked;
    public string CurrentFeedback =>
        !string.IsNullOrWhiteSpace(feedbackText) && Time.unscaledTime <= feedbackUntil ? feedbackText : "";

    public bool IsActive(int row)
        => timers != null && row >= 0 && row < timers.Length && timers[row] > 0f;

    public float GetRemaining(int row)
        => timers != null && row >= 0 && row < timers.Length ? Mathf.Max(0f, timers[row]) : 0f;

    public int EnemyCountInRow(int row)
    {
        return EnemyMover.CountInRow(row);
    }

    public float LanePressure01(int row)
    {
        return EnemyMover.LanePressure01(row, grid);
    }

    public void ApplyBalance(int newEnergyCost, float newDuration, float newFireRateMultiplier, float newDamageMultiplier)
    {
        energyCost = Mathf.Max(0, newEnergyCost);
        duration = Mathf.Max(0.1f, newDuration);
        fireRateMultiplier = Mathf.Max(0.01f, newFireRateMultiplier);
        damageMultiplier = Mathf.Max(0f, newDamageMultiplier);
    }

    public void SetUnlocked(bool value)
    {
        unlocked = value;
        if (!unlocked && timers != null)
        {
            for (int i = 0; i < timers.Length; i++)
                timers[i] = 0f;
        }
    }

    public bool PointerOverPanel(float guiX, float guiY)
    {
        return GameUiController.Instance != null && GameUiController.Instance.PointerOverPanel(guiX, guiY);
    }

    public void TryActivate(int row)
    {
        if (!unlocked)
        {
            SetFeedback("Overcharge locked for this mission.");
            return;
        }
        EnsureTimers();
        if (timers == null || row < 0 || row >= timers.Length)
        {
            SetFeedback("Invalid Overcharge lane.");
            return;
        }

        if (IsActive(row))
        {
            SetFeedback($"Lane {row + 1} Overcharge already active.");
            return;
        }

        if (EnergySystem.Instance == null || !EnergySystem.Instance.CanAfford(energyCost))
        {
            int currentEnergy = EnergySystem.Instance != null ? EnergySystem.Instance.Energy : 0;
            SetFeedback($"Need {Mathf.Max(0, energyCost - currentEnergy)} more energy for Overcharge.");
            return;
        }

        EnergySystem.Instance.TrySpend(energyCost);

        timers[row] = duration;
        AudioManager.PlaySfx(SfxType.UiClick);
        SetFeedback($"Lane {row + 1} Overcharged.");
    }

    void SetFeedback(string message)
    {
        feedbackText = message;
        feedbackUntil = Time.unscaledTime + Mathf.Max(0.2f, feedbackDuration);
    }

    void OnGUI()
    {
        if (!showDebugImGui) return;
        if (!unlocked) return;
        if (timers == null) return;

        if (activeStyle == null)
        {
            activeStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold };
            inactiveStyle = new GUIStyle(GUI.skin.button);
        }

        float x = Screen.width - 118f;
        float y = 44f;
        GUI.Label(new Rect(x, y - 22f, 110f, 20f), $"OC {energyCost}");

        for (int row = timers.Length - 1; row >= 0; row--)
        {
            bool active = IsActive(row);
            string text = active ? $"{row + 1}: {timers[row]:0}" : $"{row + 1}: OC";
            Rect rect = new Rect(x, y + (timers.Length - 1 - row) * 34f, 104f, 30f);
            if (GUI.Button(rect, text, active ? activeStyle : inactiveStyle))
                TryActivate(row);
        }
    }

    void EnsureTimers()
    {
        int rowCount = grid != null ? grid.rows : 5;
        if (timers != null && timers.Length == rowCount) return;

        float[] previous = timers;
        timers = new float[rowCount];
        if (previous == null) return;

        int copyCount = Mathf.Min(previous.Length, timers.Length);
        for (int i = 0; i < copyCount; i++)
            timers[i] = previous[i];
    }
}
