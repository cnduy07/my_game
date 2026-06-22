using System.Collections;
using UnityEngine;

// Runtime damage feedback for any object with Health.
public class DamageFeedback : MonoBehaviour
{
    public Color flashColor = Color.white;
    public float flashTime = 0.08f;
    public float shakeAmount = 0.018f;
    public float shakeTime = 0.14f;
    public float shakeCycles = 1.5f;

    private Health health;
    private SpriteRenderer[] renderers;
    private Color[] baseColors;
    private Vector3 shakeOrigin;
    private bool isEnemy;
    private bool isShaking;
    private Coroutine routine;

    void Awake()
    {
        health = GetComponent<Health>();
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        baseColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            baseColors[i] = renderers[i].color;
        isEnemy = GetComponent<EnemyMover>() != null;
    }

    void OnEnable()
    {
        if (health == null) health = GetComponent<Health>();
        if (health != null) health.Damaged += HandleDamaged;
    }

    void OnDisable()
    {
        if (health != null) health.Damaged -= HandleDamaged;
        Restore();
    }

    void HandleDamaged(Health source)
    {
        if (!isActiveAndEnabled) return;

        if (routine != null)
        {
            StopCoroutine(routine);
            Restore();
        }

        routine = StartCoroutine(FlashAndShake());
    }

    IEnumerator FlashAndShake()
    {
        float total = Mathf.Max(flashTime, shakeTime, 0.01f);
        float currentShakeAmount = isEnemy ? 0f : shakeAmount;
        if (GameSettings.ReduceShake) currentShakeAmount *= 0.4f;

        if (currentShakeAmount > 0f)
        {
            shakeOrigin = transform.localPosition;
            isShaking = true;
        }

        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null)
                renderers[i].color = flashColor;

        float timer = 0f;
        while (timer < total)
        {
            timer += Time.deltaTime;
            if (currentShakeAmount > 0f && timer <= shakeTime)
            {
                float t = Mathf.Clamp01(timer / shakeTime);
                float wave = Mathf.Sin(t * Mathf.PI * 2f * shakeCycles);
                float damp = 1f - t;
                transform.localPosition = shakeOrigin + Vector3.right * (wave * damp * currentShakeAmount);
            }

            if (timer >= flashTime)
                RestoreColors();

            yield return null;
        }

        Restore();
        routine = null;
    }

    void Restore()
    {
        RestoreColors();
        if (isShaking)
        {
            transform.localPosition = shakeOrigin;
            isShaking = false;
        }
    }

    void RestoreColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null && i < baseColors.Length)
                renderers[i].color = baseColors[i];
    }
}
