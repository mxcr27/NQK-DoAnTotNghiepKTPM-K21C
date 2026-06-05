using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;

    [Header("Cài đặt Trục X & Y")]
    public float parallaxEffect;
    public float parallaxEffectY = 1f;

    [Header("Chuyển động (Cho Mây)")]
    public float autoScrollSpeed = 0f;

    private float length;
    private float startposX, startposY;
    private float offsetX;

    void Awake()
    {
        if (cam == null) cam = Camera.main;

        startposX = transform.position.x;
        startposY = transform.position.y;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        length = (sr != null && sr.sprite != null)
            ? sr.sprite.bounds.size.x * transform.lossyScale.x
            : 0f;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        offsetX += autoScrollSpeed * Time.deltaTime;

        float camX = cam.transform.position.x;
        float distX = camX * parallaxEffect;
        float tempX = camX * (1 - parallaxEffect);

        float distY = cam.transform.position.y * parallaxEffectY;

        transform.position = new Vector3(
            startposX + distX + offsetX,
            startposY + distY,
            transform.position.z
        );

        if (length > 0)
        {
            while (tempX > startposX + offsetX + length)
                startposX += length;

            while (tempX < startposX + offsetX - length)
                startposX -= length;
        }
    }
}