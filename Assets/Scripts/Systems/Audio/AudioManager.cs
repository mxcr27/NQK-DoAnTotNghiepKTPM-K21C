using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Nguồn âm thanh (Audio Sources)")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSavedVolume();
    }

    private void LoadSavedVolume()
    {
        if (mainMixer == null) return;

        float savedBGM = PlayerPrefs.GetFloat("BGM_Vol", 1f);
        bool isBGMEnabled = PlayerPrefs.GetInt("BGM_Enable", 1) == 1;
        
        if (isBGMEnabled) mainMixer.SetFloat("BGMVolume", Mathf.Log10(savedBGM) * 20f);
        else mainMixer.SetFloat("BGMVolume", -80f);

        float savedSFX = PlayerPrefs.GetFloat("SFX_Vol", 1f);
        bool isSFXEnabled = PlayerPrefs.GetInt("SFX_Enable", 1) == 1;
        
        if (isSFXEnabled) mainMixer.SetFloat("SFXVolume", Mathf.Log10(savedSFX) * 20f);
        else mainMixer.SetFloat("SFXVolume", -80f);
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null) return;
        
        if (bgmSource.clip == bgmClip) return; 

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;
        
        sfxSource.PlayOneShot(sfxClip);
    }
}