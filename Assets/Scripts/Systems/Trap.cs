using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Cài đặt Bẫy")]
    public int damageAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerTarget = collision.GetComponent<PlayerHealth>();

        if (playerTarget != null)
        {
            playerTarget.TakeDamage(damageAmount);

            Destroy(gameObject);
        }
    }
}