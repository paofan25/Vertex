using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音频管理器
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("音频源")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("音效库")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip dashSFX;
    [SerializeField] private AudioClip landSFX;
    [SerializeField] private AudioClip hurtSFX;
    
    private Dictionary<string, AudioClip> sfxLibrary;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFXLibrary();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSFXLibrary()
    {
        sfxLibrary = new Dictionary<string, AudioClip>
        {
            { "Jump", jumpSFX },
            { "Dash", dashSFX },
            { "Land", landSFX },
            { "Hurt", hurtSFX }
        };
    }
    
    public void PlaySFX(string sfxName)
    {
        if (sfxLibrary.TryGetValue(sfxName, out AudioClip clip) && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.Play();
    }
}