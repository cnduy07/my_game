using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameUiController
{
    static bool shouldOpenMainMenuAfterReload;

    void BuildMainMenu(Transform parent)
    {
        mainMenuOverlay = new GameObject("MainMenuOverlay", typeof(RectTransform), typeof(Image));
        mainMenuOverlay.transform.SetParent(parent, false);
        Image overlayImage = mainMenuOverlay.GetComponent<Image>();
        overlayImage.color = new Color(0.002f, 0.006f, 0.01f, 0.985f);
        SetAnchor((RectTransform)mainMenuOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        BuildMainMenuBackground(mainMenuOverlay.transform);

        RectTransform titleBlock = CreatePanel("TitleBlock", mainMenuOverlay.transform, new Color(0.018f, 0.03f, 0.045f, 0.55f));
        AddFrame(titleBlock, new Color(0.04f, 0.21f, 0.28f, 0.85f), new Vector2(2f, -2f));
        SetAnchor(titleBlock, new Vector2(0.08f, 0.42f), new Vector2(0.62f, 0.72f), Vector2.zero, Vector2.zero);
        AddCardAccent(titleBlock);

        TextMeshProUGUI title = CreateText("Title", titleBlock, "CORELINE DEFENSE", 72, FontStyle.Bold, TextAnchor.MiddleLeft);
        title.color = Color.white;
        title.characterSpacing = 8f;
        SetAnchor(title.rectTransform, new Vector2(0f, 0.46f), new Vector2(1f, 1f), new Vector2(42f, 0f), new Vector2(-30f, -16f));

        mainMenuSubtitleText = CreateText("Subtitle", titleBlock, "", 22, FontStyle.Bold, TextAnchor.MiddleLeft);
        mainMenuSubtitleText.color = new Color(0.82f, 0.95f, 1f, 1f);
        SetAnchor(mainMenuSubtitleText.rectTransform, new Vector2(0f, 0.22f), new Vector2(1f, 0.48f), new Vector2(46f, 0f), new Vector2(-30f, 0f));

        TextMeshProUGUI buildText = CreateText("BuildText", titleBlock, "TACTICAL GRID DEFENSE  /  EARLY OPERATIONS", 16, FontStyle.Bold, TextAnchor.MiddleLeft);
        buildText.color = new Color(0.42f, 0.83f, 0.9f, 0.9f);
        buildText.characterSpacing = 3f;
        SetAnchor(buildText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.2f), new Vector2(46f, 8f), new Vector2(-30f, 0f));

        RectTransform commandPanel = CreatePanel("CommandPanel", mainMenuOverlay.transform, new Color(0.025f, 0.034f, 0.052f, 0.94f));
        AddFrame(commandPanel, new Color(0.07f, 0.26f, 0.34f, 0.95f), new Vector2(2f, -2f));
        AddCardAccent(commandPanel);
        SetAnchor(commandPanel, new Vector2(0.64f, 0.18f), new Vector2(0.92f, 0.78f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI commandTitle = CreateText("CommandTitle", commandPanel, "COMMAND", 26, FontStyle.Bold, TextAnchor.MiddleLeft);
        commandTitle.color = new Color(0.9f, 0.97f, 1f, 1f);
        commandTitle.characterSpacing = 5f;
        SetAnchor(commandTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(34f, -72f), new Vector2(-24f, -22f));

        TextMeshProUGUI commandStatus = CreateText("CommandStatus", commandPanel, "Coreline net online. Select an operation.", 17, FontStyle.Bold, TextAnchor.MiddleLeft);
        commandStatus.color = new Color(0.66f, 0.82f, 0.88f, 1f);
        SetAnchor(commandStatus.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(34f, -116f), new Vector2(-24f, -78f));

        RectTransform buttonColumn = new GameObject("ButtonColumn", typeof(RectTransform), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
        buttonColumn.SetParent(commandPanel.transform, false);
        SetAnchor(buttonColumn, new Vector2(0f, 0.16f), new Vector2(1f, 0.72f), new Vector2(34f, 0f), new Vector2(-34f, 0f));

        VerticalLayoutGroup layout = buttonColumn.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        Button continueButton = CreateMainMenuButton("ContinueButton", buttonColumn, "CONTINUE", true);
        continueButton.onClick.AddListener(CloseMainMenu);

        Button campaignButton = CreateMainMenuButton("CampaignButton", buttonColumn, "CAMPAIGN", false);
        campaignButton.onClick.AddListener(() =>
        {
            CloseMainMenu(false);
            OpenLevelSelect();
        });

        Button settingsButton = CreateMainMenuButton("SettingsButton", buttonColumn, "SETTINGS", false);
        settingsButton.onClick.AddListener(OpenSettingsFromMainMenu);

        Button restartMissionButton = CreateMainMenuButton("RestartButton", buttonColumn, "RESTART MISSION", false);
        restartMissionButton.onClick.AddListener(RestartLevel);

        TextMeshProUGUI footer = CreateText("Footer", commandPanel, "BUILD 0.1  /  SESSION MENU", 14, FontStyle.Bold, TextAnchor.MiddleLeft);
        footer.color = new Color(0.42f, 0.75f, 0.82f, 0.65f);
        footer.characterSpacing = 3f;
        SetAnchor(footer.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.16f), new Vector2(34f, 10f), new Vector2(-24f, -6f));

        mainMenuOverlay.SetActive(false);
    }

    void BuildMainMenuBackground(Transform parent)
    {
        Image leftShade = CreateImage("LeftShade", parent, new Color(0.02f, 0.065f, 0.09f, 0.34f));
        SetAnchor(leftShade.rectTransform, new Vector2(0f, 0f), new Vector2(0.36f, 1f), Vector2.zero, Vector2.zero);

        Image rightShade = CreateImage("RightShade", parent, new Color(0.11f, 0.02f, 0.025f, 0.22f));
        SetAnchor(rightShade.rectTransform, new Vector2(0.76f, 0f), Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform boardGhost = CreatePanel("BoardGhost", parent, new Color(0.02f, 0.038f, 0.045f, 0.55f));
        AddFrame(boardGhost, new Color(0.04f, 0.2f, 0.25f, 0.42f));
        SetAnchor(boardGhost, new Vector2(0.08f, 0.13f), new Vector2(0.58f, 0.37f), Vector2.zero, Vector2.zero);

        for (int i = 0; i < 7; i++)
        {
            float x = i / 6f;
            Image line = CreateImage($"BoardGhostVertical{i}", boardGhost, new Color(0.1f, 0.45f, 0.55f, 0.08f));
            SetAnchor(line.rectTransform, new Vector2(x, 0f), new Vector2(x, 1f), new Vector2(-1f, 0f), new Vector2(1f, 0f));
        }

        for (int i = 0; i < 3; i++)
        {
            float y = 0.25f + i * 0.25f;
            Image line = CreateImage($"BoardGhostHorizontal{i}", boardGhost, new Color(0.1f, 0.45f, 0.55f, 0.08f));
            SetAnchor(line.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }

        Image cyanRail = CreateImage("CyanRail", parent, new Color(accentColor.r, accentColor.g, accentColor.b, 0.45f));
        SetAnchor(cyanRail.rectTransform, new Vector2(0.075f, 0.13f), new Vector2(0.078f, 0.83f), Vector2.zero, Vector2.zero);

        Image redRail = CreateImage("RedRail", parent, new Color(0.85f, 0.08f, 0.05f, 0.38f));
        SetAnchor(redRail.rectTransform, new Vector2(0.925f, 0.16f), new Vector2(0.929f, 0.8f), Vector2.zero, Vector2.zero);

        for (int i = 0; i < 9; i++)
        {
            float y = 0.08f + i * 0.1f;
            Image scan = CreateImage($"Scanline{i}", parent, new Color(1f, 1f, 1f, 0.018f));
            SetAnchor(scan.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }

        TextMeshProUGUI sideLabel = CreateText("SideLabel", parent, "CORELINE NETWORK", 14, FontStyle.Bold, TextAnchor.MiddleLeft);
        sideLabel.color = new Color(0.34f, 0.82f, 0.9f, 0.42f);
        sideLabel.characterSpacing = 5f;
        SetAnchor(sideLabel.rectTransform, new Vector2(0.08f, 0.78f), new Vector2(0.44f, 0.83f), Vector2.zero, Vector2.zero);
    }

    Button CreateMainMenuButton(string name, Transform parent, string text, bool primary)
    {
        Color normal = primary ? accentColor : new Color(0.025f, 0.03f, 0.052f, 0.96f);
        Button button = CreateButton(name, parent, text, 24, normal, Color.white);
        AddFrame((RectTransform)button.transform, primary
            ? new Color(0.1f, 0.62f, 0.72f, 0.95f)
            : new Color(0.07f, 0.2f, 0.28f, 0.9f));

        LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = 62f;
        layoutElement.preferredHeight = 66f;
        return button;
    }

    void OpenMainMenu()
    {
        if (mainMenuOverlay == null) return;

        shouldOpenMainMenuAfterReload = false;
        mainMenuShownThisSession = true;
        wasPausedBeforeMainMenu = isPaused;
        mainMenuOpen = true;
        isPaused = true;
        Time.timeScale = 0f;
        RefreshMainMenu();
        mainMenuOverlay.SetActive(true);
        RefreshModalState();
    }

    void CloseMainMenu()
    {
        CloseMainMenu(true);
    }

    void CloseMainMenu(bool restorePauseState)
    {
        if (mainMenuOverlay != null)
            mainMenuOverlay.SetActive(false);

        mainMenuOpen = false;
        bool terminal = GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon);
        if (!terminal)
            SetPaused(restorePauseState ? wasPausedBeforeMainMenu : false);
    }

    void OpenSettingsFromMainMenu()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        if (mainMenuOverlay != null)
            mainMenuOverlay.SetActive(false);

        mainMenuOpen = false;
        SetPaused(true);
    }

    void ReturnToMainMenu()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        LevelDefinition level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        if (level != null)
            PlayerProgress.SelectLevel(level);

        shouldOpenMainMenuAfterReload = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
