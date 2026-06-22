using System.Text;
using UnityEngine;

public static class BalanceReportGenerator
{
    public static string BuildMarkdown(GameBalance balance)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Balance Metrics");
        sb.AppendLine();

        if (balance == null)
        {
            sb.AppendLine("GameBalance not found.");
            return sb.ToString();
        }

        AppendGlobal(sb, balance);
        AppendCombat(sb, balance);
        AppendEconomy(sb, balance);
        AppendEnemyTypeTuning(sb);
        AppendLevel(sb);
        return sb.ToString();
    }

    public static string BuildJson(GameBalance balance)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        if (balance == null)
        {
            sb.AppendLine("  \"error\": \"GameBalance not found\"");
            sb.AppendLine("}");
            return sb.ToString();
        }

        sb.AppendLine($"  \"startEnergy\": {balance.startEnergy},");
        sb.AppendLine($"  \"skyOrbValue\": {balance.skyOrbValue},");
        sb.AppendLine($"  \"skyInterval\": {Format(balance.skyInterval)},");
        sb.AppendLine($"  \"overchargeFireRateMultiplier\": {Format(balance.overchargeFireRateMultiplier)},");
        sb.AppendLine($"  \"overchargeDamageMultiplier\": {Format(balance.overchargeDamageMultiplier)},");
        sb.AppendLine($"  \"waveCount\": {balance.waveCount},");
        sb.AppendLine($"  \"baseEnemies\": {balance.baseEnemies},");
        sb.AppendLine($"  \"enemiesIncreasePerWave\": {balance.enemiesIncreasePerWave},");
        sb.AppendLine($"  \"finalWaveMultiplier\": {balance.finalWaveMultiplier}");
        sb.AppendLine("}");
        return sb.ToString();
    }

    static void AppendGlobal(StringBuilder sb, GameBalance balance)
    {
        sb.AppendLine("## Global");
        sb.AppendLine($"- Start energy: `{balance.startEnergy}`");
        sb.AppendLine($"- Sky orb: `{balance.skyOrbValue}` every `{Format(balance.skyInterval)}s`");
        sb.AppendLine($"- Waves: `{balance.waveCount}`, base `{balance.baseEnemies}`, +`{balance.enemiesIncreasePerWave}` per wave, final x`{balance.finalWaveMultiplier}`");
        sb.AppendLine($"- Overcharge: cost `{balance.overchargeEnergyCost}`, duration `{Format(balance.overchargeDuration)}s`, fire rate x`{Format(balance.overchargeFireRateMultiplier)}`, damage x`{Format(balance.overchargeDamageMultiplier)}`");
        sb.AppendLine();
    }

    static void AppendCombat(StringBuilder sb, GameBalance balance)
    {
        sb.AppendLine("## Combat Estimates");
        if (balance.units == null || balance.enemies == null)
        {
            sb.AppendLine("- Missing unit or enemy entries.");
            sb.AppendLine();
            return;
        }

        foreach (var unit in balance.units)
        {
            if (unit == null || unit.bulletDamage <= 0f || unit.fireInterval <= 0f) continue;
            bool looksLikeShooter = unit.bulletDamage > 0f && unit.prefab != null && unit.prefab.GetComponent<Shooter>() != null;
            if (!looksLikeShooter) continue;

            sb.AppendLine($"### {unit.label}");
            foreach (var enemy in balance.enemies)
            {
                if (enemy == null || enemy.maxHealth <= 0f) continue;
                float effectiveDamage = unit.bulletDamage * Mathf.Max(0f, enemy.projectileDamageMultiplier);
                int shots = Mathf.CeilToInt(enemy.maxHealth / Mathf.Max(0.01f, effectiveDamage));
                float ttk = shots * unit.fireInterval;
                float ocDamage = effectiveDamage * Mathf.Max(0f, balance.overchargeDamageMultiplier);
                float ocInterval = unit.fireInterval / Mathf.Max(0.01f, balance.overchargeFireRateMultiplier);
                int ocShots = Mathf.CeilToInt(enemy.maxHealth / Mathf.Max(0.01f, ocDamage));
                float ocTtk = ocShots * ocInterval;
                sb.AppendLine($"- vs {enemy.label}: `{shots}` shots, ~`{Format(ttk)}s`; Overcharge ~`{Format(ocTtk)}s`");
            }
            sb.AppendLine();
        }

        var bunker = FindUnit(balance, "Bunker");
        if (bunker != null)
        {
            sb.AppendLine("### Bunker survivability");
            foreach (var enemy in balance.enemies)
            {
                if (enemy == null || enemy.attackDamage <= 0f) continue;
                float surviveTime = bunker.maxHealth / enemy.attackDamage;
                sb.AppendLine($"- vs {enemy.label}: ~`{Format(surviveTime)}s` of continuous attack");
            }
            sb.AppendLine();
        }

        sb.AppendLine("### Enemy traits");
        foreach (var enemy in balance.enemies)
        {
            if (enemy == null) continue;
            sb.AppendLine($"- {enemy.label}: projectile x`{Format(enemy.projectileDamageMultiplier)}`, EMP x`{Format(enemy.empDamageMultiplier)}`, slow x`{Format(enemy.slowEffectMultiplier)}`, knockback x`{Format(enemy.knockbackMultiplier)}`, stun x`{Format(enemy.stunDurationMultiplier)}`");
        }
        sb.AppendLine();
    }

    static void AppendEnemyTypeTuning(StringBuilder sb)
    {
        sb.AppendLine("## Enemy Type Tuning");
        AppendEnemyType(sb, LevelEnemyType.Basic);
        AppendEnemyType(sb, LevelEnemyType.Armored);
        AppendEnemyType(sb, LevelEnemyType.Fast);
        AppendEnemyType(sb, LevelEnemyType.Shield);
        sb.AppendLine();
    }

    static void AppendEnemyType(StringBuilder sb, LevelEnemyType enemyType)
    {
        EnemySpawner.EnemyTypeModifier modifier = EnemySpawner.GetTypeModifier(enemyType);
        sb.AppendLine($"- {enemyType}: health x`{Format(modifier.healthMultiplier)}`, speed x`{Format(modifier.speedMultiplier)}`, attack x`{Format(modifier.attackDamageMultiplier)}`, projectile x`{Format(modifier.projectileDamageMultiplier)}`, EMP x`{Format(modifier.empDamageMultiplier)}`, slow x`{Format(modifier.slowEffectMultiplier)}`, knockback x`{Format(modifier.knockbackMultiplier)}`, stun x`{Format(modifier.stunDurationMultiplier)}`");
    }

    static void AppendEconomy(StringBuilder sb, GameBalance balance)
    {
        sb.AppendLine("## Economy Estimates");
        float skyPerSecond = balance.skyInterval > 0f ? balance.skyOrbValue / balance.skyInterval : 0f;
        sb.AppendLine($"- Passive sky income: ~`{Format(skyPerSecond)}` energy/sec");

        var reactor = FindUnit(balance, "ArcReactor");
        if (reactor != null && reactor.energyInterval > 0f)
        {
            float reactorPerSecond = reactor.energyAmount / reactor.energyInterval;
            float payback = reactorPerSecond > 0f ? reactor.cost / reactorPerSecond : 0f;
            sb.AppendLine($"- ArcReactor income: ~`{Format(reactorPerSecond)}` energy/sec, payback ~`{Format(payback)}s`");
        }

        sb.AppendLine();
    }

    static void AppendLevel(StringBuilder sb)
    {
        var levelManager = Object.FindAnyObjectByType<LevelManager>(FindObjectsInactive.Include);
        var level = levelManager != null ? levelManager.currentLevel : null;
        if (level == null) return;

        sb.AppendLine("## Level");
        sb.AppendLine($"- Current level: `{level.displayName}` (`{level.levelId}`)");
        sb.AppendLine($"- Authored waves: `{(level.useAuthoredWaves ? "on" : "off")}`");

        if (level.waves != null && level.waves.Length > 0)
        {
            for (int i = 0; i < level.waves.Length; i++)
            {
                var wave = level.waves[i];
                if (wave == null) continue;
                sb.AppendLine($"- Wave {i + 1}: `{wave.label}`, `{wave.TotalCount}` enemies{BuildGroupSummary(wave)}");
            }
        }

        sb.AppendLine();
    }

    static string BuildGroupSummary(LevelWaveDefinition wave)
    {
        if (wave == null || wave.groups == null || wave.groups.Length == 0)
            return "";

        var summary = new StringBuilder(" — ");
        bool wroteAny = false;
        foreach (var group in wave.groups)
        {
            if (group == null || group.count <= 0) continue;
            if (wroteAny) summary.Append(", ");
            summary.Append($"{group.enemyType} x{group.count}");
            wroteAny = true;
        }

        return wroteAny ? summary.ToString() : "";
    }

    static UnitBalance FindUnit(GameBalance balance, string label)
    {
        if (balance == null || balance.units == null) return null;
        foreach (var unit in balance.units)
            if (unit != null && unit.label == label)
                return unit;
        return null;
    }

    static string Format(float value)
    {
        return value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
    }
}
