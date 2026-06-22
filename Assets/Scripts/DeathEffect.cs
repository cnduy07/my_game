using UnityEngine;

public enum DeathEffectKind
{
    None,
    DefaultBurst,
    StaticBreak,
    BunkerBreak
}

// Optional override for object death VFX.
// If absent, Health falls back to a sensible non-enemy static break effect.
public class DeathEffect : MonoBehaviour
{
    public DeathEffectKind effectKind = DeathEffectKind.StaticBreak;
    public Vector3 offset;
    public bool hideRenderersOnDeath = false;

    public void Play()
    {
        Vector3 position = transform.position + offset;

        switch (effectKind)
        {
            case DeathEffectKind.None:
                break;
            case DeathEffectKind.BunkerBreak:
                CombatVfx.PlayBunkerBreak(position);
                break;
            case DeathEffectKind.DefaultBurst:
                CombatVfx.PlayDeathBurst(position);
                break;
            default:
                CombatVfx.PlayStaticBreak(position);
                break;
        }

        if (hideRenderersOnDeath)
            HideRenderers();
    }

    void HideRenderers()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var spriteRenderer in spriteRenderers)
            spriteRenderer.enabled = false;
    }
}
