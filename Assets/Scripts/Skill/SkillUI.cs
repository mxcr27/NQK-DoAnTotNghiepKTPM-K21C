using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems; 

public class SkillUI : MonoBehaviour
{
    [Header("Bảng Danh Sách Chiêu")]
    public GameObject skillPanel;
    public Transform contentParent;
    public GameObject slotPrefab;

    [Header("Bảng Túi Đồ")]
    public GameObject inventoryPanel;

    [Header("Ô Kỹ Năng Đang Dùng (Ngoài HUD)")]
    public Image hudIconK;
    public Image hudIconL;

    [Header("Ô Kỹ Năng Đang Dùng (Trong Inventory)")]
    public Image menuIconK;
    public Image menuIconL;

    [Header("Cấu hình Hiển thị")]
    public Color costTextColor = Color.yellow;

    void Start()
    {
        if (PlayerSkillManager.Instance != null)
        {
            if (PlayerSkillManager.Instance.skillSlot1 != null)
            {
                if (hudIconK != null) { hudIconK.sprite = PlayerSkillManager.Instance.skillSlot1.icon; hudIconK.color = Color.white; }
                if (menuIconK != null) { menuIconK.sprite = PlayerSkillManager.Instance.skillSlot1.icon; menuIconK.color = Color.white; }
            }
            else 
            {
                if (hudIconK != null) { hudIconK.sprite = null; hudIconK.color = Color.clear; }
                if (menuIconK != null) { menuIconK.sprite = null; menuIconK.color = Color.clear; }
            }

            if (PlayerSkillManager.Instance.skillSlot2 != null)
            {
                if (hudIconL != null) { hudIconL.sprite = PlayerSkillManager.Instance.skillSlot2.icon; hudIconL.color = Color.white; }
                if (menuIconL != null) { menuIconL.sprite = PlayerSkillManager.Instance.skillSlot2.icon; menuIconL.color = Color.white; }
            }
            else 
            {
                if (hudIconL != null) { hudIconL.sprite = null; hudIconL.color = Color.clear; }
                if (menuIconL != null) { menuIconL.sprite = null; menuIconL.color = Color.clear; }
            }

            SetupRightClickUnequip(menuIconK, 1);
            SetupRightClickUnequip(menuIconL, 2);
        }
    }

    public void UpdateSkillUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (PlayerSkillManager.Instance == null) return;

        foreach (SkillData skill in PlayerSkillManager.Instance.allGameSkills)
        {
            GameObject newSlot = Instantiate(slotPrefab, contentParent);

            Image icon = newSlot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = skill.icon;

            Transform lockOverlay = newSlot.transform.Find("Lock_Overlay"); 
            
            bool isOwned = PlayerSkillManager.Instance.unlockedSkills.Contains(skill);

            if (isOwned)
            {
                icon.color = Color.white; 
                if (lockOverlay != null) lockOverlay.gameObject.SetActive(false); 
            }
            else
            {
                icon.color = new Color(0.3f, 0.3f, 0.3f, 1f); 
                if (lockOverlay != null) lockOverlay.gameObject.SetActive(true);  
            }

            var amountText = newSlot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (amountText != null) amountText.text = "";

            EventTrigger trigger = newSlot.GetComponent<EventTrigger>();
            if (trigger == null) trigger = newSlot.AddComponent<EventTrigger>();

            EventTrigger.Entry clickEvent = new EventTrigger.Entry();
            clickEvent.eventID = EventTriggerType.PointerClick;
            clickEvent.callback.AddListener((data) => {
                PointerEventData pointerData = (PointerEventData)data;
                
                if (isOwned) 
                {
                    if (pointerData.button == PointerEventData.InputButton.Left) EquipToSlotK(skill);
                    else if (pointerData.button == PointerEventData.InputButton.Right) EquipToSlotL(skill);
                }
                else 
                {
                    if (pointerData.button == PointerEventData.InputButton.Left) TryPurchaseSkill(skill);
                }
            });
            trigger.triggers.Add(clickEvent);

            EventTrigger.Entry enterEvent = new EventTrigger.Entry();
            enterEvent.eventID = EventTriggerType.PointerEnter;
            enterEvent.callback.AddListener((data) => {
                if (TooltipManager.Instance != null)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(costTextColor);
                    string costText = isOwned ? "" : $"\n<color=#{hexColor}>Giá: {skill.goldCost} Vàng</color>";
                    
                    TooltipManager.Instance.ShowTooltip(skill.skillName, skill.description + costText);
                }
            });
            trigger.triggers.Add(enterEvent);

            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;
            exitEvent.callback.AddListener((data) => {
                if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip(); 
            });
            trigger.triggers.Add(exitEvent);
        }
    }

    private void TryPurchaseSkill(SkillData skill)
    {
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.DeductGold(skill.goldCost))
        {
            PlayerSkillManager.Instance.unlockedSkills.Add(skill);
            UpdateSkillUI(); 
        }
        else
        {
            Debug.LogWarning("Bạn không có đủ Vàng để mua chiêu này!");
        }
    }

    void EquipToSlotK(SkillData skill)
    {
        bool isSuccess = PlayerSkillManager.Instance.EquipSkill(skill, 1);
        if (isSuccess) 
        {
            if (hudIconK != null) { hudIconK.sprite = skill.icon; hudIconK.color = Color.white; }
            if (menuIconK != null) { menuIconK.sprite = skill.icon; menuIconK.color = Color.white; }
        }
    }

    void EquipToSlotL(SkillData skill)
    {
        bool isSuccess = PlayerSkillManager.Instance.EquipSkill(skill, 2); 
        if (isSuccess) 
        {
            if (hudIconL != null) { hudIconL.sprite = skill.icon; hudIconL.color = Color.white; }
            if (menuIconL != null) { menuIconL.sprite = skill.icon; menuIconL.color = Color.white; }
        }
    }

    public void OnClickUnequipSlotK()
    {
        if (PlayerSkillManager.Instance != null)
        {
            PlayerSkillManager.Instance.UnequipSkill(1);
            if (hudIconK != null) { hudIconK.sprite = null; hudIconK.color = Color.clear; }
            if (menuIconK != null) { menuIconK.sprite = null; menuIconK.color = Color.clear; } 
        }
    }

    public void OnClickUnequipSlotL()
    {
        if (PlayerSkillManager.Instance != null)
        {
            PlayerSkillManager.Instance.UnequipSkill(2);
            if (hudIconL != null) { hudIconL.sprite = null; hudIconL.color = Color.clear; }
            if (menuIconL != null) { menuIconL.sprite = null; menuIconL.color = Color.clear; } 
        }
    }

    private void SetupRightClickUnequip(Image iconRef, int slotIndex)
    {
        if (iconRef == null) return;

        GameObject slotObj = iconRef.transform.parent.gameObject; 

        EventTrigger trigger = slotObj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = slotObj.AddComponent<EventTrigger>();

        EventTrigger.Entry clickEvent = new EventTrigger.Entry();
        clickEvent.eventID = EventTriggerType.PointerClick;
        clickEvent.callback.AddListener((data) => {
            PointerEventData pointerData = (PointerEventData)data;
            
            if (pointerData.button == PointerEventData.InputButton.Right) 
            {
                if (slotIndex == 1) OnClickUnequipSlotK();
                else if (slotIndex == 2) OnClickUnequipSlotL();
            }
        });
        
        trigger.triggers.Add(clickEvent);
    }
}