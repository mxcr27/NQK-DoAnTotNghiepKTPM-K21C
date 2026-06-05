using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Cài đặt Máu")]
    public int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;

    private Coroutine flashRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        if (sr == null)
        {
            Debug.LogWarning("Không tìm thấy SpriteRenderer!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; 

        currentHealth -= damage;
        if (sr != null)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        EnemyLoot lootScript = GetComponent<EnemyLoot>();
        if (lootScript != null) 
        {
            lootScript.DropLoot();
        }
        EnemyAI aiScript = GetComponent<EnemyAI>();
        if (aiScript != null) aiScript.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (anim != null) anim.SetTrigger("Death");
        Destroy(gameObject, 2f); 
    }

    private IEnumerator FlashEffect()
    {
        sr.color = Color.red; 
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white; 
    }
}