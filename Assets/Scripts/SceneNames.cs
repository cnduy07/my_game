using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames
{
    public const string MainMenu = "MainMenuScene";
    public const string Settings = "SettingsScene";
    public const string HowToPlay = "HowToPlayScene";
    public const string MissionMap = "MissionMapScene";
    public const string Game = "GameScene";
    public const string End = "EndScene";
}

public static class FrontendNavigation
{
    static string previousScene = SceneNames.MainMenu;

    public static void LoadScene(string sceneName)
    {
        string current = SceneManager.GetActiveScene().name;
        previousScene = string.IsNullOrWhiteSpace(current) ? SceneNames.MainMenu : current;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public static void Back()
    {
        string target = string.IsNullOrWhiteSpace(previousScene) ? SceneNames.MainMenu : previousScene;
        if (target == SceneNames.Game || target == SceneManager.GetActiveScene().name)
            target = SceneNames.MainMenu;

        Time.timeScale = 1f;
        SceneManager.LoadScene(target);
    }
}
