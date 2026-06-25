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
        overlayImage.color = UiSpec.Background;
        SetAnchor((RectTransform)mainMenuOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        BuildMainMenuBackground(mainMenuOverlay.transform);

        RectTransform titleBlock = new GameObject("TitleBlock", typeof(RectTransform)).GetComponent<RectTransform>();
        titleBlock.SetParent(mainMenuOverlay.transform, false);
        SetAnchor(titleBlock, new Vector2(0.075f, 0.42f), new Vector2(0.62f, 0.74f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI title = CreateText("Title", titleBlock, "CORELINE DEFENSE", 82, FontStyle.Bold, TextAnchor.MiddleLeft);
        title.color = UiSpec.Text;
        title.characterSpacing = 0f;
        title.gameObject.AddComponent<UiTitleFlicker>();
        SetAnchor(title.rectTransform, new Vector2(0f, 0.46f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);

        mainMenuSubtitleText = CreateText("Subtitle", titleBlock, "", 18, FontStyle.Bold, TextAnchor.MiddleLeft);
        mainMenuSubtitleText.color = UiSpec.Text;
        mainMenuSubtitleText.characterSpacing = 0f;
        SetAnchor(mainMenuSubtitleText.rectTransform, new Vector2(0f, 0.22f), new Vector2(1f, 0.48f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI buildText = CreateText("BuildText", titleBlock, "CORELINE NET ONLINE\nSELECT AN OPERATION", 16, FontStyle.Bold, TextAnchor.MiddleLeft);
        buildText.color = UiSpec.TextMuted;
        buildText.characterSpacing = 0f;
        SetAnchor(buildText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.2f), Vector2.zero, Vector2.zero);

        RectTransform commandPanel = CreateColorPanel("CommandPanel", mainMenuOverlay.transform, UiSpec.Panel);
        AddFrame(commandPanel, new Color(accentColor.r, accentColor.g, accentColor.b, 0.72f), new Vector2(2f, -2f));
        AddCardAccent(commandPanel);
        AddCornerTicks(commandPanel, new Color(accentColor.r, accentColor.g, accentColor.b, 0.5f));
        SetAnchor(commandPanel, new Vector2(0.63f, 0.22f), new Vector2(0.93f, 0.78f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI commandTitle = CreateText("CommandTitle", commandPanel, "COMMAND", 20, FontStyle.Bold, TextAnchor.MiddleLeft);
        commandTitle.color = UiSpec.TextMuted;
        commandTitle.characterSpacing = 0f;
        SetAnchor(commandTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(36f, -72f), new Vector2(-24f, -24f));

        RectTransform buttonColumn = new GameObject("ButtonColumn", typeof(RectTransform), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
        buttonColumn.SetParent(commandPanel.transform, false);
        SetAnchor(buttonColumn, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        VerticalLayoutGroup layout = buttonColumn.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(
            Mathf.RoundToInt(UiSpec.CommandPanelPadding),
            Mathf.RoundToInt(UiSpec.CommandPanelPadding),
            86,
            Mathf.RoundToInt(UiSpec.CommandPanelPadding));
        layout.spacing = UiSpec.MenuButtonGap;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

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

        Button exitButton = CreateMainMenuButton("ExitButton", buttonColumn, "EXIT", false);
        exitButton.onClick.AddListener(() =>
        {
            AudioManager.PlaySfx(SfxType.UiClick);
            Application.Quit();
        });

        mainMenuOverlay.SetActive(false);
    }

    void BuildMainMenuBackground(Transform parent)
    {
        Sprite backdrop = menuHeroSprite;

        if (backdrop != null)
        {
            Image hero = CreateImage("GeneratedMenuHero", parent, new Color(1f, 1f, 1f, 0.88f));
            ApplySprite(hero, backdrop, new Color(1f, 1f, 1f, 0.9f), false);
            hero.raycastTarget = false;
            SetAnchor(hero.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image heroShade = CreateImage("GeneratedMenuHeroShade", parent, new Color(0f, 0f, 0f, 0.46f));
            heroShade.raycastTarget = false;
            SetAnchor(heroShade.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

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

        UiAmbientFx.Create((RectTransform)parent, 30);

        // Mission map now lives in its own scene; main menu stays focused on entry actions.
    }

    Button CreateMainMenuButton(string name, Transform parent, string text, bool primary)
    {
        Color textColor = text == "EXIT" ? UiSpec.Accent : UiSpec.Text;
        Button button = CreateButton(name, parent, text, 16, UiSpec.ButtonNormal, textColor);

        LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
        layoutElement.minWidth = UiSpec.MenuButtonWidth;
        layoutElement.preferredWidth = UiSpec.MenuButtonWidth;
        layoutElement.minHeight = UiSpec.MenuButtonHeight;
        layoutElement.preferredHeight = UiSpec.MenuButtonHeight;
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
        SetAnchor((RectTransform)close.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(-UiSpec.SecondaryButtonWidth * 0.5f, 28f),
            new Vector2(UiSpec.SecondaryButtonWidth * 0.5f, 28f + UiSpec.SecondaryButtonHeight));

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
