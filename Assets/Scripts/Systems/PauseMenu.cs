using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; 

    [Header("UI Panels")]
    public GameObject pauseMenuUI;   
    public GameObject optionsMenuUI; 
    public GameObject confirmExitPanel; 
    public GameObject gameOverPanel;

    [Header("Liên Kết UI Khác")]
    public InventoryUI inventoryScript; 

    void Update()
    {
        if (gameOverPanel != null && gameOverPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (TooltipManager.Instance != null)
            {
                TooltipManager.Instance.HideTooltip();
            }

            if (confirmExitPanel != null && confirmExitPanel.activeSelf)
            {
                ConfirmExitNo();
                return;
            }

            if (optionsMenuUI != null && optionsMenuUI.activeSelf)
            {
                CloseOptions();
                return; 
            }

            if (inventoryScript != null && inventoryScript.inventoryPanel.activeSelf)
            {
                inventoryScript.CloseInventory(); 
                return; 
            }

            if (ShopManager.Instance != null && ShopManager.Instance.gameObject.activeSelf)
            {
                ShopManager.Instance.CloseShop();
                return;
            }

            MenuTabManager tabManager = Object.FindFirstObjectByType<MenuTabManager>();
            if (tabManager != null && tabManager.masterMenuPanel.activeSelf)
            {
                tabManager.masterMenuPanel.SetActive(false);
                Time.timeScale = 1f; 
                return; 
            }

            if (FastTravelUIManager.Instance != null && FastTravelUIManager.Instance.fastTravelPanel.activeSelf)
            {
                FastTravelUIManager.Instance.CloseMenu();
                return; 
            }

            if (GameIsPaused) 
            {
                Resume(); 
            }
            else 
            {
                Pause(); 
            }  
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (optionsMenuUI != null) optionsMenuUI.SetActive(false); 
        if (confirmExitPanel != null) confirmExitPanel.SetActive(false);
        
        Time.timeScale = 1f; 
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
        GameIsPaused = true;
    }

    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false); 
        optionsMenuUI.SetActive(true); 
    }

    public void CloseOptions()
    {
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void ClickExitButton()
    {
        pauseMenuUI.SetActive(false);
        confirmExitPanel.SetActive(true);
    }

  public void ConfirmExitYes()
{
    if (GameDataManager.Instance != null)
    {
        PlayerData data = GameDataManager.Instance.CapturePlayerData();
        SaveSystem.SaveGame(data);
    }

    Time.timeScale = 1f; 
    GameIsPaused = false;
    SceneManager.LoadScene("MainMenu"); 
}

    public void ConfirmExitNo()
    {
        confirmExitPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}