using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.U2D.Animation;

public static class GeneratedArtApplier
{
    const string ArtFolder = "Assets/Art";
    const string UiFolder = "Assets/UI";

    [MenuItem("Tools/Art/Apply Generated Sprites To Prefabs")]
    public static void ApplyGeneratedSpritesToPrefabs()
    {
        AssetDatabase.Refresh();

        int applied = 0;
        applied += ApplySpriteRenderer("Assets/Prefabs/Bullet.prefab", $"{ArtFolder}/sprite_bullet_turret_256.png", "Bullet");
        applied += ApplySpriteRenderer("Assets/Prefabs/DroneEMP.prefab", $"{ArtFolder}/sprite_drone_emp_256.png", "DroneEMP");
        applied += ApplySpriteRenderer("Assets/Prefabs/EnergyOrb.prefab", $"{ArtFolder}/sprite_energy_orb_256.png", "EnergyOrb");
        applied += ApplySpriteRenderer("Assets/Prefabs/Lawnmower.prefab", $"{ArtFolder}/sprite_rail_cannon_256.png", "Rail Cannon / Lawnmower");
        applied += EnsurePrefabWithSprite("Assets/Prefabs/FrostProjectile.prefab", "Assets/Prefabs/Bullet.prefab", $"{ArtFolder}/sprite_projectile_frost_256.png", "FrostProjectile");
        applied += ApplyBunkerStages();
        applied += ApplyStaticPreviewSpriteRenderer("Assets/Prefabs/Unit.prefab", $"{ArtFolder}/sprite_turret_256.png", "Turret Unit");
        applied += ApplyStaticPreviewSpriteRenderer("Assets/Prefabs/SnowGun.prefab", $"{ArtFolder}/sprite_snowgun_256.png", "SnowGun");
        applied += AssignShooterProjectile("Assets/Prefabs/SnowGun.prefab", "Assets/Prefabs/FrostProjectile.prefab", "SnowGun");
        applied += ApplyStaticPreviewSpriteRenderer("Assets/Prefabs/Enemy.prefab", $"{ArtFolder}/sprite_enemy_basic_base_256.png", "Basic Enemy");
        applied += ApplyStaticPreviewSpriteRenderer("Assets/Prefabs/ArmorEnemy.prefab", $"{ArtFolder}/sprite_enemy_armored_256.png", "Armored Enemy");
        applied += EnsureEnemyVariantPrefab("Assets/Prefabs/FastEnemy.prefab", "Assets/Prefabs/Enemy.prefab", $"{ArtFolder}/sprite_enemy_fast_256.png", "FastEnemy");
        applied += EnsureEnemyVariantPrefab("Assets/Prefabs/ShieldEnemy.prefab", "Assets/Prefabs/ArmorEnemy.prefab", $"{ArtFolder}/sprite_enemy_shield_256.png", "ShieldEnemy");
        applied += AssignEnemyVariantPrefabs("Assets/Scenes/GameScene.unity");
        applied += AssignEnemyVariantPrefabs("Assets/Scenes/SampleScene.unity");
        applied += ConfigureUiSprites();
        VfxPrefabBuilder.RebuildCoreVfxPrefabs();
        applied += AssignGeneratedArtReferences("Assets/Scenes/MainMenuScene.unity");
        applied += AssignGeneratedArtReferences("Assets/Scenes/SettingsScene.unity");
        applied += AssignGeneratedArtReferences("Assets/Scenes/HowToPlayScene.unity");
        applied += AssignGeneratedArtReferences("Assets/Scenes/MissionMapScene.unity");
        applied += AssignGeneratedArtReferences("Assets/Scenes/GameScene.unity");
        applied += AssignGeneratedArtReferences("Assets/Scenes/SampleScene.unity");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated art apply complete. Updated {applied} prefab sprite mapping(s).");
    }

    static int ApplyBunkerStages()
    {
        Sprite stage1 = LoadSprite($"{ArtFolder}/sprite_bunker_stage_1_256.png");
        Sprite stage2 = LoadSprite($"{ArtFolder}/sprite_bunker_stage_2_256.png");
        Sprite stage3 = LoadSprite($"{ArtFolder}/sprite_bunker_stage_3_256.png");
        if (stage1 == null || stage2 == null || stage3 == null) return 0;

        const string prefabPath = "Assets/Prefabs/Bunker.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>(true);
            DamageStages damageStages = root.GetComponentInChildren<DamageStages>(true);

            if (renderer == null)
            {
                Debug.LogError($"{prefabPath} has no SpriteRenderer.");
                return 0;
            }

            renderer.sprite = stage1;
            renderer.size = Vector2.one;

            if (damageStages != null)
            {
                damageStages.spriteRenderer = renderer;
                damageStages.stages = new[] { stage1, stage2, stage3 };
            }
            else
            {
                Debug.LogWarning($"{prefabPath} has no DamageStages component; only the base SpriteRenderer was updated.");
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log("Applied generated bunker damage stages.");
            return 1;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static int ApplySpriteRenderer(string prefabPath, string spritePath, string label)
    {
        Sprite sprite = LoadSprite(spritePath);
        if (sprite == null) return 0;

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>(true);
            if (renderer == null)
            {
                Debug.LogError($"{prefabPath} has no SpriteRenderer.");
                return 0;
            }

            renderer.sprite = sprite;
            renderer.size = Vector2.one;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log($"Applied {label} sprite: {spritePath}");
            return 1;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static int ApplyStaticPreviewSpriteRenderer(string prefabPath, string spritePath, string label)
    {
        return ApplySpriteRendererInternal(prefabPath, spritePath, label, disableSpriteSkin: true, rootName: null);
    }

    static int EnsureEnemyVariantPrefab(string prefabPath, string sourcePrefabPath, string spritePath, string rootName)
    {
        return EnsurePrefabWithSprite(prefabPath, sourcePrefabPath, spritePath, rootName, disableSpriteSkin: true);
    }

    static int EnsurePrefabWithSprite(string prefabPath, string sourcePrefabPath, string spritePath, string rootName, bool disableSpriteSkin = false)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null &&
            !AssetDatabase.CopyAsset(sourcePrefabPath, prefabPath))
        {
            Debug.LogError($"Could not create generated prefab: {prefabPath}");
            return 0;
        }

        return ApplySpriteRendererInternal(prefabPath, spritePath, rootName, disableSpriteSkin, rootName);
    }

    static int AssignShooterProjectile(string shooterPrefabPath, string projectilePrefabPath, string label)
    {
        GameObject projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(projectilePrefabPath);
        if (projectilePrefab == null)
        {
            Debug.LogError($"Generated projectile prefab not found: {projectilePrefabPath}");
            return 0;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(shooterPrefabPath);
        try
        {
            Shooter shooter = root.GetComponentInChildren<Shooter>(true);
            if (shooter == null)
            {
                Debug.LogError($"{shooterPrefabPath} has no Shooter.");
                return 0;
            }

            if (shooter.bulletPrefab == projectilePrefab) return 0;

            shooter.bulletPrefab = projectilePrefab;
            PrefabUtility.SaveAsPrefabAsset(root, shooterPrefabPath);
            Debug.Log($"Assigned {label} projectile prefab: {projectilePrefabPath}");
            return 1;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static int ApplySpriteRendererInternal(string prefabPath, string spritePath, string label, bool disableSpriteSkin, string rootName)
    {
        Sprite sprite = LoadSprite(spritePath);
        if (sprite == null) return 0;

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            if (!string.IsNullOrEmpty(rootName))
                root.name = rootName;

            SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>(true);
            if (renderer == null)
            {
                Debug.LogError($"{prefabPath} has no SpriteRenderer.");
                return 0;
            }

            renderer.sprite = sprite;
            renderer.size = Vector2.one;

            if (disableSpriteSkin)
                DisableSpriteSkin(root, label);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log($"Applied {label} static preview sprite: {spritePath}");
            return 1;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void DisableSpriteSkin(GameObject root, string label)
    {
        SpriteSkin spriteSkin = root.GetComponentInChildren<SpriteSkin>(true);
        if (spriteSkin == null) return;

        spriteSkin.enabled = false;
        Debug.Log($"Disabled SpriteSkin on {label} because generated preview PNG is not rigged yet.");
    }

    static int AssignEnemyVariantPrefabs(string scenePath)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null) return 0;

        GameObject fastPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FastEnemy.prefab");
        GameObject shieldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ShieldEnemy.prefab");
        if (fastPrefab == null || shieldPrefab == null) return 0;

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        EnemySpawner spawner = Object.FindAnyObjectByType<EnemySpawner>(FindObjectsInactive.Include);
        if (spawner == null)
        {
            Debug.LogWarning($"{scenePath} has no EnemySpawner.");
            return 0;
        }

        bool changed = false;
        if (spawner.fastPrefab != fastPrefab)
        {
            spawner.fastPrefab = fastPrefab;
            changed = true;
        }

        if (spawner.shieldPrefab != shieldPrefab)
        {
            spawner.shieldPrefab = shieldPrefab;
            changed = true;
        }

        if (!changed) return 0;

        EditorUtility.SetDirty(spawner);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Assigned generated Fast/Shield enemy prefabs in {scenePath}.");
        return 1;
    }

    static int ConfigureUiSprites()
    {
        ConfigureSpriteImport($"{UiFolder}/button_command.png", new Vector4(34f, 26f, 34f, 26f));
        ConfigureSpriteImport($"{ArtFolder}/ui_panel_9slice_source_256.png", new Vector4(32f, 32f, 32f, 32f));
        ConfigureSpriteImport($"{ArtFolder}/menu_hero_coreline_outpost_256.png");
        ConfigureSpriteImport($"{ArtFolder}/board_coreline_combat_grid_5x9.png");
        ConfigureSpriteImport($"{ArtFolder}/icon_arc_reactor_256.png");
        ConfigureSpriteImport($"{ArtFolder}/icon_bunker_256.png");
        ConfigureSpriteImport($"{ArtFolder}/icon_drone_emp_256.png");
        return 1;
    }

    static int AssignGeneratedArtReferences(string scenePath)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null) return 0;

        Sprite menuHero = LoadSprite($"{ArtFolder}/menu_hero_coreline_outpost_256.png");
        Sprite board = LoadSprite($"{ArtFolder}/board_coreline_combat_grid_5x9.png");
        Sprite button = LoadSprite($"{UiFolder}/button_command.png");
        Sprite panel = LoadSprite($"{ArtFolder}/ui_panel_9slice_source_256.png");
        Sprite arcIcon = LoadSprite($"{ArtFolder}/icon_arc_reactor_256.png");
        Sprite turretIcon = LoadSprite($"{ArtFolder}/sprite_turret_256.png");
        Sprite bunkerIcon = LoadSprite($"{ArtFolder}/icon_bunker_256.png");
        Sprite snowIcon = LoadSprite($"{ArtFolder}/sprite_snowgun_256.png");
        Sprite droneIcon = LoadSprite($"{ArtFolder}/icon_drone_emp_256.png");

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        bool changed = false;

        foreach (FrontendUiController frontend in Object.FindObjectsByType<FrontendUiController>(FindObjectsInactive.Include))
        {
            frontend.menuHeroSprite = menuHero;
            frontend.boardBackgroundSprite = board;
            frontend.buttonSprite = button;
            frontend.panelSprite = panel;
            EditorUtility.SetDirty(frontend);
            changed = true;
        }

        foreach (GameUiController gameUi in Object.FindObjectsByType<GameUiController>(FindObjectsInactive.Include))
        {
            gameUi.menuHeroSprite = menuHero;
            gameUi.boardBackgroundSprite = board;
            gameUi.buttonSprite = button;
            gameUi.panelSprite = panel;
            gameUi.arcReactorIcon = arcIcon;
            gameUi.turretIcon = turretIcon;
            gameUi.bunkerIcon = bunkerIcon;
            gameUi.snowGunIcon = snowIcon;
            gameUi.droneEmpIcon = droneIcon;
            EditorUtility.SetDirty(gameUi);
            changed = true;
        }

        foreach (BoardVisualController boardVisual in Object.FindObjectsByType<BoardVisualController>(FindObjectsInactive.Include))
        {
            boardVisual.boardBackgroundSprite = board;
            boardVisual.boardBackgroundOpacity = 1f;
            EditorUtility.SetDirty(boardVisual);
            changed = true;
        }

        if (!changed) return 0;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Assigned generated UI/board art references in {scenePath}.");
        return 1;
    }

    static Sprite LoadSprite(string path)
    {
        ConfigureSpriteImport(path);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            Debug.LogError($"Generated sprite not found or not imported as Sprite: {path}");
        return sprite;
    }

    static Sprite LoadFirstAvailableSprite(params string[] paths)
    {
        foreach (string path in paths)
        {
            if (AssetImporter.GetAtPath(path) == null)
                continue;

            return LoadSprite(path);
        }

        Debug.LogError($"Generated sprite not found at any expected path: {string.Join(", ", paths)}");
        return null;
    }

    static void ConfigureSpriteImport(string path)
    {
        ConfigureSpriteImport(path, null);
    }

    static void ConfigureSpriteImportIfExists(string path)
    {
        if (AssetImporter.GetAtPath(path) == null)
            return;

        ConfigureSpriteImport(path, null);
    }

    static void ConfigureSpriteImport(string path, Vector4? border)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError($"No TextureImporter found for generated art: {path}");
            return;
        }

        bool changed = false;
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        int maxDimension = texture != null ? Mathf.Max(texture.width, texture.height) : 256;
        int requiredMaxTextureSize = Mathf.NextPowerOfTwo(maxDimension);

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

        if (!Mathf.Approximately(importer.spritePixelsPerUnit, maxDimension))
        {
            importer.spritePixelsPerUnit = maxDimension;
            changed = true;
        }

        if (!importer.alphaIsTransparency)
        {
            importer.alphaIsTransparency = true;
            changed = true;
        }

        if (border.HasValue && importer.spriteBorder != border.Value)
        {
            importer.spriteBorder = border.Value;
            changed = true;
        }

        if (importer.maxTextureSize < requiredMaxTextureSize)
        {
            importer.maxTextureSize = requiredMaxTextureSize;
            changed = true;
        }

        if (changed)
            importer.SaveAndReimport();
    }

}
