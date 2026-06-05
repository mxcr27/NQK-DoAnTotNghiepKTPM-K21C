using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Định danh")]
    public string waypointID;

    [Tooltip("Tên Scene chứa Waypoint (Ví dụ: Area1_F)")]
    public string sceneName;

    [Header("Trạng thái")]
    public bool isActivated = false;
    
    [Header("Hình ảnh")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    [Header("Hồi máu")]
    public bool canHeal = true;
    public float healInterval = 0.5f;
    public int healAmount = 5;
    
    private float healTimer = 0f;
    private PlayerHealth currentPlayerHealth; 
    private bool isPlayerInRange = false;

    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        if (sr != null && inactiveSprite != null)
        {
            sr.sprite = inactiveSprite;
        }

        if (GameDataManager.Instance != null && GameDataManager.Instance.worldFlags.Contains(waypointID))
        {
            SetAsActivated();
        }
    }

    void Update()
    {
        if (isActivated && canHeal && isPlayerInRange && currentPlayerHealth != null)
        {
            if (currentPlayerHealth.currentHealth < currentPlayerHealth.maxHealth)
            {
                healTimer += Time.deltaTime;

                if (healTimer >= healInterval)
                {
                    healTimer = 0f;
                    
                    currentPlayerHealth.Heal(healAmount);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentPlayerHealth = collision.GetComponent<PlayerHealth>();
            Activate(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            currentPlayerHealth = null;
            healTimer = 0f;
        }
    }

    void Activate(GameObject player)
    {
        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.checkpointPosition = transform.position;
        }

        if (!isActivated)
        {
            SetAsActivated();
        }

        if (GameDataManager.Instance != null)
        {
            bool requireSave = false;

            if (!GameDataManager.Instance.worldFlags.Contains(waypointID))
            {
                GameDataManager.Instance.worldFlags.Add(waypointID);
                requireSave = true;
            }
            int index = GameDataManager.Instance.savedWaypoints.FindIndex(wp => wp.waypointID == this.waypointID);
            
            if (index == -1) 
            {
                WaypointSaveData wpData = new WaypointSaveData { waypointID = this.waypointID, sceneName = this.sceneName };
                GameDataManager.Instance.savedWaypoints.Add(wpData);
                requireSave = true;
            }
            else 
            {
                if (GameDataManager.Instance.savedWaypoints[index].sceneName != this.sceneName)
                {
                    WaypointSaveData updatedData = GameDataManager.Instance.savedWaypoints[index];
                    updatedData.sceneName = this.sceneName; 
                    GameDataManager.Instance.savedWaypoints[index] = updatedData;
                    requireSave = true; 
                }
            }

            if (requireSave)
            {
                PlayerData dataToSave = GameDataManager.Instance.CapturePlayerData();
                dataToSave.positionX = transform.position.x;
                dataToSave.positionY = transform.position.y;

                SaveSystem.SaveGame(dataToSave);
            }
        }
    }

    void SetAsActivated()
    {
        isActivated = true;
        
        if (sr != null && activeSprite != null)
        {
            sr.sprite = activeSprite;
        }

        if (anim != null) anim.SetTrigger("Activate");
    }
}