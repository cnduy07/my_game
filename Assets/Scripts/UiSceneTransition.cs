using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UiSceneTransition
{
    public static void Play(MonoBehaviour owner, RectTransform parent, string label, Action onComplete)
    {
        if (owner == null || parent == null)
        {
            onComplete?.Invoke();
            return;
        }

        owner.StartCoroutine(PlayRoutine(parent, label, onComplete));
    }

    static IEnumerator PlayRoutine(RectTransform parent, string label, Action onComplete)
    {
        GameObject overlay = new GameObject("SceneTransition", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        overlay.transform.SetParent(parent, false);
        RectTransform overlayRect = (RectTransform)overlay.transform;
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        overlay.transform.SetAsLastSibling();

        Image shade = overlay.GetComponent<Image>();
        shade.color = new Color(0f, 0f, 0f, 0.82f);
        CanvasGroup group = overlay.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = true;

        TextMeshProUGUI text = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        text.transform.SetParent(overlay.transform, false);
        text.text = label;
        text.fontSize = 56f;
        text.fontSizeMin = 56f;
        text.fontSizeMax = 56f;
        text.enableAutoSizing = false;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = UiSpec.Text;
        text.outlineWidth = 0f;
        text.raycastTarget = false;
        UiFont.Apply(text);
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = new Vector2(0.18f, 0.46f);
        textRect.anchorMax = new Vector2(0.82f, 0.56f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        for (int i = 0; i < 9; i++)
        {
            Image bar = new GameObject($"Sweep_{i}", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            bar.transform.SetParent(overlay.transform, false);
            bar.raycastTarget = false;
            bar.color = i % 3 == 0
                ? new Color(UiSpec.Border.r, UiSpec.Border.g, UiSpec.Border.b, 0.32f)
                : new Color(0.16f, 0.18f, 0.2f, 0.5f);
            RectTransform rect = bar.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(2400f, 18f + i * 2f);
            rect.anchoredPosition = new Vector2(-1700f - i * 180f, -360f + i * 92f);
            rect.localRotation = Quaternion.Euler(0f, 0f, -18f);
        }

        float duration = 0.95f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * 1.4f));

            for (int i = 0; i < overlay.transform.childCount; i++)
            {
                RectTransform child = overlay.transform.GetChild(i) as RectTransform;
                if (child == null || child == textRect) continue;
                Vector2 pos = child.anchoredPosition;
                pos.x += Time.unscaledDeltaTime * (1900f + i * 18f);
                child.anchoredPosition = pos;
            }

            yield return null;
        }

        onComplete?.Invoke();
    }
}
