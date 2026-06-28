using UnityEngine;

public class GameStatsTracker : MonoBehaviour
{
    public static GameStatsTracker Instance { get; private set; }

    float startTime;
    public int EnemiesKilled { get; private set; }
    public int EnergyCollected { get; private set; }

    public static GameStatsTracker EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        GameObject tracker = new GameObject("GameStatsTracker");
        return tracker.AddComponent<GameStatsTracker>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EndRunSummary.Clear();
        startTime = Time.time;
        EnemiesKilled = 0;
        EnergyCollected = 0;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void RecordEnemyKilled()
    {
        if (Instance != null)
            Instance.EnemiesKilled++;
    }

    public static void RecordEnergyCollected(int amount)
    {
        if (Instance != null && amount > 0)
            Instance.EnergyCollected += amount;
    }

    public float PlayTimeSeconds => Mathf.Max(0f, Time.time - startTime);
}
