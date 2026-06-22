using UnityEngine;

public class TutorialCoach : MonoBehaviour
{
    public static TutorialCoach Instance { get; private set; }

    public bool showHints = true;
    public string energyHint = "Thu nang luong, dat ArcReactor truoc de tang kinh te.";
    public string defenseHint = "Dat Turret theo hang co dich.";
    public string bunkerHint = "Dung Bunker de cau gio cho hang bi ep.";
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

            LevelDefinition level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : null;
            if (EnemyMover.All.Count > 0)
            {
                if (level != null && level.overchargeUnlocked)
                    return overchargeHint;
                if (level != null && level.AllowsUnit("Bunker"))
                    return bunkerHint;
            }

            return defenseHint;
        }
    }
}
