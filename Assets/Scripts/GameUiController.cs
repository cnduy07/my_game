using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameUiController : MonoBehaviour
{
    public static GameUiController Instance { get; private set; }

    [Header("HUD")]
    public bool isPaused;
    public bool showMainMenuOnLaunch = true;
    public Color backgroundColor = new Color(0.035f, 0.05f, 0.075f, 0.94f);
    public Color panelColor = new Color(0.09f, 0.13f, 0.18f, 0.92f);
    public Color panelSoftColor = new Color(0.13f, 0.18f, 0.24f, 0.9f);
    public Color accentColor = new Color(0.13f, 0.82f, 0.95f, 1f);
    public Color warningColor = new Color(1f, 0.55f, 0.17f, 1f);
    public Color disabledColor = new Color(0.33f, 0.38f, 0.45f, 0.86f);

    const float ReferenceWidth = 1920f;
    const float ReferenceHeight = 1080f;
    const float SeedCardWidth = 188f;
    const float SeedCardHeight = 86f;
    const float SeedTraySpacing = 10f;
    const float SeedTrayHorizontalPadding = 28f;
    const float SeedTrayMinWidth = 420f;
    const float SeedTrayMaxWidth = 1100f;
    const float CampaignNodeWidth = 136f;
    const float CampaignNodeHeight = 96f;
    const float CampaignNodeStep = 190f;
    const float CampaignMapSidePadding = 150f;
    const float CampaignMapMinContentWidth = 1040f;

    Canvas canvas;

    TextMeshProUGUI energyText;
    TextMeshProUGUI levelText;
    TextMeshProUGUI waveText;
    RectTransform waveIntelPanel;
    TextMeshProUGUI waveIntelText;
    Button pauseButton;
    TextMeshProUGUI pauseButtonText;
    Button levelSelectTopButton;

    RectTransform seedTray;
    readonly List<SeedCard> seedCards = new List<SeedCard>();

    RectTransform overchargePanel;
    TextMeshProUGUI overchargeCostText;
    readonly List<RowButton> rowButtons = new List<RowButton>();

    RectTransform tutorialPanel;
    TextMeshProUGUI tutorialText;
    RectTransform commandStatusPanel;
    TextMeshProUGUI commandStatusText;

    GameObject modalOverlay;
    RectTransform modalCard;
    TextMeshProUGUI modalTitleText;
    TextMeshProUGUI modalSubtitleText;
    TextMeshProUGUI progressText;
    RectTransform terminalSceneBackdrop;
    TextMeshProUGUI terminalSceneText;
    Image terminalSceneTint;
    TextMeshProUGUI musicText;
    Slider musicSlider;
    TextMeshProUGUI sfxText;
    Slider sfxSlider;
    Toggle reduceShakeToggle;
    Toggle vibrationToggle;
    Button resumeButton;
    Button restartButton;
    Button modalLevelSelectButton;
    Button mainMenuButton;
    Button nextLevelButton;
    TextMeshProUGUI nextLevelButtonText;

    GameObject mainMenuOverlay;
    TextMeshProUGUI mainMenuSubtitleText;
    GameObject howToPlayOverlay;
    bool mainMenuOpen;
    bool wasPausedBeforeMainMenu;
    static bool mainMenuShownThisSession;

    GameObject levelSelectOverlay;
    RectTransform campaignMapPanel;
    RectTransform campaignMapContent;
    RectTransform campaignRouteLayer;
    RectTransform campaignNodeLayer;
    ScrollRect campaignMapScroll;
    RectTransform missionDetailPanel;
    TextMeshProUGUI missionTitleText;
    TextMeshProUGUI missionTypeText;
    TextMeshProUGUI missionStatusText;
    TextMeshProUGUI missionBriefingText;
    TextMeshProUGUI missionEnemyMixText;
    TextMeshProUGUI missionToolsText;
    TextMeshProUGUI missionPressureText;
    TextMeshProUGUI missionRewardText;
    TextMeshProUGUI missionDevModeText;
    Button missionDeployButton;
    TextMeshProUGUI missionDeployButtonText;
    readonly List<CampaignNode> levelButtons = new List<CampaignNode>();
    LevelDefinition selectedCampaignLevel;

    int lastSeedCount = -1;
    int lastRowCount = -1;
    int lastLevelCount = -1;
    bool modalBuilt;
    bool wasPausedBeforeLevelSelect;

    class SeedCard
    {
        public Button button;
        public Image frame;
        public Image cooldownFill;
        public TextMeshProUGUI label;
        public TextMeshProUGUI cost;
    }

    class RowButton
    {
        public Button button;
        public Image frame;
        public Image threatFill;
        public Image fill;
        public TextMeshProUGUI label;
    }

    class CampaignNode
    {
        public Button button;
        public Image frame;
        public RectTransform rect;
        public TextMeshProUGUI label;
        public TextMeshProUGUI type;
        public TextMeshProUGUI status;
        public Image strip;
        public Image statusDot;
        public LevelDefinition level;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BuildHud();
        if ((showMainMenuOnLaunch && !mainMenuShownThisSession) || shouldOpenMainMenuAfterReload)
            OpenMainMenu();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (isPaused) Time.timeScale = 1f;
    }

    void Update()
    {
        if (canvas == null)
            BuildHud();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (levelSelectOverlay != null && levelSelectOverlay.activeSelf)
            {
                CloseLevelSelect();
                return;
            }

            if (howToPlayOverlay != null && howToPlayOverlay.activeSelf)
            {
                CloseHowToPlay();
                return;
            }

            if (mainMenuOpen)
            {
                CloseMainMenu();
                return;
            }

            if (GameManager.Instance == null || (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsWon))
                TogglePause();
        }

        RebuildDynamicUiIfNeeded();
        RefreshHud();
        RefreshModalState();
    }

    public bool PointerOverPanel(float guiX, float guiY)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        Vector2 screenPoint = new Vector2(guiX, Screen.height - guiY);
        return IsScreenPointIn(seedTray, screenPoint) ||
               IsScreenPointIn(overchargePanel, screenPoint) ||
               (mainMenuOverlay != null && mainMenuOverlay.activeSelf && IsScreenPointIn((RectTransform)mainMenuOverlay.transform, screenPoint)) ||
               (howToPlayOverlay != null && howToPlayOverlay.activeSelf && IsScreenPointIn((RectTransform)howToPlayOverlay.transform, screenPoint)) ||
               (pauseButton != null && IsScreenPointIn((RectTransform)pauseButton.transform, screenPoint)) ||
               (levelSelectTopButton != null && IsScreenPointIn((RectTransform)levelSelectTopButton.transform, screenPoint)) ||
               (modalOverlay != null && modalOverlay.activeSelf && IsScreenPointIn((RectTransform)modalOverlay.transform, screenPoint)) ||
               (levelSelectOverlay != null && levelSelectOverlay.activeSelf && IsScreenPointIn((RectTransform)levelSelectOverlay.transform, screenPoint));
    }

    public bool IsPointerOverUi()
    {
        return PointerOverPanel(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

    void TogglePause()
    {
        SetPaused(!isPaused);
        AudioManager.PlaySfx(SfxType.UiClick);
    }

    void SetPaused(bool paused)
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon))
            paused = false;

        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        RefreshModalState();
    }

    void BuildHud()
    {
        EnsureEventSystem();

        GameObject root = new GameObject("RuntimeHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform safeAreaRoot = new GameObject("SafeAreaRoot", typeof(RectTransform), typeof(SafeAreaFitter)).GetComponent<RectTransform>();
        safeAreaRoot.SetParent(root.transform, false);
        SetAnchor(safeAreaRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        BuildTopBar(safeAreaRoot);
        BuildWaveIntelPanel(safeAreaRoot);
        BuildSeedTray(safeAreaRoot);
        BuildOverchargePanel(safeAreaRoot);
        BuildTutorialPanel(safeAreaRoot);
        BuildCommandStatusPanel(safeAreaRoot);
        BuildMainMenu(safeAreaRoot);
        BuildHowToPlayOverlay(safeAreaRoot);
        BuildModal(safeAreaRoot);
        BuildLevelSelectOverlay(safeAreaRoot);
        RebuildDynamicUiIfNeeded();
        RefreshHud();
        RefreshModalState();
    }

    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystem.transform.SetParent(transform, false);
    }

    void BuildTopBar(Transform parent)
    {
        RectTransform topBar = CreatePanel("TopStatusBar", parent, backgroundColor);
        AddCornerTicks(topBar, new Color(accentColor.r, accentColor.g, accentColor.b, 0.28f));
        SetAnchor(topBar, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -84f), new Vector2(0f, 0f));

        RectTransform energyBox = CreatePanel("EnergyBox", topBar, new Color(0.055f, 0.075f, 0.105f, 0.96f));
        AddFrame(energyBox, new Color(0.16f, 0.28f, 0.36f, 0.9f));
        SetAnchor(energyBox, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(24f, -26f), new Vector2(294f, 26f));

        energyText = CreateText("EnergyText", energyBox, "Energy: 0", 32, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(energyText.rectTransform, Vector2.zero, Vector2.one, new Vector2(18f, 0f), new Vector2(-12f, 0f));

        RectTransform levelBox = CreatePanel("LevelBox", topBar, new Color(0.05f, 0.068f, 0.095f, 0.96f));
        AddFrame(levelBox, new Color(0.14f, 0.23f, 0.3f, 0.9f));
        SetAnchor(levelBox, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-250f, -70f), new Vector2(250f, -16f));

        levelText = CreateText("LevelText", levelBox, "", 30, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(levelText.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 0f), new Vector2(-12f, 0f));

        RectTransform waveBox = CreatePanel("WaveBox", topBar, new Color(0.05f, 0.068f, 0.095f, 0.96f));
        AddFrame(waveBox, new Color(0.14f, 0.23f, 0.3f, 0.9f));
        SetAnchor(waveBox, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-560f, -26f), new Vector2(-358f, 26f));

        waveText = CreateText("WaveText", waveBox, "", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(waveText.rectTransform, Vector2.zero, Vector2.one, new Vector2(10f, 0f), new Vector2(-10f, 0f));

        levelSelectTopButton = CreateButton("LevelSelectButton", topBar, "MISSION", 19, panelSoftColor, accentColor);
        levelSelectTopButton.onClick.AddListener(OpenLevelSelect);
        AddFrame((RectTransform)levelSelectTopButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));
        SetAnchor((RectTransform)levelSelectTopButton.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-338f, -26f), new Vector2(-196f, 26f));

        pauseButton = CreateButton("PauseButton", topBar, "II", 28, panelSoftColor, accentColor);
        pauseButton.onClick.AddListener(TogglePause);
        pauseButtonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
        AddFrame((RectTransform)pauseButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));
        SetAnchor((RectTransform)pauseButton.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-166f, -26f), new Vector2(-24f, 26f));
    }

    void BuildTutorialPanel(Transform parent)
    {
        tutorialPanel = CreatePanel("TutorialHint", parent, new Color(0.035f, 0.05f, 0.075f, 0.86f));
        tutorialPanel.GetComponent<Image>().raycastTarget = false;
        SetAnchor(tutorialPanel, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(34f, 184f), new Vector2(540f, 248f));

        tutorialText = CreateText("Text", tutorialPanel, "", 22, FontStyle.Bold, TextAnchor.MiddleLeft);
        tutorialText.color = new Color(0.86f, 0.96f, 1f, 1f);
        SetAnchor(tutorialText.rectTransform, Vector2.zero, Vector2.one, new Vector2(18f, 8f), new Vector2(-18f, -8f));
        tutorialPanel.gameObject.SetActive(false);
    }

    void BuildWaveIntelPanel(Transform parent)
    {
        waveIntelPanel = CreatePanel("WaveIntel", parent, new Color(0.035f, 0.048f, 0.07f, 0.86f));
        waveIntelPanel.GetComponent<Image>().raycastTarget = false;
        AddFrame(waveIntelPanel, new Color(0.08f, 0.2f, 0.27f, 0.78f));
        AddCornerTicks(waveIntelPanel, new Color(accentColor.r, accentColor.g, accentColor.b, 0.32f));
        SetAnchor(waveIntelPanel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-360f, -142f), new Vector2(360f, -96f));

        waveIntelText = CreateText("Text", waveIntelPanel, "", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
        waveIntelText.color = new Color(0.82f, 0.95f, 1f, 1f);
        SetAnchor(waveIntelText.rectTransform, Vector2.zero, Vector2.one, new Vector2(14f, 2f), new Vector2(-14f, -2f));
        waveIntelPanel.gameObject.SetActive(false);
    }

    void BuildCommandStatusPanel(Transform parent)
    {
        commandStatusPanel = CreatePanel("CommandStatus", parent, new Color(0.035f, 0.048f, 0.07f, 0.88f));
        commandStatusPanel.GetComponent<Image>().raycastTarget = false;
        AddFrame(commandStatusPanel, new Color(0.08f, 0.2f, 0.27f, 0.78f));
        AddCornerTicks(commandStatusPanel, new Color(accentColor.r, accentColor.g, accentColor.b, 0.28f));
        SetAnchor(commandStatusPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-380f, 132f), new Vector2(380f, 176f));

        commandStatusText = CreateText("Text", commandStatusPanel, "", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
        commandStatusText.color = new Color(0.86f, 0.96f, 1f, 1f);
        SetAnchor(commandStatusText.rectTransform, Vector2.zero, Vector2.one, new Vector2(14f, 2f), new Vector2(-14f, -2f));
        commandStatusPanel.gameObject.SetActive(false);
    }

    void BuildSeedTray(Transform parent)
    {
        seedTray = CreatePanel("SeedTray", parent, new Color(0.035f, 0.048f, 0.07f, 0.97f));
        AddFrame(seedTray, new Color(0.1f, 0.2f, 0.27f, 0.9f));
        AddCornerTicks(seedTray, new Color(accentColor.r, accentColor.g, accentColor.b, 0.32f));
        SetAnchor(seedTray, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-550f, 12f), new Vector2(550f, 122f));

        HorizontalLayoutGroup layout = seedTray.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(14, 14, 12, 12);
        layout.spacing = SeedTraySpacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;
    }

    void BuildOverchargePanel(Transform parent)
    {
        overchargePanel = CreatePanel("OverchargePanel", parent, new Color(0.035f, 0.048f, 0.07f, 0.95f));
        AddFrame(overchargePanel, new Color(0.1f, 0.2f, 0.27f, 0.9f));
        AddCornerTicks(overchargePanel, new Color(accentColor.r, accentColor.g, accentColor.b, 0.34f));
        SetAnchor(overchargePanel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-190f, -206f), new Vector2(-24f, 206f));

        overchargeCostText = CreateText("OverchargeCost", overchargePanel, "OC", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(overchargeCostText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(10f, -50f), new Vector2(-10f, -8f));
    }

    void BuildModal(Transform parent)
    {
        modalOverlay = new GameObject("ModalOverlay", typeof(RectTransform), typeof(Image));
        modalOverlay.transform.SetParent(parent, false);
        Image overlayImage = modalOverlay.GetComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.42f);
        SetAnchor((RectTransform)modalOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        modalCard = CreatePanel("ModalCard", modalOverlay.transform, new Color(0.06f, 0.072f, 0.095f, 0.98f));
        AddFrame(modalCard, new Color(0.14f, 0.26f, 0.34f, 0.95f), new Vector2(2f, -2f));
        AddCornerTicks(modalCard, new Color(accentColor.r, accentColor.g, accentColor.b, 0.44f));
        SetAnchor(modalCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-430f, -286f), new Vector2(430f, 286f));

        modalTitleText = CreateText("ModalTitle", modalCard, "PAUSED", 36, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(modalTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -76f), new Vector2(-28f, -22f));

        modalSubtitleText = CreateText("ModalSubtitle", modalCard, "", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
        modalSubtitleText.color = new Color(0.82f, 0.93f, 0.98f, 1f);
        SetAnchor(modalSubtitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -116f), new Vector2(-28f, -78f));

        progressText = CreateText("ProgressText", modalCard, "", 19, FontStyle.Normal, TextAnchor.MiddleCenter);
        progressText.color = new Color(0.86f, 0.91f, 0.95f, 1f);
        SetAnchor(progressText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -166f), new Vector2(-52f, -126f));

        terminalSceneBackdrop = CreatePanel("TerminalSceneBackdrop", modalOverlay.transform, new Color(0.02f, 0.034f, 0.045f, 0.88f));
        terminalSceneBackdrop.SetSiblingIndex(0);
        SetAnchor(terminalSceneBackdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        BuildTerminalScene(terminalSceneBackdrop);

        musicText = CreateText("MusicText", modalCard, "", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(musicText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -214f), new Vector2(-52f, -178f));

        musicSlider = CreateSlider("MusicSlider", modalCard, GameSettings.MusicVolume);
        SetAnchor((RectTransform)musicSlider.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(110f, -248f), new Vector2(-110f, -224f));
        musicSlider.onValueChanged.AddListener(value =>
        {
            GameSettings.MusicVolume = value;
            AudioManager.RefreshMusic();
        });

        sfxText = CreateText("SfxText", modalCard, "", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(sfxText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -296f), new Vector2(-52f, -260f));

        sfxSlider = CreateSlider("SfxSlider", modalCard, GameSettings.SfxVolume);
        SetAnchor((RectTransform)sfxSlider.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(110f, -330f), new Vector2(-110f, -306f));
        sfxSlider.onValueChanged.AddListener(value => GameSettings.SfxVolume = value);

        reduceShakeToggle = CreateToggle("ReduceShakeToggle", modalCard, "Reduce shake");
        SetAnchor((RectTransform)reduceShakeToggle.transform, new Vector2(0f, 1f), new Vector2(0.5f, 1f), new Vector2(110f, -388f), new Vector2(-18f, -348f));
        reduceShakeToggle.onValueChanged.AddListener(value => GameSettings.ReduceShake = value);

        vibrationToggle = CreateToggle("VibrationToggle", modalCard, "Vibration");
        SetAnchor((RectTransform)vibrationToggle.transform, new Vector2(0.5f, 1f), new Vector2(1f, 1f), new Vector2(18f, -388f), new Vector2(-110f, -348f));
        vibrationToggle.onValueChanged.AddListener(value => GameSettings.VibrationEnabled = value);

        resumeButton = CreateButton("ResumeButton", modalCard, "RESUME", 22, accentColor, new Color(0.02f, 0.06f, 0.08f, 1f));
        resumeButton.onClick.AddListener(TogglePause);
        AddFrame((RectTransform)resumeButton.transform, new Color(0.08f, 0.55f, 0.65f, 0.9f));

        restartButton = CreateButton("RestartButton", modalCard, "RESTART", 22, panelSoftColor, Color.white);
        restartButton.onClick.AddListener(RestartLevel);
        AddFrame((RectTransform)restartButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));

        modalLevelSelectButton = CreateButton("ModalLevelSelectButton", modalCard, "MISSION", 22, panelSoftColor, Color.white);
        modalLevelSelectButton.onClick.AddListener(OpenLevelSelect);
        AddFrame((RectTransform)modalLevelSelectButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));

        mainMenuButton = CreateButton("MainMenuButton", modalCard, "MAIN MENU", 22, panelSoftColor, Color.white);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        AddFrame((RectTransform)mainMenuButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));

        nextLevelButton = CreateButton("NextLevelButton", modalCard, "NEXT", 22, accentColor, new Color(0.02f, 0.06f, 0.08f, 1f));
        nextLevelButton.onClick.AddListener(GoToNextLevel);
        nextLevelButtonText = nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();
        AddFrame((RectTransform)nextLevelButton.transform, new Color(0.08f, 0.55f, 0.65f, 0.9f));

        modalBuilt = true;
        modalOverlay.SetActive(false);
    }

    void BuildTerminalScene(RectTransform parent)
    {
        terminalSceneTint = CreateImage("TerminalTint", parent, new Color(0.05f, 0.42f, 0.28f, 0.22f));
        terminalSceneTint.raycastTarget = false;
        SetAnchor(terminalSceneTint.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform horizon = CreatePanel("Horizon", parent, new Color(0.02f, 0.05f, 0.06f, 0.72f));
        horizon.GetComponent<Image>().raycastTarget = false;
        SetAnchor(horizon, new Vector2(0f, 0.18f), new Vector2(1f, 0.46f), Vector2.zero, Vector2.zero);

        for (int i = 0; i < 8; i++)
        {
            float x = 0.08f + i * 0.12f;
            Image pillar = CreateImage($"TerminalPillar_{i}", horizon, new Color(0.08f, 0.22f, 0.25f, 0.34f));
            pillar.raycastTarget = false;
            SetAnchor(pillar.rectTransform, new Vector2(x, 0f), new Vector2(x + 0.025f, 1f), Vector2.zero, Vector2.zero);
        }

        RectTransform map = CreatePanel("TerminalMap", parent, new Color(0.015f, 0.026f, 0.034f, 0.64f));
        map.GetComponent<Image>().raycastTarget = false;
        AddFrame(map, new Color(0.08f, 0.32f, 0.38f, 0.46f));
        SetAnchor(map, new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.84f), Vector2.zero, Vector2.zero);

        for (int i = 0; i < 6; i++)
        {
            float y = 0.12f + i * 0.15f;
            Image route = CreateImage($"TerminalRoute_{i}", map, new Color(0.14f, 0.7f, 0.82f, 0.16f));
            route.raycastTarget = false;
            SetAnchor(route.rectTransform, new Vector2(0.06f, y), new Vector2(0.94f, y), new Vector2(0f, -2f), new Vector2(0f, 2f));
        }

        terminalSceneText = CreateText("TerminalSceneText", parent, "", 28, FontStyle.Bold, TextAnchor.MiddleCenter);
        terminalSceneText.color = new Color(0.8f, 0.97f, 1f, 0.62f);
        terminalSceneText.characterSpacing = 5f;
        SetAnchor(terminalSceneText.rectTransform, new Vector2(0f, 0.84f), new Vector2(1f, 0.94f), new Vector2(24f, 0f), new Vector2(-24f, 0f));

        for (int i = 0; i < 10; i++)
        {
            float y = 0.04f + i * 0.095f;
            Image scan = CreateImage($"TerminalScanline_{i}", parent, new Color(1f, 1f, 1f, 0.025f));
            scan.raycastTarget = false;
            SetAnchor(scan.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }
    }

    void RebuildDynamicUiIfNeeded()
    {
        SeedBar seedBar = SeedBar.Instance;
        int seedCount = seedBar != null ? seedBar.SeedCount : 0;
        if (seedCount != lastSeedCount)
            RebuildSeedCards(seedCount);

        OverchargeSystem overcharge = OverchargeSystem.Instance;
        int rowCount = overcharge != null ? overcharge.RowCount : 0;
        if (rowCount != lastRowCount)
            RebuildRowButtons(rowCount);

        LevelManager levelManager = LevelManager.Instance;
        int levelCount = levelManager != null ? levelManager.LevelCount : 0;
        if (levelCount != lastLevelCount)
            RebuildLevelButtons(levelCount);
    }

    void RebuildSeedCards(int seedCount)
    {
        foreach (SeedCard card in seedCards)
            if (card.button != null) Destroy(card.button.gameObject);
        seedCards.Clear();
        lastSeedCount = seedCount;
        ResizeSeedTray(seedCount);

        for (int i = 0; i < seedCount; i++)
        {
            int index = i;
            GameObject go = new GameObject($"SeedCard_{i}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(seedTray, false);

            Image frame = go.GetComponent<Image>();
            frame.color = panelSoftColor;
            AddFrame((RectTransform)go.transform, new Color(0.1f, 0.2f, 0.27f, 0.8f));
            AddCardAccent((RectTransform)go.transform);

            LayoutElement layout = go.GetComponent<LayoutElement>();
            layout.preferredWidth = SeedCardWidth;
            layout.preferredHeight = SeedCardHeight;
            layout.minHeight = SeedCardHeight;

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = BuildButtonColors(panelSoftColor, accentColor);
            button.onClick.AddListener(() =>
            {
                if (SeedBar.Instance != null)
                    SeedBar.Instance.SelectSeed(index);
            });

            Image cooldown = CreateImage("CooldownFill", go.transform, new Color(0f, 0f, 0f, 0.52f));
            cooldown.type = Image.Type.Filled;
            cooldown.fillMethod = Image.FillMethod.Vertical;
            cooldown.fillOrigin = (int)Image.OriginVertical.Bottom;
            SetAnchor(cooldown.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            TextMeshProUGUI label = CreateText("Label", go.transform, "", 21, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetAnchor(label.rectTransform, new Vector2(0f, 0.35f), new Vector2(1f, 1f), new Vector2(8f, -4f), new Vector2(-8f, -2f));

            TextMeshProUGUI cost = CreateText("Cost", go.transform, "", 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            cost.color = new Color(0.88f, 0.96f, 1f, 1f);
            SetAnchor(cost.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.42f), new Vector2(8f, 2f), new Vector2(-8f, -2f));

            seedCards.Add(new SeedCard
            {
                button = button,
                frame = frame,
                cooldownFill = cooldown,
                label = label,
                cost = cost
            });
        }
    }

    void ResizeSeedTray(int seedCount)
    {
        if (seedTray == null) return;

        float contentWidth = seedCount * SeedCardWidth +
                             Mathf.Max(0, seedCount - 1) * SeedTraySpacing +
                             SeedTrayHorizontalPadding;
        float width = Mathf.Clamp(contentWidth, SeedTrayMinWidth, SeedTrayMaxWidth);
        SetAnchor(seedTray, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-width * 0.5f, 12f), new Vector2(width * 0.5f, 122f));
    }

    void RebuildRowButtons(int rowCount)
    {
        foreach (RowButton rowButton in rowButtons)
            if (rowButton.button != null) Destroy(rowButton.button.gameObject);
        rowButtons.Clear();
        lastRowCount = rowCount;

        float top = -58f;
        for (int row = rowCount - 1; row >= 0; row--)
        {
            int rowIndex = row;
            int visualIndex = rowCount - 1 - row;
            GameObject go = new GameObject($"OverchargeRow_{row}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(overchargePanel, false);

            RectTransform rect = (RectTransform)go.transform;
            SetAnchor(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(12f, top - 50f - visualIndex * 58f), new Vector2(-12f, top - 4f - visualIndex * 58f));

            Image frame = go.GetComponent<Image>();
            frame.color = panelSoftColor;
            AddFrame(rect, new Color(0.1f, 0.2f, 0.27f, 0.8f));
            AddCardAccent(rect);

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = BuildButtonColors(panelSoftColor, accentColor);
            button.onClick.AddListener(() =>
            {
                if (OverchargeSystem.Instance != null)
                    OverchargeSystem.Instance.TryActivate(rowIndex);
            });

            Image threatFill = CreateImage("ThreatFill", go.transform, new Color(1f, 0.42f, 0.14f, 0.28f));
            threatFill.type = Image.Type.Filled;
            threatFill.fillMethod = Image.FillMethod.Horizontal;
            threatFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            SetAnchor(threatFill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image fill = CreateImage("ActiveFill", go.transform, new Color(0.1f, 0.85f, 1f, 0.35f));
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            SetAnchor(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            TextMeshProUGUI label = CreateText("Label", go.transform, "", 19, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 0f), new Vector2(-6f, 0f));

            rowButtons.Add(new RowButton { button = button, frame = frame, threatFill = threatFill, fill = fill, label = label });
        }
    }

    void RefreshHud()
    {
        if (energyText != null)
            energyText.text = EnergySystem.Instance != null ? $"Energy: {EnergySystem.Instance.Energy}" : "Energy: --";

        var level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        if (levelText != null)
            levelText.text = level != null ? level.displayName : "Level";

        if (waveText != null)
        {
            EnemySpawner spawner = EnemySpawner.Instance;
            waveText.text = spawner != null ? spawner.DisplayText : "";
            waveText.color = spawner != null && spawner.IsWaveWarning ? warningColor : Color.white;
        }

        RefreshWaveIntel();

        if (pauseButtonText != null)
            pauseButtonText.text = isPaused ? ">" : "II";
        if (pauseButton != null)
            pauseButton.interactable = GameManager.Instance == null || (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsWon);

        if (overchargePanel != null)
            overchargePanel.gameObject.SetActive(OverchargeSystem.Instance != null && OverchargeSystem.Instance.IsUnlocked);

        RefreshMainMenu();
        RefreshSeedCards();
        RefreshRowButtons();
        RefreshLevelButtons();
        RefreshTutorial();
        RefreshCommandStatus();
    }

    void RefreshWaveIntel()
    {
        if (waveIntelPanel == null || waveIntelText == null) return;

        EnemySpawner spawner = EnemySpawner.Instance;
        string intel = spawner != null ? spawner.WaveIntelText : "";
        bool show = !string.IsNullOrWhiteSpace(intel) &&
                    !isPaused &&
                    (GameManager.Instance == null || (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsWon));

        waveIntelPanel.gameObject.SetActive(show);
        if (show) waveIntelText.text = intel;
    }

    void RefreshMainMenu()
    {
        if (mainMenuSubtitleText == null) return;

        LevelDefinition level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        string levelName = level != null ? level.displayName : "Campaign";
        mainMenuSubtitleText.text = $"{levelName}  |  Highest cleared: {PlayerProgress.HighestCompletedLevel}";
    }

    void RefreshTutorial()
    {
        if (tutorialPanel == null || tutorialText == null) return;

        TutorialCoach coach = TutorialCoach.Instance;
        string hint = coach != null ? coach.CurrentHint : "";
        bool show = !string.IsNullOrWhiteSpace(hint) && !isPaused &&
                    (GameManager.Instance == null || (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsWon));

        tutorialPanel.gameObject.SetActive(show);
        if (show) tutorialText.text = hint;
    }

    void RefreshCommandStatus()
    {
        if (commandStatusPanel == null || commandStatusText == null) return;

        bool terminal = GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon);
        if (isPaused || terminal)
        {
            commandStatusPanel.gameObject.SetActive(false);
            return;
        }

        string message = PlacementController.Instance != null ? PlacementController.Instance.CurrentFeedback : "";
        if (string.IsNullOrWhiteSpace(message) && OverchargeSystem.Instance != null)
            message = OverchargeSystem.Instance.CurrentFeedback;
        bool transient = !string.IsNullOrWhiteSpace(message);
        if (!transient)
            message = BuildSelectedSeedStatus();

        bool show = !string.IsNullOrWhiteSpace(message);
        commandStatusPanel.gameObject.SetActive(show);
        if (!show) return;

        commandStatusText.text = message;
        commandStatusText.color = transient ? warningColor : new Color(0.86f, 0.96f, 1f, 1f);
    }

    string BuildSelectedSeedStatus()
    {
        SeedBar seedBar = SeedBar.Instance;
        if (seedBar == null || seedBar.Selected == null) return "";

        UnitType seed = seedBar.Selected;
        bool ready = seedBar.IsReady(seedBar.selectedIndex);
        bool afford = EnergySystem.Instance != null && EnergySystem.Instance.CanAfford(seed.cost);
        if (!ready)
            return $"{seed.label} selected | cooldown {seedBar.GetCooldownRemaining(seedBar.selectedIndex):0.0}s";
        if (!afford)
        {
            int currentEnergy = EnergySystem.Instance != null ? EnergySystem.Instance.Energy : 0;
            return $"{seed.label} selected | need {Mathf.Max(0, seed.cost - currentEnergy)} energy";
        }

        return $"{seed.label} selected | place on an open grid cell";
    }

    void RefreshSeedCards()
    {
        SeedBar seedBar = SeedBar.Instance;
        if (seedBar == null) return;

        for (int i = 0; i < seedCards.Count; i++)
        {
            SeedCard card = seedCards[i];
            UnitType seed = seedBar.GetSeed(i);
            if (seed == null) continue;

            bool selected = seedBar.selectedIndex == i;
            bool ready = seedBar.IsReady(i);
            bool afford = EnergySystem.Instance != null && EnergySystem.Instance.CanAfford(seed.cost);
            float cooldownNormalized = seedBar.GetCooldownNormalized(i);
            Color normalColor = selected ? accentColor : (ready && afford ? panelSoftColor : disabledColor);

            card.label.text = seed.label;
            card.cost.text = ready ? seed.cost.ToString() : $"{seed.cost}  {seedBar.GetCooldownRemaining(i):0.0}s";
            card.cooldownFill.fillAmount = cooldownNormalized;
            card.cooldownFill.gameObject.SetActive(cooldownNormalized > 0.001f);
            card.frame.color = normalColor;
            card.button.colors = BuildButtonColors(normalColor, accentColor);
            card.label.color = selected ? Color.black : Color.white;
            card.cost.color = selected ? Color.black : new Color(0.88f, 0.96f, 1f, 1f);
            card.button.interactable = selected || (ready && afford);
        }
    }

    void RefreshRowButtons()
    {
        OverchargeSystem overcharge = OverchargeSystem.Instance;
        if (overcharge == null) return;

        if (overchargeCostText != null)
            overchargeCostText.text = $"OC {overcharge.energyCost}";

        for (int i = 0; i < rowButtons.Count; i++)
        {
            int row = overcharge.RowCount - 1 - i;
            RowButton button = rowButtons[i];
            bool active = overcharge.IsActive(row);
            bool afford = EnergySystem.Instance != null && EnergySystem.Instance.CanAfford(overcharge.energyCost);
            float remaining = overcharge.GetRemaining(row);
            float fill = overcharge.duration > 0f ? remaining / overcharge.duration : 0f;
            int enemyCount = overcharge.EnemyCountInRow(row);
            float pressure = overcharge.LanePressure01(row);

            if (active)
                button.label.text = $"{row + 1}: {remaining:0.0}s";
            else if (enemyCount > 0)
                button.label.text = $"{row + 1}: {enemyCount} threat";
            else
                button.label.text = $"{row + 1}: OC";

            if (button.threatFill != null)
            {
                button.threatFill.fillAmount = pressure;
                button.threatFill.color = pressure >= 0.65f
                    ? new Color(1f, 0.24f, 0.12f, 0.38f)
                    : new Color(1f, 0.62f, 0.14f, 0.24f);
                button.threatFill.gameObject.SetActive(pressure > 0.01f && !active);
            }

            button.fill.fillAmount = Mathf.Clamp01(fill);
            button.fill.gameObject.SetActive(active);
            button.frame.color = active ? accentColor : (pressure >= 0.65f ? warningColor : (afford ? panelSoftColor : disabledColor));
            button.button.interactable = !active && afford;
        }
    }

    void RefreshModalState()
    {
        if (!modalBuilt) return;

        bool gameOver = GameManager.Instance != null && GameManager.Instance.IsGameOver;
        bool won = GameManager.Instance != null && GameManager.Instance.IsWon;
        bool missionMapOpen = levelSelectOverlay != null && levelSelectOverlay.activeSelf;
        bool show = !mainMenuOpen && !missionMapOpen && (isPaused || gameOver || won);
        modalOverlay.SetActive(show);
        if (!show) return;

        if (won)
            modalTitleText.text = "VICTORY";
        else if (gameOver)
            modalTitleText.text = "DEFEAT";
        else
            modalTitleText.text = "PAUSED";

        bool terminal = gameOver || won;
        if (terminalSceneBackdrop != null)
            terminalSceneBackdrop.gameObject.SetActive(terminal);
        if (terminalSceneText != null)
            terminalSceneText.text = won ? "SECTOR SECURED" : "CORELINE BREACHED";
        if (terminalSceneTint != null)
            terminalSceneTint.color = won ? new Color(0.05f, 0.42f, 0.28f, 0.22f) : new Color(0.62f, 0.04f, 0.02f, 0.24f);
        Image modalImage = modalCard != null ? modalCard.GetComponent<Image>() : null;
        if (modalImage != null)
            modalImage.color = won
                ? new Color(0.035f, 0.095f, 0.088f, 0.98f)
                : (gameOver ? new Color(0.105f, 0.048f, 0.048f, 0.98f) : new Color(0.06f, 0.072f, 0.095f, 0.98f));
        modalTitleText.color = won ? new Color(0.73f, 1f, 0.82f, 1f) : (gameOver ? new Color(1f, 0.72f, 0.62f, 1f) : Color.white);

        if (gameOver || won)
            SetAnchor(progressText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -220f), new Vector2(-52f, -126f));
        else
            SetAnchor(progressText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -166f), new Vector2(-52f, -126f));

        var level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        modalSubtitleText.text = level != null ? level.displayName : "";
        if (level != null)
        {
            bool completed = PlayerProgress.IsLevelCompleted(level);
            string progress = $"Cleared: {(completed ? "Yes" : "No")}   |   Highest: {PlayerProgress.HighestCompletedLevel}";
            if (won && !string.IsNullOrWhiteSpace(level.completionReward))
                progress += $"\n{level.completionReward}";
            progressText.text = progress;
        }
        else
        {
            progressText.text = "";
        }

        bool showSettings = !gameOver && !won;
        musicText.gameObject.SetActive(showSettings);
        musicSlider.gameObject.SetActive(showSettings);
        sfxText.gameObject.SetActive(showSettings);
        sfxSlider.gameObject.SetActive(showSettings);
        reduceShakeToggle.gameObject.SetActive(showSettings);
        vibrationToggle.gameObject.SetActive(showSettings);
        if (showSettings)
        {
            musicText.text = $"Music {Mathf.RoundToInt(GameSettings.MusicVolume * 100f)}%";
            musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
            sfxText.text = $"SFX {Mathf.RoundToInt(GameSettings.SfxVolume * 100f)}%";
            sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
            reduceShakeToggle.SetIsOnWithoutNotify(GameSettings.ReduceShake);
            vibrationToggle.SetIsOnWithoutNotify(GameSettings.VibrationEnabled);
            RefreshToggleVisual(reduceShakeToggle);
            RefreshToggleVisual(vibrationToggle);
        }

        resumeButton.gameObject.SetActive(!gameOver && !won);
        if (modalLevelSelectButton != null)
            modalLevelSelectButton.gameObject.SetActive(true);
        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(true);

        LevelDefinition nextLevel = LevelManager.Instance != null ? LevelManager.Instance.NextLevel : null;
        bool canPlayNext = won && nextLevel != null && LevelManager.Instance != null && LevelManager.Instance.IsLevelUnlocked(nextLevel);
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(canPlayNext);
            nextLevelButton.interactable = canPlayNext;
        }
        if (nextLevelButtonText != null && nextLevel != null)
            nextLevelButtonText.text = $"MISSION {nextLevel.levelNumber}";

        LayoutModalButtons(gameOver, won, canPlayNext);
    }

    void LayoutModalButtons(bool gameOver, bool won, bool canPlayNext)
    {
        float yMin = 34f;
        float yMax = 88f;

        if (!gameOver && !won)
        {
            SetAnchor((RectTransform)resumeButton.transform, new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(42f, yMin), new Vector2(-6f, yMax));
            SetAnchor((RectTransform)restartButton.transform, new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(6f, yMin), new Vector2(-6f, yMax));
            SetAnchor((RectTransform)modalLevelSelectButton.transform, new Vector2(0.5f, 0f), new Vector2(0.75f, 0f), new Vector2(6f, yMin), new Vector2(-6f, yMax));
            SetAnchor((RectTransform)mainMenuButton.transform, new Vector2(0.75f, 0f), new Vector2(1f, 0f), new Vector2(6f, yMin), new Vector2(-42f, yMax));
            return;
        }

        if (canPlayNext)
        {
            SetAnchor((RectTransform)restartButton.transform, new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(42f, yMin), new Vector2(-6f, yMax));
            SetAnchor((RectTransform)modalLevelSelectButton.transform, new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(6f, yMin), new Vector2(-6f, yMax));
            SetAnchor((RectTransform)mainMenuButton.transform, new Vector2(0.5f, 0f), new Vector2(0.75f, 0f), new Vector2(6f, yMin), new Vector2(-6f, yMax));
            SetAnchor((RectTransform)nextLevelButton.transform, new Vector2(0.75f, 0f), new Vector2(1f, 0f), new Vector2(6f, yMin), new Vector2(-42f, yMax));
            return;
        }

        SetAnchor((RectTransform)restartButton.transform, new Vector2(0f, 0f), new Vector2(0.333f, 0f), new Vector2(52f, yMin), new Vector2(-8f, yMax));
        SetAnchor((RectTransform)modalLevelSelectButton.transform, new Vector2(0.333f, 0f), new Vector2(0.666f, 0f), new Vector2(8f, yMin), new Vector2(-8f, yMax));
        SetAnchor((RectTransform)mainMenuButton.transform, new Vector2(0.666f, 0f), new Vector2(1f, 0f), new Vector2(8f, yMin), new Vector2(-52f, yMax));
    }

    void GoToNextLevel()
    {
        if (LevelManager.Instance == null) return;

        AudioManager.PlaySfx(SfxType.UiClick);
        LevelManager.Instance.SelectNextLevelAndReload();
    }

    void RestartLevel()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.Game);
    }

}
