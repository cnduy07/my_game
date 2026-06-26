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
    const string DefaultMusicResourcePath = "Audio/Music/hidden_labs";

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float backgroundMusicVolume = 0.28f;
    public bool playMusicOnStart = true;

    [Header("SFX")]
    public AudioSource sfxSource;
    public SfxClip[] clips;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BootstrapAudio()
    {
        EnsureInstance();
    }

    void OnValidate()
    {
        ApplyClipDefaults();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.AbsorbSerializedConfig(this);
            if (playMusicOnStart)
                Instance.PlayMusic();
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();

        EnsureDefaultMusicClip();
        ApplyClipDefaults();
        ConfigureMusicSource();
    }

    void Start()
    {
        if (playMusicOnStart)
            PlayMusic();
    }

    void Update()
    {
        RefreshMusicVolume();
    }

    public static void PlaySfx(SfxType type)
    {
        EnsureInstance(false);
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

    public static void RefreshMusic()
    {
        EnsureInstance();
        if (Instance != null)
            Instance.RefreshMusicVolume();
    }

    public static AudioManager EnsureInstance(bool startMusic = true)
    {
        if (Instance == null)
        {
            GameObject audioObject = new GameObject("AudioManager");
            Instance = audioObject.AddComponent<AudioManager>();
        }

        Instance.EnsureSources();
        Instance.EnsureDefaultMusicClip();
        Instance.ConfigureMusicSource();

        if (startMusic)
            Instance.PlayMusic();

        return Instance;
    }

    public void PlayMusic()
    {
        if (musicSource == null || backgroundMusic == null) return;

        ConfigureMusicSource();
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    void AbsorbSerializedConfig(AudioManager source)
    {
        if (source == null)
            return;

        if (backgroundMusic == null && source.backgroundMusic != null)
            backgroundMusic = source.backgroundMusic;

        backgroundMusicVolume = source.backgroundMusicVolume;

        if ((clips == null || clips.Length == 0) && source.clips != null && source.clips.Length > 0)
            clips = source.clips;

        ApplyClipDefaults();
        ConfigureMusicSource();
    }

    void EnsureDefaultMusicClip()
    {
        if (backgroundMusic == null)
            backgroundMusic = Resources.Load<AudioClip>(DefaultMusicResourcePath);
    }

    void EnsureSources()
    {
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
        musicSource.spatialBlend = 0f;
    }

    void ConfigureMusicSource()
    {
        if (musicSource == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        RefreshMusicVolume();
    }

    void RefreshMusicVolume()
    {
        if (musicSource != null)
            musicSource.volume = backgroundMusicVolume * GameSettings.MusicVolume;
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
