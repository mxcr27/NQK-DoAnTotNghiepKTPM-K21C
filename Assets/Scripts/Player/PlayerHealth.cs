using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("Giao diện UI")]
    public Slider healthSlider;
    public float drainSpeed = 5f;

    [Header("Low Health Effect")]
    public Image lowHealthOverlay; 
    public int lowHealthThreshold = 30;
    public float pulseSpeed = 5f;

    [Header("Checkpoint (Điểm hồi sinh)")]
    public Vector2 checkpointPosition;   

    [Header("I-Frames & Visuals")]
    public float invincibilityDuration = 1.5f; 
    public float blinkInterval = 0.1f;         
    public Color damageColor = Color.red;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    private PlayerController playerController; 

    [Header("Âm thanh")]
    public AudioClip hurtSound;     
    
    public bool isDead { get; private set; }
    private bool isInvincible;
    private SpriteRenderer spriteRenderer;
    private Animator anim; 
    private Rigidbody2D rb;               
    private CapsuleCollider2D capCol;     

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        currentHealth = maxHealth;
        checkpointPosition = transform.position; 
    }

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>(); 
        rb = GetComponent<Rigidbody2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        playerController = GetComponent<PlayerController>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (healthSlider != null)
        {
            if (healthSlider.maxValue != maxHealth) healthSlider.maxValue = maxHealth;
            if (healthSlider.value != currentHealth)
            {
                healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, drainSpeed * Time.deltaTime);
                if (Mathf.Abs(healthSlider.value - currentHealth) < 0.1f) healthSlider.value = currentHealth;
            }
        }

        if (lowHealthOverlay != null)
        {
            if (currentHealth <= lowHealthThreshold && currentHealth > 0 && !isDead)
            {
                float alpha = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) / 2f * 0.4f; 
                lowHealthOverlay.color = new Color(1f, 0f, 0f, alpha);
            }
            else
            {
                lowHealthOverlay.color = new Color(1f, 0f, 0f, 0f); 
            }
        }
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        if (isDead || isInvincible) return; 

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (AudioManager.Instance != null && hurtSound != null)
        {
            AudioManager.Instance.PlaySFX(hurtSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageFeedbackRoutine());
            
            if (playerController != null)
            {
                playerController.ApplyKnockback(damageSource, knockbackForce);
            }

            if (impulseSource != null) 
            {
                impulseSource.GenerateImpulseWithForce(1.5f);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    private IEnumerator DamageFeedbackRoutine()
    {
        isInvincible = true;
        if (spriteRenderer != null) spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.15f); 
        
        float elapsedTime = 0f;
        bool isTransparent = false;

        while (elapsedTime < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                Color currentColor = Color.white;
                currentColor.a = isTransparent ? 0.5f : 1f; 
                spriteRenderer.color = currentColor;
            }
            isTransparent = !isTransparent; 
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    public void IncreaseMaxHealth(int boostAmount)
    {
        maxHealth += boostAmount;
        currentHealth += boostAmount;
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        
        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }
        
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        isDead = true;

        if (healthSlider != null) healthSlider.value = 0;
        if (anim != null) anim.SetTrigger("Die");
        GetComponent<PlayerController>().enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
{
    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll; 
    }

    yield return new WaitForSeconds(0.5f); 
    if (spriteRenderer != null) spriteRenderer.enabled = false;
    GameOverUI uiManager = Object.FindFirstObjectByType<GameOverUI>();
    if (uiManager != null)
    {
        uiManager.ShowGameOverScreen();
    }
}

        public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        if (healthSlider != null) healthSlider.value = maxHealth;

        transform.position = checkpointPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white; 
        }
        
        if (rb != null) 
        {
            rb.simulated = true;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        if (capCol != null) capCol.enabled = true;
        isInvincible = false;

        GetComponent<PlayerController>().enabled = true;
        if (anim != null) anim.Play("Player_Idle"); 
    }
}