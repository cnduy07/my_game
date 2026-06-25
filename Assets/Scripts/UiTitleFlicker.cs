using TMPro;
using UnityEngine;

public class UiTitleFlicker : MonoBehaviour
{
    public float minBrightness = 0.78f;
    public float maxBrightness = 1.08f;
    public float flickerSpeed = 2.4f;
    public float attackFlicker = 0.16f;

    TextMeshProUGUI label;
    Color baseColor;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
        if (label != null)
            baseColor = label.color;
    }

    void Update()
    {
        if (label == null) return;

        float slow = Mathf.PerlinNoise(0.37f, Time.unscaledTime * flickerSpeed);
        float sharp = Mathf.PerlinNoise(4.1f, Time.unscaledTime * 16f);
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, slow);
        if (sharp > 0.82f)
            brightness -= attackFlicker;

        label.color = new Color(
            Mathf.Clamp01(baseColor.r * brightness),
            Mathf.Clamp01(baseColor.g * brightness),
            Mathf.Clamp01(baseColor.b * brightness),
            baseColor.a);
    }
}
