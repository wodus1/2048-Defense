using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
    }

    public static void PlayBGM(AudioClip clip, bool restart= false)
    {
        if (clip == null) return;

        var src = Instance.bgmSource;

        if (restart && src.isPlaying && src.clip == clip)
            return;

        src.Stop();
        src.clip = clip;
        src.volume = Instance.masterVolume * Instance.bgmVolume;
        src.Play();
    }

    public static void StopBGM()
    {
        if (Instance.bgmSource.isPlaying)
            Instance.bgmSource.Stop();
    }

    public static void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        Instance.PlayOneShot(clip, Instance.sfxVolume);
    }

    public static void PauseAll()
    {
        foreach (var src in Instance.GetComponents<AudioSource>())
            src.Pause();
    }

    public static void ResumeAll()
    {
        foreach (var src in Instance.GetComponents<AudioSource>())
            src.UnPause();
    }

    private void PlayOneShot(AudioClip clip, float channelVolume)
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.clip = clip;
        src.volume = masterVolume * channelVolume;

        src.Play();

        Destroy(src, clip.length);
    }
}