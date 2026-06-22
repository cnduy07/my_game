using System.Text;
using UnityEngine;

public enum MissionNodeType
{
    Outpost,
    ArmorGate,
    SignalRelay,
    RaiderTrack,
    ShieldColumn,
    EmpCorridor,
    VelocityNet,
    IronRain,
    BreachPoint,
    CorelineStand
}

public struct EnemyMix
{
    public int basic;
    public int armored;
    public int fast;
    public int shield;

    public int Total => basic + armored + fast + shield;
    public bool HasArmored => armored > 0;
    public bool HasFast => fast > 0;
    public bool HasShield => shield > 0;

    public void Add(LevelEnemyType type, int count)
    {
        count = Mathf.Max(0, count);
        switch (type)
        {
            case LevelEnemyType.Armored:
                armored += count;
                break;
            case LevelEnemyType.Fast:
                fast += count;
                break;
            case LevelEnemyType.Shield:
                shield += count;
                break;
            default:
                basic += count;
                break;
        }
    }

    public int Count(LevelEnemyType type)
    {
        switch (type)
        {
            case LevelEnemyType.Armored: return armored;
            case LevelEnemyType.Fast: return fast;
            case LevelEnemyType.Shield: return shield;
            default: return basic;
        }
    }
}

public static class CampaignIntel
{
    public static readonly Vector2[] DefaultMapPositions =
    {
        new Vector2(0.08f, 0.30f),
        new Vector2(0.20f, 0.52f),
        new Vector2(0.32f, 0.38f),
        new Vector2(0.44f, 0.62f),
        new Vector2(0.56f, 0.46f),
        new Vector2(0.66f, 0.26f),
        new Vector2(0.74f, 0.58f),
        new Vector2(0.83f, 0.40f),
        new Vector2(0.91f, 0.66f),
        new Vector2(0.96f, 0.34f)
    };

    public static EnemyMix BuildLevelMix(LevelDefinition level)
    {
        var mix = new EnemyMix();
        if (level == null || level.waves == null) return mix;

        foreach (LevelWaveDefinition wave in level.waves)
        {
            EnemyMix waveMix = BuildWaveMix(wave);
            mix.basic += waveMix.basic;
            mix.armored += waveMix.armored;
            mix.fast += waveMix.fast;
            mix.shield += waveMix.shield;
        }

        return mix;
    }

    public static EnemyMix BuildWaveMix(LevelWaveDefinition wave)
    {
        var mix = new EnemyMix();
        if (wave == null || wave.groups == null) return mix;

        foreach (LevelSpawnGroup group in wave.groups)
            if (group != null)
                mix.Add(group.enemyType, group.count);

        return mix;
    }

    public static string BuildMixLabel(EnemyMix mix)
    {
        if (mix.Total <= 0) return "No contacts";

        var sb = new StringBuilder();
        AppendPart(sb, "Basic", mix.basic);
        AppendPart(sb, "Armor", mix.armored);
        AppendPart(sb, "Fast", mix.fast);
        AppendPart(sb, "Shield", mix.shield);
        return sb.ToString();
    }

    public static string BuildRecommendedTools(EnemyMix mix)
    {
        var sb = new StringBuilder();

        if (mix.HasFast)
        {
            AppendTool(sb, "SnowGun");
            AppendTool(sb, "Bunker");
        }

        if (mix.HasShield)
        {
            AppendTool(sb, "DroneEMP");
            AppendTool(sb, "Overcharge");
        }

        if (mix.HasArmored)
        {
            AppendTool(sb, "Turret");
            AppendTool(sb, "DroneEMP");
        }

        if (mix.Total >= 25)
            AppendTool(sb, "ArcReactor");

        return sb.Length > 0 ? sb.ToString() : "Turret";
    }

    public static string BuildThreatLabel(EnemyMix mix)
    {
        if (mix.Total <= 0) return "No threat";

        LevelEnemyType dominant = DominantType(mix);
        switch (dominant)
        {
            case LevelEnemyType.Armored:
                return "Armored pressure";
            case LevelEnemyType.Fast:
                return "Fast breach risk";
            case LevelEnemyType.Shield:
                return "Shield column";
            default:
                return mix.Total >= 25 ? "Mass contact" : "Probe wave";
        }
    }

    public static MissionNodeType NodeTypeFor(LevelDefinition level)
    {
        if (level == null) return MissionNodeType.Outpost;

        switch (level.levelNumber)
        {
            case 2: return MissionNodeType.ArmorGate;
            case 3: return MissionNodeType.SignalRelay;
            case 4: return MissionNodeType.RaiderTrack;
            case 5: return MissionNodeType.ShieldColumn;
            case 6: return MissionNodeType.EmpCorridor;
            case 7: return MissionNodeType.VelocityNet;
            case 8: return MissionNodeType.IronRain;
            case 9: return MissionNodeType.BreachPoint;
            case 10: return MissionNodeType.CorelineStand;
            default: return MissionNodeType.Outpost;
        }
    }

    public static string NodeTypeLabel(MissionNodeType type)
    {
        switch (type)
        {
            case MissionNodeType.ArmorGate: return "Armor Gate";
            case MissionNodeType.SignalRelay: return "Signal Relay";
            case MissionNodeType.RaiderTrack: return "Raider Track";
            case MissionNodeType.ShieldColumn: return "Shield Column";
            case MissionNodeType.EmpCorridor: return "EMP Corridor";
            case MissionNodeType.VelocityNet: return "Velocity Net";
            case MissionNodeType.IronRain: return "Iron Rain";
            case MissionNodeType.BreachPoint: return "Breach Point";
            case MissionNodeType.CorelineStand: return "Coreline Stand";
            default: return "Outpost";
        }
    }

    public static int PressureScore(LevelDefinition level)
    {
        EnemyMix mix = BuildLevelMix(level);
        float score = mix.basic + mix.armored * 1.7f + mix.fast * 1.35f + mix.shield * 1.9f;

        if (level != null && level.waves != null)
            score += level.waves.Length * 3f;

        return Mathf.RoundToInt(score);
    }

    public static LevelEnemyType DominantType(EnemyMix mix)
    {
        LevelEnemyType dominant = LevelEnemyType.Basic;
        int count = mix.basic;

        if (mix.armored > count)
        {
            dominant = LevelEnemyType.Armored;
            count = mix.armored;
        }

        if (mix.fast > count)
        {
            dominant = LevelEnemyType.Fast;
            count = mix.fast;
        }

        if (mix.shield > count)
            dominant = LevelEnemyType.Shield;

        return dominant;
    }

    static void AppendPart(StringBuilder sb, string label, int count)
    {
        if (count <= 0) return;
        if (sb.Length > 0) sb.Append(" / ");
        sb.Append(label).Append(" x").Append(count);
    }

    static void AppendTool(StringBuilder sb, string tool)
    {
        string existing = sb.ToString();
        if (existing == tool || existing.Contains(", " + tool) || existing.StartsWith(tool + ", "))
            return;

        if (sb.Length > 0) sb.Append(", ");
        sb.Append(tool);
    }
}
