using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 

public class SceneController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Scene load khi bấm New Game (VD: Area1_F)")]
    public string startingSceneName = "Area1_F"; 

    [Header("Loading UI")]
    public GameObject loadingPanel;    
    public CanvasGroup canvasGroup;    
    public TextMeshProUGUI loadingText; 
    
    [Header("Menu Buttons")]
    public Button continueButton; 

    [Header("Confirmation UI")]
    public GameObject confirmNewGamePanel; 

    public float fadeSpeed = 2f;      
    public float textPulseSpeed = 3f; 

    void Start()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (confirmNewGamePanel != null) confirmNewGamePanel.SetActive(false);

        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSaveFile();
        }
    }

    public void NewGame()
    {
        if (SaveSystem.HasSaveFile())
        {
            confirmNewGamePanel.SetActive(true);
        }
        else
        {
            StartNewGameActual();
        }
    }

    public void ConfirmNewGameYes()
    {
        confirmNewGamePanel.SetActive(false);
        StartNewGameActual();
    }

    public void ConfirmNewGameNo()
    {
        confirmNewGamePanel.SetActive(false);
    }

    private void StartNewGameActual()
    {
        SaveSystem.DeleteSave(); 
        
        StartCoroutine(LoadSceneAsync(startingSceneName)); 
    }

    public void ContinueGame()
    {
        if (SaveSystem.HasSaveFile())
        {
            PlayerData data = SaveSystem.LoadGame();
            
            string sceneToLoad = string.IsNullOrEmpty(data.savedScene) ? startingSceneName : data.savedScene;
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);
        float alpha = 0;
        
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            if (loadingText != null)
            {
                float pulseAlpha = Mathf.PingPong(Time.time * textPulseSpeed, 1f);
                pulseAlpha = Mathf.Clamp(pulseAlpha, 0.2f, 1f); 
                loadingText.alpha = pulseAlpha; 
            }

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f); 
                operation.allowSceneActivation = true; 
            }
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}