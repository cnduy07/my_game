using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelDefinition currentLevel;
    public LevelCatalog levelCatalog;
    public bool applyLevelOnAwake = true;

    void Awake()
    {
        Instance = this;

        if (applyLevelOnAwake)
            ApplyCurrentLevel();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ApplyCurrentLevel()
    {
        if (currentLevel == null) return;

        GameBalance balance = GameBalance.Instance;
        if (balance == null)
            balance = FindAnyObjectByType<GameBalance>(FindObjectsInactive.Include);

        currentLevel.ApplyTo(balance);

        var spawner = FindAnyObjectByType<EnemySpawner>(FindObjectsInactive.Include);
        if (spawner != null)
            spawner.ApplyLevelDefinition(currentLevel);
    }

    public void MarkCurrentLevelCompleted()
    {
        if (currentLevel == null) return;
        PlayerProgress.MarkLevelCompleted(currentLevel);
    }

    public LevelDefinition NextLevel =>
        levelCatalog != null ? levelCatalog.GetNext(currentLevel) : null;

    public bool HasNextLevel => NextLevel != null;

    public bool SelectLevel(LevelDefinition level)
    {
        if (level == null) return false;
        currentLevel = level;
        ApplyCurrentLevel();
        return true;
    }
}
