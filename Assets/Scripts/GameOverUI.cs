using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel; 
    public Image backgroundImage;    
    public TextMeshProUGUI deathText;   
    public CanvasGroup buttonGroup;      

    [Header("Player Reference")]
    public PlayerHealth playerHealth;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(FadeInSequence());
        }
    }

    private IEnumerator FadeInSequence()
    {
        Color bgColor = backgroundImage.color;
        bgColor.a = 0f;
        backgroundImage.color = bgColor;

        Color textColor = deathText.color;
        textColor.a = 0f;
        deathText.color = textColor;

        buttonGroup.alpha = 0f;
        buttonGroup.interactable = false;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            bgColor.a = Mathf.Lerp(0f, 0.8f, t);
            backgroundImage.color = bgColor;
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 1f;
            textColor.a = Mathf.Lerp(0f, 1f, t);
            deathText.color = textColor;
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            buttonGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        
        buttonGroup.interactable = true; 
    }

    public void OnTryAgainClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (playerHealth != null)
        {
            playerHealth.Respawn();
        }
    }
}