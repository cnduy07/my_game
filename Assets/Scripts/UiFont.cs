using TMPro;
using UnityEngine;

public static class UiFont
{
    const string DefaultFontResourcePath = "Fonts/ThaleahFat SDF";

    static TMP_FontAsset cachedDefaultFont;

    public static TMP_FontAsset DefaultFont
    {
        get
        {
            if (cachedDefaultFont == null)
                cachedDefaultFont = Resources.Load<TMP_FontAsset>(DefaultFontResourcePath);
            if (!IsUsable(cachedDefaultFont))
                cachedDefaultFont = null;
            return cachedDefaultFont;
        }
    }

    public static void Apply(TextMeshProUGUI label)
    {
        if (label == null)
            return;

        TMP_FontAsset font = DefaultFont;
        if (font != null)
            label.font = font;

        label.outlineWidth = 0f;
        if (label.overflowMode == TextOverflowModes.Ellipsis)
            label.overflowMode = TextOverflowModes.Truncate;
    }

    public static void ApplyToChildren(Transform root)
    {
        if (root == null)
            return;

        TextMeshProUGUI[] labels = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < labels.Length; i++)
            Apply(labels[i]);
    }

    static bool IsUsable(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null || fontAsset.material == null)
            return false;

        try
        {
            if (fontAsset.atlasTexture == null)
                return false;

            _ = fontAsset.atlasTexture.width;
            return true;
        }
        catch (MissingReferenceException)
        {
            return false;
        }
    }
}
