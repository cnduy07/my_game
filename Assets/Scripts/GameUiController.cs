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
    public Color backgroundColor = UiSpec.Background;
    public Color panelColor = UiSpec.Panel;
    public Color panelSoftColor = UiSpec.Panel;
    public Color accentColor = UiSpec.Border;
    public Color warningColor = UiSpec.Accent;
    public Color disabledColor = UiSpec.ButtonDisabled;

    [Header("Generated Art")]
    public Sprite menuHeroSprite;
    public Sprite boardBackgroundSprite;
    public Sprite buttonSprite;
    public Sprite panelSprite;
    public Sprite arcReactorIcon;
    public Sprite turretIcon;
    public Sprite bunkerIcon;
    public Sprite snowGunIcon;
    public Sprite droneEmpIcon;

    const float ReferenceWidth = 1920f;
    const float ReferenceHeight = 1080f;
    const float SeedCardWidth = 174f;
    const float SeedCardHeight = 78f;
    const float SeedTraySpacing = 7f;
    const float SeedTrayHorizontalPadding = 24f;
    const float SeedTrayMinWidth = 420f;
    const float SeedTrayMaxWidth = 980f;
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
    RectTransform modalStatsPanel;
    readonly TextMeshProUGUI[] modalStatTexts = new TextMeshProUGUI[5];
    readonly Image[] modalStatIcons = new Image[5];
    RectTransform terminalSceneBackdrop;
    TextMeshProUGUI terminalSceneText;
    Image terminalSceneTint;
    Image modalResultGlow;
    Image modalResultSweep;
    Image modalResultCore;
    Image terminalSweepLine;
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
        public Image selectedGlow;
        public Image statusStrip;
        public Image iconBay;
        public Image icon;
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
        seedTray = CreatePanel("SeedTray", parent, new Color(0.014f, 0.02f, 0.032f, 0.96f));
        AddFrame(seedTray, new Color(0.05f, 0.18f, 0.24f, 0.84f));
        AddCornerTicks(seedTray, new Color(accentColor.r, accentColor.g, accentColor.b, 0.26f));
        SetAnchor(seedTray, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-500f, 14f), new Vector2(500f, 112f));

        HorizontalLayoutGroup layout = seedTray.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 10, 10);
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

        modalCard = CreatePanel("ModalCard", modalOverlay.transform, new Color(0.01f, 0.018f, 0.026f, 0.97f));
        AddFrame(modalCard, UiSpec.Border, new Vector2(2f, -2f));
        AddCornerTicks(modalCard, new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, 0.42f));
        SetAnchor(modalCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-430f, -286f), new Vector2(430f, 286f));

        modalResultGlow = CreateImage("ModalResultGlow", modalCard, new Color(accentColor.r, accentColor.g, accentColor.b, 0.08f));
        modalResultGlow.raycastTarget = false;
        SetAnchor(modalResultGlow.rectTransform, new Vector2(0.08f, 0.56f), new Vector2(0.92f, 0.91f), Vector2.zero, Vector2.zero);
        modalResultGlow.gameObject.SetActive(false);

        modalResultSweep = CreateImage("ModalResultSweep", modalCard, new Color(accentColor.r, accentColor.g, accentColor.b, 0.32f));
        modalResultSweep.raycastTarget = false;
        SetAnchor(modalResultSweep.rectTransform, new Vector2(0.12f, 0.69f), new Vector2(0.88f, 0.69f), new Vector2(0f, -3f), new Vector2(0f, 3f));
        modalResultSweep.gameObject.SetActive(false);

        modalResultCore = CreateImage("ModalResultCore", modalCard, new Color(0f, 0f, 0f, 0.24f));
        modalResultCore.raycastTarget = false;
        SetAnchor(modalResultCore.rectTransform, new Vector2(0.16f, 0.29f), new Vector2(0.84f, 0.62f), Vector2.zero, Vector2.zero);
        AddFrame(modalResultCore.rectTransform, new Color(0.1f, 0.32f, 0.38f, 0.34f));
        modalResultCore.gameObject.SetActive(false);

        modalTitleText = CreateText("ModalTitle", modalCard, "PAUSED", 32, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(modalTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -76f), new Vector2(-28f, -22f));

        modalSubtitleText = CreateText("ModalSubtitle", modalCard, "", 16, FontStyle.Bold, TextAnchor.MiddleCenter);
        modalSubtitleText.color = UiSpec.Text;
        SetAnchor(modalSubtitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -116f), new Vector2(-28f, -78f));

        progressText = CreateText("ProgressText", modalCard, "", 14, FontStyle.Normal, TextAnchor.MiddleCenter);
        progressText.color = UiSpec.TextMuted;
        SetAnchor(progressText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -166f), new Vector2(-52f, -126f));

        BuildModalStatsPanel();

        terminalSceneBackdrop = CreatePanel("TerminalSceneBackdrop", modalOverlay.transform, new Color(0.02f, 0.034f, 0.045f, 0.88f));
        terminalSceneBackdrop.SetSiblingIndex(0);
        SetAnchor(terminalSceneBackdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        BuildTerminalScene(terminalSceneBackdrop);
        terminalSceneBackdrop.gameObject.SetActive(false);

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

        resumeButton = CreateButton("ResumeButton", modalCard, "RESUME", 14, UiSpec.ButtonNormal, UiSpec.Text);
        resumeButton.onClick.AddListener(TogglePause);
        AddFrame((RectTransform)resumeButton.transform, new Color(0.08f, 0.55f, 0.65f, 0.9f));

        restartButton = CreateButton("RestartButton", modalCard, "RESTART", 14, UiSpec.ButtonNormal, UiSpec.Text);
        restartButton.onClick.AddListener(RestartLevel);
        AddFrame((RectTransform)restartButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));

        modalLevelSelectButton = CreateButton("ModalLevelSelectButton", modalCard, "MISSION", 14, UiSpec.ButtonNormal, UiSpec.Text);
        modalLevelSelectButton.onClick.AddListener(OpenLevelSelect);
        AddFrame((RectTransform)modalLevelSelectButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));

        mainMenuButton = CreateButton("MainMenuButton", modalCard, "MAIN MENU", 14, UiSpec.ButtonNormal, UiSpec.Text);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        AddFrame((RectTransform)mainMenuButton.transform, new Color(0.12f, 0.24f, 0.31f, 0.9f));

        nextLevelButton = CreateButton("NextLevelButton", modalCard, "NEXT", 14, UiSpec.ButtonNormal, UiSpec.Text);
        nextLevelButton.onClick.AddListener(GoToNextLevel);
        nextLevelButtonText = nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();
        AddFrame((RectTransform)nextLevelButton.transform, new Color(0.08f, 0.55f, 0.65f, 0.9f));

        modalBuilt = true;
        modalOverlay.SetActive(false);
    }

    void BuildModalStatsPanel()
    {
        modalStatsPanel = new GameObject("MissionReportStats", typeof(RectTransform)).GetComponent<RectTransform>();
        modalStatsPanel.SetParent(modalCard, false);
        SetAnchor(modalStatsPanel, new Vector2(0.16f, 0.28f), new Vector2(0.84f, 0.64f), Vector2.zero, Vector2.zero);

        string[] glyphs = { "M", "W", "K", "E", "T" };
        for (int i = 0; i < modalStatTexts.Length; i++)
        {
            RectTransform row = new GameObject($"StatRow_{i}", typeof(RectTransform)).GetComponent<RectTransform>();
            row.SetParent(modalStatsPanel, false);
            float top = 1f - i * 0.2f;
            SetAnchor(row, new Vector2(0f, top - 0.16f), new Vector2(1f, top - 0.01f), Vector2.zero, Vector2.zero);

            Image rowPlate = CreateImage("Backplate", row, new Color(0.012f, 0.035f, 0.045f, 0.62f));
            rowPlate.raycastTarget = false;
            SetAnchor(rowPlate.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image icon = CreateImage("Icon", row, new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, 0.22f));
            modalStatIcons[i] = icon;
            SetAnchor(icon.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(10f, -15f), new Vector2(54f, 15f));
            AddFrame(icon.rectTransform, new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, 0.5f));
            AddStatIconDetails(icon.rectTransform, i);

            TextMeshProUGUI glyph = CreateText("Glyph", icon.transform, glyphs[i], 9, FontStyle.Bold, TextAnchor.MiddleCenter);
            glyph.color = new Color(UiSpec.Text.r, UiSpec.Text.g, UiSpec.Text.b, 0.86f);
            SetAnchor(glyph.rectTransform, new Vector2(0.32f, 0f), Vector2.one, Vector2.zero, Vector2.zero);

            TextMeshProUGUI value = CreateText("Value", row, "", 18, FontStyle.Bold, TextAnchor.MiddleLeft);
            value.color = UiSpec.Text;
            modalStatTexts[i] = value;
            SetAnchor(value.rectTransform, Vector2.zero, Vector2.one, new Vector2(72f, 0f), new Vector2(-16f, 0f));
        }

        modalStatsPanel.gameObject.SetActive(false);
    }

    void AddStatIconDetails(RectTransform icon, int index)
    {
        Color dim = new Color(0f, 0.015f, 0.02f, 0.58f);
        Color light = new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, 0.62f);

        Image leftBar = CreateImage("IconLeftBar", icon, light);
        leftBar.raycastTarget = false;
        SetAnchor(leftBar.rectTransform, new Vector2(0f, 0.18f), new Vector2(0f, 0.82f), new Vector2(4f, 0f), new Vector2(8f, 0f));

        Image core = CreateImage("IconCore", icon, dim);
        core.raycastTarget = false;
        SetAnchor(core.rectTransform, new Vector2(0.18f, 0.26f), new Vector2(0.5f, 0.74f), Vector2.zero, Vector2.zero);

        float y = index % 2 == 0 ? 0.64f : 0.36f;
        Image tick = CreateImage("IconTick", icon, light);
        tick.raycastTarget = false;
        SetAnchor(tick.rectTransform, new Vector2(0.18f, y), new Vector2(0.5f, y), new Vector2(0f, -1.5f), new Vector2(0f, 1.5f));
    }

    void BuildTerminalScene(RectTransform parent)
    {
        if (boardBackgroundSprite != null)
        {
            Image boardArt = CreateImage("GeneratedTerminalBoard", parent, new Color(1f, 1f, 1f, 0.28f));
            ApplySprite(boardArt, boardBackgroundSprite, new Color(1f, 1f, 1f, 0.28f), false);
            boardArt.raycastTarget = false;
            SetAnchor(boardArt.rectTransform, new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.88f), Vector2.zero, Vector2.zero);
        }

        terminalSceneTint = CreateImage("TerminalTint", parent, new Color(0.02f, 0.12f, 0.14f, 0.36f));
        terminalSceneTint.raycastTarget = false;
        SetAnchor(terminalSceneTint.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image vignetteTop = CreateImage("EndSceneTopVignette", parent, new Color(0f, 0f, 0f, 0.5f));
        vignetteTop.raycastTarget = false;
        SetAnchor(vignetteTop.rectTransform, new Vector2(0f, 0.74f), Vector2.one, Vector2.zero, Vector2.zero);

        Image vignetteBottom = CreateImage("EndSceneBottomVignette", parent, new Color(0f, 0f, 0f, 0.58f));
        vignetteBottom.raycastTarget = false;
        SetAnchor(vignetteBottom.rectTransform, Vector2.zero, new Vector2(1f, 0.26f), Vector2.zero, Vector2.zero);

        RectTransform horizon = CreatePanel("Horizon", parent, new Color(0.02f, 0.05f, 0.06f, 0.72f));
        horizon.GetComponent<Image>().raycastTarget = false;
        SetAnchor(horizon, new Vector2(0f, 0.1f), new Vector2(1f, 0.4f), Vector2.zero, Vector2.zero);

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
        SetAnchor(map, new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.86f), Vector2.zero, Vector2.zero);

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

        terminalSweepLine = CreateImage("TerminalSweepLine", parent, new Color(accentColor.r, accentColor.g, accentColor.b, 0.32f));
        terminalSweepLine.raycastTarget = false;
        SetAnchor(terminalSweepLine.rectTransform, new Vector2(0.08f, 0.74f), new Vector2(0.92f, 0.74f), new Vector2(0f, -3f), new Vector2(0f, 3f));

        for (int i = 0; i < 10; i++)
        {
            float y = 0.04f + i * 0.095f;
            Image scan = CreateImage($"TerminalScanline_{i}", parent, new Color(1f, 1f, 1f, 0.025f));
            scan.raycastTarget = false;
            SetAnchor(scan.rectTransform, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, -1f), new Vector2(0f, 1f));
        }

        for (int i = 0; i < 7; i++)
        {
            float x = 0.08f + i * 0.14f;
            Image beacon = CreateImage($"EndSceneBeacon_{i}", parent, i % 2 == 0
                ? new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, 0.2f)
                : new Color(1f, 0.42f, 0.12f, 0.16f));
            beacon.raycastTarget = false;
            SetAnchor(beacon.rectTransform, new Vector2(x, 0.08f), new Vector2(x, 0.08f), new Vector2(-5f, -5f), new Vector2(5f, 5f));
        }

        UiAmbientFx.Create(parent, 18);
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
            ApplySprite(frame, buttonSprite, UiSpec.ButtonNormal, false);

            LayoutElement layout = go.GetComponent<LayoutElement>();
            layout.preferredWidth = SeedCardWidth;
            layout.preferredHeight = SeedCardHeight;
            layout.minHeight = SeedCardHeight;

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = BuildButtonColors(UiSpec.ButtonNormal, UiSpec.ButtonHover);
            button.onClick.AddListener(() =>
            {
                if (SeedBar.Instance != null)
                    SeedBar.Instance.SelectSeed(index);
            });
            go.AddComponent<PixelButtonPressOffset>();

            Image selectedGlow = CreateImage("SelectedGlow", go.transform, new Color(accentColor.r, accentColor.g, accentColor.b, 0.16f));
            selectedGlow.raycastTarget = false;
            SetAnchor(selectedGlow.rectTransform, Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f));

            Image statusStrip = CreateImage("StatusStrip", go.transform, new Color(accentColor.r, accentColor.g, accentColor.b, 0.22f));
            statusStrip.raycastTarget = false;
            statusStrip.gameObject.SetActive(false);
            SetAnchor(statusStrip.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(8f, -7f), new Vector2(-8f, -4f));

            Image cooldown = CreateImage("CooldownFill", go.transform, new Color(0f, 0f, 0f, 0.52f));
            cooldown.type = Image.Type.Filled;
            cooldown.fillMethod = Image.FillMethod.Vertical;
            cooldown.fillOrigin = (int)Image.OriginVertical.Bottom;
            SetAnchor(cooldown.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image iconBay = CreateImage("IconBay", go.transform, new Color(0f, 0.01f, 0.018f, 0.48f));
            iconBay.raycastTarget = false;
            SetAnchor(iconBay.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(10f, 11f), new Vector2(64f, -11f));
            AddFrame(iconBay.rectTransform, new Color(0.05f, 0.22f, 0.28f, 0.48f), new Vector2(1f, -1f));

            Image icon = CreateImage("Icon", go.transform, Color.white);
            icon.raycastTarget = false;
            icon.preserveAspect = true;
            SetAnchor(icon.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(12f, 13f), new Vector2(62f, -13f));

            TextMeshProUGUI label = CreateText("Label", go.transform, "", 15, FontStyle.Bold, TextAnchor.MiddleLeft);
            label.characterSpacing = 1f;
            SetAnchor(label.rectTransform, new Vector2(0f, 0.42f), new Vector2(1f, 1f), new Vector2(70f, -5f), new Vector2(-8f, -4f));

            TextMeshProUGUI cost = CreateText("Cost", go.transform, "", 14, FontStyle.Bold, TextAnchor.MiddleLeft);
            cost.color = new Color(0.88f, 0.96f, 1f, 1f);
            cost.characterSpacing = 1f;
            SetAnchor(cost.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.42f), new Vector2(70f, 4f), new Vector2(-8f, -2f));

            seedCards.Add(new SeedCard
            {
                button = button,
                frame = frame,
                selectedGlow = selectedGlow,
                statusStrip = statusStrip,
                iconBay = iconBay,
                icon = icon,
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
        SetAnchor(seedTray, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-width * 0.5f, 14f), new Vector2(width * 0.5f, 112f));
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
            Color normalColor = selected
                ? UiSpec.ButtonHover
                : (ready && afford ? UiSpec.ButtonNormal : UiSpec.ButtonDisabled);

            card.label.text = DisplaySeedLabel(seed.label);
            card.cost.text = ready ? $"E {seed.cost}" : $"{seedBar.GetCooldownRemaining(i):0.0}s";
            Sprite icon = IconForSeed(seed.label);
            if (card.icon != null)
            {
                card.icon.sprite = icon;
                card.icon.gameObject.SetActive(icon != null);
                card.icon.color = selected || (ready && afford) ? Color.white : new Color(0.58f, 0.66f, 0.72f, 0.86f);
            }
            card.cooldownFill.fillAmount = cooldownNormalized;
            card.cooldownFill.gameObject.SetActive(cooldownNormalized > 0.001f);
            card.frame.color = normalColor;
            card.button.colors = BuildButtonColors(UiSpec.ButtonNormal, UiSpec.ButtonHover);
            if (card.selectedGlow != null)
            {
                card.selectedGlow.gameObject.SetActive(selected);
                card.selectedGlow.color = new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, selected ? 0.12f : 0f);
            }
            if (card.statusStrip != null)
                card.statusStrip.gameObject.SetActive(false);
            if (card.iconBay != null)
                card.iconBay.color = selected
                    ? new Color(0.01f, 0.06f, 0.075f, 0.82f)
                    : new Color(0f, 0.01f, 0.018f, 0.48f);
            card.label.color = selected || (ready && afford) ? UiSpec.Text : UiSpec.TextMuted;
            card.cost.color = selected && !afford
                ? UiSpec.Accent
                : (selected || (ready && afford) ? UiSpec.Text : UiSpec.TextMuted);
            card.button.interactable = selected || (ready && afford);
        }
    }

    string DisplaySeedLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) return "";

        if (label.Contains("Arc")) return "ARC";
        if (label.Contains("Turret")) return "TURRET";
        if (label.Contains("Bunker")) return "BUNKER";
        if (label.Contains("Snow")) return "SNOW";
        if (label.Contains("Drone")) return "EMP";
        return label.ToUpperInvariant();
    }

    Sprite IconForSeed(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) return null;

        if (label.Contains("Arc")) return arcReactorIcon;
        if (label.Contains("Turret")) return turretIcon != null ? turretIcon : buttonSprite;
        if (label.Contains("Bunker")) return bunkerIcon;
        if (label.Contains("Snow")) return snowGunIcon != null ? snowGunIcon : turretIcon;
        if (label.Contains("Drone")) return droneEmpIcon;
        return null;
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
        SetAnchor(modalCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            terminal ? new Vector2(-560f, -330f) : new Vector2(-430f, -286f),
            terminal ? new Vector2(560f, 330f) : new Vector2(430f, 286f));
        Image overlayImage = modalOverlay != null ? modalOverlay.GetComponent<Image>() : null;
        if (overlayImage != null)
            overlayImage.color = terminal ? new Color(0f, 0f, 0f, 0.78f) : new Color(0f, 0f, 0f, 0.42f);
        if (terminalSceneBackdrop != null)
            terminalSceneBackdrop.gameObject.SetActive(terminal);
        if (terminalSceneText != null)
            terminalSceneText.text = won ? "SECTOR SECURED" : "CORELINE BREACHED";
        if (terminalSceneTint != null)
            terminalSceneTint.color = terminal
                ? (won ? new Color(0.02f, 0.24f, 0.22f, 0.4f) : new Color(0.26f, 0.05f, 0.035f, 0.42f))
                : Color.clear;
        Color resultAccent = won ? new Color(0.32f, 1f, 0.68f, 1f) : (gameOver ? new Color(1f, 0.28f, 0.16f, 1f) : accentColor);
        if (modalResultGlow != null)
            modalResultGlow.gameObject.SetActive(terminal);
        if (modalResultSweep != null)
            modalResultSweep.gameObject.SetActive(terminal);
        if (modalResultCore != null)
            modalResultCore.gameObject.SetActive(terminal);
        if (terminalSweepLine != null)
            terminalSweepLine.gameObject.SetActive(terminal);
        Image modalImage = modalCard != null ? modalCard.GetComponent<Image>() : null;
        if (modalImage != null)
            modalImage.color = terminal ? new Color(0.006f, 0.014f, 0.022f, 0.96f) : UiSpec.Panel;
        modalTitleText.color = terminal ? resultAccent : UiSpec.Text;
        modalSubtitleText.color = UiSpec.Text;
        progressText.color = terminal ? UiSpec.TextMuted : UiSpec.Text;
        if (modalStatsPanel != null)
            modalStatsPanel.gameObject.SetActive(terminal);
        if (progressText != null)
            progressText.gameObject.SetActive(!terminal);
        modalTitleText.fontSize = terminal ? 68f : 32f;
        modalTitleText.fontSizeMax = modalTitleText.fontSize;
        modalTitleText.fontSizeMin = modalTitleText.fontSize;
        modalTitleText.characterSpacing = terminal ? 5f : 0f;
        modalSubtitleText.fontSize = terminal ? 24f : 16f;
        modalSubtitleText.fontSizeMax = modalSubtitleText.fontSize;
        modalSubtitleText.fontSizeMin = modalSubtitleText.fontSize;
        modalSubtitleText.characterSpacing = terminal ? 3f : 0f;
        progressText.fontSize = terminal ? 16f : 14f;
        progressText.fontSizeMax = progressText.fontSize;
        progressText.fontSizeMin = progressText.fontSize;
        if (!terminal)
        {
            modalTitleText.rectTransform.localScale = Vector3.one;
            modalSubtitleText.rectTransform.localScale = Vector3.one;
        }

        if (gameOver || won)
        {
            SetAnchor(modalTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -154f), new Vector2(-52f, -60f));
            SetAnchor(modalSubtitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -204f), new Vector2(-52f, -158f));
            if (modalStatsPanel != null)
                SetAnchor(modalStatsPanel, new Vector2(0.27f, 0.28f), new Vector2(0.73f, 0.56f), Vector2.zero, Vector2.zero);
            if (modalResultGlow != null)
                SetAnchor(modalResultGlow.rectTransform, new Vector2(0.12f, 0.6f), new Vector2(0.88f, 0.89f), Vector2.zero, Vector2.zero);
            if (modalResultSweep != null)
                SetAnchor(modalResultSweep.rectTransform, new Vector2(0.24f, 0.59f), new Vector2(0.76f, 0.59f), new Vector2(0f, -3f), new Vector2(0f, 3f));
            if (modalResultCore != null)
                SetAnchor(modalResultCore.rectTransform, new Vector2(0.22f, 0.24f), new Vector2(0.78f, 0.57f), Vector2.zero, Vector2.zero);
        }
        else
        {
            SetAnchor(modalTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -76f), new Vector2(-28f, -22f));
            SetAnchor(modalSubtitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -116f), new Vector2(-28f, -78f));
            SetAnchor(progressText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(52f, -166f), new Vector2(-52f, -126f));
        }

        var level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        modalSubtitleText.text = terminal ? (won ? "MISSION COMPLETE" : "CORELINE BREACHED") : (level != null ? level.displayName : "");
        if (terminal)
        {
            RefreshTerminalReport(level, won, gameOver);
            RefreshTerminalEffects(won, gameOver, resultAccent);
            progressText.text = "";
        }
        else if (level != null)
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
            nextLevelButton.gameObject.SetActive(false);
            nextLevelButton.interactable = false;
        }
        if (nextLevelButtonText != null && nextLevel != null)
            nextLevelButtonText.text = $"MISSION {nextLevel.levelNumber}";

        RefreshModalButtonStyle(gameOver, won, canPlayNext);
        LayoutModalButtons(gameOver, won, canPlayNext);
    }

    void RefreshTerminalReport(LevelDefinition level, bool won, bool gameOver)
    {
        EnemySpawner spawner = EnemySpawner.Instance;
        GameStatsTracker stats = GameStatsTracker.Instance;
        string missionName = level != null ? $"{level.levelNumber:00}  {level.displayName}" : "Unknown sector";
        string wave = spawner != null ? $"{Mathf.Max(0, spawner.CurrentWave)}/{Mathf.Max(1, spawner.WaveCount)}" : "--";
        int kills = stats != null ? stats.EnemiesKilled : 0;
        int energy = stats != null ? stats.EnergyCollected : 0;
        string playTime = stats != null ? FormatTime(stats.PlayTimeSeconds) : "--:--";

        SetModalStat(0, "Mission", missionName, UiSpec.Border);
        SetModalStat(1, "Wave reached", wave, won ? new Color(0.32f, 1f, 0.68f, 1f) : UiSpec.Accent);
        SetModalStat(2, "Enemies killed", kills.ToString("N0"), UiSpec.TextMuted);
        SetModalStat(3, "Energy collected", energy.ToString("N0"), UiSpec.Border);
        SetModalStat(4, "Play time", playTime, UiSpec.TextMuted);
    }

    void RefreshTerminalEffects(bool won, bool gameOver, Color resultAccent)
    {
        float slowPulse = 0.5f + Mathf.Sin(Time.unscaledTime * 2.8f) * 0.5f;
        float sharpPulse = Mathf.PerlinNoise(8.1f, Time.unscaledTime * 9f);
        float brightness = Mathf.Lerp(0.72f, 1.08f, slowPulse);
        if (sharpPulse > 0.78f)
            brightness += 0.18f;

        Color titleColor = new Color(
            Mathf.Clamp01(resultAccent.r * brightness),
            Mathf.Clamp01(resultAccent.g * brightness),
            Mathf.Clamp01(resultAccent.b * brightness),
            1f);

        if (modalTitleText != null)
        {
            modalTitleText.color = titleColor;
            float titleScale = Mathf.Lerp(1f, 1.035f, slowPulse);
            if (sharpPulse > 0.9f)
                titleScale += 0.025f;
            modalTitleText.rectTransform.localScale = new Vector3(titleScale, titleScale, 1f);
        }
        if (modalSubtitleText != null)
        {
            modalSubtitleText.color = new Color(0.82f, 0.96f, 1f, 0.92f);
            float subtitleScale = Mathf.Lerp(1f, 1.012f, slowPulse);
            modalSubtitleText.rectTransform.localScale = new Vector3(subtitleScale, subtitleScale, 1f);
        }
        if (terminalSceneText != null)
            terminalSceneText.color = new Color(resultAccent.r, resultAccent.g, resultAccent.b, Mathf.Lerp(0.34f, 0.68f, slowPulse));
        if (terminalSceneTint != null)
            terminalSceneTint.color = won
                ? new Color(0.02f, 0.24f, 0.22f, Mathf.Lerp(0.32f, 0.44f, slowPulse))
                : new Color(0.26f, 0.05f, 0.035f, Mathf.Lerp(0.34f, 0.48f, slowPulse));
        if (modalResultGlow != null)
            modalResultGlow.color = new Color(resultAccent.r, resultAccent.g, resultAccent.b, Mathf.Lerp(0.08f, 0.16f, slowPulse));
        if (modalResultSweep != null)
            modalResultSweep.color = new Color(resultAccent.r, resultAccent.g, resultAccent.b, Mathf.Lerp(0.2f, 0.42f, slowPulse));
        if (modalResultCore != null)
        {
            Image core = modalResultCore.GetComponent<Image>();
            if (core != null)
                core.color = new Color(0.006f, 0.026f, 0.032f, 0.78f);
        }
        if (terminalSweepLine != null)
        {
            float y = Mathf.Lerp(0.18f, 0.86f, Mathf.Repeat(Time.unscaledTime * 0.18f, 1f));
            SetAnchor(terminalSweepLine.rectTransform, new Vector2(0.06f, y), new Vector2(0.94f, y), new Vector2(0f, -2f), new Vector2(0f, 2f));
            terminalSweepLine.color = new Color(resultAccent.r, resultAccent.g, resultAccent.b, 0.18f);
        }
    }

    void SetModalStat(int index, string label, string value, Color iconColor)
    {
        if (index < 0 || index >= modalStatTexts.Length) return;

        if (modalStatTexts[index] != null)
            modalStatTexts[index].text = $"{label.ToUpperInvariant()}   {value}";
        if (modalStatIcons[index] != null)
            modalStatIcons[index].color = new Color(iconColor.r, iconColor.g, iconColor.b, 0.36f);
    }

    string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.RoundToInt(seconds));
        int minutes = total / 60;
        int secs = total % 60;
        return $"{minutes:00}:{secs:00}";
    }

    void RefreshModalButtonStyle(bool gameOver, bool won, bool canPlayNext)
    {
        if (!gameOver && !won)
        {
            SetButtonStyle(resumeButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
            SetButtonStyle(restartButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
            SetButtonStyle(modalLevelSelectButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
            SetButtonStyle(mainMenuButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
            return;
        }

        SetButtonStyle(restartButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
        SetButtonStyle(modalLevelSelectButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
        SetButtonStyle(mainMenuButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
        SetButtonStyle(nextLevelButton, UiSpec.ButtonNormal, UiSpec.Text, UiSpec.Border);
    }

    void LayoutModalButtons(bool gameOver, bool won, bool canPlayNext)
    {
        float width = UiSpec.PopupButtonWidth;
        float height = UiSpec.PopupButtonHeight;
        float yMin = 34f;
        float yMax = yMin + height;

        if (!gameOver && !won)
        {
            float gap = 18f;
            float total = width * 4f + gap * 3f;
            float start = -total * 0.5f;
            SetModalButton(resumeButton, start, yMin, width, height);
            SetModalButton(restartButton, start + (width + gap), yMin, width, height);
            SetModalButton(modalLevelSelectButton, start + (width + gap) * 2f, yMin, width, height);
            SetModalButton(mainMenuButton, start + (width + gap) * 3f, yMin, width, height);
            return;
        }

        float terminalGap = 24f;
        float terminalTotal = width * 3f + terminalGap * 2f;
        float terminalStart = -terminalTotal * 0.5f;
        SetModalButton(restartButton, terminalStart, 32f, width, height);
        SetModalButton(modalLevelSelectButton, terminalStart + width + terminalGap, 32f, width, height);
        SetModalButton(mainMenuButton, terminalStart + (width + terminalGap) * 2f, 32f, width, height);
    }

    void SetModalButton(Button button, float left, float bottom, float width, float height)
    {
        if (button == null) return;
        SetAnchor((RectTransform)button.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(left, bottom),
            new Vector2(left + width, bottom + height));
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
