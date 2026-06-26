using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class GameUiController
{
    RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        ApplySprite(image, panelSprite, color, false);
        return (RectTransform)go.transform;
    }

    RectTransform CreateColorPanel(string name, Transform parent, Color color)
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

    void AddFrame(RectTransform rect, Color color)
    {
        AddFrame(rect, color, new Vector2(1.5f, -1.5f));
    }

    void AddFrame(RectTransform rect, Color color, Vector2 distance)
    {
        if (rect == null) return;
        if (rect.GetComponent<Button>() != null) return;

        var outline = rect.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = rect.gameObject.AddComponent<Outline>();

        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }

    void SetButtonStyle(Button button, Color normal, Color textColor, Color frameColor)
    {
        if (button == null) return;

        Image image = button.GetComponent<Image>();
        if (image != null)
            ApplySprite(image, buttonSprite, UiSpec.ButtonNormal, false);

        button.colors = BuildButtonColors(UiSpec.ButtonNormal, UiSpec.ButtonHover);

        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.color = textColor;
    }

    TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, FontStyle style, TextAnchor alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        float readableSize = IsInsideButton(parent) ? ButtonTextSize(size, text) : ReadableTextSize(size);
        label.text = text;
        label.fontSize = readableSize;
        label.enableAutoSizing = true;
        label.fontSizeMax = readableSize;
        label.fontSizeMin = Mathf.Max(12f, readableSize * 0.65f);
        label.fontStyle = ToTmpFontStyle(style);
        label.alignment = ToTmpAlignment(alignment);
        label.color = UiSpec.Text;
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

    float ButtonTextSize(float size, string text)
    {
        if (size >= 28f && string.IsNullOrEmpty(text))
            return size;

        float target = Mathf.Ceil(Mathf.Max(18f, size * 1.2f));
        int length = string.IsNullOrWhiteSpace(text) ? 0 : text.Trim().Length;
        if (length >= 14)
            target = Mathf.Min(target, 19f);
        else if (length >= 10)
            target = Mathf.Min(target, 21f);

        return target;
    }

    bool IsInsideButton(Transform parent)
    {
        Transform current = parent;
        while (current != null)
        {
            if (current.GetComponent<Button>() != null)
                return true;

            current = current.parent;
        }

        return false;
    }

    Button CreateButton(string name, Transform parent, string text, int size, Color normal, Color textColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        ApplySprite(image, buttonSprite, UiSpec.ButtonNormal, false);

        Button button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        button.colors = BuildButtonColors(UiSpec.ButtonNormal, UiSpec.ButtonHover);
        go.AddComponent<PixelButtonPressOffset>();
        go.AddComponent<UiInteractMotion>();

        TextMeshProUGUI label = CreateText("Text", go.transform, text, size, FontStyle.Bold, TextAnchor.MiddleCenter);
        float buttonTextSize = ButtonTextSize(size, text);
        label.fontSize = buttonTextSize;
        label.fontSizeMax = buttonTextSize;
        label.fontSizeMin = Mathf.Max(12f, buttonTextSize * 0.65f);
        label.color = textColor;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(14f, 0f), new Vector2(-14f, 0f));
        return button;
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

    void AddButtonAccent(RectTransform parent)
    {
        // `button_command` owns the visible chrome; avoid extra code-drawn notches/lines.
    }

    void AddCardAccent(RectTransform parent)
    {
        Image left = CreateImage("LeftAccent", parent, new Color(accentColor.r, accentColor.g, accentColor.b, 0.2f));
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

    Slider CreateSlider(string name, Transform parent)
    {
        return CreateSlider(name, parent, GameSettings.SfxVolume);
    }

    Slider CreateSlider(string name, Transform parent, float initialValue)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(Slider));
        root.transform.SetParent(parent, false);
        Slider slider = root.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = Mathf.Clamp01(initialValue);

        RectTransform background = CreateColorPanel("Background", root.transform, new Color(0.07f, 0.1f, 0.14f, 1f));
        SetAnchor(background, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
        fillArea.SetParent(root.transform, false);
        SetAnchor(fillArea, Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f));

        Image fill = CreateImage("Fill", fillArea, accentColor);
        SetAnchor(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image handle = CreateImage("Handle", root.transform, accentColor);
        SetAnchor(handle.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-8f, -15f), new Vector2(8f, 15f));

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

        Image box = CreateImage("Track", root.transform, new Color(0.08f, 0.12f, 0.16f, 1f));
        SetAnchor(box.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, -12f), new Vector2(44f, 12f));
        AddFrame(box.rectTransform, new Color(accentColor.r, accentColor.g, accentColor.b, 0.4f));

        Image check = CreateImage("Knob", box.transform, accentColor);
        SetAnchor(check.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(3f, -9f), new Vector2(21f, 9f));

        TextMeshProUGUI label = CreateText("Label", root.transform, labelText, 23, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(52f, 0f), Vector2.zero);

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
                ? new Color(accentColor.r * 0.3f, accentColor.g * 0.3f, accentColor.b * 0.3f, 0.9f)
                : new Color(0.08f, 0.12f, 0.16f, 1f);
        }

        if (knob != null)
        {
            knob.color = on ? accentColor : new Color(0.55f, 0.65f, 0.72f, 1f);
            SetAnchor(knob.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                on ? new Vector2(23f, -9f) : new Vector2(3f, -9f),
                on ? new Vector2(41f, 9f) : new Vector2(21f, 9f));
        }
    }

    ColorBlock BuildButtonColors(Color normal, Color highlighted)
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
