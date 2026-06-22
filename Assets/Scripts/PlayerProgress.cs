using UnityEngine;

public static class PlayerProgress
{
    const string CompletedPrefix = "progress.completed.";
    const string HighestCompletedKey = "progress.highestCompletedLevel";
    const string SelectedLevelKey = "progress.selectedLevelId";

    public static void MarkLevelCompleted(LevelDefinition level)
    {
        if (level == null) return;

        PlayerPrefs.SetInt(CompletedPrefix + level.levelId, 1);
        int highest = Mathf.Max(PlayerPrefs.GetInt(HighestCompletedKey, 0), level.levelNumber);
        PlayerPrefs.SetInt(HighestCompletedKey, highest);
        PlayerPrefs.Save();
    }

    public static bool IsLevelCompleted(LevelDefinition level)
    {
        return level != null && PlayerPrefs.GetInt(CompletedPrefix + level.levelId, 0) == 1;
    }

    public static bool IsLevelUnlocked(LevelDefinition level)
    {
        if (level == null) return false;
        return level.levelNumber <= HighestCompletedLevel + 1 || IsLevelCompleted(level);
    }

    public static void SelectLevel(LevelDefinition level)
    {
        if (level == null || string.IsNullOrWhiteSpace(level.levelId)) return;

        PlayerPrefs.SetString(SelectedLevelKey, level.levelId);
        PlayerPrefs.Save();
    }

    public static string SelectedLevelId => PlayerPrefs.GetString(SelectedLevelKey, "");

    public static int HighestCompletedLevel => PlayerPrefs.GetInt(HighestCompletedKey, 0);
}
