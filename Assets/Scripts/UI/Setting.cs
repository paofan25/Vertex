using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject settingButton;
    
    [Header("UI Elements")]
    public TMP_Text musicVolumeText;
    public TMP_Text soundEffectsText;
    public TMP_Text brightnessText;
    public TMP_Text value_musicVolumeText;
    public TMP_Text value_soundEffectsText;
    public TMP_Text value_brightnessText;

    private float musicVolume = 50f;
    private float soundEffects = 50f;
    private float brightness = 50f;

    private int currentSelection = 0;
    private TMP_Text[] settingTexts; // 设置文本
    private TMP_Text[] settingValueTexts; // 数值文本
    
    private AudioMixer audioMixer;

    void Start()
    {
        // 加载设置数值
        musicVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        soundEffects = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        brightness = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        
        settingTexts = new TMP_Text[] { musicVolumeText, soundEffectsText, brightnessText };
        settingValueTexts = new TMP_Text[] { value_musicVolumeText, value_soundEffectsText, value_brightnessText };
        UpdateUI();
        HighlightCurrentSelection();
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    // 处理键盘输入
    void HandleKeyboardInput()
    {
        // 上下导航
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection--;
            if (currentSelection < 0) currentSelection = settingValueTexts.Length - 1;
            HighlightCurrentSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection++;
            if (currentSelection >= settingValueTexts.Length) currentSelection = 0;
            HighlightCurrentSelection();
        }

        // 左右调整
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AdjustCurrentSetting(-1f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AdjustCurrentSetting(1f);
        }
        
        // 按下X键关闭设置界面
        if (Input.GetKeyDown(KeyCode.X))
        {
            CloseSettings();
            return;
        }
    }

    // 调整当前设置
    void AdjustCurrentSetting(float amount)
    {
        switch (currentSelection)
        {
            case 0: // 音乐音量
                musicVolume = Mathf.Clamp(musicVolume + amount, 0f, 100f);
                if (musicVolume > 0) 
                    AudioManager.Instance.SetMusicVolume(musicVolume / 100f);
                else 
                    AudioManager.Instance.SetMusicVolume(0.001f);
                break;
            case 1: // 音效音量
                soundEffects = Mathf.Clamp(soundEffects + amount, 0f, 100f);
                if (soundEffects > 0)
                    AudioManager.Instance.SetSoundVolume(soundEffects / 100f);
                else
                    AudioManager.Instance.SetSoundVolume(0.001f);
                break;
            case 2: // 亮度
                brightness = Mathf.Clamp(brightness + amount, 0f, 100f);
                break;
        }
        UpdateUI();
    }

    // 更新UI
    void UpdateUI()
    {
        if (value_musicVolumeText != null)
            value_musicVolumeText.text = $"{musicVolume:F0}";
        if (value_soundEffectsText != null)
            value_soundEffectsText.text = $"{soundEffects:F0}";
        if (value_brightnessText != null)
            value_brightnessText.text = $"{brightness:F0}";
    }

    // 当前高亮选项
    void HighlightCurrentSelection()
    {
        for (int i = 0; i < settingValueTexts.Length; i++)
        {
            if (settingValueTexts[i] != null)
            {
                settingTexts[i].color = (i == currentSelection) ? Color.yellow : Color.white;
                settingValueTexts[i].color = (i == currentSelection) ? Color.yellow : Color.white;
            }
        }
    }
    
    // 关闭设置界面
    public void CloseSettings()
    {
        EventSystem.current.SetSelectedGameObject(settingButton.gameObject);
        RectTransform rect = gameObject.GetComponent<RectTransform>(); // 获取RectTransform组件
        rect.DOAnchorPos(new Vector2(0, 1080), 0.5f)
            .OnComplete(() => 
            {
                gameObject.SetActive(false);
                SaveSettings();
            });
    }
    
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SoundEffects", soundEffects);
        PlayerPrefs.SetFloat("Brightness", brightness);
        PlayerPrefs.Save();
    }

    // 按钮方法保持不变
    public void OnClickMusicUp() { musicVolume = Mathf.Clamp(musicVolume + 1f, 0f, 100f); UpdateUI(); }
    public void OnClickMusicDown() { musicVolume = Mathf.Clamp(musicVolume - 1f, 0f, 100f); UpdateUI(); }
    public void OnClickSoundEffectsUp() { soundEffects = Mathf.Clamp(soundEffects + 1f, 0f, 100f); UpdateUI(); }
    public void OnClickSoundEffectsDown() { soundEffects = Mathf.Clamp(soundEffects - 1f, 0f, 100f); UpdateUI(); }
    public void OnClickBrightnessUp() { brightness = Mathf.Clamp(brightness + 1f, 0f, 100f); UpdateUI(); }
    public void OnClickBrightnessDown() { brightness = Mathf.Clamp(brightness - 1f, 0f, 100f); UpdateUI(); }
}
