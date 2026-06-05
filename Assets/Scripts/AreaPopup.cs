using UnityEngine;
using TMPro;
using System.Collections;

public class AreaPopup : MonoBehaviour
{
    [Header("Cài đặt Khu vực")]
    public string areaName = "Tên khu vực";
    public bool triggerOnStart = true;

    [Header("Tham chiếu UI")]
    public TextMeshProUGUI nameText;
    public CanvasGroup canvasGroup;

    [Header("Cài đặt Hiệu ứng")]
    public float fadeDuration = 1f;
    public float displayTime = 3f; 

    private bool hasTriggered = false;

    void Start()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        if (triggerOnStart)
        {
            ShowPopup();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggerOnStart && !hasTriggered && collision.CompareTag("Player"))
        {
            ShowPopup();
        }
    }

    public void ShowPopup()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        if (nameText != null) nameText.text = areaName;
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }
    }
}