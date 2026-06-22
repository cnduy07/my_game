using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUiController : MonoBehaviour
{
    public static GameUiController Instance { get; private set; }

    public bool isPaused;

    private Rect pauseButtonRect;
    private Rect panelRect;
    private GUIStyle titleStyle;
    private GUIStyle smallStyle;
    private GUIStyle panelStyle;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon))
            return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    public bool PointerOverPanel(float guiX, float guiY)
    {
        return pauseButtonRect.Contains(new Vector2(guiX, guiY)) ||
               (isPaused && panelRect.Contains(new Vector2(guiX, guiY)));
    }

    void TogglePause()
    {
        SetPaused(!isPaused);
        AudioManager.PlaySfx(SfxType.UiClick);
    }

    void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    void OnGUI()
    {
        EnsureStyles();
        DrawTopHud();

        if (isPaused)
            DrawPausePanel();
    }

    void DrawTopHud()
    {
        float margin = 8f;
        pauseButtonRect = new Rect(Screen.width - 58f, margin, 50f, 32f);
        GUI.enabled = GameManager.Instance == null || (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsWon);
        if (GUI.Button(pauseButtonRect, isPaused ? "Play" : "II"))
            TogglePause();
        GUI.enabled = true;

        var level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        if (level != null)
        {
            Rect levelRect = new Rect(Screen.width * 0.5f - 90f, 8f, 180f, 24f);
            GUI.Label(levelRect, level.displayName, smallStyle);
        }
    }

    void DrawPausePanel()
    {
        float width = Mathf.Min(360f, Screen.width - 32f);
        float height = 300f;
        panelRect = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
        GUI.Box(panelRect, GUIContent.none, panelStyle);

        GUILayout.BeginArea(new Rect(panelRect.x + 18f, panelRect.y + 16f, panelRect.width - 36f, panelRect.height - 32f));

        GUILayout.Label("PAUSED", titleStyle);
        DrawLevelProgress();

        GUILayout.Space(12f);
        GUILayout.Label($"SFX Volume: {GameSettings.SfxVolume:0.00}", smallStyle);
        float newVolume = GUILayout.HorizontalSlider(GameSettings.SfxVolume, 0f, 1f, GUILayout.Height(28f));
        if (!Mathf.Approximately(newVolume, GameSettings.SfxVolume))
            GameSettings.SfxVolume = newVolume;

        GUILayout.Space(8f);
        GameSettings.ReduceShake = GUILayout.Toggle(GameSettings.ReduceShake, "Reduce shake");
        GameSettings.VibrationEnabled = GUILayout.Toggle(GameSettings.VibrationEnabled, "Vibration");

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Resume", GUILayout.Height(38f)))
            TogglePause();
        if (GUILayout.Button("Restart", GUILayout.Height(38f)))
        {
            AudioManager.PlaySfx(SfxType.UiClick);
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void DrawLevelProgress()
    {
        var level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        if (level == null) return;

        bool completed = PlayerProgress.IsLevelCompleted(level);
        GUILayout.Label($"{level.displayName}  |  Completed: {(completed ? "Yes" : "No")}", smallStyle);
        GUILayout.Label($"Highest completed level: {PlayerProgress.HighestCompletedLevel}", smallStyle);
    }

    void EnsureStyles()
    {
        if (titleStyle != null) return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        panelStyle = new GUIStyle(GUI.skin.box);
    }
}
