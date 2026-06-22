using UnityEngine;

[CreateAssetMenu(menuName = "Lane Defense/Level Catalog", fileName = "LevelCatalog")]
public class LevelCatalog : ScriptableObject
{
    public LevelDefinition[] levels;

    public int Count => levels != null ? levels.Length : 0;

    public LevelDefinition GetAt(int index)
    {
        if (levels == null || index < 0 || index >= levels.Length) return null;
        return levels[index];
    }

    public LevelDefinition GetById(string levelId)
    {
        if (levels == null || string.IsNullOrWhiteSpace(levelId)) return null;

        foreach (var level in levels)
        {
            if (level != null && level.levelId == levelId)
                return level;
        }

        return null;
    }

    public LevelDefinition GetNext(LevelDefinition current)
    {
        if (levels == null || levels.Length == 0 || current == null) return null;

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == current && i + 1 < levels.Length)
                return levels[i + 1];
        }

        return null;
    }
}
