using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VfxPrefabBuilder
{
    const string VfxFolder = "Assets/Prefabs/VFX";
    const string MaterialPath = VfxFolder + "/VFX_SpriteParticle.mat";
    const string ScenePath = "Assets/Scenes/GameScene.unity";
    const string SampleScenePath = "Assets/Scenes/SampleScene.unity";
    const string ArtFolder = "Assets/Art";

    [MenuItem("Tools/VFX/Rebuild Core VFX Prefabs")]
    public static void RebuildCoreVfxPrefabs()
    {
        EnsureFolder();
        Material material = EnsureMaterial();

        GameObject muzzle = BuildSpriteEffect("VFX_MuzzleFlash", $"{ArtFolder}/vfx_muzzle_flash_256.png",
            0.12f, new Vector3(0.18f, 0.16f, 1f), new Vector3(0.42f, 0.32f, 1f), new Color(1f, 1f, 1f, 0.9f), 28);
        GameObject hit = BuildSpriteEffect("VFX_HitSpark", $"{ArtFolder}/vfx_hit_spark_256.png",
            0.2f, new Vector3(0.45f, 0.45f, 1f), new Vector3(1.05f, 1.05f, 1f), new Color(1f, 1f, 1f, 0.94f), 28, 60f);
        GameObject death = BuildSpriteEffect("VFX_EnemyDeath", $"{ArtFolder}/vfx_enemy_death_burst_256.png",
            0.36f, new Vector3(0.54f, 0.54f, 1f), new Vector3(1.12f, 1.12f, 1f), new Color(1f, 1f, 1f, 0.88f), 28);
        GameObject staticBreak = BuildSpriteEffect("VFX_StaticBreak", $"{ArtFolder}/vfx_hit_spark_256.png",
            0.32f, new Vector3(0.55f, 0.55f, 1f), new Vector3(1.25f, 1.25f, 1f), new Color(1f, 1f, 1f, 0.86f), 28, -35f);
        GameObject bunkerBreak = BuildBunkerBreak(material);
        GameObject emp = BuildSpriteEffect("VFX_EmpPulse", $"{ArtFolder}/vfx_emp_pulse_256.png",
            0.28f, new Vector3(0.18f, 0.18f, 1f), new Vector3(0.62f, 0.62f, 1f), new Color(1f, 1f, 1f, 0.68f), 27);
        GameObject rail = BuildSpriteEffect("VFX_RailCannonImpact", $"{ArtFolder}/vfx_rail_beam_source_256.png",
            0.24f, new Vector3(11.2f, 0.82f, 1f), new Vector3(11.2f, 0.44f, 1f), new Color(1f, 1f, 1f, 0.92f), 27);

        if (muzzle == null)
            muzzle = BuildBurst("VFX_MuzzleFlash", 10, 0.12f, 0.05f, 0.18f, 1.8f, 3.4f,
                new Color(1f, 0.96f, 0.32f, 1f), new Color(1f, 0.36f, 0.04f, 0.55f), material);
        if (hit == null)
            hit = BuildBurst("VFX_HitSpark", 16, 0.18f, 0.035f, 0.14f, 1.2f, 2.8f,
                new Color(1f, 0.92f, 0.45f, 1f), new Color(1f, 0.18f, 0.05f, 0.4f), material);
        if (death == null)
            death = BuildBurst("VFX_EnemyDeath", 28, 0.42f, 0.055f, 0.16f, 0.6f, 1.8f,
                new Color(0.7f, 0.74f, 0.78f, 0.9f), new Color(1f, 0.3f, 0.08f, 0.75f), material);
        if (staticBreak == null)
            staticBreak = BuildBurst("VFX_StaticBreak", 24, 0.6f, 0.08f, 0.22f, 0.25f, 1.0f,
                new Color(0.55f, 0.58f, 0.58f, 0.78f), new Color(0.95f, 0.78f, 0.36f, 0.65f), material);
        if (bunkerBreak == null)
            bunkerBreak = BuildBunkerBreak(material);
        if (emp == null)
            emp = BuildPulse("VFX_EmpPulse", new Color(0.25f, 0.9f, 1f, 0.82f), material);
        if (rail == null)
            rail = BuildPulse("VFX_RailCannonImpact", new Color(1f, 0.18f, 0.05f, 0.8f), material);

        AssignSettings(ScenePath, muzzle, hit, death, staticBreak, bunkerBreak, emp, rail);
        AssignSettings(SampleScenePath, muzzle, hit, death, staticBreak, bunkerBreak, emp, rail);
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

    static GameObject BuildSpriteEffect(
        string name,
        string spritePath,
        float lifetime,
        Vector3 startScale,
        Vector3 endScale,
        Color color,
        int sortingOrder,
        float rotationSpeed = 0f)
    {
        Sprite sprite = LoadSprite(spritePath);
        if (sprite == null) return null;

        GameObject root = new GameObject(name);
        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        VfxAutoDestroy effect = root.AddComponent<VfxAutoDestroy>();
        effect.lifetime = lifetime;
        effect.animateSprite = true;
        effect.startScale = startScale;
        effect.endScale = endScale;
        effect.rotationSpeed = rotationSpeed;

        return SavePrefab(root, name);
    }

    static GameObject BuildBunkerBreak(Material material)
    {
        Sprite deathSprite = LoadSprite($"{ArtFolder}/vfx_enemy_death_burst_256.png");
        Sprite sparkSprite = LoadSprite($"{ArtFolder}/vfx_hit_spark_256.png");
        if (deathSprite != null && sparkSprite != null)
        {
            GameObject root = new GameObject("VFX_BunkerBreak");
            SpriteRenderer death = root.AddComponent<SpriteRenderer>();
            death.sprite = deathSprite;
            death.color = new Color(1f, 1f, 1f, 0.92f);
            death.sortingOrder = 28;

            VfxAutoDestroy deathEffect = root.AddComponent<VfxAutoDestroy>();
            deathEffect.lifetime = 0.46f;
            deathEffect.animateSprite = true;
            deathEffect.startScale = new Vector3(0.68f, 0.68f, 1f);
            deathEffect.endScale = new Vector3(1.28f, 1.08f, 1f);

            GameObject sparks = new GameObject("Sparks");
            sparks.transform.SetParent(root.transform, false);
            sparks.transform.localPosition = new Vector3(0.05f, 0.08f, -0.01f);
            SpriteRenderer sparkRenderer = sparks.AddComponent<SpriteRenderer>();
            sparkRenderer.sprite = sparkSprite;
            sparkRenderer.color = new Color(1f, 1f, 1f, 0.88f);
            sparkRenderer.sortingOrder = 29;

            VfxAutoDestroy sparkEffect = sparks.AddComponent<VfxAutoDestroy>();
            sparkEffect.lifetime = 0.38f;
            sparkEffect.animateSprite = true;
            sparkEffect.startScale = new Vector3(0.42f, 0.42f, 1f);
            sparkEffect.endScale = new Vector3(0.86f, 0.78f, 1f);
            sparkEffect.rotationSpeed = -45f;

            return SavePrefab(root, "VFX_BunkerBreak");
        }

        GameObject fallbackRoot = new GameObject("VFX_BunkerBreak");
        fallbackRoot.AddComponent<VfxAutoDestroy>().lifetime = 0.9f;

        ParticleSystem smoke = fallbackRoot.AddComponent<ParticleSystem>();
        ConfigureBurst(smoke, 28, 0.09f, 0.24f, 0.2f, 0.9f,
            new Color(0.48f, 0.5f, 0.5f, 0.8f), new Color(0.9f, 0.86f, 0.72f, 0.45f), material);

        GameObject fallbackSparksGo = new GameObject("Sparks");
        fallbackSparksGo.transform.SetParent(fallbackRoot.transform, false);
        ParticleSystem fallbackSparks = fallbackSparksGo.AddComponent<ParticleSystem>();
        ConfigureBurst(fallbackSparks, 18, 0.035f, 0.08f, 1.4f, 3.4f,
            new Color(1f, 0.92f, 0.45f, 1f), new Color(1f, 0.22f, 0.08f, 0.5f), material);

        return SavePrefab(fallbackRoot, "VFX_BunkerBreak");
    }

    static Sprite LoadSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (!importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
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
        string scenePath,
        GameObject muzzle,
        GameObject hit,
        GameObject death,
        GameObject staticBreak,
        GameObject bunkerBreak,
        GameObject emp,
        GameObject rail)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null) return;
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

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
