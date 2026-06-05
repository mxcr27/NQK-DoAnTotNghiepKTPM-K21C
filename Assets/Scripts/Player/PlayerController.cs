using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    private const string AXIS_HORIZONTAL = "Horizontal";
    private const string BUTTON_JUMP = "Jump";

    [Header("Jump Settings")]
    public float coyoteTime = 0.2f;    
    private float coyoteTimeCounter;   

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Dash Settings")]
    public float dashSpeed = 24f;      
    public float dashTime = 0.2f;      
    public float dashCooldown = 0.5f;

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.2f;
    public bool isKnockedBack { get; private set; }    

    [Header("Ghost Trail")]
    public GameObject ghostPrefab;
    public float ghostSpawnInterval = 0.03f;
    private float lastGhostSpawnTime;
    private SpriteRenderer playerSR;

    [Header("Âm thanh")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    
    private bool canDash = true;       
    private bool isDashing;            
    private float facingDirection = 1f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float rayLength = 0.02f;

    private CapsuleCollider2D capsuleCol; 
    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private float originalGravity;     

    private Vector2 groundCheckSize;
    private PhysicsMaterial2D autoFrictionMat;
    private bool isAttacking;

    private WaitForSeconds dashDurationWait;
    private WaitForSeconds dashCooldownWait;

    private Animator anim;

    private PlatformEffector2D currentEffector;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCol = GetComponent<CapsuleCollider2D>(); 
        originalGravity = rb.gravityScale; 

        groundCheckSize = new Vector2(capsuleCol.bounds.size.x - 0.05f, 0.05f);
        
        autoFrictionMat = new PhysicsMaterial2D("AutoFriction");
        capsuleCol.sharedMaterial = autoFrictionMat;

        dashDurationWait = new WaitForSeconds(dashTime);
        dashCooldownWait = new WaitForSeconds(dashCooldown);

        anim = GetComponentInChildren<Animator>();

        playerSR = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) 
        {
            horizontalInput = 0f; 
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); 
            return;
        }

        if (isDashing) 
        {
            if (Time.time >= lastGhostSpawnTime + ghostSpawnInterval)
            {
                SpawnGhost();
                lastGhostSpawnTime = Time.time;
            }
            return;
        }

        if (anim != null)
        {
            isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack");
        }

        horizontalInput = Input.GetAxisRaw(AXIS_HORIZONTAL);
        
        if (anim != null) 
        {
            anim.SetBool("isWalking", horizontalInput != 0);
        }
        
        if (!isAttacking && horizontalInput != 0)
        {
            facingDirection = Mathf.Sign(horizontalInput);
            if (transform.localScale.x != facingDirection)
            {
                transform.localScale = new Vector3(facingDirection, 1f, 1f);
            }
        }

        CheckGrounded();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown(BUTTON_JUMP) && !isKnockedBack)
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && currentEffector != null)
            {
                StartCoroutine(DropDownThroughPlatform());
            }
            else if (coyoteTimeCounter > 0f) 
            {
                if (AudioManager.Instance != null) 
                {
                    AudioManager.Instance.PlaySFX(jumpSound);
                }

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); 
                
                coyoteTimeCounter = 0f;  
            }
        }
        
        if (Input.GetButtonUp(BUTTON_JUMP) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashRoutine());
        }

        if (anim != null)
        {
            anim.SetBool("isGrounded", isGrounded);
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (isKnockedBack)
        {
            if (autoFrictionMat.friction != 0f) 
            {
                autoFrictionMat.friction = 0f;
                capsuleCol.sharedMaterial = autoFrictionMat;
            }
            return; 
        }

        if (isGrounded && horizontalInput == 0f)
        {
            if (autoFrictionMat.friction != 10f) autoFrictionMat.friction = 10f; 
        }
        else
        {
            if (autoFrictionMat.friction != 0f) autoFrictionMat.friction = 0f;  
        }
        
        capsuleCol.sharedMaterial = autoFrictionMat; 

        float moveInput = horizontalInput;

        if (isAttacking)
        {
            if (moveInput != 0 && Mathf.Sign(moveInput) != facingDirection)
            {
                moveInput = 0f;
            }
        }

        float currentSpeed = isAttacking ? (moveSpeed * 0.3f) : moveSpeed;
        
        if (moveInput == 0 && !isGrounded)
        {
            float smoothX = Mathf.Lerp(rb.linearVelocity.x, 0f, 5f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
    }

    private IEnumerator DashRoutine()
    {

        if (AudioManager.Instance != null && dashSound != null)
        {
            AudioManager.Instance.PlaySFX(dashSound);
        }
        
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0f; 
        
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

        yield return dashDurationWait;

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return dashCooldownWait;
        canDash = true; 
    }

    private void CheckGrounded()
    {
        Vector2 boxOrigin = new Vector2(capsuleCol.bounds.center.x, capsuleCol.bounds.min.y + (groundCheckSize.y / 2f));
        
        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, groundCheckSize, 0f, Vector2.down, rayLength, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlatformEffector2D effector))
        {
            currentEffector = effector;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlatformEffector2D effector))
        {
            if (currentEffector == effector) 
            {
                currentEffector = null;
            }
        }
    }

    private IEnumerator DropDownThroughPlatform()
{
    PlatformEffector2D platformToReset = currentEffector;
    
    platformToReset.rotationalOffset = 180f;
    
    yield return new WaitForSeconds(0.3f);
    
    if (platformToReset != null)
    {
        platformToReset.rotationalOffset = 0f;
    }
}   

    private void OnDrawGizmos()
    {
        if (capsuleCol == null) capsuleCol = GetComponent<CapsuleCollider2D>();
        if (capsuleCol == null) return;
        
        Gizmos.color = Color.red;
        Vector2 boxSize = new Vector2(capsuleCol.bounds.size.x - 0.05f, 0.05f);
        
        Vector2 boxOrigin = new Vector2(capsuleCol.bounds.center.x, capsuleCol.bounds.min.y + (boxSize.y / 2f));
        
        Gizmos.DrawWireCube(boxOrigin + Vector2.down * rayLength, boxSize);
    }

    private void SpawnGhost()
    {
        if (ghostPrefab == null || playerSR == null) return;

        GameObject ghost = Instantiate(ghostPrefab, playerSR.transform.position, playerSR.transform.rotation);
        
        SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
        ghostSR.sprite = playerSR.sprite;
        
        ghost.transform.localScale = transform.localScale; 
    }

    public void ApplyKnockback(Transform damageSource, float knockbackForce)
    {
        StartCoroutine(KnockbackRoutine(damageSource, knockbackForce));
    }

    private IEnumerator KnockbackRoutine(Transform damageSource, float knockbackForce)
    {
        isKnockedBack = true;
        
        rb.linearVelocity = Vector2.zero;
        
        int direction = 1;
        if (damageSource != null)
        {
            direction = transform.position.x < damageSource.position.x ? -1 : 1;
        }
        else
        {
            direction = transform.localScale.x > 0 ? -1 : 1;
        }

        Vector2 force = new Vector2(direction * knockbackForce, knockbackForce * 0.5f);
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        
        isKnockedBack = false;
    }
}