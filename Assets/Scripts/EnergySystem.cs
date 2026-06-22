using UnityEngine;

// Gắn vào object "GameSystems".
// Quản lý tổng năng lượng: tự sinh nhỏ giọt theo thời gian, cộng/trừ khi đặt unit.
// Runtime HUD đọc Energy để hiển thị.
public class EnergySystem : MonoBehaviour
{
    public static EnergySystem Instance { get; private set; }

    public int startEnergy = 75;

    [Header("Mặt trời rơi từ trời")]
    public GameObject orbPrefab;        // kéo prefab EnergyOrb vào
    public GridManager grid;            // kéo object GridManager vào (để biết vùng rơi)
    public int skyOrbValue = 25;
    public float skyInterval = 8f;      // mỗi mấy giây rơi 1 mặt trời

    public int Energy { get; private set; }
    public bool showDebugImGui;
    private float skyTimer;
    private GUIStyle style;

    void Awake()
    {
        Instance = this;
        Energy = startEnergy;
    }

    void Start()
    {
        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplyEnergySystem(this);
    }

    void Update()
    {
        skyTimer += Time.deltaTime;
        if (skyTimer >= skyInterval)
        {
            skyTimer = 0f;
            SpawnSkyOrb();
        }
    }

    void SpawnSkyOrb()
    {
        if (orbPrefab == null || grid == null) return;
        float x = Random.Range(grid.origin.x, grid.origin.x + (grid.cols - 1) * grid.cellSize);
        float topY = grid.origin.y + grid.rows * grid.cellSize;                       // sinh phía trên lưới
        float targetY = Random.Range(grid.origin.y, grid.origin.y + (grid.rows - 1) * grid.cellSize);
        SpawnOrb(new Vector3(x, topY, -2f), skyOrbValue, targetY);   // z=-2: luôn nằm trước unit/enemy
    }

    // Dùng chung cho cả mặt trời trời lẫn ArcReactor.
    public void SpawnOrb(Vector3 pos, int value, float targetY)
    {
        if (orbPrefab == null) return;
        GameObject o = ObjectPooler.Spawn(orbPrefab, pos, Quaternion.identity);
        if (o == null) return;

        var orb = o.GetComponent<EnergyOrb>();
        if (orb != null) { orb.value = value; orb.targetY = targetY; }
    }

    public void Add(int amount) => Energy += amount;

    public void ApplyBalance(int newStartEnergy, int newSkyOrbValue, float newSkyInterval, bool resetEnergy = true)
    {
        startEnergy = Mathf.Max(0, newStartEnergy);
        skyOrbValue = Mathf.Max(0, newSkyOrbValue);
        skyInterval = Mathf.Max(0.1f, newSkyInterval);

        if (resetEnergy)
        {
            Energy = startEnergy;
            skyTimer = 0f;
        }
    }

    public bool CanAfford(int cost) => Energy >= cost;

    public bool TrySpend(int cost)
    {
        if (Energy < cost) return false;
        Energy -= cost;
        return true;
    }

    void OnGUI()
    {
        if (!showDebugImGui) return;
        if (style == null)
            style = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        GUI.Label(new Rect(10, 8, 260, 30), $"Energy: {Energy}", style);
    }
}
