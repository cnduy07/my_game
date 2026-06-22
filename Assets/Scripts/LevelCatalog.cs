using UnityEngine;

[CreateAssetMenu(menuName = "Lane Defense/Level Catalog", fileName = "LevelCatalog")]
public class LevelCatalog : ScriptableObject
{
    public LevelDefinition[] levels;

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
