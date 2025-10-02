using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

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
    
    [Header("音频混音器")]
    [SerializeField] private AudioMixer audioMixer; // 音频混音器
    
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

    private void Start()
    {
        InitializeAudioMixer(); // 初始化音频
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

    // 初始化音频
    public void InitializeAudioMixer()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f)); // 获取并设置音乐音量
        SetSoundVolume(PlayerPrefs.GetFloat("SoundEffects", 0.5f)); // 获取并设置音效音量
    }
    
    // 设置总音量
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume / 100f) * 20); // 转换为分贝
    }

    // 设置背景音乐音量
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume / 100f) * 20);
    }

    // 设置音效音量
    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SoundEffects", Mathf.Log10(volume / 100f) * 20);
    }

    // 静音或取消静音
    public void Mute(bool isMuted)
    {
        audioMixer.SetFloat("MasterVolume", isMuted ? -80f : 0f); // -80dB表示静音
    }
}