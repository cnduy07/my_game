using UnityEngine;

// Gắn vào object "GameSystems".
// Sinh địch theo ĐỢT (wave). Mỗi đợt nhiều địch hơn + tỉ lệ địch giáp cao hơn.
// Hết địch trong đợt -> chờ sạch màn -> đợt kế. Qua đợt cuối (huge wave) -> THẮNG.
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Tham chiếu")]
    public GridManager grid;          // kéo GridManager vào
    public GameObject enemyPrefab;    // địch cơ bản
    public GameObject armoredPrefab;  // địch giáp (tuỳ chọn; trống -> luôn dùng địch cơ bản)

    [Header("Cấu hình wave")]
    public int waveCount = 5;
    public int baseEnemies = 3;             // số địch đợt 1
    public int enemiesIncreasePerWave = 2;  // mỗi đợt thêm bao nhiêu
    public int finalWaveMultiplier = 2;     // đợt cuối nhân lên thành "huge wave"
    public float startDelay = 8f;           // chờ trước đợt đầu để người chơi kịp bố trí
    public float timeBetweenSpawns = 2.5f;  // giãn cách sinh trong 1 đợt
    public float timeBetweenWaves = 12f;    // nghỉ giữa các đợt

    enum Phase { PreStart, Spawning, WaitingClear, BetweenWaves, Won }
    Phase phase = Phase.PreStart;

    int currentWave = 0;
    int toSpawn = 0;
    float timer = 0f;
    GUIStyle style;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplyEnemySpawner(this);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        switch (phase)
        {
            case Phase.PreStart:
                timer += Time.deltaTime;
                if (timer >= startDelay) BeginNextWave();
                break;

            case Phase.Spawning:
                timer += Time.deltaTime;
                if (timer >= timeBetweenSpawns)
                {
                    timer = 0f;
                    SpawnOne();
                    toSpawn--;
                    if (toSpawn <= 0) phase = Phase.WaitingClear;
                }
                break;

            case Phase.WaitingClear:
                if (EnemyMover.All.Count == 0)
                {
                    if (currentWave >= waveCount)
                    {
                        phase = Phase.Won;
                        if (GameManager.Instance != null) GameManager.Instance.Win();
                    }
                    else
                    {
                        timer = 0f;
                        phase = Phase.BetweenWaves;
                    }
                }
                break;

            case Phase.BetweenWaves:
                timer += Time.deltaTime;
                if (timer >= timeBetweenWaves) BeginNextWave();
                break;
        }
    }

    void BeginNextWave()
    {
        currentWave++;
        int count = baseEnemies + (currentWave - 1) * enemiesIncreasePerWave;
        if (currentWave >= waveCount) count *= finalWaveMultiplier;   // huge wave cuối
        toSpawn = count;
        timer = 0f;
        phase = Phase.Spawning;
        Debug.Log($"Wave {currentWave}/{waveCount} bắt đầu — {count} địch");
    }

    void SpawnOne()
    {
        int row = Random.Range(0, grid.rows);
        int col = grid.cols - 1;
        Vector3 pos = grid.CellToWorld(col, row);
        pos.z = -1f;

        // Tỉ lệ địch giáp tăng dần theo đợt, tối đa ~60%.
        float armorChance = (armoredPrefab != null)
            ? Mathf.Clamp01((currentWave - 1) / (float)Mathf.Max(1, waveCount - 1)) * 0.6f
            : 0f;
        GameObject prefab = (armoredPrefab != null && Random.value < armorChance) ? armoredPrefab : enemyPrefab;

        GameObject e = Instantiate(prefab, pos, Quaternion.identity);
        var mover = e.GetComponent<EnemyMover>();
        if (mover != null) { mover.grid = grid; mover.row = row; }

        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplyEnemy(e, prefab);
    }

    public void ApplyWaveBalance(
        int newWaveCount,
        int newBaseEnemies,
        int newEnemiesIncreasePerWave,
        int newFinalWaveMultiplier,
        float newStartDelay,
        float newTimeBetweenSpawns,
        float newTimeBetweenWaves)
    {
        waveCount = Mathf.Max(1, newWaveCount);
        baseEnemies = Mathf.Max(0, newBaseEnemies);
        enemiesIncreasePerWave = Mathf.Max(0, newEnemiesIncreasePerWave);
        finalWaveMultiplier = Mathf.Max(1, newFinalWaveMultiplier);
        startDelay = Mathf.Max(0f, newStartDelay);
        timeBetweenSpawns = Mathf.Max(0.1f, newTimeBetweenSpawns);
        timeBetweenWaves = Mathf.Max(0f, newTimeBetweenWaves);
    }

    void OnGUI()
    {
        if (phase == Phase.Won) return;

        string txt = (phase == Phase.PreStart) ? "Chuẩn bị..." : $"Wave {currentWave}/{waveCount}";
        if (style == null)
            style = new GUIStyle(GUI.skin.label)
            { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperRight };
        GUI.Label(new Rect(Screen.width - 210, 8, 200, 28), txt, style);
    }
}
