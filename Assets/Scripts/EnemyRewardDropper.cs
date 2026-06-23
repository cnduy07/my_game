using UnityEngine;

public class EnemyRewardDropper : MonoBehaviour
{
    public float dropChance = 0.2f;
    public int energyValue = 25;

    Health health;

    public void Initialize(float chance, int value)
    {
        dropChance = Mathf.Clamp01(chance);
        energyValue = Mathf.Max(0, value);
        Subscribe();
    }

    void OnEnable()
    {
        Subscribe();
    }

    void OnDisable()
    {
        if (health != null)
            health.Died -= OnDied;
    }

    void Subscribe()
    {
        Health nextHealth = GetComponent<Health>();
        if (nextHealth == null || nextHealth == health) return;

        if (health != null)
            health.Died -= OnDied;

        health = nextHealth;
        health.Died += OnDied;
    }

    void OnDied(Health _)
    {
        if (EnergySystem.Instance == null || Random.value > dropChance) return;
        EnergySystem.Instance.SpawnRewardOrb(transform.position, energyValue);
    }
}
