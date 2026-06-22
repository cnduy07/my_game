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
    public bool useAuthoredWaves = false;
    public LevelWaveDefinition[] authoredWaves;
    public bool showDebugImGui;

    enum Phase { PreStart, Spawning, WaitingClear, BetweenWaves, Won }
    Phase phase = Phase.PreStart;

    int currentWave = 0;
    int toSpawn = 0;
    GameObject[] currentWaveQueue;
    int currentWaveQueueIndex;
    float timer = 0f;
    GUIStyle style;

    public int CurrentWave => currentWave;
    public int WaveCount => waveCount;
    public bool IsWaveWarning => (phase == Phase.PreStart && RemainingPhaseTime(startDelay) <= 3f) ||
                                 (phase == Phase.BetweenWaves && RemainingPhaseTime(timeBetweenWaves) <= 3f);
    public string DisplayText
    {
        get
        {
            switch (phase)
            {
                case Phase.PreStart:
                    return $"Deploy: {Mathf.CeilToInt(RemainingPhaseTime(startDelay))}s";
                case Phase.Spawning:
                    return $"Wave {currentWave}/{waveCount}";
                case Phase.WaitingClear:
                    return $"Clear wave {currentWave}/{waveCount}";
                case Phase.BetweenWaves:
                    return $"Next wave: {Mathf.CeilToInt(RemainingPhaseTime(timeBetweenWaves))}s";
                default:
                    return "";
            }
        }
    }

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
                if (EnemyMover.ActiveOrDyingCount == 0)
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
        currentWaveQueue = BuildWaveQueue(currentWave);
        currentWaveQueueIndex = 0;
        toSpawn = currentWaveQueue != null ? currentWaveQueue.Length : 0;
        timer = 0f;
        phase = toSpawn > 0 ? Phase.Spawning : Phase.WaitingClear;
        Debug.Log($"Wave {currentWave}/{waveCount} started - {toSpawn} enemies");
    }

    float RemainingPhaseTime(float duration)
    {
        return Mathf.Max(0f, duration - timer);
    }

    void SpawnOne()
    {
        int row = Random.Range(0, grid.rows);
        int col = grid.cols - 1;
        Vector3 pos = grid.CellToWorld(col, row);
        pos.z = -1f;

        GameObject prefab = NextEnemyPrefab();

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

    public void ApplyLevelDefinition(LevelDefinition level)
    {
        useAuthoredWaves = level != null && level.useAuthoredWaves && level.waves != null && level.waves.Length > 0;
        authoredWaves = useAuthoredWaves ? level.waves : null;
        if (useAuthoredWaves)
            waveCount = authoredWaves.Length;
    }

    GameObject[] BuildWaveQueue(int waveNumber)
    {
        if (useAuthoredWaves && authoredWaves != null && waveNumber - 1 < authoredWaves.Length)
            return BuildAuthoredWaveQueue(authoredWaves[waveNumber - 1]);

        int count = baseEnemies + (waveNumber - 1) * enemiesIncreasePerWave;
        if (waveNumber >= waveCount) count *= finalWaveMultiplier;

        var queue = new GameObject[count];
        for (int i = 0; i < queue.Length; i++)
            queue[i] = RollFormulaPrefab(waveNumber);
        return queue;
    }

    GameObject[] BuildAuthoredWaveQueue(LevelWaveDefinition wave)
    {
        if (wave == null || wave.TotalCount <= 0)
            return new GameObject[0];

        timeBetweenSpawns = Mathf.Max(0.1f, wave.timeBetweenSpawns);
        timeBetweenWaves = Mathf.Max(0f, wave.timeBeforeNextWave);

        var queue = new GameObject[wave.TotalCount];
        int index = 0;
        foreach (var group in wave.groups)
        {
            if (group == null) continue;

            for (int i = 0; i < group.count && index < queue.Length; i++)
                queue[index++] = PrefabFor(group.enemyType);
        }

        Shuffle(queue);
        return queue;
    }

    GameObject NextEnemyPrefab()
    {
        if (currentWaveQueue != null && currentWaveQueueIndex < currentWaveQueue.Length)
        {
            GameObject prefab = currentWaveQueue[currentWaveQueueIndex++];
            if (prefab != null) return prefab;
        }

        return RollFormulaPrefab(currentWave);
    }

    GameObject RollFormulaPrefab(int waveNumber)
    {
        float armorChance = (armoredPrefab != null)
            ? Mathf.Clamp01((waveNumber - 1) / (float)Mathf.Max(1, waveCount - 1)) * 0.6f
            : 0f;
        return (armoredPrefab != null && Random.value < armorChance) ? armoredPrefab : enemyPrefab;
    }

    GameObject PrefabFor(LevelEnemyType enemyType)
    {
        return enemyType == LevelEnemyType.Armored && armoredPrefab != null ? armoredPrefab : enemyPrefab;
    }

    void Shuffle(GameObject[] queue)
    {
        for (int i = 0; i < queue.Length; i++)
        {
            int swapIndex = Random.Range(i, queue.Length);
            GameObject temp = queue[i];
            queue[i] = queue[swapIndex];
            queue[swapIndex] = temp;
        }
    }

    void OnGUI()
    {
        if (!showDebugImGui) return;
        if (phase == Phase.Won) return;

        string txt = DisplayText;
        if (style == null)
            style = new GUIStyle(GUI.skin.label)
            { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperRight };
        GUI.Label(new Rect(Screen.width - 210, 8, 200, 28), txt, style);
    }
}
