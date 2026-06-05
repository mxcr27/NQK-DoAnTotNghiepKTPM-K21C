using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class InventoryUI : MonoBehaviour
{
    [Header("Bảng Túi Đồ")]
    public GameObject inventoryPanel; 
    public GameObject slotPrefab;     
    public Transform contentParent;   

    [Header("Bảng Kỹ Năng")]
    public GameObject skillPanel;     

    [Header("Ô Vũ Khí Đang Dùng")]
    public Image hudWeaponIcon;  
    public Image menuWeaponIcon; 

    void Start()
    {
        SyncWeaponUI();
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        if (skillPanel != null) skillPanel.SetActive(false);
        UpdateUI();
        Time.timeScale = 0;
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        
        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
        
        Time.timeScale = 1; 
    }

    public void UpdateUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var slot in PlayerInventory.Instance.inventorySlots)
        {
            if (slot.itemData.itemName == "Gold") continue;

            GameObject newSlot = Instantiate(slotPrefab, contentParent);

            Image icon = newSlot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = slot.itemData.icon;

            TextMeshProUGUI txt = newSlot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
            txt.text = slot.amount > 1 ? slot.amount.ToString() : ""; 

            Button btn = newSlot.GetComponent<Button>();
            if (btn == null) btn = newSlot.AddComponent<Button>();
            btn.onClick.AddListener(() => {
                OnItemClicked(slot);
            });

            EventTrigger trigger = newSlot.GetComponent<EventTrigger>();
            if (trigger == null) trigger = newSlot.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEvent = new EventTrigger.Entry();
            enterEvent.eventID = EventTriggerType.PointerEnter;
            enterEvent.callback.AddListener((data) => {
                if (TooltipManager.Instance != null)
                {
                    TooltipManager.Instance.ShowTooltip(slot.itemData.itemName, slot.itemData.description);
                }
            });
            trigger.triggers.Add(enterEvent);

            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;
            exitEvent.callback.AddListener((data) => {
                if (TooltipManager.Instance != null)
                {
                    TooltipManager.Instance.HideTooltip(); 
                }
            });
            trigger.triggers.Add(exitEvent);
        }
    }

    void OnItemClicked(InventorySlot clickedSlot)
    {
        if (clickedSlot.itemData is WeaponData)
        {
            WeaponData weaponToEquip = (WeaponData)clickedSlot.itemData;
            PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
            
            if (playerCombat != null)
            {
                if (playerCombat.currentWeapon != null)
                {
                    PlayerInventory.Instance.AddItem(playerCombat.currentWeapon, 1);
                }

                PlayerInventory.Instance.inventorySlots.Remove(clickedSlot);
                weaponToEquip.Use(playerCombat.gameObject);
                SyncWeaponUI();
                
                if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
                
                UpdateUI();
            }
        }
    }

    public void SyncWeaponUI()
    {
        PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
        if (playerCombat != null && playerCombat.currentWeapon != null)
        {
            if (hudWeaponIcon != null) { hudWeaponIcon.sprite = playerCombat.currentWeapon.icon; hudWeaponIcon.color = Color.white; }
            if (menuWeaponIcon != null) { menuWeaponIcon.sprite = playerCombat.currentWeapon.icon; menuWeaponIcon.color = Color.white; }
        }
    }
}