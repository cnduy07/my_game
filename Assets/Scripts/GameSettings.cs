using UnityEngine;

public static class GameSettings
{
    const string SfxVolumeKey = "settings.sfxVolume";
    const string ReduceShakeKey = "settings.reduceShake";
    const string VibrationKey = "settings.vibration";

    public static float SfxVolume
    {
        get => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        set
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    public static bool ReduceShake
    {
        get => PlayerPrefs.GetInt(ReduceShakeKey, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(ReduceShakeKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool VibrationEnabled
    {
        get => PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(VibrationKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
