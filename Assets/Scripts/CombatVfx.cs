using UnityEngine;

// Lightweight code-generated combat VFX for the prototype.
// Later, these entry points can be replaced by polished prefab-based effects.
public static class CombatVfx
{
    private static Material lineMaterial;

    public static void PlayMuzzleFlash(Vector3 position)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.muzzleFlashPrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        CreateLine("MuzzleFlash", position, position + Vector3.right * 0.32f, 0.08f,
            new Color(1f, 0.95f, 0.35f, 1f), new Color(1f, 0.35f, 0.05f, 0.65f), 0.055f);
        CreateLine("MuzzleCore", position + Vector3.up * 0.035f, position + Vector3.right * 0.22f, 0.035f,
            Color.white, new Color(1f, 0.85f, 0.2f, 0.4f), 0.045f);
        CreateParticleBurst("MuzzleEmbers", position + Vector3.right * 0.12f, 5, 0.06f,
            new ParticleSystem.MinMaxCurve(0.08f, 0.14f),
            new ParticleSystem.MinMaxCurve(0.35f, 0.75f),
            new ParticleSystem.MinMaxCurve(0.025f, 0.055f),
            new ParticleSystem.MinMaxGradient(new Color(1f, 0.92f, 0.35f, 0.95f), new Color(1f, 0.35f, 0.05f, 0.45f)),
            0.04f, 0f, 0.28f);
    }

    public static void PlayHitSpark(Vector3 position)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.hitSparkPrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        for (int i = 0; i < 5; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            if (dir == Vector2.zero) dir = Vector2.right;
            float length = Random.Range(0.12f, 0.28f);
            CreateLine("HitSpark", position, position + (Vector3)(dir * length), 0.025f,
                new Color(1f, 0.95f, 0.55f, 1f), new Color(1f, 0.25f, 0.05f, 0.1f), 0.12f);
        }
        CreateParticleBurst("HitCore", position, 6, 0.08f,
            new ParticleSystem.MinMaxCurve(0.09f, 0.18f),
            new ParticleSystem.MinMaxCurve(0.25f, 0.65f),
            new ParticleSystem.MinMaxCurve(0.035f, 0.075f),
            new ParticleSystem.MinMaxGradient(new Color(0.75f, 0.96f, 1f, 0.86f), new Color(1f, 0.5f, 0.16f, 0.72f)),
            0.035f, 0f, 0.32f);
    }

    public static void PlayDeathBurst(Vector3 position)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.enemyDeathPrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        CreateParticleBurst("DeathBurst", position, 18, 0.18f,
            new ParticleSystem.MinMaxCurve(0.22f, 0.45f),
            new ParticleSystem.MinMaxCurve(0.8f, 1.7f),
            new ParticleSystem.MinMaxCurve(0.045f, 0.11f),
            new ParticleSystem.MinMaxGradient(new Color(0.65f, 0.68f, 0.7f, 0.95f), new Color(1f, 0.42f, 0.08f, 0.95f)),
            0.18f, 0.15f, 0.9f);
        CreateLine("DeathCoreA", position + Vector3.left * 0.18f, position + Vector3.right * 0.22f, 0.055f,
            new Color(0.9f, 1f, 1f, 0.95f), new Color(1f, 0.28f, 0.08f, 0.15f), 0.16f);
        CreateLine("DeathCoreB", position + Vector3.down * 0.16f, position + Vector3.up * 0.2f, 0.05f,
            new Color(0.9f, 1f, 1f, 0.85f), new Color(1f, 0.28f, 0.08f, 0.1f), 0.15f);
    }

    public static void PlayStaticBreak(Vector3 position)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.staticBreakPrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        CreateParticleBurst("StaticBreakSmoke", position, 14, 0.24f,
            new ParticleSystem.MinMaxCurve(0.35f, 0.75f),
            new ParticleSystem.MinMaxCurve(0.25f, 0.8f),
            new ParticleSystem.MinMaxCurve(0.08f, 0.18f),
            new ParticleSystem.MinMaxGradient(new Color(0.35f, 0.36f, 0.37f, 0.8f), new Color(0.75f, 0.76f, 0.72f, 0.45f)),
            0.22f, 0.05f, 1.2f);
        PlayHitSpark(position + Vector3.up * 0.08f);
    }

    public static void PlayBunkerBreak(Vector3 position)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.bunkerBreakPrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        PlayStaticBreak(position);

        for (int i = 0; i < 7; i++)
        {
            Vector2 dir = new Vector2(Random.Range(-0.7f, 0.9f), Random.Range(0.2f, 0.85f)).normalized;
            float length = Random.Range(0.12f, 0.34f);
            CreateLine("MetalDebris", position, position + (Vector3)(dir * length), 0.035f,
                new Color(0.82f, 0.78f, 0.68f, 1f), new Color(0.35f, 0.36f, 0.38f, 0.15f), 0.22f);
        }
    }

    public static void PlayPulse(Vector3 position, float radius, Color color)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.empPulsePrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        float startRadius = Mathf.Max(0.01f, radius * 0.65f);

        GameObject go = new GameObject("PulseVfx");
        go.transform.position = position;

        var line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = 48;
        line.startWidth = 0.035f;
        line.endWidth = 0.035f;
        line.startColor = color;
        line.endColor = new Color(color.r, color.g, color.b, 0.1f);
        line.sortingOrder = 23;
        line.sharedMaterial = GetLineMaterial();

        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = i / (float)line.positionCount * Mathf.PI * 2f;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * startRadius, Mathf.Sin(angle) * startRadius, 0f));
        }

        var fade = go.AddComponent<VfxFade>();
        fade.life = 0.22f;
        fade.scaleTo = radius / startRadius;

        CreateParticleBurst("PulseSparks", position, 20, 0.12f,
            new ParticleSystem.MinMaxCurve(0.16f, 0.28f),
            new ParticleSystem.MinMaxCurve(0.5f, 1.1f),
            new ParticleSystem.MinMaxCurve(0.035f, 0.075f),
            new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, 0.95f), new Color(1f, 1f, 1f, 0.4f)),
            Mathf.Max(0.1f, radius * 0.18f), 0f, 0.55f);
    }

    public static void PlayRailCannonBeam(Vector3 position)
    {
        if (TryPlayPrefab(CombatVfxSettings.Instance != null ? CombatVfxSettings.Instance.railCannonBeamPrefab : null, position))
            return;
        if (!FallbackEnabled()) return;

        position.z = -3f;
        Vector3 start = position + Vector3.right * 0.2f;
        Vector3 end = position + Vector3.right * 11.2f;
        CreateLine("RailBeamGlow", start, end, 0.22f,
            new Color(1f, 0.2f, 0.08f, 0.72f), new Color(1f, 0.6f, 0.12f, 0.24f), 0.2f);
        CreateLine("RailBeamCore", start, end, 0.075f,
            Color.white, new Color(1f, 0.82f, 0.45f, 0.55f), 0.14f);
        CreateParticleBurst("RailMuzzleBurst", start, 18, 0.08f,
            new ParticleSystem.MinMaxCurve(0.16f, 0.28f),
            new ParticleSystem.MinMaxCurve(0.9f, 1.8f),
            new ParticleSystem.MinMaxCurve(0.04f, 0.09f),
            new ParticleSystem.MinMaxGradient(new Color(1f, 0.85f, 0.35f, 0.95f), new Color(1f, 0.18f, 0.05f, 0.55f)),
            0.12f, 0f, 0.48f);
    }

    public static void PlayEnergyCollect(Vector3 position)
    {
        if (!FallbackEnabled()) return;

        position.z = -3f;
        PlayPulse(position, 0.55f, new Color(0.35f, 0.95f, 1f, 0.72f));
        CreateParticleBurst("EnergyCollect", position, 14, 0.12f,
            new ParticleSystem.MinMaxCurve(0.18f, 0.32f),
            new ParticleSystem.MinMaxCurve(0.45f, 1.05f),
            new ParticleSystem.MinMaxCurve(0.045f, 0.095f),
            new ParticleSystem.MinMaxGradient(new Color(0.72f, 1f, 1f, 0.95f), new Color(0.12f, 0.82f, 0.95f, 0.42f)),
            0.09f, -0.08f, 0.55f);
    }

    private static bool TryPlayPrefab(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return false;
        position.z = -3f;
        Object.Instantiate(prefab, position, Quaternion.identity);
        return true;
    }

    private static bool FallbackEnabled()
    {
        return CombatVfxSettings.Instance == null || CombatVfxSettings.Instance.useCodeGeneratedFallback;
    }

    private static void CreateLine(string name, Vector3 start, Vector3 end, float width, Color startColor, Color endColor, float life)
    {
        GameObject go = new GameObject(name);
        var line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = width;
        line.endWidth = 0.01f;
        line.startColor = startColor;
        line.endColor = endColor;
        line.sortingOrder = 25;
        line.sharedMaterial = GetLineMaterial();
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        var fade = go.AddComponent<VfxFade>();
        fade.life = life;
        fade.scaleTo = 1f;
    }

    private static void CreateParticleBurst(
        string name,
        Vector3 position,
        short count,
        float duration,
        ParticleSystem.MinMaxCurve lifetime,
        ParticleSystem.MinMaxCurve speed,
        ParticleSystem.MinMaxCurve size,
        ParticleSystem.MinMaxGradient color,
        float radius,
        float gravity,
        float destroyAfter)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = duration;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.gravityModifier = gravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 24;
        renderer.material = GetLineMaterial();

        Object.Destroy(go, destroyAfter);
        ps.Play();
    }

    private static Material GetLineMaterial()
    {
        if (lineMaterial != null) return lineMaterial;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        lineMaterial = new Material(shader);
        return lineMaterial;
    }
}

class VfxFade : MonoBehaviour
{
    public float life = 0.12f;
    public float scaleTo = 1f;

    private float timer;
    private Vector3 startScale;
    private LineRenderer line;
    private Color startColor;
    private Color endColor;

    void Awake()
    {
        startScale = transform.localScale;
        line = GetComponent<LineRenderer>();
        if (line != null)
        {
            startColor = line.startColor;
            endColor = line.endColor;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = life > 0f ? Mathf.Clamp01(timer / life) : 1f;

        if (scaleTo != 1f)
            transform.localScale = Vector3.Lerp(startScale, startScale * scaleTo, t);

        if (line != null)
        {
            float alpha = 1f - t;
            line.startColor = WithAlpha(startColor, startColor.a * alpha);
            line.endColor = WithAlpha(endColor, endColor.a * alpha);
        }

        if (t >= 1f)
            Destroy(gameObject);
    }

    private static Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}
