using UnityEngine;

public class BossArenaManager : MonoBehaviour
{
    [Header("Định danh Sự kiện (BẮT BUỘC DUY NHẤT)")]
    public string bossEventID = "Boss1_Defeated"; 

    [Header("Cửa khóa Arena")]
    public GameObject leftDoor;
    public GameObject rightDoor;

    [Header("Tham chiếu")]
    public BossAI1 boss;
    public PlayerHealth playerHealth; 

    private bool arenaActivated = false;
    private bool waitingForRespawn = false; 

    private void Start()
    {
        if (!string.IsNullOrEmpty(bossEventID) && GameDataManager.Instance != null && GameDataManager.Instance.worldFlags.Contains(bossEventID))
        {
            if (boss != null) Destroy(boss.gameObject); 
            if (leftDoor != null) leftDoor.SetActive(false);
            if (rightDoor != null) rightDoor.SetActive(false);
            
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            
            this.enabled = false; 
            return; 
        }

        if (leftDoor != null) leftDoor.SetActive(false);
        if (rightDoor != null) rightDoor.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!arenaActivated && collision.CompareTag("Player"))
        {
            if (playerHealth != null && !playerHealth.isDead)
            {
                ActivateArena();
            }
        }
    }

    void ActivateArena()
    {
        arenaActivated = true;
        waitingForRespawn = false; 
        
        if (leftDoor != null) leftDoor.SetActive(true);
        if (rightDoor != null) rightDoor.SetActive(true);

        if (boss != null)
        {
            boss.currentState = BossAI1.BossState.Chase;
        }
        if (FastTravelUIManager.Instance != null)
        {
            FastTravelUIManager.Instance.SetFastTravelLockedState(true);
        }
        GetComponent<BossBGMController>().StartBossMusic();
    }

    void Update()
    {
        if (!arenaActivated) return;

        if (boss != null && boss.currentState == BossAI1.BossState.Dead)
        {
            OpenArena();
            return; 
        }

        if (playerHealth != null)
        {
            if (playerHealth.isDead && !waitingForRespawn)
            {
                waitingForRespawn = true; 
                
                if (boss != null) 
                {
                    boss.currentState = BossAI1.BossState.Idle;
                    boss.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                }
            }
            else if (!playerHealth.isDead && waitingForRespawn)
            {
                ResetArena(); 
            }
        }
    }

    void OpenArena()
    {
        if (leftDoor != null) leftDoor.SetActive(false);
        if (rightDoor != null) rightDoor.SetActive(false);
        
        if (boss != null)
        {
            EnemyLoot bossLoot = boss.GetComponent<EnemyLoot>();
            if (bossLoot != null)
            {
                bossLoot.DropLoot();
            }
        }
        
        if (!string.IsNullOrEmpty(bossEventID) && GameDataManager.Instance != null)
        {
            if (!GameDataManager.Instance.worldFlags.Contains(bossEventID))
            {
                GameDataManager.Instance.worldFlags.Add(bossEventID);
            }
        }

        if (FastTravelUIManager.Instance != null)
        {
            FastTravelUIManager.Instance.SetFastTravelLockedState(false);
        }

        GetComponent<BossBGMController>().StopBossMusic();
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 
    }

    void ResetArena()
    {
        arenaActivated = false; 
        waitingForRespawn = false; 

        if (leftDoor != null) leftDoor.SetActive(false);
        if (rightDoor != null) rightDoor.SetActive(false);

        if (boss != null)
        {
            boss.ResetBoss();
        }

        if (FastTravelUIManager.Instance != null)
        {
            FastTravelUIManager.Instance.SetFastTravelLockedState(false);
        }

        GetComponent<BossBGMController>().StopBossMusic();
    }
}