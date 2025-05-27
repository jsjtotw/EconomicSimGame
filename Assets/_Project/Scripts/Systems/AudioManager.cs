// File: Assets/Scripts/Systems/AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sfxSounds;
    [SerializeField] private Sound[] musicSounds; // If you plan background music

    private Dictionary<string, AudioClip> sfxClipMap;
    private AudioSource sfxSource; // Single AudioSource for SFX

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = null; // Can assign mixer group here if you set one up

            sfxClipMap = new Dictionary<string, AudioClip>();
            foreach (Sound s in sfxSounds)
            {
                if (!sfxClipMap.ContainsKey(s.name))
                {
                    sfxClipMap.Add(s.name, s.clip);
                }
                else
                {
                    Debug.LogWarning($"Duplicate SFX name in AudioManager: {s.name}");
                }
            }
        }
    }

    public void PlaySFX(string soundName)
    {
        if (sfxClipMap.TryGetValue(soundName, out AudioClip clip))
        {
            Sound s = System.Array.Find(sfxSounds, sound => sound.name == soundName);
            if (s != null)
            {
                sfxSource.volume = s.volume;
                sfxSource.pitch = s.pitch;
                sfxSource.PlayOneShot(clip); // Play as a one-shot to avoid interrupting current sounds
            }
        }
        else
        {
            Debug.LogWarning($"SFX sound '{soundName}' not found!");
        }
    }

    // You can add PlayMusic, StopMusic, SetVolume methods here later.
}