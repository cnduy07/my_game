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
    public GameObject fastPrefab;     // tuỳ chọn; trống -> dùng enemyPrefab + stat modifier
    public GameObject shieldPrefab;   // tuỳ chọn; trống -> dùng armoredPrefab + stat modifier

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
    public bool balanceSpawnRows = true;
    public int maxSameRowStreak = 2;
    public bool showEnemyTypeBadges = true;
    public bool showDebugImGui;

    enum Phase { PreStart, Spawning, WaitingClear, BetweenWaves, Won }
    Phase phase = Phase.PreStart;

    int currentWave = 0;
    int toSpawn = 0;
    GameObject[] currentWaveQueue;
    LevelEnemyType[] currentWaveTypes;
    int currentWaveQueueIndex;
    float timer = 0f;
    int lastSpawnRow = -1;
    int sameRowStreak = 0;
    GUIStyle style;

    public struct EnemyTypeModifier
    {
        public float healthMultiplier;
        public float speedMultiplier;
        public float attackDamageMultiplier;
        public float projectileDamageMultiplier;
        public float empDamageMultiplier;
        public float slowEffectMultiplier;
        public float knockbackMultiplier;
        public float stunDurationMultiplier;

        public EnemyTypeModifier(
            float healthMultiplier,
            float speedMultiplier,
            float attackDamageMultiplier,
            float projectileDamageMultiplier,
            float empDamageMultiplier,
            float slowEffectMultiplier,
            float knockbackMultiplier,
            float stunDurationMultiplier)
        {
            this.healthMultiplier = healthMultiplier;
            this.speedMultiplier = speedMultiplier;
            this.attackDamageMultiplier = attackDamageMultiplier;
            this.projectileDamageMultiplier = projectileDamageMultiplier;
            this.empDamageMultiplier = empDamageMultiplier;
            this.slowEffectMultiplier = slowEffectMultiplier;
            this.knockbackMultiplier = knockbackMultiplier;
            this.stunDurationMultiplier = stunDurationMultiplier;
        }

        public static EnemyTypeModifier Identity => new EnemyTypeModifier(1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
    }

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
    public string WaveIntelText
    {
        get
        {
            if (phase == Phase.Won) return "";

            EnemyMix mix = BuildVisibleWaveMix();
            if (mix.Total <= 0) return "";

            int waveNumber = VisibleWaveNumber();
            return $"Wave {waveNumber}/{waveCount}: {CampaignIntel.BuildThreatLabel(mix)} | {CampaignIntel.BuildMixLabel(mix)}";
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
        currentWaveTypes = BuildWaveTypeQueue(currentWave);
        currentWaveQueue = BuildWaveQueue(currentWaveTypes);
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
        int row = ChooseSpawnRow();
        int col = grid.cols - 1;
        Vector3 pos = grid.CellToWorld(col, row);
        pos.z = -1f;

        GameObject prefab = NextEnemyPrefab();
        LevelEnemyType enemyType = NextEnemyType();

        GameObject e = Instantiate(prefab, pos, Quaternion.identity);
        var mover = e.GetComponent<EnemyMover>();
        if (mover != null) { mover.grid = grid; mover.row = row; }

        if (GameBalance.Instance != null)
            GameBalance.Instance.ApplyEnemy(e, prefab);
        ApplyEnemyTypeModifiers(e, enemyType);
        ApplyEnemyTypeVisualFallback(e, enemyType);
    }

    int ChooseSpawnRow()
    {
        int rowCount = grid != null ? Mathf.Max(1, grid.rows) : 5;
        int row = balanceSpawnRows ? ChooseBalancedRow(rowCount) : Random.Range(0, rowCount);

        if (row == lastSpawnRow)
            sameRowStreak++;
        else
        {
            lastSpawnRow = row;
            sameRowStreak = 1;
        }

        return row;
    }

    int ChooseBalancedRow(int rowCount)
    {
        int minCount = int.MaxValue;
        for (int row = 0; row < rowCount; row++)
            minCount = Mathf.Min(minCount, EnemyMover.CountInRow(row));

        int selected = Random.Range(0, rowCount);
        int seenCandidates = 0;
        for (int row = 0; row < rowCount; row++)
        {
            if (EnemyMover.CountInRow(row) != minCount) continue;
            if (row == lastSpawnRow && sameRowStreak >= Mathf.Max(1, maxSameRowStreak)) continue;

            seenCandidates++;
            if (Random.Range(0, seenCandidates) == 0)
                selected = row;
        }

        if (seenCandidates > 0)
            return selected;

        for (int row = 0; row < rowCount; row++)
            if (EnemyMover.CountInRow(row) == minCount)
                return row;

        return Random.Range(0, rowCount);
    }

    public void ApplyWaveBalance(
        int newWaveCount,
        int newBaseEnemies,
        int newEnemiesIncreasePerWave,
        int newFinalWaveMultiplier,
        float newStartDelay,
        float newTimeBetweenSpawns,
        float newTimeBetweenWaves,
        bool newBalanceSpawnRows,
        int newMaxSameRowStreak)
    {
        waveCount = Mathf.Max(1, newWaveCount);
        baseEnemies = Mathf.Max(0, newBaseEnemies);
        enemiesIncreasePerWave = Mathf.Max(0, newEnemiesIncreasePerWave);
        finalWaveMultiplier = Mathf.Max(1, newFinalWaveMultiplier);
        startDelay = Mathf.Max(0f, newStartDelay);
        timeBetweenSpawns = Mathf.Max(0.1f, newTimeBetweenSpawns);
        timeBetweenWaves = Mathf.Max(0f, newTimeBetweenWaves);
        balanceSpawnRows = newBalanceSpawnRows;
        maxSameRowStreak = Mathf.Max(1, newMaxSameRowStreak);
    }

    public void ApplyLevelDefinition(LevelDefinition level)
    {
        useAuthoredWaves = level != null && level.useAuthoredWaves && level.waves != null && level.waves.Length > 0;
        authoredWaves = useAuthoredWaves ? level.waves : null;
        if (useAuthoredWaves)
            waveCount = authoredWaves.Length;
    }

    GameObject[] BuildWaveQueue(LevelEnemyType[] waveTypes)
    {
        if (waveTypes == null)
            return new GameObject[0];

        var queue = new GameObject[waveTypes.Length];
        for (int i = 0; i < queue.Length; i++)
            queue[i] = PrefabFor(waveTypes[i]);
        return queue;
    }

    LevelEnemyType[] BuildWaveTypeQueue(int waveNumber)
    {
        if (useAuthoredWaves && authoredWaves != null && waveNumber - 1 < authoredWaves.Length)
            return BuildAuthoredWaveTypeQueue(authoredWaves[waveNumber - 1]);

        int count = baseEnemies + (waveNumber - 1) * enemiesIncreasePerWave;
        if (waveNumber >= waveCount) count *= finalWaveMultiplier;

        var queue = new LevelEnemyType[count];
        for (int i = 0; i < queue.Length; i++)
            queue[i] = RollFormulaType(waveNumber);
        return queue;
    }

    EnemyMix BuildVisibleWaveMix()
    {
        int waveNumber = VisibleWaveNumber();
        if (waveNumber <= 0) return new EnemyMix();

        if ((phase == Phase.Spawning || phase == Phase.WaitingClear) && currentWaveTypes != null && currentWaveTypes.Length > 0)
            return BuildMixFromQueue(currentWaveTypes);

        if (useAuthoredWaves && authoredWaves != null && waveNumber - 1 < authoredWaves.Length)
            return CampaignIntel.BuildWaveMix(authoredWaves[waveNumber - 1]);

        return BuildFormulaWaveMix(waveNumber);
    }

    int VisibleWaveNumber()
    {
        switch (phase)
        {
            case Phase.PreStart:
                return 1;
            case Phase.Spawning:
            case Phase.WaitingClear:
                return currentWave;
            case Phase.BetweenWaves:
                return Mathf.Min(currentWave + 1, waveCount);
            default:
                return 0;
        }
    }

    EnemyMix BuildMixFromQueue(LevelEnemyType[] queue)
    {
        var mix = new EnemyMix();
        if (queue == null) return mix;

        foreach (LevelEnemyType enemyType in queue)
            mix.Add(enemyType, 1);

        return mix;
    }

    EnemyMix BuildFormulaWaveMix(int waveNumber)
    {
        var mix = new EnemyMix();
        int count = baseEnemies + (waveNumber - 1) * enemiesIncreasePerWave;
        if (waveNumber >= waveCount) count *= finalWaveMultiplier;

        for (int i = 0; i < count; i++)
            mix.Add(EstimateFormulaType(waveNumber, i, count), 1);

        return mix;
    }

    LevelEnemyType EstimateFormulaType(int waveNumber, int index, int count)
    {
        float progress = Mathf.Clamp01((waveNumber - 1) / (float)Mathf.Max(1, waveCount - 1));
        if (waveNumber >= 5 && index < Mathf.RoundToInt(count * progress * 0.18f))
            return LevelEnemyType.Shield;
        if (waveNumber >= 4 && index < Mathf.RoundToInt(count * progress * 0.32f))
            return LevelEnemyType.Fast;
        if (armoredPrefab != null && index < Mathf.RoundToInt(count * progress * 0.6f))
            return LevelEnemyType.Armored;
        return LevelEnemyType.Basic;
    }

    LevelEnemyType[] BuildAuthoredWaveTypeQueue(LevelWaveDefinition wave)
    {
        if (wave == null || wave.TotalCount <= 0)
            return new LevelEnemyType[0];

        timeBetweenSpawns = Mathf.Max(0.1f, wave.timeBetweenSpawns);
        timeBetweenWaves = Mathf.Max(0f, wave.timeBeforeNextWave);

        var queue = new LevelEnemyType[wave.TotalCount];
        int index = 0;
        foreach (var group in wave.groups)
        {
            if (group == null) continue;

            for (int i = 0; i < group.count && index < queue.Length; i++)
                queue[index++] = group.enemyType;
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

    LevelEnemyType NextEnemyType()
    {
        if (currentWaveTypes != null && currentWaveQueueIndex - 1 >= 0 && currentWaveQueueIndex - 1 < currentWaveTypes.Length)
            return currentWaveTypes[currentWaveQueueIndex - 1];

        return RollFormulaType(currentWave);
    }

    GameObject RollFormulaPrefab(int waveNumber)
    {
        return PrefabFor(RollFormulaType(waveNumber));
    }

    LevelEnemyType RollFormulaType(int waveNumber)
    {
        float progress = Mathf.Clamp01((waveNumber - 1) / (float)Mathf.Max(1, waveCount - 1));
        float roll = Random.value;
        if (waveNumber >= 4 && roll < progress * 0.18f)
            return LevelEnemyType.Fast;
        if (waveNumber >= 5 && roll < progress * 0.28f)
            return LevelEnemyType.Shield;
        if (armoredPrefab != null && roll < progress * 0.6f)
            return LevelEnemyType.Armored;
        return LevelEnemyType.Basic;
    }

    public static EnemyTypeModifier GetTypeModifier(LevelEnemyType enemyType)
    {
        switch (enemyType)
        {
            case LevelEnemyType.Fast:
                return new EnemyTypeModifier(
                    healthMultiplier: 0.7f,
                    speedMultiplier: 1.55f,
                    attackDamageMultiplier: 0.8f,
                    projectileDamageMultiplier: 1.1f,
                    empDamageMultiplier: 1.15f,
                    slowEffectMultiplier: 1.25f,
                    knockbackMultiplier: 1.1f,
                    stunDurationMultiplier: 1f);
            case LevelEnemyType.Shield:
                return new EnemyTypeModifier(
                    healthMultiplier: 1.35f,
                    speedMultiplier: 0.82f,
                    attackDamageMultiplier: 1.1f,
                    projectileDamageMultiplier: 0.65f,
                    empDamageMultiplier: 1.45f,
                    slowEffectMultiplier: 0.75f,
                    knockbackMultiplier: 0.45f,
                    stunDurationMultiplier: 0.65f);
            default:
                return EnemyTypeModifier.Identity;
        }
    }

    GameObject PrefabFor(LevelEnemyType enemyType)
    {
        switch (enemyType)
        {
            case LevelEnemyType.Armored:
                return armoredPrefab != null ? armoredPrefab : enemyPrefab;
            case LevelEnemyType.Fast:
                return fastPrefab != null ? fastPrefab : enemyPrefab;
            case LevelEnemyType.Shield:
                if (shieldPrefab != null) return shieldPrefab;
                return armoredPrefab != null ? armoredPrefab : enemyPrefab;
            default:
                return enemyPrefab;
        }
    }

    void Shuffle(LevelEnemyType[] queue)
    {
        for (int i = 0; i < queue.Length; i++)
        {
            int swapIndex = Random.Range(i, queue.Length);
            LevelEnemyType temp = queue[i];
            queue[i] = queue[swapIndex];
            queue[swapIndex] = temp;
        }
    }

    void ApplyEnemyTypeModifiers(GameObject enemy, LevelEnemyType enemyType)
    {
        if (enemy == null) return;

        var health = enemy.GetComponent<Health>();
        var mover = enemy.GetComponent<EnemyMover>();
        var traits = enemy.GetComponent<EnemyTraits>();
        EnemyTypeModifier modifier = GetTypeModifier(enemyType);

        if (health != null)
            health.SetMaxHealth(health.maxHealth * modifier.healthMultiplier, true);

        if (mover != null)
        {
            mover.speed *= modifier.speedMultiplier;
            mover.attackDamage *= modifier.attackDamageMultiplier;
        }

        if (traits != null)
        {
            traits.projectileDamageMultiplier *= modifier.projectileDamageMultiplier;
            traits.empDamageMultiplier *= modifier.empDamageMultiplier;
            traits.slowEffectMultiplier *= modifier.slowEffectMultiplier;
            traits.knockbackMultiplier *= modifier.knockbackMultiplier;
            traits.stunDurationMultiplier *= modifier.stunDurationMultiplier;
        }
    }

    void ApplyEnemyTypeVisualFallback(GameObject enemy, LevelEnemyType enemyType)
    {
        if (!showEnemyTypeBadges || enemy == null || enemyType == LevelEnemyType.Basic) return;
        if (enemy.transform.Find("EnemyTypeBadge") != null) return;

        Color color;
        Vector2 size;
        switch (enemyType)
        {
            case LevelEnemyType.Fast:
                color = new Color(0.1f, 0.9f, 1f, 0.95f);
                size = new Vector2(0.12f, 0.36f);
                break;
            case LevelEnemyType.Shield:
                color = new Color(0.65f, 0.34f, 1f, 0.95f);
                size = new Vector2(0.18f, 0.42f);
                break;
            default:
                color = new Color(1f, 0.42f, 0.12f, 0.95f);
                size = new Vector2(0.14f, 0.34f);
                break;
        }

        GameObject badge = new GameObject("EnemyTypeBadge");
        badge.transform.SetParent(enemy.transform, false);
        badge.transform.localPosition = new Vector3(-0.28f, 0.34f, -0.02f);
        badge.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = badge.AddComponent<SpriteRenderer>();
        renderer.sprite = RuntimePixelSprite();
        renderer.color = color;
        renderer.sortingOrder = 18;
    }

    static Sprite runtimePixelSprite;

    static Sprite RuntimePixelSprite()
    {
        if (runtimePixelSprite != null) return runtimePixelSprite;

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        runtimePixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        runtimePixelSprite.hideFlags = HideFlags.HideAndDontSave;
        return runtimePixelSprite;
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
