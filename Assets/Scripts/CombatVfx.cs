using UnityEngine;

// Lightweight code-generated combat VFX for the prototype.
// Later, these entry points can be replaced by polished prefab-based effects.
public static class CombatVfx
{
    private static Material lineMaterial;

    public static void PlayMuzzleFlash(Vector3 position)
    {
        position.z = -3f;
        CreateLine("MuzzleFlash", position, position + Vector3.right * 0.32f, 0.08f,
            new Color(1f, 0.95f, 0.35f, 1f), new Color(1f, 0.35f, 0.05f, 0.65f), 0.055f);
        CreateLine("MuzzleCore", position + Vector3.up * 0.035f, position + Vector3.right * 0.22f, 0.035f,
            Color.white, new Color(1f, 0.85f, 0.2f, 0.4f), 0.045f);
    }

    public static void PlayHitSpark(Vector3 position)
    {
        position.z = -3f;
        for (int i = 0; i < 5; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            if (dir == Vector2.zero) dir = Vector2.right;
            float length = Random.Range(0.12f, 0.28f);
            CreateLine("HitSpark", position, position + (Vector3)(dir * length), 0.025f,
                new Color(1f, 0.95f, 0.55f, 1f), new Color(1f, 0.25f, 0.05f, 0.1f), 0.12f);
        }
    }

    public static void PlayDeathBurst(Vector3 position)
    {
        position.z = -3f;
        GameObject go = new GameObject("DeathBurst");
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.18f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.22f, 0.45f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.7f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.045f, 0.11f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.65f, 0.68f, 0.7f, 0.95f),
            new Color(1f, 0.42f, 0.08f, 0.95f));
        main.gravityModifier = 0.15f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 18) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.18f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 24;
        renderer.material = GetLineMaterial();

        Object.Destroy(go, 0.9f);
        ps.Play();
    }

    public static void PlayPulse(Vector3 position, float radius, Color color)
    {
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
