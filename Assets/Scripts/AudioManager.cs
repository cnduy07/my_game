using UnityEngine;

public enum SfxType
{
    Shoot,
    Hit,
    EnemyDeath,
    UnitBreak,
    EnergyCollect,
    EmpBurst,
    RailCannon,
    UiClick,
    Win,
    GameOver
}

[System.Serializable]
public class SfxClip
{
    public SfxType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.96f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.04f;
}

// Gắn vào object "GameSystems" hoặc "AudioManager".
// Không có clip thì mọi lệnh Play vẫn an toàn, game chỉ im lặng.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource sfxSource;
    public SfxClip[] clips;

    void OnValidate()
    {
        ApplyClipDefaults();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        ApplyClipDefaults();
    }

    public static void PlaySfx(SfxType type)
    {
        if (Instance != null)
            Instance.Play(type);
    }

    void Play(SfxType type)
    {
        if (sfxSource == null || clips == null) return;

        SfxClip cue = null;
        foreach (var c in clips)
        {
            if (c != null && c.type == type && c.clip != null)
            {
                cue = c;
                break;
            }
        }

        if (cue == null) return;

        sfxSource.pitch = Random.Range(cue.pitchMin, cue.pitchMax);
        sfxSource.PlayOneShot(cue.clip, cue.volume * GameSettings.SfxVolume);
    }

    void ApplyClipDefaults()
    {
        if (clips == null) return;

        foreach (var c in clips)
        {
            if (c == null) continue;
            if (c.volume <= 0f) c.volume = 1f;
            if (c.pitchMin <= 0f) c.pitchMin = 0.96f;
            if (c.pitchMax <= 0f) c.pitchMax = 1.04f;
            if (c.pitchMax < c.pitchMin) c.pitchMax = c.pitchMin;
        }
    }
}
