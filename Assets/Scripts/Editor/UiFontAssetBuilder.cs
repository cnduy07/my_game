using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public static class UiFontAssetBuilder
{
    const string SourceFontPath = "Assets/Thaleah_PixelFont/Materials/ThaleahFat_TTF.ttf";
    const string ResourcesFolder = "Assets/Resources";
    const string FontsFolder = "Assets/Resources/Fonts";
    const string TmpFontAssetPath = "Assets/Resources/Fonts/ThaleahFat SDF.asset";
    const string PreloadedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,:;!?+-*/_[]()<>|%#&'\" ";

    static readonly string[] UiScenePaths =
    {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/SettingsScene.unity",
        "Assets/Scenes/HowToPlayScene.unity",
        "Assets/Scenes/MissionMapScene.unity",
    };

    [MenuItem("Tools/Coreline/Apply Thaleah Pixel Font")]
    public static void ApplyThaleahPixelFont()
    {
        TMP_FontAsset fontAsset = EnsureThaleahTmpFontAsset();
        if (fontAsset == null)
            return;

        TMP_Settings settings = Resources.Load<TMP_Settings>("TMP Settings");
        if (settings != null)
        {
            SerializedObject serializedSettings = new SerializedObject(settings);
            SerializedProperty defaultFont = serializedSettings.FindProperty("m_defaultFontAsset");
            if (defaultFont != null)
            {
                defaultFont.objectReferenceValue = fontAsset;
                serializedSettings.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(settings);
            }
        }

        ApplyFontToOpenAndSavedScenes(fontAsset);
        AssetDatabase.SaveAssets();
        Debug.Log($"Applied Thaleah pixel font: {TmpFontAssetPath}");
    }

    public static TMP_FontAsset EnsureThaleahTmpFontAsset()
    {
        TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TmpFontAssetPath);
        if (IsUsableFontAsset(existing))
            return existing;

        if (existing != null)
        {
            AssetDatabase.DeleteAsset(TmpFontAssetPath);
            AssetDatabase.Refresh();
        }

        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(SourceFontPath);
        if (sourceFont == null)
        {
            Debug.LogError($"Missing source font: {SourceFontPath}");
            return null;
        }

        Directory.CreateDirectory(ResourcesFolder);
        Directory.CreateDirectory(FontsFolder);

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            16,
            1,
            GlyphRenderMode.RASTER,
            512,
            512,
            AtlasPopulationMode.Dynamic,
            true);
        fontAsset.name = "ThaleahFat SDF";
        fontAsset.TryAddCharacters(PreloadedCharacters, out string missingCharacters);
        if (!string.IsNullOrEmpty(missingCharacters))
            Debug.LogWarning($"Thaleah TMP font missing preloaded characters: {missingCharacters}");

        Texture2D atlasTexture = fontAsset.atlasTexture;
        if (atlasTexture != null)
            atlasTexture.name = "ThaleahFat Atlas";
        if (fontAsset.material != null)
            fontAsset.material.name = "ThaleahFat Material";

        AssetDatabase.CreateAsset(fontAsset, TmpFontAssetPath);
        if (atlasTexture != null)
            AssetDatabase.AddObjectToAsset(atlasTexture, fontAsset);
        if (fontAsset.material != null)
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);

        EditorUtility.SetDirty(fontAsset);
        if (atlasTexture != null)
            EditorUtility.SetDirty(atlasTexture);
        if (fontAsset.material != null)
            EditorUtility.SetDirty(fontAsset.material);

        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(TmpFontAssetPath);
        return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TmpFontAssetPath);
    }

    static bool IsUsableFontAsset(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null || fontAsset.material == null)
            return false;

        try
        {
            if (fontAsset.atlasTexture == null)
                return false;

            _ = fontAsset.atlasTexture.width;
            return true;
        }
        catch (MissingReferenceException)
        {
            return false;
        }
    }

    static void ApplyFontToOpenAndSavedScenes(TMP_FontAsset fontAsset)
    {
        string activeScenePath = EditorSceneManager.GetActiveScene().path;

        foreach (string scenePath in UiScenePaths)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            TextMeshProUGUI[] labels = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);
            foreach (TextMeshProUGUI label in labels)
            {
                label.font = fontAsset;
                label.outlineWidth = 0f;
                if (label.overflowMode == TextOverflowModes.Ellipsis)
                    label.overflowMode = TextOverflowModes.Truncate;
                EditorUtility.SetDirty(label);
            }

            EditorSceneManager.SaveScene(scene);
        }

        if (!string.IsNullOrWhiteSpace(activeScenePath))
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
    }
}
