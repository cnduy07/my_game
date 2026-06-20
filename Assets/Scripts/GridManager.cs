using UnityEngine;

// Gắn vào object "GridManager".
// Quản lý lưới 9 cột x 5 hàng, đổi (cột,hàng) <-> toạ độ world, theo dõi ô nào đã có Unit.
public class GridManager : MonoBehaviour
{
    public int cols = 9;
    public int rows = 5;
    public float cellSize = 1f;
    public Vector2 origin = Vector2.zero;   // world pos của ô (0,0)
    public GameObject tilePrefab;           // kéo prefab Tile vào đây

    private GameObject[,] units;

    void Awake()
    {
        units = new GameObject[cols, rows];
        SpawnTiles();
    }

    void SpawnTiles()
    {
        if (tilePrefab == null) return;
        for (int c = 0; c < cols; c++)
            for (int r = 0; r < rows; r++)
            {
                GameObject t = Instantiate(tilePrefab, CellToWorld(c, r), Quaternion.identity, transform);
                t.name = $"Tile_{c}_{r}";
            }
    }

    public Vector3 CellToWorld(int col, int row)
        => new Vector3(origin.x + col * cellSize, origin.y + row * cellSize, 0f);

    public bool WorldToCell(Vector3 world, out int col, out int row)
    {
        col = Mathf.RoundToInt((world.x - origin.x) / cellSize);
        row = Mathf.RoundToInt((world.y - origin.y) / cellSize);
        return IsInside(col, row);
    }

    public bool IsInside(int col, int row)
        => col >= 0 && col < cols && row >= 0 && row < rows;

    // Lưu ý: object đã Destroy sẽ được Unity coi như null, nên ô tự "trống" lại.
    public bool IsEmpty(int col, int row)
        => IsInside(col, row) && units[col, row] == null;

    public void RegisterUnit(int col, int row, GameObject unit)
    {
        if (IsInside(col, row)) units[col, row] = unit;
    }

    public GameObject GetUnitAt(int col, int row)
        => IsInside(col, row) ? units[col, row] : null;

    public int ColFromWorldX(float x)
        => Mathf.RoundToInt((x - origin.x) / cellSize);
}
