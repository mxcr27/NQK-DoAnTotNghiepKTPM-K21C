using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FastTravelUIManager : MonoBehaviour
{
    public static FastTravelUIManager Instance;

    [Header("UI Elements")]
    public GameObject fastTravelPanel;    
    public Transform buttonContainer;     
    public GameObject waypointButtonPrefab; 

    [Header("UI Thông báo")]
    public TextMeshProUGUI warningMessageText;

    [Header("Combat & Balance")]
    public float dangerRadius = 12f;       
    public string enemyTag = "Enemy";      
    public string bossTag = "Boss";        

    private GameObject currentPlayer;
    private bool isFastTravelForceDisabled = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (fastTravelPanel != null) fastTravelPanel.SetActive(false); 
    }

    void Update()
    {
        if (PauseMenu.GameIsPaused) return;
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isFastTravelForceDisabled)
            {
                ShowWarningMessage("Không thể mở bản đồ trong khu vực đấu Boss!");
                return;
            }

            if (fastTravelPanel.activeSelf) 
            {
                CloseMenu();
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) 
                {
                    if (IsDangerNearby(player.transform.position))
                    {
                        ShowWarningMessage("Không thể Fast Travel ngay lúc này!");
                        return; 
                    }
                    ShowMenu(player);
                }
                else 
                {
                    Debug.LogWarning("Không tìm thấy Player!");
                }
            }
        }
    }

    public void SetFastTravelLockedState(bool isLocked)
    {
        isFastTravelForceDisabled = isLocked;
        
        if (isLocked && fastTravelPanel != null && fastTravelPanel.activeSelf)
        {
            CloseMenu();
        }
    }

    private bool IsDangerNearby(Vector2 playerPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerPosition, dangerRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(enemyTag) || col.CompareTag(bossTag))
            {
                return true; 
            }
        }
        return false; 
    }

    public void ShowMenu(GameObject player)
    {
        currentPlayer = player;
        fastTravelPanel.SetActive(true);
        Time.timeScale = 0f; 

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        if (GameDataManager.Instance != null && GameDataManager.Instance.savedWaypoints != null)
        {
            foreach (WaypointSaveData wp in GameDataManager.Instance.savedWaypoints)
            {
                GameObject btnObj = Instantiate(waypointButtonPrefab, buttonContainer);
                
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>(); 
                if (btnText != null) btnText.text = wp.waypointID.Replace("WP_", "");

                Button btn = btnObj.GetComponent<Button>();
                string targetID = wp.waypointID; 
                string targetScene = wp.sceneName; 

                btn.onClick.AddListener(() => TeleportTo(targetID, targetScene));
            }
        }
    }

    public void CloseMenu()
    {
        fastTravelPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    private void TeleportTo(string targetID, string targetScene)
    {
        CloseMenu(); 

        if (targetScene == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            Waypoint[] allWaypoints = Object.FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
            foreach (var wp in allWaypoints)
            {
                if (wp.waypointID == targetID)
                {
                    currentPlayer.transform.position = wp.transform.position;
                    PlayerHealth health = currentPlayer.GetComponent<PlayerHealth>();
                    if (health != null) health.checkpointPosition = wp.transform.position;
                    return;
                }
            }
        }
        else 
        {            
            PlayerPrefs.SetString("TransitionSpawnID", targetID);
            PlayerPrefs.Save();

            if (GameDataManager.Instance != null)
            {
                PlayerData tempData = GameDataManager.Instance.CapturePlayerData();
                tempData.savedScene = targetScene; 
                SaveSystem.SaveGame(tempData);
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
        }
    }

    private Coroutine warningCoroutine;

    public void ShowWarningMessage(string message)
    {
        if (warningMessageText == null) return;

        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningRoutine(message));
    }

    private System.Collections.IEnumerator WarningRoutine(string message)
    {
        warningMessageText.text = message;
        
        warningMessageText.gameObject.SetActive(true); 
        
        yield return new WaitForSecondsRealtime(2f); 

        warningMessageText.gameObject.SetActive(false); 
    }
}