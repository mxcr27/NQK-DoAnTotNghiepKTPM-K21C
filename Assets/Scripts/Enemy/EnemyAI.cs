using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Wait, Investigate, Chase, Attack }
    
    [Header("Trạng thái FSM")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Tuần tra")]
    public float speed = 2f;
    public float patrolDistance = 5f; 
    public enum StartDirection { Left, Right } 
    public StartDirection startingDirection = StartDirection.Left;

    [Header("Raycast")]
    public Transform edgeCheck;          
    public float edgeCheckDistance = 1f; 
    public float wallCheckDistance = 0.2f;
    public LayerMask groundLayer;        
    
    [Header("Idle")]
    public float waitTime = 2.0f; 
    private float waitCounter;    

    [Header("Cài đặt Tấn công & Rượt đuổi")]
    public Transform player;          
    public float chaseRange = 6f;     
    public float detectionHeight = 2f;
    public float chaseSpeed = 3.5f;   
    public float attackRange = 1.5f;  
    public float attackCooldown = 2f;
    
    [Header("Cài đặt Mất dấu")]
    public float investigateTime = 1f; 

    [Header("Sát thương")]
    public Transform attackPoint;     
    public float attackRadius = 0.5f; 
    public LayerMask playerLayer;     
    public int damageAmount = 20;     

    private float cooldownTimer;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 startPosition; 
    private float targetVelocityX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = Random.Range(1, 100); 

        startPosition = transform.position;

        if (startingDirection == StartDirection.Left) Flip(-1);
        else Flip(1);
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (currentState == EnemyState.Attack)
        {
            targetVelocityX = 0f;
            return;
        }

        bool canMoveForward = true;
        if (edgeCheck != null)
        {
            bool groundAhead = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDistance, groundLayer);
            float facingDir = Mathf.Sign(transform.localScale.x);
            bool wallAhead = Physics2D.Raycast(edgeCheck.position, new Vector2(facingDir, 0), wallCheckDistance, groundLayer);
            canMoveForward = groundAhead && !wallAhead;
        }

        if (player != null)
        {
            float distanceX = player.position.x - transform.position.x;
            float absDistanceX = Mathf.Abs(distanceX);
            float distanceY = Mathf.Abs(player.position.y - transform.position.y);

            float facingDirection = Mathf.Sign(transform.localScale.x);
            float directionToPlayer = Mathf.Sign(distanceX);

            bool isInVisionBox = (absDistanceX <= chaseRange) && (distanceY <= detectionHeight) && (facingDirection == directionToPlayer);
            
            bool isCloseBehind = (absDistanceX <= attackRange + 1f) && (distanceY <= detectionHeight);

            if ((isInVisionBox || isCloseBehind) && currentState != EnemyState.Chase)
            {
                currentState = EnemyState.Chase;
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolRoutine(canMoveForward);
                break;
            case EnemyState.Wait:
                WaitAtPoint();
                break;
            case EnemyState.Investigate:
                InvestigateRoutine(canMoveForward);
                break;
            case EnemyState.Chase:
                ExecuteChaseLogic(canMoveForward);
                break;
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    void PatrolRoutine(bool canMove)
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);
        bool reachedLimit = false;
        
        if (facingDirection > 0 && transform.position.x >= startPosition.x + patrolDistance) reachedLimit = true;
        if (facingDirection < 0 && transform.position.x <= startPosition.x - patrolDistance) reachedLimit = true;

        if (canMove && !reachedLimit) 
        {
            targetVelocityX = facingDirection * speed;
        }
        else
        {
            targetVelocityX = 0f;
            waitCounter = waitTime;
            currentState = EnemyState.Wait;
        }
    }

    void WaitAtPoint()
    {
        targetVelocityX = 0f;
        waitCounter -= Time.deltaTime;
        if (waitCounter <= 0)
        {
            float facingDirection = Mathf.Sign(transform.localScale.x);
            Flip(-facingDirection); 
            currentState = EnemyState.Patrol;
        }
    }

    void ExecuteChaseLogic(bool canMove)
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceX = player.position.x - transform.position.x;
        float directionToPlayer = Mathf.Sign(distanceX);

        if (distanceToPlayer > chaseRange)
        {
            float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
            if (Mathf.Abs(player.position.x - transform.position.x) > 0.1f) 
            {
                Flip(dirToPlayer);
            }

            waitCounter = investigateTime;  
            currentState = EnemyState.Investigate;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            targetVelocityX = 0f;
            if (anim != null) anim.SetBool("isWalking", false);
            
            if (Mathf.Abs(distanceX) > 0.1f) Flip(directionToPlayer);

            if (cooldownTimer <= 0)
            {
                currentState = EnemyState.Attack;
                if (anim != null) anim.SetTrigger("Attack"); 
                cooldownTimer = attackCooldown; 
            }
            return;
        }

        if (Mathf.Abs(distanceX) > 0.1f) 
        {
            targetVelocityX = canMove ? directionToPlayer * chaseSpeed : 0f;
            Flip(directionToPlayer); 
        }
        else 
        {
            targetVelocityX = 0f;
        }
    }

    void InvestigateRoutine(bool canMove)
    {
        waitCounter -= Time.deltaTime;
        float facingDirection = Mathf.Sign(transform.localScale.x);
        
        targetVelocityX = canMove ? facingDirection * chaseSpeed : 0f;

        if (waitCounter <= 0)
        {
            targetVelocityX = 0f;
            waitCounter = waitTime; 
            currentState = EnemyState.Wait;
        }
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider2D p in hitPlayers)
        {
            PlayerHealth health = p.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damageAmount, transform);
        }
    }

    public void ResetAttack() 
    { 
        currentState = EnemyState.Chase; 
    }

    void UpdateAnimation()
    {
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(targetVelocityX) > 0.1f;
            anim.SetBool("isWalking", isMoving);
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

        if (edgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * edgeCheckDistance);
            float facingDir = transform.localScale.x > 0 ? 1f : -1f;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + new Vector3(facingDir * wallCheckDistance, 0, 0));
        }

        if (!Application.isPlaying) 
        {
            Gizmos.color = Color.yellow;
            Vector2 lPoint = new Vector2(transform.position.x - patrolDistance, transform.position.y);
            Vector2 rPoint = new Vector2(transform.position.x + patrolDistance, transform.position.y);
            Gizmos.DrawLine(lPoint, rPoint);
            Gizmos.DrawWireSphere(lPoint, 0.2f);
            Gizmos.DrawWireSphere(rPoint, 0.2f);
        }

        Gizmos.color = Color.green;
        float currentFacing = transform.localScale.x > 0 ? 1f : -1f;
        Vector3 sightCenter = transform.position + new Vector3(currentFacing * (chaseRange / 2f), 0, 0);
        Vector3 sightSize = new Vector3(chaseRange, detectionHeight * 2f, 0);
        Gizmos.DrawWireCube(sightCenter, sightSize);
    }

    private void OnEnable()
    {
        currentState = EnemyState.Patrol; 

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            targetVelocityX = 0f;
        }

        if (anim != null)
        {
            anim.Rebind(); 
            anim.Update(0f);     
            anim.Play("Idle");   
        }
    }
}