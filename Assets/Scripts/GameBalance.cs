using UnityEngine;

// Central balance/tuning point for the playable prototype.
// Keep prefab defaults sane, then override gameplay-facing numbers here at runtime.
public class GameBalance : MonoBehaviour
{
    public static GameBalance Instance { get; private set; }

    [Header("Units")]
    public UnitBalance[] units;

    [Header("Enemies")]
    public EnemyBalance[] enemies;

    [Header("Energy")]
    public int startEnergy = 75;
    public int skyOrbValue = 25;
    public float skyInterval = 8f;

    [Header("Waves")]
    public int waveCount = 5;
    public int baseEnemies = 3;
    public int enemiesIncreasePerWave = 2;
    public int finalWaveMultiplier = 2;
    public float startDelay = 8f;
    public float timeBetweenSpawns = 2f;
    public float timeBetweenWaves = 12f;

    [Header("Overcharge")]
    public int overchargeEnergyCost = 50;
    public float overchargeDuration = 6f;
    public float overchargeFireRateMultiplier = 1.6f;
    public float overchargeDamageMultiplier = 1.25f;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnValidate()
    {
        ClampValues();

        if (Application.isPlaying)
            ApplyRuntimeTuning();
    }

    void ClampValues()
    {
        startEnergy = Mathf.Max(0, startEnergy);
        skyOrbValue = Mathf.Max(0, skyOrbValue);
        skyInterval = Mathf.Max(0.1f, skyInterval);
        waveCount = Mathf.Max(1, waveCount);
        baseEnemies = Mathf.Max(0, baseEnemies);
        enemiesIncreasePerWave = Mathf.Max(0, enemiesIncreasePerWave);
        finalWaveMultiplier = Mathf.Max(1, finalWaveMultiplier);
        startDelay = Mathf.Max(0f, startDelay);
        timeBetweenSpawns = Mathf.Max(0.1f, timeBetweenSpawns);
        timeBetweenWaves = Mathf.Max(0f, timeBetweenWaves);
        overchargeEnergyCost = Mathf.Max(0, overchargeEnergyCost);
        overchargeDuration = Mathf.Max(0.1f, overchargeDuration);
        overchargeFireRateMultiplier = Mathf.Max(0.01f, overchargeFireRateMultiplier);
        overchargeDamageMultiplier = Mathf.Max(0f, overchargeDamageMultiplier);
    }

    public void ApplyRuntimeTuning()
    {
        ApplySeedBar(SeedBar.Instance);

        if (EnergySystem.Instance != null)
            EnergySystem.Instance.ApplyBalance(startEnergy, skyOrbValue, skyInterval, false);

        if (EnemySpawner.Instance != null)
            ApplyEnemySpawner(EnemySpawner.Instance);

        ApplyOverchargeSystem(OverchargeSystem.Instance);

        var identities = Object.FindObjectsByType<BalanceIdentity>(FindObjectsInactive.Exclude);
        foreach (var identity in identities)
        {
            if (identity == null || identity.sourcePrefab == null) continue;

            if (identity.isEnemy)
                ApplyEnemy(identity.gameObject, identity.sourcePrefab, false);
            else
                ApplyUnit(identity.gameObject, identity.sourcePrefab, false);
        }
    }

    public void ApplySeedBar(SeedBar seedBar)
    {
        if (seedBar == null || seedBar.seeds == null || units == null) return;

        foreach (var seed in seedBar.seeds)
        {
            UnitBalance balance = FindUnit(seed.prefab);
            if (balance == null) continue;

            if (!string.IsNullOrWhiteSpace(balance.label)) seed.label = balance.label;
            seed.cost = Mathf.Max(0, balance.cost);
            seed.cooldown = Mathf.Max(0f, balance.cooldown);
        }
    }

    public void ApplyUnit(GameObject unit, GameObject sourcePrefab, bool refillHealth = true)
    {
        UnitBalance balance = FindUnit(sourcePrefab);
        if (unit == null || balance == null) return;

        RememberSource(unit, sourcePrefab, false);

        var health = unit.GetComponent<Health>();
        if (health != null)
        {
            health.deathAnimTime = Mathf.Max(0f, balance.deathAnimTime);
            health.SetMaxHealth(balance.maxHealth, refillHealth);
        }

        var shooter = unit.GetComponent<Shooter>();
        if (shooter != null)
        {
            shooter.fireInterval = Mathf.Max(0.01f, balance.fireInterval);
            shooter.bulletSpeed = balance.bulletSpeed;
            shooter.bulletDamage = balance.bulletDamage;
            shooter.bulletSlowFactor = Mathf.Clamp(balance.bulletSlowFactor, 0.01f, 1f);
            shooter.bulletSlowDuration = Mathf.Max(0f, balance.bulletSlowDuration);
        }

        var producer = unit.GetComponent<EnergyProducer>();
        if (producer != null)
        {
            producer.amount = Mathf.Max(0, balance.energyAmount);
            producer.interval = Mathf.Max(0.1f, balance.energyInterval);
        }

        var bomb = unit.GetComponent<BombUnit>();
        if (bomb != null)
        {
            bomb.fuse = Mathf.Max(0f, balance.empFuse);
            bomb.radius = Mathf.Max(0f, balance.empRadius);
            bomb.damage = Mathf.Max(0f, balance.empDamage);
        }
    }

    public void ApplyEnemy(GameObject enemy, GameObject sourcePrefab, bool refillHealth = true)
    {
        EnemyBalance balance = FindEnemy(sourcePrefab);
        if (enemy == null || balance == null) return;

        RememberSource(enemy, sourcePrefab, true);

        var health = enemy.GetComponent<Health>();
        if (health != null)
        {
            health.deathAnimTime = Mathf.Max(0f, balance.deathAnimTime);
            health.SetMaxHealth(balance.maxHealth, refillHealth);
        }

        var mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.speed = Mathf.Max(0f, balance.speed);
            mover.attackDamage = Mathf.Max(0f, balance.attackDamage);
            mover.attackSfxInterval = Mathf.Max(0.05f, balance.attackSfxInterval);
        }
    }

    public void ApplyEnergySystem(EnergySystem energySystem)
    {
        if (energySystem == null) return;
        energySystem.ApplyBalance(startEnergy, skyOrbValue, skyInterval);
    }

    public void ApplyEnemySpawner(EnemySpawner spawner)
    {
        if (spawner == null) return;
        spawner.ApplyWaveBalance(
            waveCount,
            baseEnemies,
            enemiesIncreasePerWave,
            finalWaveMultiplier,
            startDelay,
            timeBetweenSpawns,
            timeBetweenWaves);
    }

    public void ApplyOverchargeSystem(OverchargeSystem overcharge)
    {
        if (overcharge == null) return;
        overcharge.ApplyBalance(
            overchargeEnergyCost,
            overchargeDuration,
            overchargeFireRateMultiplier,
            overchargeDamageMultiplier);
    }

    UnitBalance FindUnit(GameObject prefab)
    {
        if (prefab == null || units == null) return null;
        foreach (var entry in units)
            if (entry != null && entry.prefab == prefab)
                return entry;
        return null;
    }

    EnemyBalance FindEnemy(GameObject prefab)
    {
        if (prefab == null || enemies == null) return null;
        foreach (var entry in enemies)
            if (entry != null && entry.prefab == prefab)
                return entry;
        return null;
    }

    void RememberSource(GameObject target, GameObject sourcePrefab, bool isEnemy)
    {
        var identity = target.GetComponent<BalanceIdentity>();
        if (identity == null) identity = target.AddComponent<BalanceIdentity>();
        identity.sourcePrefab = sourcePrefab;
        identity.isEnemy = isEnemy;
    }
}

[System.Serializable]
public class UnitBalance
{
    public string label = "Unit";
    public GameObject prefab;
    public int cost = 50;
    public float cooldown = 5f;
    public float maxHealth = 100f;
    public float deathAnimTime = 0.8f;

    [Header("Shooter")]
    public float fireInterval = 1.2f;
    public float bulletSpeed = 6f;
    public float bulletDamage = 25f;
    public float bulletSlowFactor = 1f;
    public float bulletSlowDuration = 0f;

    [Header("ArcReactor")]
    public int energyAmount = 25;
    public float energyInterval = 5f;

    [Header("DroneEMP")]
    public float empFuse = 1.2f;
    public float empRadius = 1.5f;
    public float empDamage = 1000f;
}

[System.Serializable]
public class EnemyBalance
{
    public string label = "Enemy";
    public GameObject prefab;
    public float maxHealth = 100f;
    public float deathAnimTime = 0.8f;
    public float speed = 0.3f;
    public float attackDamage = 25f;
    public float attackSfxInterval = 1.1f;
}
