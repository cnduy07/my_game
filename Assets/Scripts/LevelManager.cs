using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelDefinition currentLevel;
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
    }

    public void MarkCurrentLevelCompleted()
    {
        if (currentLevel == null) return;
        PlayerProgress.MarkLevelCompleted(currentLevel);
    }
}
