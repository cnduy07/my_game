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

        CheckGameBalance(balance, checks);
        CheckAudio(audio, checks);
        CheckPrefabs(balance, checks);
        CheckAnimatorContracts(balance, checks);

        WriteReports(balance, checks);
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
        if (balance.overchargeFireRateMultiplier <= 1f)
            checks.Add(CheckResult.Warn("GameBalance", "Overcharge fire-rate multiplier is not above 1."));
        if (balance.skyInterval <= 0f)
            checks.Add(CheckResult.Fail("GameBalance", "Sky interval must be above 0."));
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
