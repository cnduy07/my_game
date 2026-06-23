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

        var outline = rect.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = rect.gameObject.AddComponent<Outline>();

        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
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
        label.fontSizeMin = Mathf.Max(11f, size * 0.62f);
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
        AddFrame((RectTransform)go.transform, new Color(0.1f, 0.2f, 0.27f, 0.85f));
        AddButtonAccent((RectTransform)go.transform);

        Button button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        button.colors = BuildButtonColors(normal, accentColor);

        TextMeshProUGUI label = CreateText("Text", go.transform, text, size, FontStyle.Bold, TextAnchor.MiddleCenter);
        label.color = textColor;
        SetAnchor(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 0f), new Vector2(-6f, 0f));
        return button;
    }

    void AddButtonAccent(RectTransform parent)
    {
        Image top = CreateImage("TopAccent", parent, new Color(accentColor.r, accentColor.g, accentColor.b, 0.28f));
        top.raycastTarget = false;
        SetAnchor(top.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(8f, -5f), new Vector2(-8f, -2f));

        Image bottom = CreateImage("BottomShade", parent, new Color(0f, 0f, 0f, 0.2f));
        bottom.raycastTarget = false;
        SetAnchor(bottom.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(8f, 2f), new Vector2(-8f, 5f));
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

        RectTransform background = CreatePanel("Background", root.transform, new Color(0.23f, 0.27f, 0.32f, 1f));
        SetAnchor(background, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
        fillArea.SetParent(root.transform, false);
        SetAnchor(fillArea, Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f));

        Image fill = CreateImage("Fill", fillArea, accentColor);
        SetAnchor(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image handle = CreateImage("Handle", root.transform, accentColor);
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

        Image box = CreateImage("Track", root.transform, new Color(0.08f, 0.12f, 0.16f, 1f));
        SetAnchor(box.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, -12f), new Vector2(44f, 12f));
        AddFrame(box.rectTransform, new Color(accentColor.r, accentColor.g, accentColor.b, 0.4f));

        Image check = CreateImage("Knob", box.transform, accentColor);
        SetAnchor(check.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(3f, -9f), new Vector2(21f, 9f));

        TextMeshProUGUI label = CreateText("Label", root.transform, labelText, 20, FontStyle.Bold, TextAnchor.MiddleLeft);
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
