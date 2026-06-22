using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUiController : MonoBehaviour
{
    public static GameUiController Instance { get; private set; }

    [Header("HUD")]
    public bool isPaused;
    public Color backgroundColor = new Color(0.035f, 0.05f, 0.075f, 0.94f);
    public Color panelColor = new Color(0.09f, 0.13f, 0.18f, 0.92f);
    public Color panelSoftColor = new Color(0.13f, 0.18f, 0.24f, 0.9f);
    public Color accentColor = new Color(0.13f, 0.82f, 0.95f, 1f);
    public Color warningColor = new Color(1f, 0.55f, 0.17f, 1f);
    public Color disabledColor = new Color(0.33f, 0.38f, 0.45f, 0.86f);

    const float ReferenceWidth = 1920f;
    const float ReferenceHeight = 1080f;

    Canvas canvas;

    TextMeshProUGUI energyText;
    TextMeshProUGUI levelText;
    TextMeshProUGUI waveText;
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

    GameObject modalOverlay;
    TextMeshProUGUI modalTitleText;
    TextMeshProUGUI modalSubtitleText;
    TextMeshProUGUI progressText;
    TextMeshProUGUI sfxText;
    Slider sfxSlider;
    Toggle reduceShakeToggle;
    Toggle vibrationToggle;
    Button resumeButton;
    Button modalLevelSelectButton;
    Button nextLevelButton;
    TextMeshProUGUI nextLevelButtonText;

    GameObject levelSelectOverlay;
    RectTransform levelListContainer;
    readonly List<LevelButton> levelButtons = new List<LevelButton>();

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
        public Image fill;
        public TextMeshProUGUI label;
    }

    class LevelButton
    {
        public Button button;
        public Image frame;
        public TextMeshProUGUI label;
        public TextMeshProUGUI status;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BuildHud();
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

        BuildTopBar(root.transform);
        BuildSeedTray(root.transform);
        BuildOverchargePanel(root.transform);
        BuildTutorialPanel(root.transform);
        BuildModal(root.transform);
        BuildLevelSelectOverlay(root.transform);
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
        SetAnchor(topBar, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -116f), new Vector2(0f, 0f));

        energyText = CreateText("EnergyText", topBar, "Energy: 0", 44, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(energyText.rectTransform, new Vector2(0f, 0f), new Vector2(0.32f, 1f), new Vector2(34f, 0f), new Vector2(-8f, 0f));

        levelText = CreateText("LevelText", topBar, "", 34, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(levelText.rectTransform, new Vector2(0.34f, 0f), new Vector2(0.66f, 1f), Vector2.zero, Vector2.zero);

        waveText = CreateText("WaveText", topBar, "", 28, FontStyle.Bold, TextAnchor.MiddleRight);
        SetAnchor(waveText.rectTransform, new Vector2(0.67f, 0f), new Vector2(0.84f, 1f), Vector2.zero, new Vector2(-18f, 0f));

        levelSelectTopButton = CreateButton("LevelSelectButton", topBar, "MISSIONS", 24, panelSoftColor, accentColor);
        levelSelectTopButton.onClick.AddListener(OpenLevelSelect);
        SetAnchor((RectTransform)levelSelectTopButton.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-336f, -36f), new Vector2(-176f, 36f));

        pauseButton = CreateButton("PauseButton", topBar, "II", 34, panelSoftColor, accentColor);
        pauseButton.onClick.AddListener(TogglePause);
        pauseButtonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
        SetAnchor((RectTransform)pauseButton.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-158f, -36f), new Vector2(-28f, 36f));
    }

    void BuildTutorialPanel(Transform parent)
    {
        tutorialPanel = CreatePanel("TutorialHint", parent, new Color(0.035f, 0.05f, 0.075f, 0.86f));
        SetAnchor(tutorialPanel, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(34f, 176f), new Vector2(650f, 256f));

        tutorialText = CreateText("Text", tutorialPanel, "", 22, FontStyle.Bold, TextAnchor.MiddleLeft);
        tutorialText.color = new Color(0.86f, 0.96f, 1f, 1f);
        SetAnchor(tutorialText.rectTransform, Vector2.zero, Vector2.one, new Vector2(18f, 8f), new Vector2(-18f, -8f));
        tutorialPanel.gameObject.SetActive(false);
    }

    void BuildSeedTray(Transform parent)
    {
        seedTray = CreatePanel("SeedTray", parent, new Color(0.035f, 0.048f, 0.07f, 0.97f));
        SetAnchor(seedTray, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-660f, 18f), new Vector2(660f, 166f));

        HorizontalLayoutGroup layout = seedTray.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 16, 16);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
    }

    void BuildOverchargePanel(Transform parent)
    {
        overchargePanel = CreatePanel("OverchargePanel", parent, new Color(0.035f, 0.048f, 0.07f, 0.95f));
        SetAnchor(overchargePanel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-224f, -230f), new Vector2(-24f, 230f));

        overchargeCostText = CreateText("OverchargeCost", overchargePanel, "OC", 26, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(overchargeCostText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(12f, -60f), new Vector2(-12f, -10f));
    }

    void BuildModal(Transform parent)
    {
        modalOverlay = new GameObject("ModalOverlay", typeof(RectTransform), typeof(Image));
        modalOverlay.transform.SetParent(parent, false);
        Image overlayImage = modalOverlay.GetComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.42f);
        SetAnchor((RectTransform)modalOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform card = CreatePanel("ModalCard", modalOverlay.transform, new Color(0.075f, 0.09f, 0.12f, 0.97f));
        SetAnchor(card, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-360f, -250f), new Vector2(360f, 250f));

        modalTitleText = CreateText("ModalTitle", card, "PAUSED", 42, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(modalTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -92f), new Vector2(-28f, -28f));

        modalSubtitleText = CreateText("ModalSubtitle", card, "", 24, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(modalSubtitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -140f), new Vector2(-28f, -96f));

        progressText = CreateText("ProgressText", card, "", 22, FontStyle.Normal, TextAnchor.MiddleCenter);
        SetAnchor(progressText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(28f, -188f), new Vector2(-28f, -144f));

        sfxText = CreateText("SfxText", card, "", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(sfxText.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(40f, 42f), new Vector2(-40f, 82f));

        sfxSlider = CreateSlider("SfxSlider", card);
        SetAnchor((RectTransform)sfxSlider.transform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(80f, 8f), new Vector2(-80f, 36f));
        sfxSlider.onValueChanged.AddListener(value => GameSettings.SfxVolume = value);

        reduceShakeToggle = CreateToggle("ReduceShakeToggle", card, "Reduce shake");
        SetAnchor((RectTransform)reduceShakeToggle.transform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(90f, -54f), new Vector2(-8f, -16f));
        reduceShakeToggle.onValueChanged.AddListener(value => GameSettings.ReduceShake = value);

        vibrationToggle = CreateToggle("VibrationToggle", card, "Vibration");
        SetAnchor((RectTransform)vibrationToggle.transform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(8f, -54f), new Vector2(-90f, -16f));
        vibrationToggle.onValueChanged.AddListener(value => GameSettings.VibrationEnabled = value);

        resumeButton = CreateButton("ResumeButton", card, "RESUME", 24, accentColor, Color.white);
        resumeButton.onClick.AddListener(TogglePause);
        SetAnchor((RectTransform)resumeButton.transform, new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(70f, 38f), new Vector2(-10f, 96f));

        Button restartButton = CreateButton("RestartButton", card, "RESTART", 24, panelSoftColor, Color.white);
        restartButton.onClick.AddListener(RestartLevel);
        SetAnchor((RectTransform)restartButton.transform, new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(10f, 38f), new Vector2(-70f, 96f));

        modalLevelSelectButton = CreateButton("ModalLevelSelectButton", card, "MISSIONS", 22, panelSoftColor, Color.white);
        modalLevelSelectButton.onClick.AddListener(OpenLevelSelect);
        SetAnchor((RectTransform)modalLevelSelectButton.transform, new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(70f, 110f), new Vector2(-10f, 162f));

        nextLevelButton = CreateButton("NextLevelButton", card, "NEXT", 22, accentColor, Color.white);
        nextLevelButton.onClick.AddListener(GoToNextLevel);
        nextLevelButtonText = nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();
        SetAnchor((RectTransform)nextLevelButton.transform, new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(10f, 110f), new Vector2(-70f, 162f));

        modalBuilt = true;
        modalOverlay.SetActive(false);
    }

    void BuildLevelSelectOverlay(Transform parent)
    {
        levelSelectOverlay = new GameObject("LevelSelectOverlay", typeof(RectTransform), typeof(Image));
        levelSelectOverlay.transform.SetParent(parent, false);
        Image overlayImage = levelSelectOverlay.GetComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.64f);
        SetAnchor((RectTransform)levelSelectOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform card = CreatePanel("LevelSelectCard", levelSelectOverlay.transform, new Color(0.06f, 0.075f, 0.1f, 0.98f));
        SetAnchor(card, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-430f, -310f), new Vector2(430f, 310f));

        TextMeshProUGUI title = CreateText("Title", card, "SELECT MISSION", 38, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(30f, -86f), new Vector2(-30f, -24f));

        TextMeshProUGUI subtitle = CreateText("Subtitle", card, "Complete the previous mission to unlock the next one.", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
        subtitle.color = new Color(0.8f, 0.9f, 0.96f, 1f);
        SetAnchor(subtitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(42f, -128f), new Vector2(-42f, -88f));

        levelListContainer = CreatePanel("LevelList", card, new Color(0.025f, 0.035f, 0.052f, 0.9f));
        SetAnchor(levelListContainer, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(56f, 104f), new Vector2(-56f, -148f));

        VerticalLayoutGroup layout = levelListContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(14, 14, 14, 14);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        Button closeButton = CreateButton("CloseButton", card, "CLOSE", 24, panelSoftColor, Color.white);
        closeButton.onClick.AddListener(CloseLevelSelect);
        SetAnchor((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-150f, 34f), new Vector2(150f, 88f));

        levelSelectOverlay.SetActive(false);
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

        for (int i = 0; i < seedCount; i++)
        {
            int index = i;
            GameObject go = new GameObject($"SeedCard_{i}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(seedTray, false);

            Image frame = go.GetComponent<Image>();
            frame.color = panelSoftColor;

            LayoutElement layout = go.GetComponent<LayoutElement>();
            layout.preferredWidth = 226f;
            layout.preferredHeight = 112f;
            layout.minHeight = 112f;

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

            TextMeshProUGUI label = CreateText("Label", go.transform, "", 26, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetAnchor(label.rectTransform, new Vector2(0f, 0.36f), new Vector2(1f, 1f), new Vector2(8f, -6f), new Vector2(-8f, -4f));

            TextMeshProUGUI cost = CreateText("Cost", go.transform, "", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
            cost.color = new Color(0.88f, 0.96f, 1f, 1f);
            SetAnchor(cost.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.42f), new Vector2(8f, 2f), new Vector2(-8f, 0f));

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

    void RebuildRowButtons(int rowCount)
    {
        foreach (RowButton rowButton in rowButtons)
            if (rowButton.button != null) Destroy(rowButton.button.gameObject);
        rowButtons.Clear();
        lastRowCount = rowCount;

        float top = -68f;
        for (int row = rowCount - 1; row >= 0; row--)
        {
            int rowIndex = row;
            int visualIndex = rowCount - 1 - row;
            GameObject go = new GameObject($"OverchargeRow_{row}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(overchargePanel, false);

            RectTransform rect = (RectTransform)go.transform;
            SetAnchor(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, top - 60f - visualIndex * 66f), new Vector2(-14f, top - 6f - visualIndex * 66f));

            Image frame = go.GetComponent<Image>();
            frame.color = panelSoftColor;

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = BuildButtonColors(panelSoftColor, accentColor);
            button.onClick.AddListener(() =>
            {
                if (OverchargeSystem.Instance != null)
                    OverchargeSystem.Instance.TryActivate(rowIndex);
            });

            Image fill = CreateImage("ActiveFill", go.transform, new Color(0.1f, 0.85f, 1f, 0.35f));
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            SetAnchor(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            TextMeshProUGUI label = CreateText("Label", go.transform, "", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 0f), new Vector2(-6f, 0f));

            rowButtons.Add(new RowButton { button = button, frame = frame, fill = fill, label = label });
        }
    }

    void RebuildLevelButtons(int levelCount)
    {
        foreach (LevelButton levelButton in levelButtons)
            if (levelButton.button != null) Destroy(levelButton.button.gameObject);
        levelButtons.Clear();
        lastLevelCount = levelCount;

        if (levelListContainer == null) return;

        LevelManager levelManager = LevelManager.Instance;
        for (int i = 0; i < levelCount; i++)
        {
            LevelDefinition level = levelManager != null ? levelManager.GetLevelAt(i) : null;
            if (level == null) continue;

            LevelDefinition capturedLevel = level;
            GameObject go = new GameObject($"LevelButton_{i}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(levelListContainer, false);

            LayoutElement layout = go.GetComponent<LayoutElement>();
            layout.preferredHeight = 82f;
            layout.minHeight = 82f;

            Image frame = go.GetComponent<Image>();
            frame.color = panelSoftColor;

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = BuildButtonColors(panelSoftColor, accentColor);
            button.onClick.AddListener(() => SelectLevel(capturedLevel));

            TextMeshProUGUI label = CreateText("Label", go.transform, "", 24, FontStyle.Bold, TextAnchor.MiddleLeft);
            SetAnchor(label.rectTransform, new Vector2(0f, 0f), new Vector2(0.68f, 1f), new Vector2(20f, 0f), new Vector2(-8f, 0f));

            TextMeshProUGUI status = CreateText("Status", go.transform, "", 20, FontStyle.Bold, TextAnchor.MiddleRight);
            SetAnchor(status.rectTransform, new Vector2(0.68f, 0f), new Vector2(1f, 1f), new Vector2(8f, 0f), new Vector2(-20f, 0f));

            levelButtons.Add(new LevelButton
            {
                button = button,
                frame = frame,
                label = label,
                status = status
            });
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
            waveText.text = EnemySpawner.Instance != null ? EnemySpawner.Instance.DisplayText : "";

        if (pauseButtonText != null)
            pauseButtonText.text = isPaused ? ">" : "II";
        if (pauseButton != null)
            pauseButton.interactable = GameManager.Instance == null || (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsWon);

        if (overchargePanel != null)
            overchargePanel.gameObject.SetActive(OverchargeSystem.Instance != null && OverchargeSystem.Instance.IsUnlocked);

        RefreshSeedCards();
        RefreshRowButtons();
        RefreshLevelButtons();
        RefreshTutorial();
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

            card.label.text = seed.label;
            card.cost.text = ready ? seed.cost.ToString() : $"{seed.cost}  {seedBar.GetCooldownRemaining(i):0.0}s";
            card.cooldownFill.fillAmount = cooldownNormalized;
            card.cooldownFill.gameObject.SetActive(cooldownNormalized > 0.001f);
            card.frame.color = selected ? accentColor : (ready && afford ? panelSoftColor : disabledColor);
            card.button.interactable = ready && afford;
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

            button.label.text = active ? $"{row + 1}: {remaining:0.0}s" : $"{row + 1}: OC";
            button.fill.fillAmount = Mathf.Clamp01(fill);
            button.fill.gameObject.SetActive(active);
            button.frame.color = active ? accentColor : (afford ? panelSoftColor : disabledColor);
            button.button.interactable = !active && afford;
        }
    }

    void RefreshLevelButtons()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null) return;

        for (int i = 0; i < levelButtons.Count; i++)
        {
            LevelDefinition level = levelManager.GetLevelAt(i);
            if (level == null) continue;

            LevelButton button = levelButtons[i];
            bool current = level == levelManager.currentLevel;
            bool completed = PlayerProgress.IsLevelCompleted(level);
            bool unlocked = PlayerProgress.IsLevelUnlocked(level);

            button.label.text = $"{level.levelNumber:00}  {level.displayName}";
            if (current)
                button.status.text = completed ? "ACTIVE / CLEARED" : "ACTIVE";
            else if (completed)
                button.status.text = "CLEARED";
            else
                button.status.text = unlocked ? "UNLOCKED" : "LOCKED";

            button.frame.color = current ? accentColor : (unlocked ? panelSoftColor : disabledColor);
            button.button.interactable = unlocked;
        }
    }

    void RefreshModalState()
    {
        if (!modalBuilt) return;

        bool gameOver = GameManager.Instance != null && GameManager.Instance.IsGameOver;
        bool won = GameManager.Instance != null && GameManager.Instance.IsWon;
        bool show = isPaused || gameOver || won;
        modalOverlay.SetActive(show);
        if (!show) return;

        if (won)
            modalTitleText.text = "VICTORY";
        else if (gameOver)
            modalTitleText.text = "DEFEAT";
        else
            modalTitleText.text = "PAUSED";

        var level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
        modalSubtitleText.text = level != null ? level.displayName : "";
        if (level != null)
        {
            bool completed = PlayerProgress.IsLevelCompleted(level);
            progressText.text = $"Cleared: {(completed ? "Yes" : "No")}   |   Highest cleared: {PlayerProgress.HighestCompletedLevel}";
        }
        else
        {
            progressText.text = "";
        }

        sfxText.text = $"SFX: {GameSettings.SfxVolume:0.00}";
        sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
        reduceShakeToggle.SetIsOnWithoutNotify(GameSettings.ReduceShake);
        vibrationToggle.SetIsOnWithoutNotify(GameSettings.VibrationEnabled);

        resumeButton.gameObject.SetActive(!gameOver && !won);
        if (modalLevelSelectButton != null)
            modalLevelSelectButton.gameObject.SetActive(true);

        LevelDefinition nextLevel = LevelManager.Instance != null ? LevelManager.Instance.NextLevel : null;
        bool canPlayNext = won && nextLevel != null && PlayerProgress.IsLevelUnlocked(nextLevel);
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(canPlayNext);
            nextLevelButton.interactable = canPlayNext;
        }
        if (nextLevelButtonText != null && nextLevel != null)
            nextLevelButtonText.text = $"MISSION {nextLevel.levelNumber}";
    }

    void OpenLevelSelect()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        RebuildDynamicUiIfNeeded();
        RefreshLevelButtons();

        wasPausedBeforeLevelSelect = isPaused;
        bool terminal = GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon);
        if (!terminal)
            SetPaused(true);

        if (levelSelectOverlay != null)
            levelSelectOverlay.SetActive(true);
    }

    void CloseLevelSelect()
    {
        AudioManager.PlaySfx(SfxType.UiClick);

        if (levelSelectOverlay != null)
            levelSelectOverlay.SetActive(false);

        bool terminal = GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon);
        if (!terminal)
            SetPaused(wasPausedBeforeLevelSelect);
    }

    void SelectLevel(LevelDefinition level)
    {
        if (LevelManager.Instance == null) return;

        AudioManager.PlaySfx(SfxType.UiClick);
        LevelManager.Instance.SelectLevelAndReload(level);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
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

    TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, FontStyle style, TextAnchor alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.fontStyle = ToTmpFontStyle(style);
        label.alignment = ToTmpAlignment(alignment);
        label.color = Color.white;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Ellipsis;
        return label;
    }

    Button CreateButton(string name, Transform parent, string text, int size, Color normal, Color textColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = normal;

        Button button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        button.colors = BuildButtonColors(normal, accentColor);

        TextMeshProUGUI label = CreateText("Text", go.transform, text, size, FontStyle.Bold, TextAnchor.MiddleCenter);
        label.color = textColor;
        SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 0f), new Vector2(-6f, 0f));
        return button;
    }

    Slider CreateSlider(string name, Transform parent)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(Slider));
        root.transform.SetParent(parent, false);
        Slider slider = root.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = GameSettings.SfxVolume;

        RectTransform background = CreatePanel("Background", root.transform, new Color(0.23f, 0.27f, 0.32f, 1f));
        SetAnchor(background, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
        fillArea.SetParent(root.transform, false);
        SetAnchor(fillArea, Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f));

        Image fill = CreateImage("Fill", fillArea, accentColor);
        SetAnchor(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image handle = CreateImage("Handle", root.transform, Color.white);
        SetAnchor(handle.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-12f, -16f), new Vector2(12f, 16f));

        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        return slider;
    }

    Toggle CreateToggle(string name, Transform parent, string labelText)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        root.transform.SetParent(parent, false);
        Toggle toggle = root.GetComponent<Toggle>();

        Image box = CreateImage("Box", root.transform, panelSoftColor);
        SetAnchor(box.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, -15f), new Vector2(30f, 15f));

        Image check = CreateImage("Checkmark", box.transform, accentColor);
        SetAnchor(check.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 6f), new Vector2(-6f, -6f));

        TextMeshProUGUI label = CreateText("Label", root.transform, labelText, 20, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(42f, 0f), Vector2.zero);

        toggle.targetGraphic = box;
        toggle.graphic = check;
        return toggle;
    }

    ColorBlock BuildButtonColors(Color normal, Color highlighted)
    {
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = normal;
        colors.highlightedColor = Color.Lerp(normal, highlighted, 0.35f);
        colors.pressedColor = highlighted;
        colors.selectedColor = Color.Lerp(normal, highlighted, 0.25f);
        colors.disabledColor = disabledColor;
        colors.colorMultiplier = 1f;
        return colors;
    }

    void SetAnchor(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    bool IsScreenPointIn(RectTransform rect, Vector2 screenPoint)
    {
        return rect != null && rect.gameObject.activeInHierarchy &&
               RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, null);
    }

    FontStyles ToTmpFontStyle(FontStyle style)
    {
        switch (style)
        {
            case FontStyle.Bold:
                return FontStyles.Bold;
            case FontStyle.Italic:
                return FontStyles.Italic;
            case FontStyle.BoldAndItalic:
                return FontStyles.Bold | FontStyles.Italic;
            default:
                return FontStyles.Normal;
        }
    }

    TextAlignmentOptions ToTmpAlignment(TextAnchor alignment)
    {
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.Left;
            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
            default:
                return TextAlignmentOptions.Center;
        }
    }
}
