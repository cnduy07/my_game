using UnityEngine;

public class TutorialCoach : MonoBehaviour
{
    public static TutorialCoach Instance { get; private set; }

    public bool showHints = true;
    public string energyHint = "Thu nang luong, dat ArcReactor truoc de tang kinh te.";
    public string defenseHint = "Dat Turret/SnowGun theo hang co dich.";
    public string overchargeHint = "Dung OC de cuu hang dang bi ep.";

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public string CurrentHint
    {
        get
        {
            if (!showHints) return "";
            if (EnergySystem.Instance != null && EnergySystem.Instance.Energy < 100)
                return energyHint;
            if (EnemyMover.All.Count > 0)
                return overchargeHint;
            return defenseHint;
        }
    }
}
