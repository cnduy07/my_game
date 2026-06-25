using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FrontendUiSceneBaker
{
    static readonly string[] FrontendScenePaths =
    {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/SettingsScene.unity",
        "Assets/Scenes/HowToPlayScene.unity",
        "Assets/Scenes/MissionMapScene.unity",
    };

    [MenuItem("Tools/Coreline/Rebuild Current Frontend Authored UI")]
    public static void RebuildCurrentFrontendScene()
    {
        FrontendUiController controller = Object.FindAnyObjectByType<FrontendUiController>();
        if (controller == null)
        {
            Debug.LogWarning("No FrontendUiController found in the active scene.");
            return;
        }

        controller.RebuildAuthoredFrontendUi();
        EditorSceneManager.SaveScene(controller.gameObject.scene);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Coreline/Rebuild All Frontend Authored UI")]
    public static void RebuildAllFrontendScenes()
    {
        foreach (string scenePath in FrontendScenePaths)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            FrontendUiController controller = Object.FindAnyObjectByType<FrontendUiController>();
            if (controller == null)
            {
                Debug.LogWarning($"No FrontendUiController found in {scenePath}.");
                continue;
            }

            controller.RebuildAuthoredFrontendUi();
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Rebuilt authored frontend UI in {scenePath}.");
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Coreline/Rebuild Settings Authored UI")]
    public static void RebuildSettingsFrontendScene()
    {
        RebuildFrontendScene("Assets/Scenes/SettingsScene.unity");
    }

    static void RebuildFrontendScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        FrontendUiController controller = Object.FindAnyObjectByType<FrontendUiController>();
        if (controller == null)
        {
            Debug.LogWarning($"No FrontendUiController found in {scenePath}.");
            return;
        }

        controller.RebuildAuthoredFrontendUi();
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log($"Rebuilt authored frontend UI in {scenePath}.");
    }
}
