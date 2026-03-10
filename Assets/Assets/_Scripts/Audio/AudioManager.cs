using UnityEngine;

public enum SoundType
{
    Flip,
    Match,
    Fail,
    LevelComplete,
    GameOver
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource source;

    public AudioClip flip;
    public AudioClip match;
    public AudioClip fail;
    public AudioClip levelComplete;
    public AudioClip gameOver;

    private void Awake()
    {
        Instance = this;
    }

    public void Play(SoundType type)
    {
        switch (type)
        {
            case SoundType.Flip:
                source.PlayOneShot(flip);
                break;

            case SoundType.Match:
                source.PlayOneShot(match);
                break;

            case SoundType.Fail:
                source.PlayOneShot(fail);
                break;

            case SoundType.LevelComplete:
                source.PlayOneShot(levelComplete);
                break;

            case SoundType.GameOver:
                source.PlayOneShot(gameOver);
                break;
        }
    }
}