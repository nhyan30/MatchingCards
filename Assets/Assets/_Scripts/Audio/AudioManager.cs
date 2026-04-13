using UnityEngine;
public enum SoundType
{
    Flip,
    Match,
    Fail,
    LevelComplete
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   // Dedicated source for background music (loops)
    public AudioSource sfxSource;     // Dedicated source for SFX (one-shot)

    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("SFX Clips")]
    public AudioClip flip;
    public AudioClip match;
    public AudioClip fail;
    public AudioClip levelComplete;

    [Header("Volume Settings")]
    [Range(0f, 1f)] private float musicVolume = 0.5f;
    [Range(0f, 1f)] private float sfxVolume = 1f;

    [Header("Gameplay Ducking")]
    [Range(0f, 1f)][SerializeField] private float gameplayDuckFactor = 0.4f; // Music reduced to 40% during gameplay
    private bool isDucked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volume settings
        musicVolume = SaveSystem.GetMusicVolume();
        sfxVolume = SaveSystem.GetSFXVolume();

        // Apply volumes to sources
        InitializeAudio();
        ApplySFXVolume();
    }

    private void InitializeAudio()
    {
        // Create audio sources if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // Apply volumes
        ApplyMusicVolume();
        sfxSource.volume = sfxVolume;

        // Assign audio clip
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
        }
    }

    private void Start()
    {
        // Start background music automatically
        PlayBackgroundMusic();
    }

    #region Background Music

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Reduces background music volume during gameplay (ducking).
    /// Actual volume = musicVolume * gameplayDuckFactor
    /// </summary>
    public void DuckMusicForGameplay()
    {
        isDucked = true;
        ApplyMusicVolume();
    }

    /// <summary>
    /// Restores background music to full volume (e.g., back to main menu).
    /// </summary>
    public void RestoreMusicVolume()
    {
        isDucked = false;
        ApplyMusicVolume();
    }

    #endregion

    #region SFX Playback

    public void Play(SoundType type)
    {
        if (sfxSource == null) return;

        AudioClip clip = null;

        switch (type)
        {
            case SoundType.Flip:
                clip = flip;
                break;

            case SoundType.Match:
                clip = match;
                break;

            case SoundType.Fail:
                clip = fail;
                break;

            case SoundType.LevelComplete:
                clip = levelComplete;
                break;
        }

        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    #endregion

    #region Volume Control

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        SaveSystem.SaveMusicVolume(musicVolume);
        ApplyMusicVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveSystem.SaveSFXVolume(sfxVolume);
        ApplySFXVolume();
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    private void ApplyMusicVolume()
    {
        if (musicSource != null)
        {
            float effectiveVolume = isDucked ? musicVolume * gameplayDuckFactor : musicVolume;
            musicSource.volume = effectiveVolume;
        }
    }

    private void ApplySFXVolume()
    {
        // SFX volume is applied per-PlayOneShot call via the volume parameter,
        // so we just store it. No need to modify the AudioSource.
    }

    #endregion
}