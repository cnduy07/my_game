using TMPro;
using UnityEngine;

public class UiTitleFlicker : MonoBehaviour
{
    public float minBrightness = 0.78f;
    public float maxBrightness = 1.08f;
    public float flickerSpeed = 2.4f;
    public float attackFlicker = 0.16f;
    public float warningBlend = 0.62f;
    public Color redAlert = new Color(1f, 0.2f, 0.08f, 1f);
    public Color amberAlert = new Color(1f, 0.72f, 0.18f, 1f);

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

        Color lit = new Color(
            Mathf.Clamp01(baseColor.r * brightness),
            Mathf.Clamp01(baseColor.g * brightness),
            Mathf.Clamp01(baseColor.b * brightness),
            baseColor.a);

        float alarm = Mathf.SmoothStep(0.62f, 0.98f, sharp);
        float swap = Mathf.PerlinNoise(8.7f, Time.unscaledTime * 6.4f);
        Color alarmColor = Color.Lerp(amberAlert, redAlert, swap > 0.52f ? 1f : 0f);
        label.color = Color.Lerp(lit, alarmColor, alarm * warningBlend);
    }
}
