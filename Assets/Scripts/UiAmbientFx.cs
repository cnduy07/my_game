using UnityEngine;
using UnityEngine.UI;

public class UiAmbientFx : MonoBehaviour
{
    class Streak
    {
        public RectTransform rect;
        public Image image;
        public float speed;
        public float phase;
    }

    readonly Streak[] streaks = new Streak[32];
    RectTransform root;
    Vector2 area = new Vector2(1920f, 1080f);
    Sprite projectileSprite;
    bool horizontalProjectiles;

    public static UiAmbientFx Create(RectTransform parent, int count = 28, Sprite projectileSprite = null, bool horizontalProjectiles = false)
    {
        if (parent == null) return null;

        Transform existing = parent.Find("AmbientPixelRain");
        if (existing != null && existing.TryGetComponent(out UiAmbientFx existingFx))
        {
            existingFx.Configure(projectileSprite, horizontalProjectiles);
            return existingFx;
        }

        RectTransform layer = new GameObject("AmbientPixelRain", typeof(RectTransform)).GetComponent<RectTransform>();
        layer.SetParent(parent, false);
        layer.anchorMin = Vector2.zero;
        layer.anchorMax = Vector2.one;
        layer.offsetMin = Vector2.zero;
        layer.offsetMax = Vector2.zero;
        layer.SetAsLastSibling();

        UiAmbientFx fx = layer.gameObject.AddComponent<UiAmbientFx>();
        fx.Configure(projectileSprite, horizontalProjectiles);
        fx.Build(Mathf.Clamp(count, 8, fx.streaks.Length));
        return fx;
    }

    void Configure(Sprite sprite, bool horizontal)
    {
        projectileSprite = sprite;
        horizontalProjectiles = horizontal && sprite != null;
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
            streakRect.sizeDelta = horizontalProjectiles
                ? new Vector2(Random.Range(34f, 62f), Random.Range(12f, 20f))
                : new Vector2(Random.Range(44f, 180f), Random.Range(2f, 5f));
            streakRect.localRotation = Quaternion.Euler(0f, 0f, horizontalProjectiles ? Random.Range(-4f, 3f) : -18f);

            Image image = streakRect.GetComponent<Image>();
            image.raycastTarget = false;
            if (horizontalProjectiles)
            {
                image.sprite = projectileSprite;
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
                image.color = i % 5 == 0
                    ? new Color(1f, 0.52f, 0.2f, 0.32f)
                    : new Color(0.32f, 0.38f, 0.42f, 0.3f);
            }
            else
            {
                image.color = i % 4 == 0
                    ? new Color(0.34f, 0.42f, 0.48f, 0.22f)
                    : new Color(0.08f, 0.16f, 0.18f, 0.18f);
            }

            streaks[i] = new Streak
            {
                rect = streakRect,
                image = image,
                speed = horizontalProjectiles ? Random.Range(360f, 760f) : Random.Range(90f, 210f),
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
            if (horizontalProjectiles)
            {
                pos.x += streak.speed * Time.unscaledDeltaTime;
                pos.y += Mathf.Sin(Time.unscaledTime * 3.2f + streak.phase) * 7f * Time.unscaledDeltaTime;
            }
            else
            {
                pos.x += streak.speed * 0.42f * Time.unscaledDeltaTime;
                pos.y -= streak.speed * Time.unscaledDeltaTime;
            }
            streak.rect.anchoredPosition = pos;

            if (pos.y < -area.y - 120f || pos.x > area.x + 240f)
                ResetStreak(streak, false);
        }
    }

    void ResetStreak(Streak streak, bool initial)
    {
        float x = horizontalProjectiles
            ? (initial ? Random.Range(-area.x, area.x + 80f) : Random.Range(-360f, -80f))
            : (initial ? Random.Range(-240f, area.x + 80f) : Random.Range(-260f, area.x * 0.55f));
        float y = initial ? Random.Range(-80f, area.y + 160f) : Random.Range(80f, area.y + 220f);
        streak.rect.anchoredPosition = new Vector2(x, -y);
    }
}
