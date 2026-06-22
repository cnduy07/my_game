using UnityEngine;
using UnityEngine.SceneManagement;

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
        ResolveSelectedLevel();

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

    public int LevelCount => levelCatalog != null ? levelCatalog.Count : 0;

    public LevelDefinition GetLevelAt(int index)
    {
        return levelCatalog != null ? levelCatalog.GetAt(index) : null;
    }

    public bool SelectLevel(LevelDefinition level)
    {
        if (level == null) return false;
        currentLevel = level;
        PlayerProgress.SelectLevel(level);
        ApplyCurrentLevel();
        return true;
    }

    public bool SelectLevelAndReload(LevelDefinition level)
    {
        if (level == null || !PlayerProgress.IsLevelUnlocked(level)) return false;

        currentLevel = level;
        PlayerProgress.SelectLevel(level);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        return true;
    }

    public bool SelectNextLevelAndReload()
    {
        return SelectLevelAndReload(NextLevel);
    }

    void ResolveSelectedLevel()
    {
        if (levelCatalog == null) return;

        LevelDefinition selected = levelCatalog.GetById(PlayerProgress.SelectedLevelId);
        if (selected != null && PlayerProgress.IsLevelUnlocked(selected))
        {
            currentLevel = selected;
            return;
        }

        if (currentLevel == null || !PlayerProgress.IsLevelUnlocked(currentLevel))
            currentLevel = levelCatalog.GetAt(0);
    }
}
