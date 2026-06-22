using UnityEngine;

public static class PlayerProgress
{
    const string CompletedPrefix = "progress.completed.";
    const string HighestCompletedKey = "progress.highestCompletedLevel";

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

    public static int HighestCompletedLevel => PlayerPrefs.GetInt(HighestCompletedKey, 0);
}
