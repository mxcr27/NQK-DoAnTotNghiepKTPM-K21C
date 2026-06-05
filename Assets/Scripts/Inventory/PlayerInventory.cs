using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int amount;

    public InventorySlot(ItemData item, int amount)
    {
        this.itemData = item;
        this.amount = amount;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Dữ Liệu Tổng (Cho Save/Load)")]
    public List<ItemData> allGameItems = new List<ItemData>();

    [Header("Danh sách vật phẩm")]
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    [Header("Debug Settings")]
    public ItemData debugWeapon;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            if (debugWeapon != null)
            {
                AddItem(debugWeapon, 1);
            }
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        // =======================================================
        // TRƯỜNG HỢP 1: Vật phẩm CÓ THỂ cộng dồn (Vàng, Thuốc...)
        // =======================================================
        if (item.isStackable)
        {
            // 1a. Tìm xem trong túi có món này chưa để cộng dồn
            foreach (var slot in inventorySlots)
            {
                if (slot.itemData == item)
                {
                    slot.amount += amount;
                    NotifyUI(item, slot.amount);
                    return; // Xong việc thì thoát hàm luôn
                }
            }
            
            // 1b. Nếu chưa có ô nào (nhặt lần đầu), tạo ô mới
            inventorySlots.Add(new InventorySlot(item, amount));
            NotifyUI(item, amount);
        }
        // =======================================================
        // TRƯỜNG HỢP 2: Vật phẩm KHÔNG THỂ cộng dồn (Vũ Khí...)
        // =======================================================
        else
        {
            // Bắt buộc mỗi món đồ phải nằm ở một ô riêng biệt (amount = 1)
            // Nếu nhặt 1 lúc 4 cây kiếm, vòng lặp này sẽ chạy 4 lần để tạo 4 ô
            for (int i = 0; i < amount; i++)
            {
                inventorySlots.Add(new InventorySlot(item, 1));
                NotifyUI(item, 1);
            }
        }
    }

    private void NotifyUI(ItemData item, int currentAmount)
    {
        // Nếu là vàng thì báo cho HUD cập nhật con số
        if (item.itemName == "Gold" && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGoldUI(currentAmount);
        }
        
        Debug.Log($"[Inventory] Đã nhận: {item.itemName} x{currentAmount}");
    }

    public int GetTotalGold()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.itemData != null && slot.itemData.itemName == "Gold")
            {
                return slot.amount; 
            }
        }
        return 0; 
    }

    // Hàm mới để trừ vàng khi mua đồ
    public bool DeductGold(int amountToDeduct)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.itemData != null && slot.itemData.itemName == "Gold")
            {
                if (slot.amount >= amountToDeduct)
                {
                    slot.amount -= amountToDeduct;
                    
                    // Báo cho UI cập nhật lại số vàng trên màn hình
                    if (UIManager.Instance != null) UIManager.Instance.UpdateGoldUI(slot.amount);
                    return true; // Trừ tiền thành công
                }
            }
        }
        return false; // Không đủ tiền
    }

    // =======================================================
    // [MỚI THÊM]: Hàm xóa hoặc giảm số lượng vật phẩm khi BÁN
    // =======================================================
    public void RemoveItem(ItemData item, int amountToRemove)
    {
        // Duyệt ngược list để an toàn khi xóa phần tử
        for (int i = inventorySlots.Count - 1; i >= 0; i--)
        {
            if (inventorySlots[i].itemData == item)
            {
                inventorySlots[i].amount -= amountToRemove;
                
                // Nếu số lượng về 0 hoặc âm -> Xóa hẳn cái ô đó khỏi túi đồ
                if (inventorySlots[i].amount <= 0)
                {
                    inventorySlots.RemoveAt(i);
                }
                
                Debug.Log($"[Inventory] Đã bán/xóa: {item.itemName} x{amountToRemove}");
                return;
            }
        }
    }
}