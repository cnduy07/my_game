using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum FrontendScreenMode
{
    Auto,
    MainMenu,
    Settings,
    HowToPlay,
    MissionMap
}

public class FrontendUiController : MonoBehaviour
{
    const float ReferenceWidth = 1920f;
    const float ReferenceHeight = 1080f;
    const float NodeStep = 190f;
    const float NodeSidePadding = 150f;
    const float MapMinWidth = 1160f;

    public FrontendScreenMode screenMode = FrontendScreenMode.Auto;
    public LevelCatalog levelCatalog;
    public bool unlockAllLevelsForTesting = true;

    [Header("Generated Art")]
    public Sprite menuHeroSprite;
    public Sprite boardBackgroundSprite;
    public Sprite buttonSprite;
    public Sprite panelSprite;

    readonly Color bg = new Color(0.012f, 0.018f, 0.03f, 1f);
    readonly Color panel = new Color(0.035f, 0.05f, 0.075f, 0.94f);
    readonly Color panelSoft = new Color(0.08f, 0.115f, 0.155f, 0.95f);
    readonly Color accent = new Color(0.12f, 0.82f, 0.95f, 1f);
    readonly Color hot = new Color(1f, 0.28f, 0.22f, 1f);
    readonly Color dim = new Color(0.38f, 0.45f, 0.52f, 0.85f);

    Canvas canvas;
    RectTransform root;
    TextMeshProUGUI musicValueText;
    TextMeshProUGUI sfxValueText;
    Toggle reduceShakeToggle;
    Toggle vibrationToggle;
    RectTransform mapPanel;
    RectTransform mapContent;
    RectTransform routeLayer;
    RectTransform nodeLayer;
    ScrollRect mapScroll;
    RectTransform detailPanel;
    TextMeshProUGUI missionStatusText;
    TextMeshProUGUI missionDevModeText;
    TextMeshProUGUI missionTitleText;
    TextMeshProUGUI missionTypeText;
    TextMeshProUGUI missionBriefingText;
    TextMeshProUGUI missionEnemyMixText;
    TextMeshProUGUI missionToolsText;
    TextMeshProUGUI missionPressureText;
    TextMeshProUGUI missionRewardText;
    Button deployButton;
    TextMeshProUGUI deployButtonText;
    LevelDefinition selectedLevel;

    void Start()
    {
        EnsureRenderCamera();
        EnsureEventSystem();
        BuildCanvas();
        BuildBackground();

        switch (ResolveMode())
        {
            case FrontendScreenMode.Settings:
                BuildSettings();
                break;
            case FrontendScreenMode.HowToPlay:
                BuildHowToPlay();
                break;
            case FrontendScreenMode.MissionMap:
                BuildMissionMap();
                break;
            default:
                BuildMainMenu();
                break;
        }
    }

    FrontendScreenMode ResolveMode()
    {
        if (screenMode != FrontendScreenMode.Auto)
            return screenMode;

        string scene = SceneManager.GetActiveScene().name;
        if (scene == SceneNames.Settings) return FrontendScreenMode.Settings;
        if (scene == SceneNames.HowToPlay) return FrontendScreenMode.HowToPlay;
        if (scene == SceneNames.MissionMap) return FrontendScreenMode.MissionMap;
        return FrontendScreenMode.MainMenu;
    }

    void BuildCanvas()
    {
        GameObject go = new GameObject("FrontendCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.transform.SetParent(transform, false);
        canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
        scaler.matchWidthOrHeight = 0.5f;

        root = new GameObject("SafeAreaRoot", typeof(RectTransform), typeof(SafeAreaFitter)).GetComponent<RectTransform>();
        root.SetParent(go.transform, false);
        SetAnchor(root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    void BuildBackground()
    {
        Image baseImage = CreateImage("Background", root, bg);
        ApplySprite(baseImage, menuHeroSprite, menuHeroSprite != null ? new Color(1f, 1f, 1f, 0.38f) : bg, false);
        SetAnchor(baseImage.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        AddBackgroundGrid(root);

        Image leftBand = CreateImage("LeftCommandBand", root, new Color(0.03f, 0.12f, 0.16f, 0.18f));
        SetAnchor(leftBand.rectTransform, new Vector2(0f, 0f), new Vector2(0.32f, 1f), Vector2.zero, Vector2.zero);

        Image redBand = CreateImage("ThreatBand", root, new Color(0.55f, 0.05f, 0.04f, 0.12f));
        SetAnchor(redBand.rectTransform, new Vector2(0.74f, 0f), Vector2.one, Vector2.zero, Vector2.zero);

        for (int i = 0; i < 10; i++)
        {
            float y = 0.06f + i * 0.095f;
            Image scan = CreateImage($"Scanline_{i}", root, new Color(1f, 1f, 1f, 0.018f));
            scan.raycastTarget = false;
            SetAnchor(scan.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }

        Image rail = CreateImage("DiagonalRail", root, new Color(0.08f, 0.24f, 0.28f, 0.3f));
        rail.raycastTarget = false;
        RectTransform railRect = rail.rectTransform;
        railRect.anchorMin = new Vector2(0.5f, 0.5f);
        railRect.anchorMax = new Vector2(0.5f, 0.5f);
        railRect.sizeDelta = new Vector2(2300f, 18f);
        railRect.anchoredPosition = new Vector2(80f, -120f);
        railRect.localRotation = Quaternion.Euler(0f, 0f, 16f);

        UiAmbientFx.Create(root, 30);
    }

    void AddBackgroundGrid(RectTransform parent)
    {
        for (int i = 1; i < 12; i++)
        {
            float x = i / 12f;
            Image line = CreateImage($"BgGridV_{i}", parent, new Color(0.08f, 0.22f, 0.28f, 0.08f));
            line.raycastTarget = false;
            SetAnchor(line.rectTransform, new Vector2(x, 0f), new Vector2(x, 1f), new Vector2(-1f, 0f), new Vector2(1f, 0f));
        }

        for (int i = 1; i < 7; i++)
        {
            float y = i / 7f;
            Image line = CreateImage($"BgGridH_{i}", parent, new Color(0.08f, 0.22f, 0.28f, 0.06f));
            line.raycastTarget = false;
            SetAnchor(line.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }
    }

    void BuildMapField(RectTransform parent)
    {
        if (boardBackgroundSprite != null)
        {
            Image boardArt = CreateImage("GeneratedBoardArt", parent, new Color(1f, 1f, 1f, 0.54f));
            ApplySprite(boardArt, boardBackgroundSprite, new Color(1f, 1f, 1f, 0.54f), false);
            boardArt.raycastTarget = false;
            SetAnchor(boardArt.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        Image leftField = CreateImage("MapDefenseField", parent, new Color(accent.r, accent.g, accent.b, 0.08f));
        leftField.raycastTarget = false;
        SetAnchor(leftField.rectTransform, new Vector2(0f, 0f), new Vector2(0.12f, 1f), Vector2.zero, Vector2.zero);

        Image threatField = CreateImage("MapThreatField", parent, new Color(hot.r, hot.g, hot.b, 0.08f));
        threatField.raycastTarget = false;
        SetAnchor(threatField.rectTransform, new Vector2(0.84f, 0f), Vector2.one, Vector2.zero, Vector2.zero);

        for (int i = 1; i < 8; i++)
        {
            float x = i / 8f;
            Image line = CreateImage($"MapGridV_{i}", parent, new Color(0.12f, 0.58f, 0.68f, 0.08f));
            line.raycastTarget = false;
            SetAnchor(line.rectTransform, new Vector2(x, 0f), new Vector2(x, 1f), new Vector2(-1f, 0f), new Vector2(1f, 0f));
        }

        for (int i = 1; i < 5; i++)
        {
            float y = i / 5f;
            Image line = CreateImage($"MapGridH_{i}", parent, new Color(0.12f, 0.58f, 0.68f, 0.06f));
            line.raycastTarget = false;
            SetAnchor(line.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }

        TextMeshProUGUI label = CreateText("MapFieldLabel", parent, "SECTOR ROUTE", 13, FontStyle.Bold, TextAnchor.MiddleLeft);
        label.color = new Color(0.62f, 0.94f, 1f, 0.42f);
        label.characterSpacing = 4f;
        SetAnchor(label.rectTransform, new Vector2(0f, 0f), new Vector2(0.42f, 0f), new Vector2(24f, 16f), new Vector2(0f, 46f));
    }

    void BuildMainMenu()
    {
        RectTransform title = CreatePanel("TitleBlock", root, new Color(0.02f, 0.035f, 0.052f, 0.78f));
        AddFrame(title, new Color(0.06f, 0.3f, 0.38f, 0.85f), new Vector2(2f, -2f));
        SetAnchor(title, new Vector2(0.08f, 0.56f), new Vector2(0.58f, 0.8f), Vector2.zero, Vector2.zero);
        AddAccent(title);
        AddCornerTicks(title, new Color(accent.r, accent.g, accent.b, 0.7f));

        TextMeshProUGUI name = CreateText("Title", title, "CORELINE DEFENSE", 74, FontStyle.Bold, TextAnchor.MiddleLeft);
        name.characterSpacing = 6f;
        name.gameObject.AddComponent<UiTitleFlicker>();
        SetAnchor(name.rectTransform, new Vector2(0f, 0.36f), Vector2.one, new Vector2(44f, 0f), new Vector2(-24f, -12f));

        TextMeshProUGUI sub = CreateText("Subtitle", title, "ROBOT SIEGE  /  TACTICAL GRID DEFENSE", 18, FontStyle.Bold, TextAnchor.MiddleLeft);
        sub.color = new Color(0.66f, 0.9f, 0.96f, 1f);
        sub.characterSpacing = 3f;
        SetAnchor(sub.rectTransform, Vector2.zero, new Vector2(1f, 0.36f), new Vector2(48f, 8f), new Vector2(-24f, 0f));

        RectTransform command = CreateColorPanel("CommandPanel", root, new Color(0.025f, 0.034f, 0.052f, 0.96f));
        AddFrame(command, new Color(0.09f, 0.33f, 0.4f, 0.95f), new Vector2(2f, -2f));
        AddAccent(command);
        AddCornerTicks(command, new Color(accent.r, accent.g, accent.b, 0.62f));
        SetAnchor(command, new Vector2(0.63f, 0.22f), new Vector2(0.93f, 0.78f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI header = CreateText("Header", command, "COMMAND", 27, FontStyle.Bold, TextAnchor.MiddleLeft);
        header.characterSpacing = 5f;
        SetAnchor(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(36f, -76f), new Vector2(-24f, -22f));

        RectTransform buttons = new GameObject("Buttons", typeof(RectTransform), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
        buttons.SetParent(command, false);
        SetAnchor(buttons, new Vector2(0f, 0.18f), new Vector2(1f, 0.72f), new Vector2(36f, 0f), new Vector2(-36f, 0f));
        VerticalLayoutGroup layout = buttons.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 16f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        CreateMenuButton("StartGame", buttons, "START GAME", true, () => Load(SceneNames.MissionMap));
        CreateMenuButton("Settings", buttons, "SETTING", false, () => Load(SceneNames.Settings));
        CreateMenuButton("HowToPlay", buttons, "HOW TO PLAY", false, () => Load(SceneNames.HowToPlay));
        Button exitButton = CreateMenuButton("Exit", buttons, "EXIT", false, Quit);
        AddFrame((RectTransform)exitButton.transform, new Color(hot.r, hot.g, hot.b, 0.7f));
        TextMeshProUGUI exitLabel = exitButton.GetComponentInChildren<TextMeshProUGUI>();
        if (exitLabel != null)
            exitLabel.color = new Color(1f, 0.62f, 0.58f, 1f);

        TextMeshProUGUI footer = CreateText("Footer", command, "MISSION MAP ONLINE", 13, FontStyle.Bold, TextAnchor.MiddleLeft);
        footer.color = new Color(0.48f, 0.75f, 0.82f, 0.74f);
        footer.characterSpacing = 2f;
        SetAnchor(footer.rectTransform, Vector2.zero, new Vector2(1f, 0.16f), new Vector2(36f, 8f), new Vector2(-20f, 0f));

        RectTransform signal = CreatePanel("SignalPanel", root, new Color(0.012f, 0.026f, 0.036f, 0.5f));
        AddFrame(signal, new Color(0.05f, 0.22f, 0.27f, 0.5f));
        AddCornerTicks(signal, new Color(accent.r, accent.g, accent.b, 0.42f));
        SetAnchor(signal, new Vector2(0.09f, 0.2f), new Vector2(0.55f, 0.48f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI signalText = CreateText("SignalText", signal, "CORELINE NET ONLINE\nSELECT AN OPERATION", 21, FontStyle.Bold, TextAnchor.MiddleLeft);
        signalText.color = new Color(0.7f, 0.94f, 1f, 0.86f);
        signalText.characterSpacing = 2f;
        SetAnchor(signalText.rectTransform, Vector2.zero, Vector2.one, new Vector2(34f, 0f), new Vector2(-24f, 0f));
    }

    void BuildSettings()
    {
        BuildHeader("SETTING", "Tune audio and comfort options.");

        RectTransform card = CreatePanel("SettingsCard", root, new Color(0.032f, 0.046f, 0.068f, 0.96f));
        AddFrame(card, new Color(0.1f, 0.34f, 0.42f, 0.9f), new Vector2(2f, -2f));
        AddAccent(card);
        AddCornerTicks(card, new Color(accent.r, accent.g, accent.b, 0.62f));
        SetAnchor(card, new Vector2(0.2f, 0.15f), new Vector2(0.8f, 0.76f), Vector2.zero, Vector2.zero);

        RectTransform audio = CreatePanel("AudioPanel", card, new Color(0.055f, 0.072f, 0.1f, 0.86f));
        AddFrame(audio, new Color(0.1f, 0.24f, 0.31f, 0.82f));
        AddCornerTicks(audio, new Color(accent.r, accent.g, accent.b, 0.36f));
        SetAnchor(audio, new Vector2(0.08f, 0.5f), new Vector2(0.92f, 0.88f), Vector2.zero, Vector2.zero);
        AddSectionTitle(audio, "AUDIO");
        CreateSliderRow(audio, "Music", GameSettings.MusicVolume, 0.58f, value =>
        {
            GameSettings.MusicVolume = value;
            if (musicValueText != null) musicValueText.text = Percent(value);
            AudioManager.RefreshMusic();
        }, out musicValueText);
        CreateSliderRow(audio, "SFX", GameSettings.SfxVolume, 0.26f, value =>
        {
            GameSettings.SfxVolume = value;
            if (sfxValueText != null) sfxValueText.text = Percent(value);
        }, out sfxValueText);

        RectTransform comfort = CreatePanel("ComfortPanel", card, new Color(0.055f, 0.072f, 0.1f, 0.86f));
        AddFrame(comfort, new Color(0.1f, 0.24f, 0.31f, 0.82f));
        AddCornerTicks(comfort, new Color(accent.r, accent.g, accent.b, 0.36f));
        SetAnchor(comfort, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.44f), Vector2.zero, Vector2.zero);
        AddSectionTitle(comfort, "COMFORT");
        reduceShakeToggle = CreateSettingsToggle(comfort, "Reduce shake", GameSettings.ReduceShake, new Vector2(0.08f, 0.18f), value => GameSettings.ReduceShake = value);
        vibrationToggle = CreateSettingsToggle(comfort, "Vibration", GameSettings.VibrationEnabled, new Vector2(0.55f, 0.18f), value => GameSettings.VibrationEnabled = value);

        Button back = CreateButton("BackButton", root, "BACK", 16, panelSoft, Color.white);
        back.onClick.AddListener(GoBack);
        SetAnchor((RectTransform)back.transform, new Vector2(0.5f, 0.06f), new Vector2(0.5f, 0.06f),
            new Vector2(-UiSpec.SecondaryButtonWidth * 0.5f, -UiSpec.SecondaryButtonHeight * 0.5f),
            new Vector2(UiSpec.SecondaryButtonWidth * 0.5f, UiSpec.SecondaryButtonHeight * 0.5f));
    }

    void BuildHowToPlay()
    {
        BuildHeader("HOW TO PLAY", "Hold the grid, collect energy, clear the route.");

        RectTransform content = CreatePanel("HowToPlayContent", root, new Color(0.025f, 0.037f, 0.055f, 0.9f));
        AddFrame(content, new Color(0.1f, 0.33f, 0.4f, 0.85f), new Vector2(2f, -2f));
        AddCornerTicks(content, new Color(accent.r, accent.g, accent.b, 0.48f));
        SetAnchor(content, new Vector2(0.12f, 0.16f), new Vector2(0.88f, 0.74f), Vector2.zero, Vector2.zero);

        CreateHowToCard(content, "01", "Build The Line", "Place ArcReactors for energy, then deploy Turrets, Bunkers, SnowGuns, and EMP Drones on open cells.", new Vector2(0.04f, 0.54f), new Vector2(0.48f, 0.9f));
        CreateHowToCard(content, "02", "Collect Energy", "Click energy orbs before they fade. Armored enemies can drop bonus energy when defeated.", new Vector2(0.52f, 0.54f), new Vector2(0.96f, 0.9f));
        CreateHowToCard(content, "03", "Read The Lanes", "Watch wave intel and lane pressure. Use Overcharge on rows that are close to breaking.", new Vector2(0.04f, 0.12f), new Vector2(0.48f, 0.48f));
        CreateHowToCard(content, "04", "Win The Sector", "Survive every wave. If enemies breach after the rail cannon is spent, the sector falls.", new Vector2(0.52f, 0.12f), new Vector2(0.96f, 0.48f));

        Button back = CreateButton("BackButton", root, "BACK", 16, panelSoft, Color.white);
        back.onClick.AddListener(GoBack);
        SetAnchor((RectTransform)back.transform, new Vector2(0.5f, 0.06f), new Vector2(0.5f, 0.06f),
            new Vector2(-UiSpec.SecondaryButtonWidth * 0.5f, -UiSpec.SecondaryButtonHeight * 0.5f),
            new Vector2(UiSpec.SecondaryButtonWidth * 0.5f, UiSpec.SecondaryButtonHeight * 0.5f));
    }

    void BuildMissionMap()
    {
        BuildHeader("MISSION MAP", "Select a sector, review enemy intel, then deploy.");

        RectTransform frame = CreatePanel("MissionFrame", root, new Color(0.025f, 0.037f, 0.055f, 0.92f));
        AddFrame(frame, new Color(0.1f, 0.33f, 0.4f, 0.85f), new Vector2(2f, -2f));
        AddCornerTicks(frame, new Color(accent.r, accent.g, accent.b, 0.5f));
        SetAnchor(frame, new Vector2(0.05f, 0.12f), new Vector2(0.95f, 0.78f), Vector2.zero, Vector2.zero);

        mapPanel = CreatePanel("CampaignMap", frame, new Color(0.014f, 0.027f, 0.04f, 0.96f));
        AddFrame(mapPanel, new Color(0.08f, 0.18f, 0.24f, 0.9f));
        SetAnchor(mapPanel, new Vector2(0.035f, 0.09f), new Vector2(0.68f, 0.91f), Vector2.zero, Vector2.zero);
        BuildMapField(mapPanel);
        mapPanel.gameObject.AddComponent<RectMask2D>();

        mapScroll = mapPanel.gameObject.AddComponent<ScrollRect>();
        mapScroll.horizontal = true;
        mapScroll.vertical = false;
        mapScroll.movementType = ScrollRect.MovementType.Clamped;
        mapScroll.scrollSensitivity = 48f;
        mapScroll.inertia = true;
        mapScroll.decelerationRate = 0.12f;
        mapScroll.viewport = mapPanel;

        mapContent = new GameObject("CampaignMapContent", typeof(RectTransform)).GetComponent<RectTransform>();
        mapContent.SetParent(mapPanel, false);
        mapContent.pivot = new Vector2(0f, 0.5f);
        SetAnchor(mapContent, new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero, Vector2.zero);
        mapScroll.content = mapContent;

        routeLayer = new GameObject("RouteLayer", typeof(RectTransform)).GetComponent<RectTransform>();
        routeLayer.SetParent(mapContent, false);
        SetAnchor(routeLayer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        nodeLayer = new GameObject("NodeLayer", typeof(RectTransform)).GetComponent<RectTransform>();
        nodeLayer.SetParent(mapContent, false);
        SetAnchor(nodeLayer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        BuildMissionDetail(frame);
        RebuildMissionNodes();
        RefreshMissionDetail();
        FocusSelectedMission();

        Button back = CreateButton("BackButton", root, "BACK", 16, panelSoft, Color.white);
        back.onClick.AddListener(GoBack);
        SetAnchor((RectTransform)back.transform, new Vector2(0.08f, 0.045f), new Vector2(0.08f, 0.045f),
            new Vector2(-UiSpec.SecondaryButtonWidth * 0.5f, -UiSpec.SecondaryButtonHeight * 0.5f),
            new Vector2(UiSpec.SecondaryButtonWidth * 0.5f, UiSpec.SecondaryButtonHeight * 0.5f));
    }

    void BuildMissionDetail(RectTransform frame)
    {
        detailPanel = CreatePanel("MissionDetail", frame, new Color(0.045f, 0.06f, 0.085f, 0.96f));
        AddFrame(detailPanel, new Color(0.11f, 0.27f, 0.34f, 0.9f));
        AddAccent(detailPanel);
        AddCornerTicks(detailPanel, new Color(accent.r, accent.g, accent.b, 0.5f));
        SetAnchor(detailPanel, new Vector2(0.705f, 0.09f), new Vector2(0.965f, 0.91f), Vector2.zero, Vector2.zero);

        missionStatusText = CreateText("Status", detailPanel, "", 16, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionStatusText.color = accent;
        SetAnchor(missionStatusText.rectTransform, new Vector2(0f, 1f), new Vector2(0.55f, 1f), new Vector2(24f, -46f), new Vector2(-8f, -14f));

        missionDevModeText = CreateText("DevMode", detailPanel, "", 12, FontStyle.Bold, TextAnchor.MiddleRight);
        missionDevModeText.color = hot;
        SetAnchor(missionDevModeText.rectTransform, new Vector2(0.45f, 1f), new Vector2(1f, 1f), new Vector2(8f, -46f), new Vector2(-22f, -14f));

        missionTitleText = CreateText("Title", detailPanel, "", 27, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(missionTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -96f), new Vector2(-24f, -48f));

        missionTypeText = CreateText("Type", detailPanel, "", 17, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionTypeText.color = new Color(0.78f, 0.92f, 0.98f, 1f);
        SetAnchor(missionTypeText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -130f), new Vector2(-24f, -96f));

        missionBriefingText = CreateText("Briefing", detailPanel, "", 16, FontStyle.Normal, TextAnchor.UpperLeft);
        missionBriefingText.color = new Color(0.82f, 0.9f, 0.94f, 1f);
        SetAnchor(missionBriefingText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -244f), new Vector2(-24f, -142f));

        missionEnemyMixText = CreateText("EnemyMix", detailPanel, "", 16, FontStyle.Bold, TextAnchor.UpperLeft);
        SetAnchor(missionEnemyMixText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -312f), new Vector2(-24f, -252f));

        missionToolsText = CreateText("Tools", detailPanel, "", 16, FontStyle.Bold, TextAnchor.UpperLeft);
        missionToolsText.color = new Color(0.78f, 0.95f, 1f, 1f);
        SetAnchor(missionToolsText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -430f), new Vector2(-24f, -316f));

        missionPressureText = CreateText("Pressure", detailPanel, "", 15, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionPressureText.color = new Color(1f, 0.68f, 0.22f, 1f);
        SetAnchor(missionPressureText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 130f), new Vector2(-24f, 160f));

        missionRewardText = CreateText("Reward", detailPanel, "", 15, FontStyle.Normal, TextAnchor.UpperLeft);
        missionRewardText.color = new Color(0.78f, 0.88f, 0.94f, 1f);
        SetAnchor(missionRewardText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 92f), new Vector2(-24f, 124f));

        deployButton = CreateButton("DeployButton", detailPanel, "DEPLOY", 16, UiSpec.ButtonNormal, UiSpec.Text);
        deployButton.onClick.AddListener(DeploySelectedMission);
        deployButtonText = deployButton.GetComponentInChildren<TextMeshProUGUI>();
        SetAnchor((RectTransform)deployButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(-UiSpec.SecondaryButtonWidth * 0.5f, 22f),
            new Vector2(UiSpec.SecondaryButtonWidth * 0.5f, 22f + UiSpec.SecondaryButtonHeight));
    }

    void RebuildMissionNodes()
    {
        foreach (Transform child in routeLayer)
            Destroy(child.gameObject);
        foreach (Transform child in nodeLayer)
            Destroy(child.gameObject);

        int count = levelCatalog != null ? levelCatalog.Count : 0;
        float width = Mathf.Max(MapMinWidth, NodeSidePadding * 2f + Mathf.Max(0, count - 1) * NodeStep);
        SetAnchor(mapContent, new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero, new Vector2(width, 0f));
        BuildRoutes(count);

        for (int i = 0; i < count; i++)
        {
            LevelDefinition level = levelCatalog.GetAt(i);
            if (level == null) continue;
            if (selectedLevel == null && level.levelId == PlayerProgress.SelectedLevelId)
                selectedLevel = level;
        }

        if (selectedLevel == null && count > 0)
            selectedLevel = levelCatalog.GetAt(Mathf.Clamp(PlayerProgress.HighestCompletedLevel, 0, count - 1));

        for (int i = 0; i < count; i++)
        {
            LevelDefinition level = levelCatalog.GetAt(i);
            if (level == null) continue;

            LevelDefinition captured = level;
            MissionNodeType nodeType = CampaignIntel.NodeTypeFor(level);
            bool completed = PlayerProgress.IsLevelCompleted(level);
            bool unlocked = IsUnlocked(level);
            bool selected = level == selectedLevel;

            GameObject go = new GameObject($"MissionNode_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(nodeLayer, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(136f, 96f);
            rect.anchoredPosition = NodePosition(i);

            Image frame = go.GetComponent<Image>();
            frame.color = selected ? new Color(0.018f, 0.09f, 0.11f, 0.98f) : (unlocked ? ColorForNode(nodeType) : new Color(0.12f, 0.135f, 0.17f, 0.96f));
            AddFrame(rect, selected
                ? new Color(accent.r, accent.g, accent.b, 0.92f)
                : (completed ? new Color(0.45f, 1f, 0.68f, 0.86f) : new Color(0.1f, 0.26f, 0.34f, 0.86f)));
            AddMissionNodeDecor(rect, selected, completed, unlocked, nodeType);

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = ButtonColors(frame.color, accent);
            button.interactable = unlocked;
            UiInteractMotion motion = go.AddComponent<UiInteractMotion>();
            motion.hoverScale = 1.08f;
            motion.selectedScale = 1.06f;
            motion.pulseAmplitude = 0.022f;
            motion.SetSelected(selected && unlocked);
            button.onClick.AddListener(() =>
            {
                selectedLevel = captured;
                PlayerProgress.SelectLevel(captured);
                AudioManager.PlaySfx(SfxType.UiClick);
                RebuildMissionNodes();
                RefreshMissionDetail();
                FocusSelectedMission();
            });

            TextMeshProUGUI number = CreateText("Number", go.transform, level.levelNumber.ToString("00"), 24, FontStyle.Bold, TextAnchor.MiddleCenter);
            number.color = selected ? new Color(0.82f, 1f, 1f, 1f) : Color.white;
            SetAnchor(number.rectTransform, new Vector2(0f, 0.42f), Vector2.one, new Vector2(8f, -2f), new Vector2(-8f, -2f));

            TextMeshProUGUI type = CreateText("Type", go.transform, CampaignIntel.NodeTypeLabel(nodeType), 11, FontStyle.Bold, TextAnchor.MiddleCenter);
            type.color = selected ? new Color(0.38f, 0.92f, 1f, 1f) : new Color(0.82f, 0.93f, 0.98f, 1f);
            SetAnchor(type.rectTransform, new Vector2(0f, 0.16f), new Vector2(1f, 0.48f), new Vector2(5f, 0f), new Vector2(-5f, 0f));

            TextMeshProUGUI status = CreateText("Status", go.transform, unlocked ? (completed ? "CLEAR" : "READY") : "LOCKED", 11, FontStyle.Bold, TextAnchor.MiddleCenter);
            status.color = selected ? new Color(0.82f, 1f, 1f, 0.92f) : (unlocked ? Color.white : new Color(0.6f, 0.64f, 0.68f, 1f));
            SetAnchor(status.rectTransform, Vector2.zero, new Vector2(1f, 0.22f), new Vector2(5f, 0f), new Vector2(-5f, 1f));
        }
    }

    void BuildRoutes(int count)
    {
        for (int i = 0; i < count - 1; i++)
        {
            Vector2 from = NodePosition(i);
            Vector2 to = NodePosition(i + 1);
            CreateRoute(from, new Vector2(to.x, from.y));
            CreateRoute(new Vector2(to.x, from.y), to);
        }
    }

    void CreateRoute(Vector2 from, Vector2 to)
    {
        if (Vector2.Distance(from, to) < 0.01f) return;

        Image glow = CreateImage("RouteGlow", routeLayer, new Color(accent.r, accent.g, accent.b, 0.14f));
        ConfigureRouteRect(glow.rectTransform, from, to, 18f);

        Image baseLine = CreateImage("RouteBase", routeLayer, new Color(0f, 0f, 0f, 0.42f));
        ConfigureRouteRect(baseLine.rectTransform, from, to, 10f);

        Image line = CreateImage("Route", routeLayer, new Color(0.12f, 0.62f, 0.74f, 0.72f));
        ConfigureRouteRect(line.rectTransform, from, to, 5f);
    }

    void ConfigureRouteRect(RectTransform rect, Vector2 from, Vector2 to, float thickness)
    {
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        if (Mathf.Abs(from.y - to.y) <= Mathf.Abs(from.x - to.x))
        {
            rect.anchoredPosition = new Vector2((from.x + to.x) * 0.5f, from.y);
            rect.sizeDelta = new Vector2(Mathf.Abs(to.x - from.x), thickness);
        }
        else
        {
            rect.anchoredPosition = new Vector2(from.x, (from.y + to.y) * 0.5f);
            rect.sizeDelta = new Vector2(thickness, Mathf.Abs(to.y - from.y));
        }
    }

    void RefreshMissionDetail()
    {
        LevelDefinition level = selectedLevel;
        if (level == null)
        {
            deployButton.interactable = false;
            return;
        }

        bool unlocked = IsUnlocked(level);
        bool completed = PlayerProgress.IsLevelCompleted(level);
        EnemyMix mix = CampaignIntel.BuildLevelMix(level);
        MissionNodeType nodeType = CampaignIntel.NodeTypeFor(level);

        missionStatusText.text = unlocked ? (completed ? "CLEAR" : "READY") : "LOCKED";
        missionStatusText.color = unlocked ? accent : dim;
        missionDevModeText.text = PlayerProgress.HighestCompletedLevel >= level.levelNumber ? "" : "";
        missionTitleText.text = $"{level.levelNumber:00}  {level.displayName}";
        missionTypeText.text = CampaignIntel.NodeTypeLabel(nodeType);
        missionBriefingText.text = !unlocked
            ? "Complete the previous mission to unlock this sector."
            : (!string.IsNullOrWhiteSpace(level.missionBriefing) ? level.missionBriefing : "Ready for deployment.");
        missionEnemyMixText.text = $"Enemy mix\n{CampaignIntel.BuildMixLabel(mix)}";
        missionToolsText.text = $"Recommended tools\n{CampaignIntel.BuildRecommendedToolsStack(mix)}";
        missionPressureText.text = $"Pressure score: {CampaignIntel.PressureScore(level)}";
        missionRewardText.text = completed && !string.IsNullOrWhiteSpace(level.completionReward)
            ? level.completionReward
            : "Clear the sector to advance the campaign route.";

        deployButton.interactable = unlocked;
        deployButtonText.text = unlocked ? "DEPLOY" : "LOCKED";
    }

    void FocusSelectedMission()
    {
        if (selectedLevel == null || levelCatalog == null || mapScroll == null) return;

        int index = 0;
        for (int i = 0; i < levelCatalog.Count; i++)
        {
            if (levelCatalog.GetAt(i) == selectedLevel)
            {
                index = i;
                break;
            }
        }

        Canvas.ForceUpdateCanvases();
        float contentWidth = Mathf.Max(MapMinWidth, mapContent.rect.width);
        float viewportWidth = Mathf.Max(1f, mapPanel.rect.width);
        float scrollableWidth = Mathf.Max(1f, contentWidth - viewportWidth);
        float targetX = NodePosition(index).x - viewportWidth * 0.42f;
        mapScroll.horizontalNormalizedPosition = Mathf.Clamp01(targetX / scrollableWidth);
    }

    void DeploySelectedMission()
    {
        if (selectedLevel == null || !IsUnlocked(selectedLevel)) return;

        PlayerProgress.SelectLevel(selectedLevel);
        AudioManager.PlaySfx(SfxType.UiClick);
        Time.timeScale = 1f;
        LevelDefinition level = selectedLevel;
        UiSceneTransition.Play(this, root, $"DEPLOYING SECTOR {level.levelNumber:00}", () =>
        {
            FrontendNavigation.LoadScene(SceneNames.Game);
        });
    }

    bool IsUnlocked(LevelDefinition level)
    {
        return level != null && (unlockAllLevelsForTesting || PlayerProgress.IsLevelUnlocked(level));
    }

    Vector2 NodePosition(int index)
    {
        return new Vector2(NodeSidePadding + index * NodeStep, NodeOffsetY(index));
    }

    float NodeOffsetY(int index)
    {
        switch (index % 10)
        {
            case 1: return 96f;
            case 2: return -34f;
            case 3: return 132f;
            case 4: return 12f;
            case 5: return -128f;
            case 6: return 102f;
            case 7: return -60f;
            case 8: return 148f;
            case 9: return -100f;
            default: return -88f;
        }
    }

    Color ColorForNode(MissionNodeType type)
    {
        switch (type)
        {
            case MissionNodeType.ArmorGate:
            case MissionNodeType.IronRain:
                return new Color(0.19f, 0.2f, 0.24f, 0.96f);
            case MissionNodeType.RaiderTrack:
            case MissionNodeType.VelocityNet:
                return new Color(0.1f, 0.2f, 0.24f, 0.96f);
            case MissionNodeType.ShieldColumn:
            case MissionNodeType.EmpCorridor:
                return new Color(0.15f, 0.16f, 0.25f, 0.96f);
            case MissionNodeType.CorelineStand:
                return new Color(0.32f, 0.08f, 0.12f, 0.96f);
            default:
                return panelSoft;
        }
    }

    void BuildHeader(string title, string subtitle)
    {
        TextMeshProUGUI heading = CreateText("ScreenTitle", root, title, 56, FontStyle.Bold, TextAnchor.MiddleCenter);
        heading.characterSpacing = 5f;
        SetAnchor(heading.rectTransform, new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.94f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI sub = CreateText("ScreenSubtitle", root, subtitle, 18, FontStyle.Bold, TextAnchor.MiddleCenter);
        sub.color = new Color(0.66f, 0.86f, 0.92f, 1f);
        sub.characterSpacing = 2f;
        SetAnchor(sub.rectTransform, new Vector2(0.18f, 0.78f), new Vector2(0.82f, 0.83f), Vector2.zero, Vector2.zero);
    }

    void AddSectionTitle(RectTransform parent, string title)
    {
        TextMeshProUGUI label = CreateText("SectionTitle", parent, title, 26, FontStyle.Bold, TextAnchor.MiddleLeft);
        label.characterSpacing = 3f;
        SetAnchor(label.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(34f, -66f), new Vector2(-24f, -18f));
    }

    void CreateSliderRow(RectTransform parent, string label, float value, float y, UnityEngine.Events.UnityAction<float> onChanged, out TextMeshProUGUI valueText)
    {
        TextMeshProUGUI labelText = CreateText($"{label}Label", parent, label, 21, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(labelText.rectTransform, new Vector2(0.08f, y + 0.06f), new Vector2(0.32f, y + 0.18f), Vector2.zero, Vector2.zero);

        valueText = CreateText($"{label}Value", parent, Percent(value), 19, FontStyle.Bold, TextAnchor.MiddleRight);
        valueText.color = new Color(0.8f, 0.92f, 0.96f, 1f);
        SetAnchor(valueText.rectTransform, new Vector2(0.78f, y + 0.06f), new Vector2(0.92f, y + 0.18f), Vector2.zero, Vector2.zero);

        Slider slider = CreateSlider($"{label}Slider", parent, value);
        SetAnchor((RectTransform)slider.transform, new Vector2(0.08f, y - 0.05f), new Vector2(0.92f, y - 0.05f), new Vector2(0f, -8f), new Vector2(0f, 8f));
        slider.onValueChanged.AddListener(onChanged);
    }

    Toggle CreateSettingsToggle(RectTransform parent, string label, bool value, Vector2 anchor, UnityEngine.Events.UnityAction<bool> onChanged)
    {
        Toggle toggle = CreateToggle(label.Replace(" ", ""), parent, label);
        toggle.SetIsOnWithoutNotify(value);
        RefreshToggleVisual(toggle);
        toggle.onValueChanged.AddListener(onChanged);
        SetAnchor((RectTransform)toggle.transform, anchor, anchor + new Vector2(0.36f, 0.18f), Vector2.zero, Vector2.zero);
        return toggle;
    }

    void CreateHowToCard(RectTransform parent, string step, string title, string body, Vector2 min, Vector2 max)
    {
        RectTransform card = CreatePanel($"HowTo_{step}", parent, new Color(0.05f, 0.065f, 0.09f, 0.92f));
        AddFrame(card, new Color(0.09f, 0.24f, 0.31f, 0.9f));
        AddAccent(card);
        SetAnchor(card, min, max, Vector2.zero, Vector2.zero);

        TextMeshProUGUI number = CreateText("Step", card, step, 28, FontStyle.Bold, TextAnchor.MiddleLeft);
        number.color = accent;
        SetAnchor(number.rectTransform, new Vector2(0f, 1f), new Vector2(0.22f, 1f), new Vector2(28f, -76f), new Vector2(0f, -20f));

        TextMeshProUGUI head = CreateText("Title", card, title, 25, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(head.rectTransform, new Vector2(0.18f, 1f), new Vector2(1f, 1f), new Vector2(0f, -76f), new Vector2(-24f, -20f));

        TextMeshProUGUI copy = CreateText("Body", card, body, 18, FontStyle.Normal, TextAnchor.UpperLeft);
        copy.color = new Color(0.82f, 0.9f, 0.94f, 1f);
        SetAnchor(copy.rectTransform, Vector2.zero, Vector2.one, new Vector2(28f, 26f), new Vector2(-28f, -92f));
    }

    Button CreateMenuButton(string name, Transform parent, string text, bool primary, UnityEngine.Events.UnityAction action)
    {
        Button button = CreateButton(name, parent, text, 16, UiSpec.ButtonNormal, text == "EXIT" ? UiSpec.Accent : UiSpec.Text);
        button.onClick.AddListener(action);
        LayoutElement layout = button.gameObject.AddComponent<LayoutElement>();
        layout.minWidth = UiSpec.MenuButtonWidth;
        layout.preferredWidth = UiSpec.MenuButtonWidth;
        layout.minHeight = UiSpec.MenuButtonHeight;
        layout.preferredHeight = UiSpec.MenuButtonHeight;
        return button;
    }

    Button CreateButton(string name, Transform parent, string text, int size, Color normal, Color textColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        ApplySprite(image, buttonSprite, UiSpec.ButtonNormal, false);

        Button button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        button.colors = ButtonColors(UiSpec.ButtonNormal, UiSpec.ButtonHover);
        go.AddComponent<PixelButtonPressOffset>();
        go.AddComponent<UiInteractMotion>();

        TextMeshProUGUI label = CreateText("Text", go.transform, text, size, FontStyle.Bold, TextAnchor.MiddleCenter);
        label.color = textColor;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 0f), new Vector2(-6f, 0f));
        return button;
    }

    Slider CreateSlider(string name, Transform parent, float initialValue)
    {
        GameObject rootObj = new GameObject(name, typeof(RectTransform), typeof(Slider));
        rootObj.transform.SetParent(parent, false);
        Slider slider = rootObj.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = Mathf.Clamp01(initialValue);

        RectTransform background = CreateColorPanel("Background", rootObj.transform, new Color(0.07f, 0.1f, 0.14f, 1f));
        SetAnchor(background, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
        fillArea.SetParent(rootObj.transform, false);
        SetAnchor(fillArea, Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f));

        Image fill = CreateImage("Fill", fillArea, accent);
        SetAnchor(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image handle = CreateImage("Handle", rootObj.transform, accent);
        SetAnchor(handle.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-8f, -15f), new Vector2(8f, 15f));

        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        return slider;
    }

    Toggle CreateToggle(string name, Transform parent, string labelText)
    {
        GameObject rootObj = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        rootObj.transform.SetParent(parent, false);
        Toggle toggle = rootObj.GetComponent<Toggle>();

        Image box = CreateImage("Track", rootObj.transform, new Color(0.08f, 0.12f, 0.16f, 1f));
        SetAnchor(box.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, -12f), new Vector2(44f, 12f));
        AddFrame(box.rectTransform, new Color(accent.r, accent.g, accent.b, 0.4f));

        Image check = CreateImage("Knob", box.transform, accent);
        SetAnchor(check.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(3f, -9f), new Vector2(21f, 9f));

        TextMeshProUGUI label = CreateText("Label", rootObj.transform, labelText, 20, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(label.rectTransform, new Vector2(0f, 0f), Vector2.one, new Vector2(52f, 0f), Vector2.zero);

        toggle.targetGraphic = box;
        toggle.graphic = null;
        toggle.onValueChanged.AddListener(_ => RefreshToggleVisual(toggle));
        RefreshToggleVisual(toggle);
        return toggle;
    }

    void RefreshToggleVisual(Toggle toggle)
    {
        if (toggle == null) return;

        Transform trackTransform = toggle.transform.Find("Track");
        Image track = trackTransform != null ? trackTransform.GetComponent<Image>() : null;
        Transform knobTransform = trackTransform != null ? trackTransform.Find("Knob") : null;
        Image knob = knobTransform != null ? knobTransform.GetComponent<Image>() : null;

        bool on = toggle.isOn;
        if (track != null)
        {
            track.color = on
                ? new Color(accent.r * 0.3f, accent.g * 0.3f, accent.b * 0.3f, 0.9f)
                : new Color(0.08f, 0.12f, 0.16f, 1f);
        }

        if (knob != null)
        {
            knob.color = on ? accent : new Color(0.55f, 0.65f, 0.72f, 1f);
            SetAnchor(knob.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                on ? new Vector2(23f, -9f) : new Vector2(3f, -9f),
                on ? new Vector2(41f, 9f) : new Vector2(21f, 9f));
        }
    }

    RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        ApplySprite(go.GetComponent<Image>(), panelSprite, color, false);
        return (RectTransform)go.transform;
    }

    RectTransform CreateColorPanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return (RectTransform)go.transform;
    }

    Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    void ApplySprite(Image image, Sprite sprite, Color color, bool preserveAspect)
    {
        if (image == null) return;

        image.color = color;
        if (sprite == null) return;

        image.sprite = sprite;
        image.type = HasBorder(sprite) ? Image.Type.Sliced : Image.Type.Simple;
        image.preserveAspect = preserveAspect;
    }

    bool HasBorder(Sprite sprite)
    {
        return sprite != null && sprite.border.sqrMagnitude > 0.01f;
    }

    TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, FontStyle style, TextAnchor alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.enableAutoSizing = true;
        label.fontSizeMax = size;
        label.fontSizeMin = size;
        label.fontStyle = ToTmpFontStyle(style);
        label.alignment = ToTmpAlignment(alignment);
        label.color = UiSpec.Text;
        label.outlineWidth = 0f;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Ellipsis;
        return label;
    }

    void AddFrame(RectTransform rect, Color color)
    {
        AddFrame(rect, color, new Vector2(1.5f, -1.5f));
    }

    void AddFrame(RectTransform rect, Color color, Vector2 distance)
    {
        if (rect == null) return;
        if (rect.GetComponent<Button>() != null) return;

        Outline outline = rect.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = rect.gameObject.AddComponent<Outline>();

        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }

    void AddAccent(RectTransform parent)
    {
        Image left = CreateImage("LeftAccent", parent, new Color(accent.r, accent.g, accent.b, 0.24f));
        left.raycastTarget = false;
        SetAnchor(left.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(3f, 8f), new Vector2(7f, -8f));
    }

    void AddCornerTicks(RectTransform parent, Color color)
    {
        Image tlH = CreateImage("CornerTopLeftH", parent, color);
        tlH.raycastTarget = false;
        SetAnchor(tlH.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(12f, -5f), new Vector2(58f, -2f));

        Image tlV = CreateImage("CornerTopLeftV", parent, color);
        tlV.raycastTarget = false;
        SetAnchor(tlV.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(12f, -48f), new Vector2(15f, -5f));

        Image brH = CreateImage("CornerBottomRightH", parent, color);
        brH.raycastTarget = false;
        SetAnchor(brH.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-58f, 2f), new Vector2(-12f, 5f));

        Image brV = CreateImage("CornerBottomRightV", parent, color);
        brV.raycastTarget = false;
        SetAnchor(brV.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-15f, 5f), new Vector2(-12f, 48f));
    }

    void AddMissionNodeDecor(RectTransform parent, bool selected, bool completed, bool unlocked, MissionNodeType nodeType)
    {
        Color stripColor = selected
            ? new Color(0.02f, 0.08f, 0.1f, 0.45f)
            : (completed ? new Color(0.3f, 0.9f, 0.62f, 0.42f) : new Color(accent.r, accent.g, accent.b, unlocked ? 0.3f : 0.1f));
        Image strip = CreateImage("StatusStrip", parent, stripColor);
        strip.raycastTarget = false;
        SetAnchor(strip.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(10f, -8f), new Vector2(-10f, -4f));

        Color dotColor = completed
            ? new Color(0.3f, 0.9f, 0.62f, 0.95f)
            : (unlocked ? new Color(1f, 0.68f, 0.22f, 0.9f) : dim);
        Image dot = CreateImage("StatusDot", parent, dotColor);
        dot.raycastTarget = false;
        SetAnchor(dot.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -22f), new Vector2(-12f, -10f));

        Image nodeAccent = CreateImage("NodeAccent", parent, ColorForNode(nodeType));
        nodeAccent.raycastTarget = false;
        SetAnchor(nodeAccent.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(4f, 10f), new Vector2(8f, -10f));
    }

    void AddButtonAccent(RectTransform parent)
    {
        // `button_command` is the shared button chrome; keep frontend buttons asset-led.
    }

    ColorBlock ButtonColors(Color normal, Color highlighted)
    {
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = UiSpec.ButtonNormal;
        colors.highlightedColor = UiSpec.ButtonHover;
        colors.pressedColor = UiSpec.ButtonPressed;
        colors.selectedColor = UiSpec.ButtonHover;
        colors.disabledColor = UiSpec.ButtonDisabled;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.05f;
        return colors;
    }

    void SetAnchor(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    FontStyles ToTmpFontStyle(FontStyle style)
    {
        switch (style)
        {
            case FontStyle.Bold: return FontStyles.Bold;
            case FontStyle.Italic: return FontStyles.Italic;
            case FontStyle.BoldAndItalic: return FontStyles.Bold | FontStyles.Italic;
            default: return FontStyles.Normal;
        }
    }

    TextAlignmentOptions ToTmpAlignment(TextAnchor alignment)
    {
        switch (alignment)
        {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
            default: return TextAlignmentOptions.Center;
        }
    }

    string Percent(float value)
    {
        return $"{Mathf.RoundToInt(Mathf.Clamp01(value) * 100f)}%";
    }

    void Load(string sceneName)
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        FrontendNavigation.LoadScene(sceneName);
    }

    void GoBack()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        FrontendNavigation.Back();
    }

    void Quit()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        Application.Quit();
    }

    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystem.transform.SetParent(transform, false);
    }

    void EnsureRenderCamera()
    {
        if (Camera.main != null || FindAnyObjectByType<Camera>() != null) return;

        GameObject cameraObject = new GameObject("Frontend Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = bg;
        camera.orthographic = true;
        camera.orthographicSize = 5f;
    }
}
