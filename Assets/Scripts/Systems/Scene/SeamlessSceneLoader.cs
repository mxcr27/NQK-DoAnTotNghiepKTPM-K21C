using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SeamlessSceneLoader : MonoBehaviour
{
    [Header("Cài đặt Chuyển cảnh")]
    public string sceneToLoad;
    
    [Header("Định vị Vị trí")]
    [Tooltip("ID của cửa bên kia (VD: Cua_HangDong_1)")]
    public string targetSpawnID; 

    private bool isTransitioning = false; 

    private float safeCooldown = 0.5f;

    void Update()
    {
        if (safeCooldown > 0)
        {
            safeCooldown -= Time.deltaTime;
        }
    }

    public void StartPreloadingScene()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning && safeCooldown <= 0f)
        {
            isTransitioning = true;
            StartCoroutine(PerformTransition());
        }
    }

    private IEnumerator PerformTransition()
    {
        PlayerPrefs.SetString("TransitionSpawnID", targetSpawnID);
        PlayerPrefs.Save();

        if (GameDataManager.Instance != null)
        {
            PlayerData tempData = GameDataManager.Instance.CapturePlayerData();
            tempData.savedScene = sceneToLoad; 
            SaveSystem.SaveGame(tempData);
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = true;

        yield return op;
    }
}