using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialUIGroup 
{
    public string groupID;     
    public GameObject uiGroup; 
}

public class TutorialPanel : MonoBehaviour
{
    public static TutorialPanel Instance { get; private set; }

    [Header("Thành phần UI Core")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI instructionText;

    [Header("Kho chứa các nhóm Nút bấm")]
    public List<TutorialUIGroup> tutorialGroups = new List<TutorialUIGroup>();

    [Header("Cài đặt")]
    public float fadeSpeed = 3f;
    public float displayTime = 3f;

    private Coroutine currentTutorialRoutine;

    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void ShowTutorial(string requestedID, string text)
    {
        if (currentTutorialRoutine != null)
        {
            StopCoroutine(currentTutorialRoutine);
        }

        gameObject.SetActive(true);
        currentTutorialRoutine = StartCoroutine(HandleTutorialTransition(requestedID, text));
    }

    private IEnumerator HandleTutorialTransition(string requestedID, string text)
    {
        if (canvasGroup.alpha > 0f)
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime * (fadeSpeed * 4f);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
        instructionText.text = text;
        
        foreach (var group in tutorialGroups)
        {
            if (group.uiGroup != null) group.uiGroup.SetActive(false);
        }

        TutorialUIGroup targetGroup = tutorialGroups.Find(g => g.groupID == requestedID);
        if (targetGroup != null && targetGroup.uiGroup != null)
        {
            targetGroup.uiGroup.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Tutorial Group nào có ID là: " + requestedID);
        }

        while (canvasGroup.alpha < 1f) 
        { 
            canvasGroup.alpha += Time.deltaTime * fadeSpeed; 
            yield return null; 
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        while (canvasGroup.alpha > 0f) 
        { 
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed; 
            yield return null; 
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        currentTutorialRoutine = null;
    }
}