using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Equipment")]
    public WeaponData defaultWeapon; 
    public WeaponData currentWeapon; 

    [Header("References")]
    public Transform attackPoint;           
    public LayerMask enemyLayers;

    [Header("Âm thanh Chiến đấu")]
    public AudioClip[] slashSounds; 
    
    private Animator anim; 
    private float nextAttackTime = 0f;
    
    private CinemachineImpulseSource impulseSource;
    private PlayerHealth playerHealth;

    void Start()
    {
        if (currentWeapon == null) currentWeapon = defaultWeapon;
        anim = GetComponentInChildren<Animator>();

        impulseSource = GetComponent<CinemachineImpulseSource>();

        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.isDead) return;

        if (Time.timeScale == 0f) return; 

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.J)) 
            {
                PerformAttack();
                nextAttackTime = Time.time + 1f / currentWeapon.attackRate;
            }
        }
    }

    void PerformAttack()
{
    if (anim != null) anim.SetTrigger("Attack");

    if (slashSounds.Length > 0 && AudioManager.Instance != null)
        {
            int randomIndex = Random.Range(0, slashSounds.Length);
            AudioManager.Instance.PlaySFX(slashSounds[randomIndex]);
        }

    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, currentWeapon.range, enemyLayers);
    bool hasHitSomeone = false;

    foreach (Collider2D hitObj in hitObjects)
    {
        IDamageable damageableTarget = hitObj.GetComponent<IDamageable>();

        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(currentWeapon.damage);
            hasHitSomeone = true;
        }
    }

    if (hasHitSomeone)
    {
        if (impulseSource != null) impulseSource.GenerateImpulseWithForce(0.5f);
        StartCoroutine(Hitstop(0.05f)); 
    }
}

    private IEnumerator Hitstop(float duration)
    {
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(duration); 
        Time.timeScale = 1f; 
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.white;
        if (currentWeapon != null) Gizmos.DrawWireSphere(attackPoint.position, currentWeapon.range);
    }
}