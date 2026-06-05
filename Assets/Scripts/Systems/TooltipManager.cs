using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))] 
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    
    [Header("Settings")]
    public float delayTime = 0.5f;

    [Header("Offset")]
    public Vector2 offset = new Vector2(15f, -15f); 
    
    private Coroutine showCoroutine; 
    private CanvasGroup canvasGroup; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        canvasGroup = GetComponent<CanvasGroup>();
        
        HideTooltip(); 
    }

    void Update()
    {
        transform.position = (Vector2)Input.mousePosition + offset;
    }

    public void ShowTooltip(string itemName, string itemDesc)
    {
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        
        showCoroutine = StartCoroutine(WaitAndShow(itemName, itemDesc));
    }

    public void HideTooltip()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        
        canvasGroup.alpha = 0f; 
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator WaitAndShow(string itemName, string itemDesc)
    {
        yield return new WaitForSecondsRealtime(delayTime); 

        nameText.text = itemName;
        descriptionText.text = itemDesc;
        
        canvasGroup.alpha = 1f; 
    }
}