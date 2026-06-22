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

    private float[] timers;
    private GUIStyle activeStyle;
    private GUIStyle inactiveStyle;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int rowCount = grid != null ? grid.rows : 5;
        timers = new float[rowCount];
    }

    void Update()
    {
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
        if (sys == null || !sys.IsActive(row)) return 1f;
        return sys.fireRateMultiplier;
    }

    public static float DamageMultiplierForRow(int row)
    {
        var sys = Instance;
        if (sys == null || !sys.IsActive(row)) return 1f;
        return sys.damageMultiplier;
    }

    bool IsActive(int row)
        => timers != null && row >= 0 && row < timers.Length && timers[row] > 0f;

    public bool PointerOverPanel(float guiX, float guiY)
    {
        if (timers == null) return false;

        float x = Screen.width - 118f;
        float y = 22f;
        float h = 34f * timers.Length + 44f;
        return guiX >= x && guiX <= x + 110f && guiY >= y && guiY <= y + h;
    }

    void TryActivate(int row)
    {
        if (timers == null || row < 0 || row >= timers.Length) return;
        if (IsActive(row)) return;
        if (EnergySystem.Instance == null || !EnergySystem.Instance.TrySpend(energyCost)) return;

        timers[row] = duration;
        AudioManager.PlaySfx(SfxType.UiClick);
    }

    void OnGUI()
    {
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
}
