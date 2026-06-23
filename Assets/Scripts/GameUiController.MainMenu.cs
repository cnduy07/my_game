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

        RectTransform titleBlock = CreatePanel("TitleBlock", mainMenuOverlay.transform, new Color(0.018f, 0.03f, 0.045f, 0.44f));
        AddFrame(titleBlock, new Color(0.04f, 0.21f, 0.28f, 0.85f), new Vector2(2f, -2f));
        SetAnchor(titleBlock, new Vector2(0.08f, 0.42f), new Vector2(0.62f, 0.72f), Vector2.zero, Vector2.zero);
        AddCardAccent(titleBlock);
        AddCornerTicks(titleBlock, new Color(accentColor.r, accentColor.g, accentColor.b, 0.55f));

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
        AddCornerTicks(commandPanel, new Color(accentColor.r, accentColor.g, accentColor.b, 0.6f));
        SetAnchor(commandPanel, new Vector2(0.64f, 0.2f), new Vector2(0.92f, 0.78f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI commandTitle = CreateText("CommandTitle", commandPanel, "MISSION CONTROL", 25, FontStyle.Bold, TextAnchor.MiddleLeft);
        commandTitle.color = new Color(0.9f, 0.97f, 1f, 1f);
        commandTitle.characterSpacing = 5f;
        SetAnchor(commandTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(34f, -72f), new Vector2(-24f, -22f));

        TextMeshProUGUI commandStatus = CreateText("CommandStatus", commandPanel, "Campaign route armed. Choose your next move.", 17, FontStyle.Bold, TextAnchor.MiddleLeft);
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

        Button startButton = CreateMainMenuButton("StartButton", buttonColumn, "START GAME", true);
        startButton.onClick.AddListener(() =>
        {
            AudioManager.PlaySfx(SfxType.UiClick);
            Time.timeScale = 1f;
            FrontendNavigation.LoadScene(SceneNames.MissionMap);
        });

        Button settingsButton = CreateMainMenuButton("SettingsButton", buttonColumn, "SETTING", false);
        settingsButton.onClick.AddListener(OpenSettingsFromMainMenu);

        Button howToPlayButton = CreateMainMenuButton("HowToPlayButton", buttonColumn, "HOW TO PLAY", false);
        howToPlayButton.onClick.AddListener(OpenHowToPlay);

        TextMeshProUGUI footer = CreateText("Footer", commandPanel, "MISSION MAP ONLINE", 14, FontStyle.Bold, TextAnchor.MiddleLeft);
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
        SetAnchor(boardGhost, new Vector2(0.08f, 0.12f), new Vector2(0.58f, 0.35f), Vector2.zero, Vector2.zero);

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

        // Mission map now lives in its own scene; main menu stays focused on entry actions.
    }

    Button CreateMainMenuButton(string name, Transform parent, string text, bool primary)
    {
        Color normal = primary ? accentColor : new Color(0.025f, 0.03f, 0.052f, 0.96f);
        Color textColor = primary ? new Color(0.02f, 0.06f, 0.08f, 1f) : Color.white;
        Button button = CreateButton(name, parent, text, 24, normal, textColor);
        AddFrame((RectTransform)button.transform, primary
            ? new Color(0.1f, 0.62f, 0.72f, 0.95f)
            : new Color(0.07f, 0.2f, 0.28f, 0.9f));

        LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = 62f;
        layoutElement.preferredHeight = 66f;
        return button;
    }

    void BuildMainMenuMissionMap(Transform parent)
    {
        RectTransform map = CreatePanel("MissionMapBackdrop", parent, new Color(0.018f, 0.028f, 0.045f, 0.7f));
        AddFrame(map, new Color(0.07f, 0.3f, 0.38f, 0.52f));
        SetAnchor(map, new Vector2(0.14f, 0.24f), new Vector2(0.61f, 0.57f), Vector2.zero, Vector2.zero);

        int count = LevelManager.Instance != null ? Mathf.Max(1, LevelManager.Instance.LevelCount) : 10;
        int visibleCount = Mathf.Min(count, 7);
        Vector2[] points = new Vector2[visibleCount];

        for (int i = 0; i < visibleCount; i++)
        {
            float x = 0.09f + i * (0.82f / Mathf.Max(1, visibleCount - 1));
            float y = 0.24f + (i % 3 == 1 ? 0.42f : (i % 3 == 2 ? 0.18f : 0.08f));
            points[i] = new Vector2(x, y);
        }

        for (int i = 0; i < visibleCount - 1; i++)
            CreateMainMenuMapLine(map, points[i], points[i + 1], i <= PlayerProgress.HighestCompletedLevel);

        for (int i = 0; i < visibleCount; i++)
            CreateMainMenuMapNode(map, i + 1, points[i], i < PlayerProgress.HighestCompletedLevel, i == PlayerProgress.HighestCompletedLevel);

        TextMeshProUGUI mapLabel = CreateText("MissionMapLabel", map, "MISSION MAP", 16, FontStyle.Bold, TextAnchor.MiddleLeft);
        mapLabel.color = new Color(0.58f, 0.94f, 1f, 0.78f);
        mapLabel.characterSpacing = 3f;
        SetAnchor(mapLabel.rectTransform, new Vector2(0f, 0f), new Vector2(0.45f, 0f), new Vector2(24f, 18f), new Vector2(0f, 48f));
    }

    void CreateMainMenuMapLine(RectTransform parent, Vector2 from, Vector2 to, bool active)
    {
        Vector2 delta = to - from;
        Image line = CreateImage("MapRoute", parent, active ? new Color(0.16f, 0.86f, 0.96f, 0.5f) : new Color(0.2f, 0.32f, 0.38f, 0.32f));
        RectTransform rect = line.rectTransform;
        rect.anchorMin = from;
        rect.anchorMax = from;
        rect.pivot = new Vector2(0f, 0.5f);
        rect.sizeDelta = new Vector2(delta.magnitude * 620f, 5f);
        rect.anchoredPosition = Vector2.zero;
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    void CreateMainMenuMapNode(RectTransform parent, int number, Vector2 point, bool cleared, bool current)
    {
        Image node = CreateImage($"MapNode_{number}", parent, current ? accentColor : (cleared ? new Color(0.3f, 0.9f, 0.62f, 0.92f) : new Color(0.16f, 0.19f, 0.26f, 0.95f)));
        SetAnchor(node.rectTransform, point, point, new Vector2(-22f, -22f), new Vector2(22f, 22f));
        AddFrame(node.rectTransform, current ? new Color(0.9f, 1f, 1f, 0.9f) : new Color(0.1f, 0.28f, 0.35f, 0.7f));

        TextMeshProUGUI label = CreateText("NodeLabel", node.transform, number.ToString("00"), 14, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(2f, 0f), new Vector2(-2f, 0f));
    }

    void BuildHowToPlayOverlay(Transform parent)
    {
        howToPlayOverlay = new GameObject("HowToPlayOverlay", typeof(RectTransform), typeof(Image));
        howToPlayOverlay.transform.SetParent(parent, false);
        Image overlay = howToPlayOverlay.GetComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0.72f);
        SetAnchor((RectTransform)howToPlayOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform card = CreatePanel("HowToPlayCard", howToPlayOverlay.transform, new Color(0.045f, 0.058f, 0.082f, 0.98f));
        AddFrame(card, new Color(0.12f, 0.36f, 0.43f, 0.95f), new Vector2(2f, -2f));
        AddCornerTicks(card, new Color(accentColor.r, accentColor.g, accentColor.b, 0.5f));
        SetAnchor(card, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-420f, -260f), new Vector2(420f, 260f));

        TextMeshProUGUI title = CreateText("Title", card, "HOW TO PLAY", 34, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(32f, -78f), new Vector2(-32f, -24f));

        TextMeshProUGUI body = CreateText("Body", card,
            "Collect energy orbs, place units on open grid cells, and hold each lane until the wave is clear.\n\nArmored enemies can drop bonus energy. Use EMP and overcharge when a lane starts to buckle.\n\nClear missions to open the next sector on the map.",
            22, FontStyle.Normal, TextAnchor.UpperLeft);
        body.color = new Color(0.84f, 0.93f, 0.97f, 1f);
        SetAnchor(body.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(64f, 96f), new Vector2(-64f, -112f));

        Button close = CreateButton("CloseButton", card, "BACK", 22, accentColor, Color.white);
        close.onClick.AddListener(CloseHowToPlay);
        SetAnchor((RectTransform)close.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-120f, 28f), new Vector2(120f, 82f));

        howToPlayOverlay.SetActive(false);
    }

    void OpenHowToPlay()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        if (howToPlayOverlay != null)
            howToPlayOverlay.SetActive(true);
    }

    void CloseHowToPlay()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        if (howToPlayOverlay != null)
            howToPlayOverlay.SetActive(false);
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
        Time.timeScale = 1f;
        FrontendNavigation.LoadScene(SceneNames.Settings);
    }

    void ReturnToMainMenu()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        LevelDefinition level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        if (level != null)
            PlayerProgress.SelectLevel(level);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}
