using UnityEngine;
using UnityEngine.UI;

public class UiAmbientFx : MonoBehaviour
{
    class Streak
    {
        public RectTransform rect;
        public float speed;
        public float phase;
    }

    readonly Streak[] streaks = new Streak[32];
    RectTransform root;
    Vector2 area = new Vector2(1920f, 1080f);

    public static UiAmbientFx Create(RectTransform parent, int count = 28)
    {
        if (parent == null) return null;

        RectTransform layer = new GameObject("AmbientPixelRain", typeof(RectTransform)).GetComponent<RectTransform>();
        layer.SetParent(parent, false);
        layer.anchorMin = Vector2.zero;
        layer.anchorMax = Vector2.one;
        layer.offsetMin = Vector2.zero;
        layer.offsetMax = Vector2.zero;
        layer.SetAsLastSibling();

        UiAmbientFx fx = layer.gameObject.AddComponent<UiAmbientFx>();
        fx.Build(Mathf.Clamp(count, 8, fx.streaks.Length));
        return fx;
    }

    void Build(int count)
    {
        root = transform as RectTransform;
        Rect rootRect = root != null ? root.rect : new Rect(0f, 0f, area.x, area.y);
        area = new Vector2(Mathf.Max(1920f, rootRect.width), Mathf.Max(1080f, rootRect.height));

        for (int i = 0; i < count; i++)
        {
            RectTransform streakRect = new GameObject($"DebrisStreak_{i}", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            streakRect.SetParent(transform, false);
            streakRect.anchorMin = new Vector2(0f, 1f);
            streakRect.anchorMax = new Vector2(0f, 1f);
            streakRect.pivot = new Vector2(0.5f, 0.5f);
            streakRect.sizeDelta = new Vector2(Random.Range(44f, 180f), Random.Range(2f, 5f));
            streakRect.localRotation = Quaternion.Euler(0f, 0f, -18f);

            Image image = streakRect.GetComponent<Image>();
            image.raycastTarget = false;
            image.color = i % 4 == 0
                ? new Color(0.34f, 0.42f, 0.48f, 0.22f)
                : new Color(0.08f, 0.16f, 0.18f, 0.18f);

            streaks[i] = new Streak
            {
                rect = streakRect,
                speed = Random.Range(90f, 210f),
                phase = Random.Range(0f, 100f)
            };
            ResetStreak(streaks[i], true);
        }
    }

    void Update()
    {
        for (int i = 0; i < streaks.Length; i++)
        {
            Streak streak = streaks[i];
            if (streak == null || streak.rect == null) continue;

            Vector2 pos = streak.rect.anchoredPosition;
            pos.x += streak.speed * 0.42f * Time.unscaledDeltaTime;
            pos.y -= streak.speed * Time.unscaledDeltaTime;
            streak.rect.anchoredPosition = pos;

            if (pos.y < -area.y - 120f || pos.x > area.x + 240f)
                ResetStreak(streak, false);
        }
    }

    void ResetStreak(Streak streak, bool initial)
    {
        float x = initial ? Random.Range(-240f, area.x + 80f) : Random.Range(-260f, area.x * 0.55f);
        float y = initial ? Random.Range(-80f, area.y + 160f) : Random.Range(80f, area.y + 220f);
        streak.rect.anchoredPosition = new Vector2(x, -y);
    }
}
