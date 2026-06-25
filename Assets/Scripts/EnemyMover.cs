using System.Collections.Generic;
using UnityEngine;

// Moves an enemy left across its assigned lane and handles contact attacks.
public class EnemyMover : MonoBehaviour
{
    public GridManager grid;
    public int row;
    public float speed = 0.27f;
    public float attackDamage = 30f;

    float slowFactor = 1f;
    private float slowTimer = 0f;
    private float stunTimer = 0f;
    public float attackSfxInterval = 0.65f;
    private float attackSfxTimer = 0f;

    private bool caught = false;
    private CharacterAnimator anim;
    private EnemyTraits traits;

    // Danh sách mọi địch đang sống, để Shooter và Projectile tra cứu (khỏi cần collider).
    public static readonly List<EnemyMover> All = new List<EnemyMover>();
    static int activeOrDyingCount;
    bool registeredLifetime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetSceneState()
    {
        All.Clear();
        activeOrDyingCount = 0;
    }

    void Awake()
    {
        anim = GetComponent<CharacterAnimator>();
        traits = GetComponent<EnemyTraits>();
    }

    void OnEnable()
    {
        if (!registeredLifetime)
        {
            registeredLifetime = true;
            activeOrDyingCount++;
        }

        if (!All.Contains(this))
            All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    void OnDestroy()
    {
        if (!registeredLifetime) return;

        registeredLifetime = false;
        activeOrDyingCount = Mathf.Max(0, activeOrDyingCount - 1);
    }

    public void ApplySlow(float factor, float duration)
    {
        EnsureTraits();
        if (traits != null)
        {
            factor = traits.ModifySlowFactor(factor);
            duration = traits.ModifySlowDuration(duration);
        }

        if (duration <= 0f || factor >= 1f) return;
        if (factor < slowFactor) slowFactor = factor;
        slowTimer = Mathf.Max(slowTimer, duration);
    }

    public void ApplyKnockback(float distance)
    {
        EnsureTraits();
        if (traits != null)
            distance = traits.ModifyKnockback(distance);

        if (distance <= 0f) return;
        transform.position += Vector3.right * distance;
    }

    public void ApplyStun(float duration)
    {
        EnsureTraits();
        if (traits != null)
            duration = traits.ModifyStunDuration(duration);

        if (duration <= 0f) return;
        stunTimer = Mathf.Max(stunTimer, duration);
    }

    void Update()
    {
        if (grid == null) return;
        if (caught) return;

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) slowFactor = 1f;
        }

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            if (anim != null) anim.SetWalking(false);
            return;
        }

        int col = grid.ColFromWorldX(transform.position.x);
        GameObject unit = grid.GetUnitAt(col, row);

        if (unit != null)
        {
            var hp = unit.GetComponent<Health>();
            if (hp != null && hp.IsAlive)
            {
                if (anim != null) { anim.SetWalking(false); anim.TriggerAttack(); }
                attackSfxTimer -= Time.deltaTime;
                if (attackSfxTimer <= 0f)
                {
                    AudioManager.PlaySfx(SfxType.Hit);
                    attackSfxTimer = attackSfxInterval;
                }
                hp.TakeDamage(attackDamage * Time.deltaTime);
                return;
            }
        }

        attackSfxTimer = 0f;
        if (anim != null) anim.SetWalking(true);
        transform.position += Vector3.left * speed * slowFactor * Time.deltaTime;

        if (transform.position.x < grid.origin.x - 0.5f)
        {
            if (GameManager.Instance != null && GameManager.Instance.TryLawnmower(row))
            {
                caught = true;
                return;
            }

            if (GameManager.Instance != null) GameManager.Instance.GameOver(row);
            Destroy(gameObject);
        }
    }

    public float CurrentX => transform.position.x;
    public static int ActiveOrDyingCount => activeOrDyingCount;

    public static int CountInRow(int row)
    {
        int count = 0;
        foreach (EnemyMover enemy in All)
            if (enemy != null && enemy.row == row)
                count++;

        return count;
    }

    public static float LanePressure01(int row, GridManager grid)
    {
        if (grid == null) return 0f;

        float leftEdge = grid.origin.x - grid.cellSize * 0.5f;
        float rightEdge = grid.origin.x + (grid.cols - 1) * grid.cellSize;
        float pressure = 0f;
        int count = 0;

        foreach (EnemyMover enemy in All)
        {
            if (enemy == null || enemy.row != row) continue;

            count++;
            float enemyPressure = Mathf.InverseLerp(rightEdge, leftEdge, enemy.CurrentX);
            pressure = Mathf.Max(pressure, enemyPressure);
        }

        if (count > 1)
            pressure += Mathf.Min(0.35f, (count - 1) * 0.08f);

        return Mathf.Clamp01(pressure);
    }

    void EnsureTraits()
    {
        if (traits == null)
            traits = GetComponent<EnemyTraits>();
    }
}
