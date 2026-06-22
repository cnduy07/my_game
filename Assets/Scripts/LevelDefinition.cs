using UnityEngine;

[CreateAssetMenu(menuName = "Lane Defense/Level Definition", fileName = "LevelDefinition")]
public class LevelDefinition : ScriptableObject
{
    public string levelId = "level_01";
    public string displayName = "Level 1";
    public int levelNumber = 1;
    [TextArea(2, 4)] public string missionBriefing = "";
    [TextArea(2, 4)] public string completionReward = "";
    public LevelBalanceSettings balance = new LevelBalanceSettings();
    public string[] allowedUnitLabels;
    public bool overchargeUnlocked = true;
    public bool useAuthoredWaves = false;
    public LevelWaveDefinition[] waves;

    public bool HasUnitRestrictions => allowedUnitLabels != null && allowedUnitLabels.Length > 0;

    public bool AllowsUnit(string label)
    {
        if (!HasUnitRestrictions) return true;
        if (string.IsNullOrWhiteSpace(label)) return false;

        foreach (string allowed in allowedUnitLabels)
        {
            if (string.Equals(allowed, label, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public void ApplyTo(GameBalance gameBalance)
    {
        if (gameBalance == null || balance == null) return;

        gameBalance.startEnergy = balance.startEnergy;
        gameBalance.skyOrbValue = balance.skyOrbValue;
        gameBalance.skyInterval = balance.skyInterval;
        gameBalance.waveCount = balance.waveCount;
        gameBalance.baseEnemies = balance.baseEnemies;
        gameBalance.enemiesIncreasePerWave = balance.enemiesIncreasePerWave;
        gameBalance.finalWaveMultiplier = balance.finalWaveMultiplier;
        gameBalance.startDelay = balance.startDelay;
        gameBalance.timeBetweenSpawns = balance.timeBetweenSpawns;
        gameBalance.timeBetweenWaves = balance.timeBetweenWaves;
        gameBalance.balanceSpawnRows = balance.balanceSpawnRows;
        gameBalance.maxSameRowStreak = balance.maxSameRowStreak;
        gameBalance.overchargeEnergyCost = balance.overchargeEnergyCost;
        gameBalance.overchargeDuration = balance.overchargeDuration;
        gameBalance.overchargeFireRateMultiplier = balance.overchargeFireRateMultiplier;
        gameBalance.overchargeDamageMultiplier = balance.overchargeDamageMultiplier;
    }
}

[System.Serializable]
public class LevelBalanceSettings
{
    public int startEnergy = 75;
    public int skyOrbValue = 25;
    public float skyInterval = 8f;

    public int waveCount = 5;
    public int baseEnemies = 3;
    public int enemiesIncreasePerWave = 2;
    public int finalWaveMultiplier = 2;
    public float startDelay = 8f;
    public float timeBetweenSpawns = 2f;
    public float timeBetweenWaves = 12f;
    public bool balanceSpawnRows = true;
    public int maxSameRowStreak = 2;

    public int overchargeEnergyCost = 50;
    public float overchargeDuration = 6f;
    public float overchargeFireRateMultiplier = 2.5f;
    public float overchargeDamageMultiplier = 1.25f;
}

public enum LevelEnemyType
{
    Basic,
    Armored,
    Fast,
    Shield
}

[System.Serializable]
public class LevelWaveDefinition
{
    public string label = "Wave";
    public float timeBetweenSpawns = 2f;
    public float timeBeforeNextWave = 12f;
    public LevelSpawnGroup[] groups;

    public int TotalCount
    {
        get
        {
            if (groups == null) return 0;

            int total = 0;
            foreach (var group in groups)
                if (group != null)
                    total += Mathf.Max(0, group.count);
            return total;
        }
    }
}

[System.Serializable]
public class LevelSpawnGroup
{
    public LevelEnemyType enemyType;
    public int count = 1;
}
