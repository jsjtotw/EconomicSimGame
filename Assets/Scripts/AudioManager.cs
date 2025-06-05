using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private AudioClip buttonClickClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[AudioManager] Instance created and marked DontDestroyOnLoad.");
        }
        else
        {
            Debug.Log("[AudioManager] Duplicate detected. Destroying extra instance.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        buttonClickClip = Resources.Load<AudioClip>("Audio/DM-CGS-01");
        if (buttonClickClip != null)
        {
            Debug.Log("[AudioManager] Button click sound loaded successfully.");
        }
        else
        {
            Debug.LogError("[AudioManager] Failed to load button click sound from Resources/Audio/DM-CGS-01.wav.");
        }

        if (musicSource != null && musicSource.clip != null)
        {
            PlayMusic(musicSource.clip);
        }
        else
        {
            Debug.LogWarning("[AudioManager] No music clip assigned in musicSource.");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlaySFX() called with null clip.");
            return;
        }

        Debug.Log($"[AudioManager] Playing SFX: {clip.name}");
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonSound()
    {
        if (buttonClickClip != null)
        {
            PlaySFX(buttonClickClip);
        }
        else
        {
            Debug.LogWarning("[AudioManager] Button sound not available.");
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlayMusic() called with null clip.");
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        Debug.Log($"[AudioManager] Playing music: {clip.name}");
    }
}
 