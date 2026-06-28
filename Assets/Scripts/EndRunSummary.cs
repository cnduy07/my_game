using UnityEngine;

public static class EndRunSummary
{
    public static bool HasData { get; private set; }
    public static bool Won { get; private set; }
    public static int BreachedRow { get; private set; }
    public static string MissionName { get; private set; } = "Unknown Sector";
    public static int MissionNumber { get; private set; }
    public static int WaveReached { get; private set; }
    public static int WaveTotal { get; private set; }
    public static int EnemiesKilled { get; private set; }
    public static int EnergyCollected { get; private set; }
    public static float PlayTimeSeconds { get; private set; }
    public static string Intel { get; private set; } = "";

    public static void Clear()
    {
        HasData = false;
        Won = false;
        BreachedRow = -1;
        MissionName = "Unknown Sector";
        MissionNumber = 0;
        WaveReached = 0;
        WaveTotal = 0;
        EnemiesKilled = 0;
        EnergyCollected = 0;
        PlayTimeSeconds = 0f;
        Intel = "";
    }

    public static void Capture(bool won, int breachedRow)
    {
        Won = won;
        BreachedRow = breachedRow;

        LevelDefinition level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        MissionName = level != null && !string.IsNullOrWhiteSpace(level.displayName) ? level.displayName : "Unknown Sector";
        MissionNumber = level != null ? level.levelNumber : 0;
        Intel = level != null && !string.IsNullOrWhiteSpace(level.missionBriefing)
            ? level.missionBriefing
            : (won ? "Sector secured. Route remains online." : "Coreline breached. Rebuild the defense grid.");

        EnemySpawner spawner = EnemySpawner.Instance;
        WaveTotal = spawner != null ? Mathf.Max(1, spawner.WaveCount) : 1;
        WaveReached = spawner != null ? spawner.ReportWaveReached(won) : (won ? WaveTotal : 0);

        GameStatsTracker stats = GameStatsTracker.Instance;
        EnemiesKilled = stats != null ? stats.EnemiesKilled : 0;
        EnergyCollected = stats != null ? stats.EnergyCollected : 0;
        PlayTimeSeconds = stats != null ? stats.PlayTimeSeconds : 0f;
        HasData = true;
    }
}
