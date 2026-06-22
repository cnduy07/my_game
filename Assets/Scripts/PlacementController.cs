using UnityEngine;

// Gắn vào object "GameSystems".
// Click chuột trái vào ô trống -> đặt unit ĐANG CHỌN trên seed bar (nếu đủ năng lượng & hết cooldown).
public class PlacementController : MonoBehaviour
{
    public GridManager grid;        // kéo object GridManager vào
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
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

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0f;

        // 1. Ưu tiên nhặt mặt trời (kể cả khi chưa chọn seed nào).
        if (EnergyOrb.TryCollectAt(world)) return;

        // 2. Đặt unit đang chọn.
        if (bar == null || !bar.CanPlaceSelected()) return;

        if (grid.WorldToCell(world, out int col, out int row) && grid.IsEmpty(col, row))
        {
            Vector3 pos = grid.CellToWorld(col, row);
            pos.z = -1f;   // đẩy ra trước Tile để không bị che
            GameObject unit = Instantiate(bar.Selected.prefab, pos, Quaternion.identity);
            grid.RegisterUnit(col, row, unit);

            var shooter = unit.GetComponent<Shooter>();
            if (shooter != null)
            {
                shooter.grid = grid;
                shooter.row = row;
            }

            if (GameBalance.Instance != null)
                GameBalance.Instance.ApplyUnit(unit, bar.Selected.prefab);

            bar.OnPlacedSelected();   // trừ năng lượng + bật cooldown
            AudioManager.PlaySfx(SfxType.UiClick);
        }
    }
}
