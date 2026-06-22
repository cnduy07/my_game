using UnityEngine;

public static class PlayerProgress
{
    const int CurrentSaveVersion = 1;
    const string SaveVersionKey = "progress.saveVersion";
    const string CompletedPrefix = "progress.completed.";
    const string HighestCompletedKey = "progress.highestCompletedLevel";
    const string SelectedLevelKey = "progress.selectedLevelId";
    const string CompletedCountKey = "progress.completedCount";
    const string LastCompletedLevelKey = "progress.lastCompletedLevelId";

    public static void MarkLevelCompleted(LevelDefinition level)
    {
        if (level == null) return;

        EnsureInitialized();
        bool alreadyCompleted = PlayerPrefs.GetInt(CompletedPrefix + level.levelId, 0) == 1;
        PlayerPrefs.SetInt(CompletedPrefix + level.levelId, 1);
        int highest = Mathf.Max(PlayerPrefs.GetInt(HighestCompletedKey, 0), level.levelNumber);
        PlayerPrefs.SetInt(HighestCompletedKey, highest);
        PlayerPrefs.SetString(LastCompletedLevelKey, level.levelId);
        if (!alreadyCompleted)
            PlayerPrefs.SetInt(CompletedCountKey, PlayerPrefs.GetInt(CompletedCountKey, 0) + 1);
        PlayerPrefs.Save();
    }

    public static bool IsLevelCompleted(LevelDefinition level)
    {
        EnsureInitialized();
        return level != null && PlayerPrefs.GetInt(CompletedPrefix + level.levelId, 0) == 1;
    }

    public static bool IsLevelUnlocked(LevelDefinition level)
    {
        EnsureInitialized();
        if (level == null) return false;
        return level.levelNumber <= HighestCompletedLevel + 1 || IsLevelCompleted(level);
    }

    public static void SelectLevel(LevelDefinition level)
    {
        if (level == null || string.IsNullOrWhiteSpace(level.levelId)) return;

        EnsureInitialized();
        PlayerPrefs.SetString(SelectedLevelKey, level.levelId);
        PlayerPrefs.Save();
    }

    public static string SelectedLevelId
    {
        get
        {
            EnsureInitialized();
            return PlayerPrefs.GetString(SelectedLevelKey, "");
        }
    }

    public static int HighestCompletedLevel
    {
        get
        {
            EnsureInitialized();
            return PlayerPrefs.GetInt(HighestCompletedKey, 0);
        }
    }

    public static int CompletedCount
    {
        get
        {
            EnsureInitialized();
            return PlayerPrefs.GetInt(CompletedCountKey, 0);
        }
    }

    public static string LastCompletedLevelId
    {
        get
        {
            EnsureInitialized();
            return PlayerPrefs.GetString(LastCompletedLevelKey, "");
        }
    }

    public static int SaveVersion
    {
        get
        {
            EnsureInitialized();
            return PlayerPrefs.GetInt(SaveVersionKey, CurrentSaveVersion);
        }
    }

    static void EnsureInitialized()
    {
        int version = PlayerPrefs.GetInt(SaveVersionKey, 0);
        if (version >= CurrentSaveVersion) return;

        if (!PlayerPrefs.HasKey(CompletedCountKey))
            PlayerPrefs.SetInt(CompletedCountKey, Mathf.Max(0, PlayerPrefs.GetInt(HighestCompletedKey, 0)));
        PlayerPrefs.SetInt(SaveVersionKey, CurrentSaveVersion);
        PlayerPrefs.Save();
    }
}
