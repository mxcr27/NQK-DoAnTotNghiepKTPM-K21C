using UnityEngine;

public class BossBGMController : MonoBehaviour
{
    [Header("Nhạc BGM Boss")]
    public AudioClip bossMusic;
    private AudioClip savedMapMusic;

    public void StartBossMusic()
    {
        if (AudioManager.Instance != null && bossMusic != null)
        {
            savedMapMusic = AudioManager.Instance.bgmSource.clip;
            AudioManager.Instance.PlayBGM(bossMusic);
        }
    }

    public void StopBossMusic()
    {
        if (AudioManager.Instance != null && savedMapMusic != null)
        {
            AudioManager.Instance.PlayBGM(savedMapMusic);
        }
    }
}