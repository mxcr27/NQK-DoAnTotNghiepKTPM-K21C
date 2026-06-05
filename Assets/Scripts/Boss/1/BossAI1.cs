using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAI1 : MonoBehaviour, IDamageable
{
    public enum BossState { Idle, Chase, Attack, Dead }
    
    [Header("Trạng thái FSM")]
    public BossState currentState = BossState.Idle;

    [Header("Cài đặt Máu (Boss Health)")]
    public int maxHealth = 500;         
    private int currentHealth;

    [Header("Giao diện UI cho Boss")]
    public GameObject bossHUDObject; 
    
    public Image healthBarFill;         

    [Header("Cài đặt Di chuyển")]
    public Transform player;
    public float chaseSpeed = 3f;
    
    [Header("Cài đặt Tấn công (Cận chiến)")]
    public float attackRange = 2f;      
    public float attackCooldown = 2.5f; 
    public Transform centerPoint;
    
    [Header("Cài đặt Sát thương")]
    public Transform attackPoint;       
    public float attackRadius = 1f;     
    public LayerMask playerLayer;       
    public int damageAttack1 = 20;      
    public int damageAttack2 = 35;      

    [Header("Cài đặt Hiệu ứng & Chết")]
    public float flashDuration = 0.15f; 
    public float deathFadeDelay = 3f;   
    public float deathFadeDuration = 1.5f; 
    
    public GameObject deathEffectPrefab; 

    private float cooldownTimer;
    private Rigidbody2D rb;
    private Animator anim;
    private float targetVelocityX;
    
    private SpriteRenderer sr;
    private Coroutine flashRoutine;

    private int currentAttackType = 1; 
    private Vector3 startPosition; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); 
        sr = GetComponentInChildren<SpriteRenderer>(); 

        currentHealth = maxHealth;
        startPosition = transform.position;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        currentState = BossState.Idle;

        if (bossHUDObject != null) bossHUDObject.SetActive(false);
        UpdateHealthUI(); 
    }

    void Update()
    {
        if (currentState == BossState.Dead) return;

        cooldownTimer -= Time.deltaTime;

        if (currentState != BossState.Idle && bossHUDObject != null && !bossHUDObject.activeSelf)
        {
            bossHUDObject.SetActive(true);
            UpdateHealthUI();
        }

        switch (currentState)
        {
            case BossState.Idle:
                targetVelocityX = 0f; 
                break;
                
            case BossState.Chase:
                ExecuteChaseLogic();
                break;
                
            case BossState.Attack:
                targetVelocityX = 0f; 
                break;
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (currentState != BossState.Dead)
        {
            rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        }
    }

    void ExecuteChaseLogic()
    {
        if (player == null) return;

        Vector3 bossCenter = (centerPoint != null) ? centerPoint.position : transform.position;
        float distanceX = player.position.x - transform.position.x;
        float directionToPlayer = Mathf.Sign(distanceX);
        float distanceToPlayer = Vector2.Distance(bossCenter, player.position);

        if (distanceToPlayer <= attackRange)
        {
            targetVelocityX = 0f; 
            
            if (Mathf.Abs(distanceX) > 0.6f) Flip(directionToPlayer);

            if (cooldownTimer <= 0)
            {
                currentState = BossState.Attack;
                currentAttackType = Random.Range(1, 3); 
                
                if (currentAttackType == 1) anim.SetTrigger("Attack1");
                else anim.SetTrigger("Attack2");
                
                cooldownTimer = attackCooldown; 
            }
            return;
        }

        if (Mathf.Abs(distanceX) > 0.6f) 
        {
            targetVelocityX = directionToPlayer * chaseSpeed;
            Flip(directionToPlayer); 
        }
        else 
        {
            targetVelocityX = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= damage;
        Debug.Log("Boss trúng đòn! Máu còn lại: " + currentHealth);

        UpdateHealthUI();

        if (sr != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0) Die();
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            float fillRatio = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = fillRatio;
        }
    }

    // 🚀 ĐÃ SỬA: Dùng biến flashDuration từ Inspector
    private IEnumerator FlashEffect()
    {
        sr.color = Color.red; 
        yield return new WaitForSeconds(flashDuration); 
        sr.color = Color.white; 
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider2D p in hitPlayers)
        {
            PlayerHealth health = p.GetComponent<PlayerHealth>();
            if (health != null) 
            {
                int finalDamage = (currentAttackType == 1) ? damageAttack1 : damageAttack2;
                health.TakeDamage(finalDamage, transform);
            }
        }
    }

    public void ResetAttack() 
    { 
        if (currentState != BossState.Dead) currentState = BossState.Chase; 
    }

    public void Die()
    {
        currentState = BossState.Dead;
        targetVelocityX = 0f;
        
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // Khóa cứng xác Boss, chống rơi rớt
        }

        if (anim != null) anim.SetTrigger("Die");
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (bossHUDObject != null) bossHUDObject.SetActive(false);

        // 🚀 KÍCH HOẠT TIẾN TRÌNH LÀM MỜ XÁC BOSS
        if (sr != null)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            Destroy(gameObject, deathFadeDelay);
        }

        Debug.Log("Boss đã gục ngã hoàn toàn!");
    }

    // 🚀 HÀM MỚI: LÀM MỜ VÀ XÓA XÁC BOSS BẰNG BIẾN TÙY CHỈNH
    private IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(deathFadeDelay); 
        
        Color originalColor = sr.color;

        for (float t = 0; t < deathFadeDuration; t += Time.deltaTime)
        {
            if (sr == null) yield break; 
            
            float normalizedTime = t / deathFadeDuration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, normalizedTime); 
            sr.color = newColor;
            
            yield return null; 
        }

        // Sinh ra hiệu ứng hạt (nếu bạn có gắn chóp khói/nổ vào Inspector)
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); 
    }

    public void ResetBoss()
    {
        if (currentState == BossState.Dead) return;

        currentHealth = maxHealth;          
        transform.position = startPosition; 
        currentState = BossState.Idle;      
        targetVelocityX = 0f;

        if (bossHUDObject != null) bossHUDObject.SetActive(false);
        UpdateHealthUI(); 

        if (anim != null) 
        {
            anim.SetBool("isRunning", false);
            anim.Play("Idle"); 
        }
    }

    void UpdateAnimation()
    {
        if (anim != null && currentState != BossState.Attack && currentState != BossState.Dead)
        {
            bool isMoving = Mathf.Abs(targetVelocityX) > 0.1f;
            anim.SetBool("isRunning", isMoving);
        }
    }

    void Flip(float directionX)
    {
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * directionX;
        transform.localScale = localScale;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
        Vector3 bossCenter = (centerPoint != null) ? centerPoint.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(bossCenter, attackRange);
    }

    private void OnDisable()
    {
        if (currentState == BossState.Dead)
        {
            Destroy(gameObject);
        }
    }
}