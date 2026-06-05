using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DemoEndTrigger : MonoBehaviour
{
    [Header("Giao diện End Game")]
    public CanvasGroup endScreenCanvasGroup;
    
    [Header("Cài đặt")]
    public float fadeSpeed = 1f;
    public string mainMenuScene = "MainMenu";

    private bool isEnding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isEnding)
        {
            isEnding = true;
            StartCoroutine(PlayEndGameSequence());
        }
    }

    private IEnumerator PlayEndGameSequence()
    {
        Time.timeScale = 0f;

        if (endScreenCanvasGroup != null)
        {
            endScreenCanvasGroup.gameObject.SetActive(true);
            
            endScreenCanvasGroup.interactable = true;
            endScreenCanvasGroup.blocksRaycasts = true;
            
            endScreenCanvasGroup.alpha = 0f;
            while (endScreenCanvasGroup.alpha < 1f)
            {
                endScreenCanvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }
            endScreenCanvasGroup.alpha = 1f;
        }

    }
    public void ReturnToMainMenu()
    {
        if (GameDataManager.Instance != null)
        {
            PlayerData data = GameDataManager.Instance.CapturePlayerData();
            SaveSystem.SaveGame(data);
        }

        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuScene))
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}