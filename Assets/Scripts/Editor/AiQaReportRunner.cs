using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AiQaReportRunner
{
    const string DefaultScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/AI QA/Run Full Check")]
    public static void RunFullCheck()
    {
        EnsureSceneLoaded();

        var checks = new List<CheckResult>();
        GameBalance balance = UnityEngine.Object.FindAnyObjectByType<GameBalance>(FindObjectsInactive.Include);
        AudioManager audio = UnityEngine.Object.FindAnyObjectByType<AudioManager>(FindObjectsInactive.Include);
        LevelManager levelManager = UnityEngine.Object.FindAnyObjectByType<LevelManager>(FindObjectsInactive.Include);
        GameUiController ui = UnityEngine.Object.FindAnyObjectByType<GameUiController>(FindObjectsInactive.Include);
        CombatVfxSettings vfxSettings = UnityEngine.Object.FindAnyObjectByType<CombatVfxSettings>(FindObjectsInactive.Include);
        TutorialCoach tutorialCoach = UnityEngine.Object.FindAnyObjectByType<TutorialCoach>(FindObjectsInactive.Include);
        RuntimeQualitySettings runtimeQuality = UnityEngine.Object.FindAnyObjectByType<RuntimeQualitySettings>(FindObjectsInactive.Include);

        CheckGameBalance(balance, checks);
        CheckLevelManager(levelManager, balance, checks);
        CheckUi(ui, checks);
        CheckRuntimeFoundations(vfxSettings, tutorialCoach, runtimeQuality, checks);
        CheckAudio(audio, checks);
        CheckPrefabs(balance, checks);
        CheckAnimatorContracts(balance, checks);
        CheckProjectReadiness(levelManager, checks);

        WriteReports(balance, checks);
    }

    static void CheckUi(GameUiController ui, List<CheckResult> checks)
    {
        if (ui == null)
        {
            checks.Add(CheckResult.Warn("UI", "No GameUiController found in loaded scene."));
            return;
        }

        if (!ui.enabled)
            checks.Add(CheckResult.Fail("UI", "GameUiController is disabled."));
    }

    static void EnsureSceneLoaded()
    {
        if (UnityEngine.Object.FindAnyObjectByType<GameBalance>(FindObjectsInactive.Include) != null)
            return;

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(DefaultScenePath) != null)
            EditorSceneManager.OpenScene(DefaultScenePath, OpenSceneMode.Single);
    }

    static void CheckGameBalance(GameBalance balance, List<CheckResult> checks)
    {
        if (balance == null)
        {
            checks.Add(CheckResult.Fail("GameBalance", "No GameBalance found in loaded scene."));
            return;
        }

        if (balance.units == null || balance.units.Length == 0)
            checks.Add(CheckResult.Fail("GameBalance", "No unit balance entries."));
        if (balance.enemies == null || balance.enemies.Length == 0)
            checks.Add(CheckResult.Fail("GameBalance", "No enemy balance entries."));
        CheckUnitProjectileEffects(balance, checks);
        if (balance.overchargeFireRateMultiplier <= 1f)
            checks.Add(CheckResult.Warn("GameBalance", "Overcharge fire-rate multiplier is not above 1."));
        if (balance.skyInterval <= 0f)
            checks.Add(CheckResult.Fail("GameBalance", "Sky interval must be above 0."));
    }

    static void CheckUnitProjectileEffects(GameBalance balance, List<CheckResult> checks)
    {
        if (balance == null || balance.units == null) return;

        bool foundSnowGun = false;
        bool snowGunHasSlow = false;
        foreach (UnitBalance unit in balance.units)
        {
            if (unit == null) continue;

            if (unit.projectileHitEffects != null)
            {
                foreach (ProjectileHitEffect effect in unit.projectileHitEffects)
                {
                    if (effect == null) continue;
                    if (!Enum.IsDefined(typeof(ProjectileEffectType), effect.type))
                        checks.Add(CheckResult.Fail("Projectile Effect", $"{unit.label} has undefined projectile effect type {effect.type}."));
                    if (effect.type == ProjectileEffectType.Slow && (effect.value <= 0f || effect.value >= 1f || effect.duration <= 0f))
                        checks.Add(CheckResult.Fail("Projectile Effect", $"{unit.label} slow effect must use value 0..1 and duration above 0."));
                    if ((effect.type == ProjectileEffectType.Knockback || effect.type == ProjectileEffectType.Stun) && effect.duration < 0f)
                        checks.Add(CheckResult.Fail("Projectile Effect", $"{unit.label} effect duration cannot be negative."));
                }
            }

            if (!string.Equals(unit.label, "SnowGun", StringComparison.OrdinalIgnoreCase))
                continue;

            foundSnowGun = true;
            snowGunHasSlow = HasSlowProjectileEffect(unit);
        }

        if (foundSnowGun && !snowGunHasSlow)
            checks.Add(CheckResult.Fail("Projectile Effect", "SnowGun should define a Slow ProjectileHitEffect in GameBalance."));
    }

    static bool HasSlowProjectileEffect(UnitBalance unit)
    {
        if (unit == null) return false;

        if (unit.projectileHitEffects != null)
        {
            foreach (ProjectileHitEffect effect in unit.projectileHitEffects)
                if (effect != null && effect.type == ProjectileEffectType.Slow && effect.value > 0f && effect.value < 1f && effect.duration > 0f)
                    return true;
        }

        return unit.bulletSlowDuration > 0f && unit.bulletSlowFactor > 0f && unit.bulletSlowFactor < 1f;
    }

    static void CheckLevelManager(LevelManager levelManager, GameBalance balance, List<CheckResult> checks)
    {
        if (levelManager == null)
        {
            checks.Add(CheckResult.Warn("Level", "No LevelManager found in loaded scene."));
            return;
        }

        if (levelManager.currentLevel == null)
        {
            checks.Add(CheckResult.Warn("Level", "LevelManager has no currentLevel assigned."));
            return;
        }

        if (string.IsNullOrWhiteSpace(levelManager.currentLevel.levelId))
            checks.Add(CheckResult.Fail("Level", "Current level has empty levelId."));
        if (levelManager.currentLevel.levelNumber <= 0)
            checks.Add(CheckResult.Warn("Level", "Current levelNumber should be above 0."));
        if (levelManager.currentLevel.useAuthoredWaves &&
            (levelManager.currentLevel.waves == null || levelManager.currentLevel.waves.Length == 0))
            checks.Add(CheckResult.Fail("Level", "useAuthoredWaves is true but no waves are defined."));

        if (levelManager.levelCatalog == null)
        {
            checks.Add(CheckResult.Warn("Level", "LevelManager has no LevelCatalog assigned."));
        }
        else if (levelManager.levelCatalog.levels == null || levelManager.levelCatalog.levels.Length == 0)
        {
            checks.Add(CheckResult.Warn("Level", "LevelCatalog has no levels."));
        }
        else
        {
            CheckLevelCatalog(levelManager.levelCatalog, balance, checks);
        }
    }

    static void CheckLevelCatalog(LevelCatalog catalog, GameBalance balance, List<CheckResult> checks)
    {
        var ids = new HashSet<string>();
        var unitLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int expectedNumber = 1;
        bool hasFastEnemy = false;
        bool hasShieldEnemy = false;

        if (balance != null && balance.units != null)
        {
            foreach (var unit in balance.units)
                if (unit != null && !string.IsNullOrWhiteSpace(unit.label))
                    unitLabels.Add(unit.label);
        }

        for (int i = 0; i < catalog.levels.Length; i++)
        {
            LevelDefinition level = catalog.levels[i];
            if (level == null)
            {
                checks.Add(CheckResult.Fail("Level", $"LevelCatalog slot {i} is empty."));
                continue;
            }

            if (string.IsNullOrWhiteSpace(level.levelId))
            {
                checks.Add(CheckResult.Fail("Level", $"{level.name} has empty levelId."));
            }
            else if (!ids.Add(level.levelId))
            {
                checks.Add(CheckResult.Fail("Level", $"Duplicate levelId '{level.levelId}'."));
            }

            if (level.levelNumber != expectedNumber)
                checks.Add(CheckResult.Warn("Level", $"{level.name} levelNumber is {level.levelNumber}, expected {expectedNumber}."));

            if (string.IsNullOrWhiteSpace(level.missionBriefing))
                checks.Add(CheckResult.Warn("Level", $"{level.name} has no mission briefing copy."));
            if (string.IsNullOrWhiteSpace(level.completionReward))
                checks.Add(CheckResult.Warn("Level", $"{level.name} has no completion reward copy."));

            if (level.useAuthoredWaves && (level.waves == null || level.waves.Length == 0))
                checks.Add(CheckResult.Fail("Level", $"{level.name} uses authored waves but has no waves."));
            else if (level.waves != null)
                CheckLevelWaves(level, checks, ref hasFastEnemy, ref hasShieldEnemy);

            if (level.allowedUnitLabels != null)
            {
                foreach (string label in level.allowedUnitLabels)
                {
                    if (string.IsNullOrWhiteSpace(label))
                        checks.Add(CheckResult.Fail("Level", $"{level.name} has an empty allowed unit label."));
                    else if (unitLabels.Count > 0 && !unitLabels.Contains(label))
                        checks.Add(CheckResult.Fail("Level", $"{level.name} allows unknown unit label '{label}'."));
                }
            }

            expectedNumber++;
        }

        if (catalog.levels.Length >= 4 && !hasFastEnemy)
            checks.Add(CheckResult.Warn("Level", "LevelCatalog has 4+ levels but no authored Fast enemy group."));
        if (catalog.levels.Length >= 5 && !hasShieldEnemy)
            checks.Add(CheckResult.Warn("Level", "LevelCatalog has 5+ levels but no authored Shield enemy group."));

        CheckEnemyTypeTuning(checks);
    }

    static void CheckLevelWaves(
        LevelDefinition level,
        List<CheckResult> checks,
        ref bool hasFastEnemy,
        ref bool hasShieldEnemy)
    {
        for (int waveIndex = 0; waveIndex < level.waves.Length; waveIndex++)
        {
            LevelWaveDefinition wave = level.waves[waveIndex];
            if (wave == null)
            {
                checks.Add(CheckResult.Fail("Level", $"{level.name} wave {waveIndex + 1} is empty."));
                continue;
            }

            if (wave.TotalCount <= 0)
                checks.Add(CheckResult.Fail("Level", $"{level.name} wave {waveIndex + 1} has no enemies."));
            if (wave.timeBetweenSpawns <= 0f)
                checks.Add(CheckResult.Fail("Level", $"{level.name} wave {waveIndex + 1} spawn interval must be above 0."));

            if (wave.groups == null) continue;
            foreach (LevelSpawnGroup group in wave.groups)
            {
                if (group == null) continue;
                if (group.count <= 0)
                    checks.Add(CheckResult.Warn("Level", $"{level.name} wave {waveIndex + 1} has a non-positive {group.enemyType} group."));
                if (!Enum.IsDefined(typeof(LevelEnemyType), group.enemyType))
                    checks.Add(CheckResult.Fail("Level", $"{level.name} wave {waveIndex + 1} has undefined enemy type {group.enemyType}."));

                if (group.enemyType == LevelEnemyType.Fast && group.count > 0)
                    hasFastEnemy = true;
                if (group.enemyType == LevelEnemyType.Shield && group.count > 0)
                    hasShieldEnemy = true;
            }
        }
    }

    static void CheckEnemyTypeTuning(List<CheckResult> checks)
    {
        EnemySpawner.EnemyTypeModifier fast = EnemySpawner.GetTypeModifier(LevelEnemyType.Fast);
        if (fast.healthMultiplier >= 1f || fast.speedMultiplier <= 1f)
            checks.Add(CheckResult.Fail("Enemy Type", "Fast enemy must have lower health and higher speed than baseline."));
        if (fast.slowEffectMultiplier <= 1f)
            checks.Add(CheckResult.Warn("Enemy Type", "Fast enemy should stay vulnerable to slow effects."));

        EnemySpawner.EnemyTypeModifier shield = EnemySpawner.GetTypeModifier(LevelEnemyType.Shield);
        if (shield.healthMultiplier <= 1f || shield.speedMultiplier >= 1f)
            checks.Add(CheckResult.Fail("Enemy Type", "Shield enemy must have higher health and lower speed than baseline."));
        if (shield.projectileDamageMultiplier >= 1f || shield.empDamageMultiplier <= 1f)
            checks.Add(CheckResult.Fail("Enemy Type", "Shield enemy must resist projectiles and stay vulnerable to EMP."));
    }

    static void CheckRuntimeFoundations(
        CombatVfxSettings vfxSettings,
        TutorialCoach tutorialCoach,
        RuntimeQualitySettings runtimeQuality,
        List<CheckResult> checks)
    {
        if (vfxSettings == null)
            checks.Add(CheckResult.Warn("VFX", "No CombatVfxSettings found; code-generated fallback only."));
        else if (!vfxSettings.useCodeGeneratedFallback &&
                 vfxSettings.muzzleFlashPrefab == null &&
                 vfxSettings.hitSparkPrefab == null &&
                 vfxSettings.enemyDeathPrefab == null)
            checks.Add(CheckResult.Fail("VFX", "CombatVfxSettings disables fallback but has no core VFX prefabs."));

        if (tutorialCoach == null)
        {
            checks.Add(CheckResult.Warn("Tutorial", "No TutorialCoach found; onboarding hints disabled."));
        }
        else if (tutorialCoach.hintDisplayDuration <= 0f)
        {
            checks.Add(CheckResult.Warn("Tutorial", "TutorialCoach hintDisplayDuration should be above 0."));
        }

        if (runtimeQuality == null)
        {
            checks.Add(CheckResult.Warn("Runtime", "No RuntimeQualitySettings found."));
        }
        else if (runtimeQuality.targetFrameRate < 30)
        {
            checks.Add(CheckResult.Fail("Runtime", "Target frame rate should be at least 30."));
        }
    }

    static void CheckAudio(AudioManager audio, List<CheckResult> checks)
    {
        if (audio == null)
        {
            checks.Add(CheckResult.Warn("Audio", "No AudioManager found in loaded scene."));
            return;
        }

        foreach (SfxType type in Enum.GetValues(typeof(SfxType)))
        {
            bool found = false;
            if (audio.clips != null)
            {
                foreach (var clip in audio.clips)
                {
                    if (clip == null || clip.type != type) continue;
                    found = true;
                    if (clip.clip == null)
                        checks.Add(CheckResult.Warn("Audio", $"{type} has an entry but no clip assigned."));
                    if (clip.volume <= 0f)
                        checks.Add(CheckResult.Warn("Audio", $"{type} volume is 0."));
                    break;
                }
            }

            if (!found)
                checks.Add(CheckResult.Warn("Audio", $"Missing SFX entry for {type}."));
        }
    }

    static void CheckPrefabs(GameBalance balance, List<CheckResult> checks)
    {
        if (balance == null) return;

        if (balance.units != null)
        {
            foreach (var unit in balance.units)
            {
                if (unit == null) continue;
                if (unit.prefab == null)
                {
                    checks.Add(CheckResult.Fail("Unit Prefab", $"{unit.label} has no prefab."));
                    continue;
                }

                if (unit.prefab.GetComponent<Health>() == null)
                    checks.Add(CheckResult.Warn("Unit Prefab", $"{unit.label} has no Health."));

                var shooter = unit.prefab.GetComponent<Shooter>();
                if (shooter != null && shooter.bulletPrefab == null)
                    checks.Add(CheckResult.Fail("Unit Prefab", $"{unit.label} Shooter has no bullet prefab."));

                var stages = unit.prefab.GetComponent<DamageStages>();
                if (stages != null && (stages.stages == null || stages.stages.Length < 2))
                    checks.Add(CheckResult.Warn("Unit Prefab", $"{unit.label} DamageStages should have at least 2 stages."));
            }
        }

        if (balance.enemies != null)
        {
            foreach (var enemy in balance.enemies)
            {
                if (enemy == null) continue;
                if (enemy.prefab == null)
                {
                    checks.Add(CheckResult.Fail("Enemy Prefab", $"{enemy.label} has no prefab."));
                    continue;
                }

                if (enemy.prefab.GetComponent<Health>() == null)
                    checks.Add(CheckResult.Fail("Enemy Prefab", $"{enemy.label} has no Health."));
                if (enemy.prefab.GetComponent<EnemyMover>() == null)
                    checks.Add(CheckResult.Fail("Enemy Prefab", $"{enemy.label} has no EnemyMover."));

                if (enemy.projectileDamageMultiplier < 0f ||
                    enemy.empDamageMultiplier < 0f ||
                    enemy.slowEffectMultiplier < 0f ||
                    enemy.knockbackMultiplier < 0f ||
                    enemy.stunDurationMultiplier < 0f)
                {
                    checks.Add(CheckResult.Fail("Enemy Traits", $"{enemy.label} has a negative trait multiplier."));
                }
            }
        }
    }

    static void CheckAnimatorContracts(GameBalance balance, List<CheckResult> checks)
    {
        if (balance == null) return;

        CheckAnimatorEntries(balance.units, checks);
        CheckAnimatorEntries(balance.enemies, checks);
    }

    static void CheckAnimatorEntries<T>(T[] entries, List<CheckResult> checks)
    {
        if (entries == null) return;

        foreach (var entry in entries)
        {
            GameObject prefab = null;
            string label = "Unknown";

            if (entry is UnitBalance unit)
            {
                prefab = unit.prefab;
                label = unit.label;
            }
            else if (entry is EnemyBalance enemy)
            {
                prefab = enemy.prefab;
                label = enemy.label;
            }

            if (prefab == null) continue;
            var bridge = prefab.GetComponent<CharacterAnimator>();
            if (bridge == null || bridge.animator == null || bridge.animator.runtimeAnimatorController == null)
                continue;

            var animator = bridge.animator;
            bool hasAttack = HasParameter(animator, "Attack");
            bool hasDie = HasParameter(animator, "Die");
            bool hasWalking = HasParameter(animator, "Walking");
            if (!hasAttack) checks.Add(CheckResult.Fail("Animator", $"{label} animator missing Attack trigger."));
            if (!hasDie) checks.Add(CheckResult.Fail("Animator", $"{label} animator missing Die trigger."));
            if (prefab.GetComponent<EnemyMover>() != null && !hasWalking)
                checks.Add(CheckResult.Fail("Animator", $"{label} animator missing Walking bool."));
        }
    }

    static bool HasParameter(Animator animator, string name)
    {
        foreach (var parameter in animator.parameters)
            if (parameter.name == name)
                return true;
        return false;
    }

    static void CheckProjectReadiness(LevelManager levelManager, List<CheckResult> checks)
    {
        CheckBuildScenes(checks);
        CheckPlayerMetadata(checks);
        CheckLevelAssets(levelManager != null ? levelManager.levelCatalog : null, checks);
    }

    static void CheckBuildScenes(List<CheckResult> checks)
    {
        bool hasEnabledScene = false;
        bool hasDefaultScene = false;

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            hasEnabledScene = true;
            if (string.Equals(scene.path, DefaultScenePath, StringComparison.OrdinalIgnoreCase))
                hasDefaultScene = true;
        }

        if (!hasEnabledScene)
            checks.Add(CheckResult.Fail("Build", "No enabled scenes in Build Settings."));
        else if (!hasDefaultScene)
            checks.Add(CheckResult.Warn("Build", $"{DefaultScenePath} is not enabled in Build Settings."));
    }

    static void CheckPlayerMetadata(List<CheckResult> checks)
    {
        if (string.IsNullOrWhiteSpace(PlayerSettings.productName))
            checks.Add(CheckResult.Fail("Build", "PlayerSettings.productName is empty."));
        else if (string.Equals(PlayerSettings.productName, "my_game", StringComparison.OrdinalIgnoreCase))
            checks.Add(CheckResult.Warn("Build", "PlayerSettings.productName still looks like a project placeholder."));

        if (string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion))
            checks.Add(CheckResult.Warn("Build", "PlayerSettings.bundleVersion is empty."));
    }

    static void CheckLevelAssets(LevelCatalog catalog, List<CheckResult> checks)
    {
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var catalogLevels = new HashSet<LevelDefinition>();
        if (catalog != null && catalog.levels != null)
        {
            foreach (LevelDefinition level in catalog.levels)
                if (level != null)
                    catalogLevels.Add(level);
        }

        string[] guids = AssetDatabase.FindAssets("t:LevelDefinition", new[] { "Assets/Levels" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelDefinition level = AssetDatabase.LoadAssetAtPath<LevelDefinition>(path);
            if (level == null) continue;

            if (!string.IsNullOrWhiteSpace(level.levelId) && !ids.Add(level.levelId))
                checks.Add(CheckResult.Fail("Level", $"Duplicate LevelDefinition asset id '{level.levelId}' at {path}."));

            if (catalog != null && !catalogLevels.Contains(level))
                checks.Add(CheckResult.Warn("Level", $"{path} is not included in LevelCatalog."));
        }
    }

    static void WriteReports(GameBalance balance, List<CheckResult> checks)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string reportDir = Path.Combine(projectRoot, "AIReports");
        Directory.CreateDirectory(reportDir);

        string markdown = BuildQaMarkdown(checks) + "\n\n" + BalanceReportGenerator.BuildMarkdown(balance);
        File.WriteAllText(Path.Combine(reportDir, "latest_ai_qa_report.md"), markdown);
        File.WriteAllText(Path.Combine(reportDir, "latest_balance_metrics.json"), BalanceReportGenerator.BuildJson(balance));

        Debug.Log($"AI QA report written to {reportDir}");
        AssetDatabase.Refresh();
    }

    static string BuildQaMarkdown(List<CheckResult> checks)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# AI QA Report");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        int failCount = 0;
        int warnCount = 0;
        foreach (var check in checks)
        {
            if (check.level == CheckLevel.Fail) failCount++;
            if (check.level == CheckLevel.Warn) warnCount++;
        }

        sb.AppendLine($"Summary: `{failCount}` fail, `{warnCount}` warn");
        sb.AppendLine();

        if (checks.Count == 0)
        {
            sb.AppendLine("No issues found.");
            return sb.ToString();
        }

        foreach (var check in checks)
            sb.AppendLine($"- **{check.level}** `{check.category}`: {check.message}");

        return sb.ToString();
    }

    enum CheckLevel { Warn, Fail }

    struct CheckResult
    {
        public CheckLevel level;
        public string category;
        public string message;

        public static CheckResult Warn(string category, string message)
        {
            return new CheckResult { level = CheckLevel.Warn, category = category, message = message };
        }

        public static CheckResult Fail(string category, string message)
        {
            return new CheckResult { level = CheckLevel.Fail, category = category, message = message };
        }
    }
}
