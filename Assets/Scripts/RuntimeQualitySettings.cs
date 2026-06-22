using UnityEngine;

public class RuntimeQualitySettings : MonoBehaviour
{
    public int targetFrameRate = 60;
    public bool disableVSync = true;
    public bool preventScreenSleep = true;
    public bool allowMultiTouch = false;

    void Awake()
    {
        Apply();
    }

    void OnValidate()
    {
        targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 120);
    }

    public void Apply()
    {
        if (disableVSync)
            QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = targetFrameRate;
        Screen.sleepTimeout = preventScreenSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        Input.multiTouchEnabled = allowMultiTouch;
    }
}
