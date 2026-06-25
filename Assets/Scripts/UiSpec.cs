using UnityEngine;

public static class UiSpec
{
    public static readonly Color Background = new Color32(0x0D, 0x11, 0x17, 0xFF);
    public static readonly Color Panel = new Color32(0x11, 0x18, 0x27, 0xFF);
    public static readonly Color Border = new Color32(0x1E, 0x90, 0xA8, 0xFF);
    public static readonly Color Accent = new Color32(0xFF, 0x6B, 0x35, 0xFF);
    public static readonly Color Text = new Color32(0xE8, 0xEA, 0xF0, 0xFF);
    public static readonly Color TextMuted = new Color32(0x6B, 0x72, 0x80, 0xFF);

    public static readonly Color ButtonNormal = Color.white;
    public static readonly Color ButtonHover = new Color(0.85f, 0.95f, 1f, 1f);
    public static readonly Color ButtonPressed = new Color(0.6f, 0.75f, 0.9f, 1f);
    public static readonly Color ButtonDisabled = new Color(0.4f, 0.4f, 0.4f, 0.6f);

    public const float MenuButtonWidth = 220f;
    public const float MenuButtonHeight = 72f;
    public const float PopupButtonWidth = 172f;
    public const float PopupButtonHeight = 58f;
    public const float SecondaryButtonWidth = 188f;
    public const float SecondaryButtonHeight = 58f;
    public const float MenuButtonGap = 14f;
    public const float CommandPanelPadding = 24f;
}
