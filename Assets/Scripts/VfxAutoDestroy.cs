using UnityEngine;

public class VfxAutoDestroy : MonoBehaviour
{
    public float lifetime = 1f;
    public bool animateSprite;
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = Vector3.one;
    public float rotationSpeed;
    public bool useUnscaledTime = true;

    SpriteRenderer spriteRenderer;
    Color startColor = Color.white;
    float age;

    void OnEnable()
    {
        age = 0f;
        if (!animateSprite)
        {
            Destroy(gameObject, Mathf.Max(0.05f, lifetime));
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            startColor = spriteRenderer.color;
        transform.localScale = startScale;
    }

    void Update()
    {
        if (!animateSprite) return;

        age += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float t = lifetime > 0f ? Mathf.Clamp01(age / lifetime) : 1f;

        transform.localScale = Vector3.LerpUnclamped(startScale, endScale, t);
        if (Mathf.Abs(rotationSpeed) > 0.01f)
            transform.Rotate(0f, 0f, rotationSpeed * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime), Space.Self);

        if (spriteRenderer != null)
        {
            Color color = startColor;
            color.a *= 1f - t;
            spriteRenderer.color = color;
        }

        if (t >= 1f)
            Destroy(gameObject);
    }
}
