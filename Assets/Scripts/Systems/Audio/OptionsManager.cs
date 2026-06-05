using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Kết nối Hệ thống")]
    public AudioMixer mainMixer;

    [Header("UI Nhạc nền (BGM)")]
    public Slider bgmSlider;
    public Toggle bgmToggle;

    [Header("UI Hiệu ứng (SFX)")]
    public Slider sfxSlider;
    public Toggle sfxToggle;

    void Start()
    {
        float savedBGM = PlayerPrefs.GetFloat("BGM_Vol", 1f);
        bgmSlider.value = savedBGM;
        
        bool isBGMEnabled = PlayerPrefs.GetInt("BGM_Enable", 1) == 1;
        bgmToggle.isOn = isBGMEnabled;
        SetBGMEnabled(isBGMEnabled); 

        float savedSFX = PlayerPrefs.GetFloat("SFX_Vol", 1f);
        sfxSlider.value = savedSFX;
        
        bool isSFXEnabled = PlayerPrefs.GetInt("SFX_Enable", 1) == 1;
        sfxToggle.isOn = isSFXEnabled;
        SetSFXEnabled(isSFXEnabled);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        bgmToggle.onValueChanged.AddListener(SetBGMEnabled);
        
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        sfxToggle.onValueChanged.AddListener(SetSFXEnabled);
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmToggle.isOn)
        {
            mainMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20f);
        }
        PlayerPrefs.SetFloat("BGM_Vol", volume);
    }

    public void SetBGMEnabled(bool isEnabled)
    {
        if (isEnabled) 
        {
            mainMixer.SetFloat("BGMVolume", Mathf.Log10(bgmSlider.value) * 20f);
        }
        else 
        {
            mainMixer.SetFloat("BGMVolume", -80f); 
        }
        
        PlayerPrefs.SetInt("BGM_Enable", isEnabled ? 1 : 0);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxToggle.isOn)
        {
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);
        }
        PlayerPrefs.SetFloat("SFX_Vol", volume);
    }

    public void SetSFXEnabled(bool isEnabled)
    {
        if (isEnabled) 
        {
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20f);
        }
        else 
        {
            mainMixer.SetFloat("SFXVolume", -80f);
        }
        
        PlayerPrefs.SetInt("SFX_Enable", isEnabled ? 1 : 0);
    }
}