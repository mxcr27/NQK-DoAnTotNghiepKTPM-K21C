using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [Header("Nhạc BGM")]
    public AudioClip bgmClip;

    void Start()
    {
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip);
        }
    }
}