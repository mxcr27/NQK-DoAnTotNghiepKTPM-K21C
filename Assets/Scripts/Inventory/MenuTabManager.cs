using UnityEngine;
using UnityEngine.UI;

public class MenuTabManager : MonoBehaviour
{
    [Header("Giao Diện")]
    public GameObject masterMenuPanel;

    [Header("Nội Dung Tab")]
    public GameObject skillPanel;
    public GameObject inventoryPanel;

    [Header("Hình ảnh của các Nút Tab")]
    public Image inventoryTabImg;
    public Image skillTabImg;

    [Header("Cấu hình màu")]
    public Color activeColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color inactiveColor = Color.white;

    [Header("Skill UI")]
    public SkillUI skillUI;
    public InventoryUI inventoryUI;

    void Start()
    {
        masterMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (PauseMenu.GameIsPaused) return;
        
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.B))
        {
            bool isActive = !masterMenuPanel.activeSelf;
            masterMenuPanel.SetActive(isActive);
            Time.timeScale = isActive ? 0 : 1;

            if (isActive)
            {
                OpenWeaponTab(); 
            }
            else 
            {
                if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && masterMenuPanel.activeSelf)
        {
            masterMenuPanel.SetActive(false);
            Time.timeScale = 1;
            if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
        }
    }

    public void OpenSkillTab()
    {
        skillPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        
        UpdateTabColors(false);

        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
        if (skillUI != null) skillUI.UpdateSkillUI(); 
    }

    public void OpenWeaponTab()
    {
        skillPanel.SetActive(false);
        inventoryPanel.SetActive(true);
        
        UpdateTabColors(true);

        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
        if (inventoryUI != null) inventoryUI.UpdateUI();
    }

    private void UpdateTabColors(bool isInventory)
    {
        if (inventoryTabImg != null && skillTabImg != null)
        {
            inventoryTabImg.color = isInventory ? activeColor : inactiveColor;
            skillTabImg.color = isInventory ? inactiveColor : activeColor;
        }
    }
}