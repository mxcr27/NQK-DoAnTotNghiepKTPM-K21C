using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostTrail : MonoBehaviour
{
    [Header("Tốc độ mờ")]
    public float fadeSpeed = 4f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Color currentColor = sr.color;
        currentColor.a -= fadeSpeed * Time.deltaTime;
        sr.color = currentColor;

        if (currentColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}