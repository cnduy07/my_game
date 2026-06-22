using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VfxPrefabBuilder
{
    const string VfxFolder = "Assets/Prefabs/VFX";
    const string MaterialPath = VfxFolder + "/VFX_SpriteParticle.mat";
    const string ScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/VFX/Rebuild Core VFX Prefabs")]
    public static void RebuildCoreVfxPrefabs()
    {
        EnsureFolder();
        Material material = EnsureMaterial();

        GameObject muzzle = BuildBurst("VFX_MuzzleFlash", 10, 0.12f, 0.05f, 0.18f, 1.8f, 3.4f,
            new Color(1f, 0.96f, 0.32f, 1f), new Color(1f, 0.36f, 0.04f, 0.55f), material);
        GameObject hit = BuildBurst("VFX_HitSpark", 16, 0.18f, 0.035f, 0.14f, 1.2f, 2.8f,
            new Color(1f, 0.92f, 0.45f, 1f), new Color(1f, 0.18f, 0.05f, 0.4f), material);
        GameObject death = BuildBurst("VFX_EnemyDeath", 28, 0.42f, 0.055f, 0.16f, 0.6f, 1.8f,
            new Color(0.7f, 0.74f, 0.78f, 0.9f), new Color(1f, 0.3f, 0.08f, 0.75f), material);
        GameObject staticBreak = BuildBurst("VFX_StaticBreak", 24, 0.6f, 0.08f, 0.22f, 0.25f, 1.0f,
            new Color(0.55f, 0.58f, 0.58f, 0.78f), new Color(0.95f, 0.78f, 0.36f, 0.65f), material);
        GameObject bunkerBreak = BuildBunkerBreak(material);
        GameObject emp = BuildPulse("VFX_EmpPulse", new Color(0.25f, 0.9f, 1f, 0.82f), material);
        GameObject rail = BuildPulse("VFX_RailCannonImpact", new Color(1f, 0.18f, 0.05f, 0.8f), material);

        AssignSettings(muzzle, hit, death, staticBreak, bunkerBreak, emp, rail);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Core VFX prefabs rebuilt and assigned.");
    }

    static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(VfxFolder))
            AssetDatabase.CreateFolder("Assets/Prefabs", "VFX");
    }

    static Material EnsureMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (material != null) return material;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");

        material = new Material(shader);
        AssetDatabase.CreateAsset(material, MaterialPath);
        return material;
    }

    static GameObject BuildBurst(
        string name,
        short count,
        float autoDestroy,
        float minSize,
        float maxSize,
        float minSpeed,
        float maxSpeed,
        Color colorA,
        Color colorB,
        Material material)
    {
        GameObject root = new GameObject(name);
        root.AddComponent<VfxAutoDestroy>().lifetime = autoDestroy;
        ParticleSystem ps = root.AddComponent<ParticleSystem>();
        ConfigureBurst(ps, count, minSize, maxSize, minSpeed, maxSpeed, colorA, colorB, material);
        return SavePrefab(root, name);
    }

    static GameObject BuildBunkerBreak(Material material)
    {
        GameObject root = new GameObject("VFX_BunkerBreak");
        root.AddComponent<VfxAutoDestroy>().lifetime = 0.9f;

        ParticleSystem smoke = root.AddComponent<ParticleSystem>();
        ConfigureBurst(smoke, 28, 0.09f, 0.24f, 0.2f, 0.9f,
            new Color(0.48f, 0.5f, 0.5f, 0.8f), new Color(0.9f, 0.86f, 0.72f, 0.45f), material);

        GameObject sparksGo = new GameObject("Sparks");
        sparksGo.transform.SetParent(root.transform, false);
        ParticleSystem sparks = sparksGo.AddComponent<ParticleSystem>();
        ConfigureBurst(sparks, 18, 0.035f, 0.08f, 1.4f, 3.4f,
            new Color(1f, 0.92f, 0.45f, 1f), new Color(1f, 0.22f, 0.08f, 0.5f), material);

        return SavePrefab(root, "VFX_BunkerBreak");
    }

    static GameObject BuildPulse(string name, Color color, Material material)
    {
        GameObject root = new GameObject(name);
        root.AddComponent<VfxAutoDestroy>().lifetime = 0.5f;
        ParticleSystem ps = root.AddComponent<ParticleSystem>();
        ConfigureBurst(ps, 36, 0.035f, 0.1f, 1.0f, 2.4f, color, new Color(color.r, color.g, color.b, 0.2f), material);

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.4f;
        shape.arc = 360f;
        return SavePrefab(root, name);
    }

    static void ConfigureBurst(
        ParticleSystem ps,
        short count,
        float minSize,
        float maxSize,
        float minSpeed,
        float maxSpeed,
        Color colorA,
        Color colorB,
        Material material)
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.12f;
        main.loop = false;
        main.playOnAwake = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.12f, 0.45f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = new ParticleSystem.MinMaxGradient(colorA, colorB);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.12f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 26;
        renderer.material = material;
    }

    static GameObject SavePrefab(GameObject root, string name)
    {
        string path = $"{VfxFolder}/{name}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static void AssignSettings(
        GameObject muzzle,
        GameObject hit,
        GameObject death,
        GameObject staticBreak,
        GameObject bunkerBreak,
        GameObject emp,
        GameObject rail)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null) return;
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        CombatVfxSettings settings = Object.FindAnyObjectByType<CombatVfxSettings>(FindObjectsInactive.Include);
        if (settings == null) return;

        settings.muzzleFlashPrefab = muzzle;
        settings.hitSparkPrefab = hit;
        settings.enemyDeathPrefab = death;
        settings.staticBreakPrefab = staticBreak;
        settings.bunkerBreakPrefab = bunkerBreak;
        settings.empPulsePrefab = emp;
        settings.railCannonBeamPrefab = rail;
        settings.useCodeGeneratedFallback = true;

        EditorUtility.SetDirty(settings);
        EditorSceneManager.MarkSceneDirty(settings.gameObject.scene);
        EditorSceneManager.SaveScene(settings.gameObject.scene);
    }
}
