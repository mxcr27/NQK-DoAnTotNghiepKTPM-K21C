using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Cài đặt Sát thương & Bẫy")]
    public bool instantGameOver = false;

    [Tooltip("Lượng máu bị trừ")]
    public int zoneDamage = 20;

    [Header("Cài đặt Dịch chuyển")]
    public bool teleportToCheckpoint = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null && !playerHealth.isDead)
            {
                if (instantGameOver)
                {
                    playerHealth.TakeDamage(9999);
                }
                else
                {

                    playerHealth.TakeDamage(zoneDamage);

                    if (!playerHealth.isDead && teleportToCheckpoint)
                    {
                        collision.transform.position = playerHealth.checkpointPosition;

                        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.linearVelocity = Vector2.zero;
                        }
                    }
                }
            }
        }
        else if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Destroy(collision.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }
}