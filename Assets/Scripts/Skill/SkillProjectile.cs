using UnityEngine;

[RequireComponent(typeof(SkillEntity), typeof(Rigidbody2D))]
public class SkillProjectile : MonoBehaviour
{
    [Header("Cài đặt bay")]
    public float speed = 15f;           
    public float autoDestroyTime = 3f;  
    public GameObject hitEffectPrefab;

    private SkillEntity entity;
    private Rigidbody2D rb;

    void Start()
    {
        entity = GetComponent<SkillEntity>();
        rb = GetComponent<Rigidbody2D>();
        float direction = 1f;

        if (entity != null && entity.caster != null)
        {
            direction = Mathf.Sign(entity.caster.transform.localScale.x);
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(speed * direction, 0f); 
        }

        if (direction < 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }

        Destroy(gameObject, autoDestroyTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (entity != null && entity.caster != null)
        {
            if (col.gameObject == entity.caster || col.transform.IsChildOf(entity.caster.transform))
            {
                return; 
            }
        }

        IDamageable target = col.GetComponentInParent<IDamageable>();
        
        if (target != null)
        {
            target.TakeDamage(entity.damage);
            ExplodeAndDestroy();
        }
        else if (col.CompareTag("Ground"))
        {
            ExplodeAndDestroy();
        }
    }

    void ExplodeAndDestroy()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}