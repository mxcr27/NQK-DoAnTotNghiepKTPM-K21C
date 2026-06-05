using UnityEngine;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 1.5f;

    void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            StartCoroutine(FadeInRoutine());
        }
    }

    private IEnumerator FadeInRoutine()
    {
        float alpha = 1f;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}