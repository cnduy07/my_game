using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    public Sprite menuHeroSprite;
    public Sprite boardBackgroundSprite;
    public Sprite buttonSprite;
    public Sprite panelSprite;
    public Sprite bulletRainSprite;

    const float ReferenceWidth = 1920f;
    const float ReferenceHeight = 1080f;

    readonly Color background = UiSpec.Background;
    readonly Color panel = UiSpec.Panel;
    readonly Color cyan = UiSpec.Border;
    readonly Color orange = UiSpec.Accent;
    readonly Color text = UiSpec.Text;
    readonly Color muted = UiSpec.TextMuted;

    void Start()
    {
        Time.timeScale = 1f;
        EnsureEventSystem();
        BuildUi();
    }

    void BuildUi()
    {
        GameObject rootObject = new GameObject("EndSceneCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        rootObject.transform.SetParent(transform, false);

        Canvas canvas = rootObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 120;

        CanvasScaler scaler = rootObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform fullScreenRoot = new GameObject("FullScreenRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        fullScreenRoot.SetParent(rootObject.transform, false);
        SetAnchor(fullScreenRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform safeAreaRoot = new GameObject("SafeAreaRoot", typeof(RectTransform), typeof(SafeAreaFitter)).GetComponent<RectTransform>();
        safeAreaRoot.SetParent(rootObject.transform, false);
        SetAnchor(safeAreaRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        BuildBackdrop(fullScreenRoot);
        UiAmbientFx.Create(fullScreenRoot, 32, bulletRainSprite, bulletRainSprite != null);
        BuildTitleBlock(safeAreaRoot);
        BuildCommandReport(safeAreaRoot);
    }

    void BuildBackdrop(RectTransform root)
    {
        Image baseLayer = CreateImage("Base", root, background);
        SetAnchor(baseLayer.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image hero = CreateImage("HeroBackdrop", root, Color.white);
        ApplySprite(hero, menuHeroSprite, new Color(0.58f, 0.64f, 0.76f, 0.74f), true);
        SetAnchor(hero.rectTransform, Vector2.zero, Vector2.one, new Vector2(-44f, -26f), new Vector2(44f, 26f));

        Image board = CreateImage("BoardEcho", root, Color.white);
        ApplySprite(board, boardBackgroundSprite, new Color(0.28f, 0.42f, 0.5f, 0.18f), true);
        SetAnchor(board.rectTransform, new Vector2(0.06f, 0.08f), new Vector2(0.62f, 0.42f), Vector2.zero, Vector2.zero);

        Image outcomeWash = CreateImage("OutcomeWash", root, EndRunSummary.Won
            ? new Color(0.02f, 0.16f, 0.16f, 0.28f)
            : new Color(0.24f, 0.035f, 0.02f, 0.34f));
        SetAnchor(outcomeWash.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image rightScrim = CreateImage("RightCommandScrim", root, new Color(0.01f, 0.015f, 0.026f, 0.62f));
        SetAnchor(rightScrim.rectTransform, new Vector2(0.58f, 0f), Vector2.one, Vector2.zero, Vector2.zero);
    }

    void BuildTitleBlock(RectTransform root)
    {
        TextMeshProUGUI title = CreateText("GameTitle", root, "CORELINE DEFENSE", 72, FontStyle.Bold, TextAnchor.MiddleLeft);
        title.characterSpacing = 8f;
        title.color = new Color(0.92f, 0.97f, 1f, 1f);
        title.gameObject.AddComponent<UiTitleFlicker>();
        SetAnchor(title.rectTransform, new Vector2(0.075f, 0.58f), new Vector2(0.58f, 0.78f), Vector2.zero, Vector2.zero);

        string mission = EndRunSummary.MissionNumber > 0
            ? $"MISSION {EndRunSummary.MissionNumber:00} / {EndRunSummary.MissionName}"
            : EndRunSummary.MissionName;
        TextMeshProUGUI missionText = CreateText("MissionName", root, mission, 34, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionText.characterSpacing = 2f;
        missionText.color = new Color(0.76f, 0.96f, 1f, 1f);
        SetAnchor(missionText.rectTransform, new Vector2(0.08f, 0.49f), new Vector2(0.55f, 0.57f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI outcome = CreateText("Outcome", root,
            EndRunSummary.Won ? "VICTORY // MISSION COMPLETE" : "DEFEAT // CORELINE BREACHED",
            28, FontStyle.Bold, TextAnchor.MiddleLeft);
        outcome.characterSpacing = 3f;
        outcome.color = EndRunSummary.Won ? cyan : orange;
        SetAnchor(outcome.rectTransform, new Vector2(0.08f, 0.43f), new Vector2(0.55f, 0.49f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI intel = CreateText("Intel", root, EndRunSummary.Intel, 22, FontStyle.Bold, TextAnchor.UpperLeft);
        intel.color = new Color(0.75f, 0.88f, 0.95f, 0.94f);
        SetAnchor(intel.rectTransform, new Vector2(0.08f, 0.32f), new Vector2(0.52f, 0.42f), Vector2.zero, Vector2.zero);

        BuildButtons(root);
    }

    void BuildCommandReport(RectTransform root)
    {
        RectTransform command = CreatePanel("CommandReport", root, new Color(panel.r, panel.g, panel.b, 0.96f));
        SetAnchor(command, new Vector2(0.64f, 0.17f), new Vector2(0.94f, 0.84f), Vector2.zero, Vector2.zero);
        AddCorner(command, new Vector2(0f, 1f), new Vector2(22f, -16f), new Vector2(96f, -12f));
        AddCorner(command, new Vector2(0f, 1f), new Vector2(22f, -82f), new Vector2(26f, -16f));
        AddCorner(command, new Vector2(1f, 0f), new Vector2(-96f, 12f), new Vector2(-22f, 16f));
        AddCorner(command, new Vector2(1f, 0f), new Vector2(-26f, 16f), new Vector2(-22f, 82f));

        TextMeshProUGUI header = CreateText("Header", command, EndRunSummary.Won ? "VICTORY REPORT" : "DEFEAT REPORT", 28, FontStyle.Bold, TextAnchor.MiddleLeft);
        header.characterSpacing = 3f;
        header.color = EndRunSummary.Won ? cyan : orange;
        SetAnchor(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(48f, -80f), new Vector2(-40f, -24f));

        AddStat(command, 0, "WAVE REACHED", $"{EndRunSummary.WaveReached}/{EndRunSummary.WaveTotal}", "WV");
        AddStat(command, 1, "ENEMIES KILLED", EndRunSummary.EnemiesKilled.ToString("N0"), "K");
        AddStat(command, 2, "ENERGY COLLECTED", EndRunSummary.EnergyCollected.ToString("N0"), "E");
        AddStat(command, 3, "PLAY TIME", FormatTime(EndRunSummary.PlayTimeSeconds), "T");
    }

    void AddStat(RectTransform parent, int index, string label, string value, string iconText)
    {
        float y = -132f - index * 80f;
        RectTransform row = CreatePanel($"Stat_{index}", parent, new Color(0.02f, 0.035f, 0.052f, 0.68f));
        SetAnchor(row, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(44f, y - 48f), new Vector2(-44f, y));

        RectTransform icon = CreatePanel("Icon", row, new Color(cyan.r, cyan.g, cyan.b, 0.2f));
        SetAnchor(icon, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, -18f), new Vector2(52f, 18f));
        TextMeshProUGUI iconLabel = CreateText("IconText", icon, iconText, 14, FontStyle.Bold, TextAnchor.MiddleCenter);
        iconLabel.color = cyan;
        iconLabel.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchor(iconLabel.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        TextMeshProUGUI name = CreateText("Label", row, label, 17, FontStyle.Bold, TextAnchor.MiddleLeft);
        name.color = new Color(0.68f, 0.82f, 0.9f, 1f);
        name.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchor(name.rectTransform, Vector2.zero, new Vector2(0.7f, 1f), new Vector2(68f, 0f), new Vector2(-8f, 0f));

        TextMeshProUGUI val = CreateText("Value", row, value, 19, FontStyle.Bold, TextAnchor.MiddleRight);
        val.color = text;
        val.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchor(val.rectTransform, new Vector2(0.62f, 0f), Vector2.one, new Vector2(0f, 0f), new Vector2(-18f, 0f));
    }

    void BuildButtons(RectTransform root)
    {
        float width = 214f;
        float height = 66f;
        float gap = 24f;
        float total = width * 3f + gap * 2f;
        float start = 0.08f * ReferenceWidth;
        float bottom = 0.2f * ReferenceHeight;

        Button restart = CreateButton("RestartButton", root, "RESTART", 20, text);
        restart.onClick.AddListener(() => Load(SceneNames.Game));
        SetFixedAnchor(restart.transform as RectTransform, start, bottom, width, height);

        Button mission = CreateButton("MissionButton", root, "MISSION", 20, text);
        mission.onClick.AddListener(() => Load(SceneNames.MissionMap));
        SetFixedAnchor(mission.transform as RectTransform, start + width + gap, bottom, width, height);

        Button menu = CreateButton("MainMenuButton", root, "MAIN MENU", 20, text);
        menu.onClick.AddListener(() => Load(SceneNames.MainMenu));
        SetFixedAnchor(menu.transform as RectTransform, start + total - width, bottom, width, height);
    }

    void SetFixedAnchor(RectTransform rect, float left, float bottom, float width, float height)
    {
        SetAnchor(rect, Vector2.zero, Vector2.zero, new Vector2(left, bottom), new Vector2(left + width, bottom + height));
    }

    void Load(string sceneName)
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystem.transform.SetParent(transform, false);
    }

    RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        Image image = CreateImage(name, parent, color);
        ApplySprite(image, panelSprite, color, false);
        return image.rectTransform;
    }

    Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    TextMeshProUGUI CreateText(string name, Transform parent, string value, int size, FontStyle style, TextAnchor alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        float readableSize = ReadableTextSize(size);
        label.text = value;
        label.fontSize = readableSize;
        label.enableAutoSizing = true;
        label.fontSizeMax = readableSize;
        label.fontSizeMin = Mathf.Max(12f, readableSize * 0.65f);
        label.fontStyle = style == FontStyle.Bold ? FontStyles.Bold : FontStyles.Normal;
        label.alignment = ToTmpAlignment(alignment);
        label.color = text;
        label.outlineWidth = 0f;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Truncate;
        UiFont.Apply(label);
        return label;
    }

    float ReadableTextSize(float size)
    {
        if (size >= 56f)
            return size;

        if (size >= 34f)
            return Mathf.Ceil(size * 1.22f);

        return Mathf.Ceil(Mathf.Max(22f, size * 1.55f));
    }

    Button CreateButton(string name, Transform parent, string labelText, int size, Color textColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        ApplySprite(image, buttonSprite, UiSpec.ButtonNormal, false);

        Button button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = UiSpec.ButtonNormal;
        colors.highlightedColor = UiSpec.ButtonHover;
        colors.pressedColor = UiSpec.ButtonPressed;
        colors.selectedColor = UiSpec.ButtonHover;
        colors.disabledColor = UiSpec.ButtonDisabled;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.05f;
        button.colors = colors;
        go.AddComponent<PixelButtonPressOffset>();
        go.AddComponent<UiInteractMotion>();

        TextMeshProUGUI label = CreateText("Text", go.transform, labelText, size, FontStyle.Bold, TextAnchor.MiddleCenter);
        float buttonTextSize = ButtonTextSize(size, labelText);
        label.fontSize = buttonTextSize;
        label.fontSizeMax = buttonTextSize;
        label.fontSizeMin = Mathf.Max(12f, buttonTextSize * 0.65f);
        label.color = textColor;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(14f, 0f), new Vector2(-14f, 0f));
        return button;
    }

    float ButtonTextSize(float size, string textValue)
    {
        float target = Mathf.Ceil(Mathf.Max(18f, size * 1.2f));
        int length = string.IsNullOrWhiteSpace(textValue) ? 0 : textValue.Trim().Length;
        if (length >= 14)
            target = Mathf.Min(target, 19f);
        else if (length >= 10)
            target = Mathf.Min(target, 21f);

        return target;
    }

    void ApplySprite(Image image, Sprite sprite, Color color, bool preserveAspect)
    {
        if (image == null) return;
        image.color = color;
        if (sprite == null) return;

        image.sprite = sprite;
        image.type = sprite.border.sqrMagnitude > 0.01f ? Image.Type.Sliced : Image.Type.Simple;
        image.preserveAspect = preserveAspect;
    }

    void AddCorner(RectTransform parent, Vector2 anchor, Vector2 min, Vector2 max)
    {
        Image corner = CreateImage("Corner", parent, new Color(cyan.r, cyan.g, cyan.b, 0.82f));
        SetAnchor(corner.rectTransform, anchor, anchor, min, max);
    }

    void SetAnchor(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    TextAlignmentOptions ToTmpAlignment(TextAnchor alignment)
    {
        switch (alignment)
        {
            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.Left;
            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.Right;
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
            default:
                return TextAlignmentOptions.Center;
        }
    }

    string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.RoundToInt(seconds));
        return $"{total / 60:00}:{total % 60:00}";
    }
}
