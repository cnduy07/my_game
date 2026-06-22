using UnityEngine;

public class CombatVfxSettings : MonoBehaviour
{
    public static CombatVfxSettings Instance { get; private set; }

    [Header("Prefab Overrides")]
    public GameObject muzzleFlashPrefab;
    public GameObject hitSparkPrefab;
    public GameObject enemyDeathPrefab;
    public GameObject staticBreakPrefab;
    public GameObject bunkerBreakPrefab;
    public GameObject empPulsePrefab;
    public GameObject railCannonBeamPrefab;

    [Header("Fallback")]
    public bool useCodeGeneratedFallback = true;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
