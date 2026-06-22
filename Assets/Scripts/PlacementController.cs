using UnityEngine;

// Gắn vào object "GameSystems".
// Click chuột trái vào ô trống -> đặt unit ĐANG CHỌN trên seed bar (nếu đủ năng lượng & hết cooldown).
public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }

    public GridManager grid;        // kéo object GridManager vào
    public float feedbackDuration = 2.2f;

    private Camera cam;
    string feedbackText = "";
    float feedbackUntil;

    public string CurrentFeedback =>
        !string.IsNullOrWhiteSpace(feedbackText) && Time.unscaledTime <= feedbackUntil ? feedbackText : "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var bar = SeedBar.Instance;

        // Bỏ qua nếu con trỏ đang ở trên thanh seed (click chọn packet, không phải đặt unit).
        float guiY = Screen.height - Input.mousePosition.y;
        if (bar != null && bar.PointerOverBar(Input.mousePosition.x, guiY)) return;
        if (OverchargeSystem.Instance != null &&
            OverchargeSystem.Instance.PointerOverPanel(Input.mousePosition.x, guiY)) return;
        if (GameUiController.Instance != null &&
            GameUiController.Instance.PointerOverPanel(Input.mousePosition.x, guiY)) return;

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0f;

        // 1. Ưu tiên nhặt mặt trời (kể cả khi chưa chọn seed nào).
        if (EnergyOrb.TryCollectAt(world)) return;

        // 2. Đặt unit đang chọn.
        if (bar == null)
        {
            SetFeedback("Command deck unavailable.");
            return;
        }

        UnitType selected = bar.Selected;
        if (selected == null || selected.prefab == null)
        {
            SetFeedback("Select a unit from the command deck.");
            return;
        }

        if (!bar.IsReady(bar.selectedIndex))
        {
            SetFeedback($"{selected.label} is cooling down: {bar.GetCooldownRemaining(bar.selectedIndex):0.0}s");
            return;
        }

        if (EnergySystem.Instance == null || !EnergySystem.Instance.CanAfford(selected.cost))
        {
            int currentEnergy = EnergySystem.Instance != null ? EnergySystem.Instance.Energy : 0;
            SetFeedback($"Need {Mathf.Max(0, selected.cost - currentEnergy)} more energy for {selected.label}.");
            return;
        }

        if (!grid.WorldToCell(world, out int col, out int row))
        {
            SetFeedback("Select a valid grid cell.");
            return;
        }

        if (grid.HasEnemyAt(col, row))
        {
            SetFeedback("Enemy blocking that cell.");
            return;
        }

        if (!grid.IsEmpty(col, row))
        {
            SetFeedback("Cell already occupied.");
            return;
        }

        Vector3 pos = grid.CellToWorld(col, row);
        pos.z = -1f;   // đẩy ra trước Tile để không bị che
        GameObject unit = Instantiate(selected.prefab, pos, Quaternion.identity);
        grid.RegisterUnit(col, row, unit);

        var shooter = unit.GetComponent<Shooter>();
        if (shooter != null)
        {
            shooter.grid = grid;
            shooter.row = row;
        }

        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplyUnit(unit, selected.prefab);

        bar.OnPlacedSelected();   // trừ năng lượng + bật cooldown
        AudioManager.PlaySfx(SfxType.UiClick);
        SetFeedback($"Placed {selected.label} in lane {row + 1}.");
    }

    void SetFeedback(string message)
    {
        feedbackText = message;
        feedbackUntil = Time.unscaledTime + Mathf.Max(0.2f, feedbackDuration);
    }
}
