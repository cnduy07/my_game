using UnityEngine;

public class TutorialCoach : MonoBehaviour
{
    public static TutorialCoach Instance { get; private set; }

    public bool showHints = true;
    public string energyHint = "Collect energy and place ArcReactors early to build your economy.";
    public string defenseHint = "Place Turrets in lanes with incoming enemies.";
    public string bunkerHint = "Use Bunkers to buy time in pressured lanes.";
    public string overchargeHint = "Use Overcharge to stabilize a lane under pressure.";

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
