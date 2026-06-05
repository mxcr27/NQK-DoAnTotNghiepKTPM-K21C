using UnityEngine;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ShopItem
{
    public ItemData item; 
    public int price;     
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    [Header("UI References")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;
    public GameObject shopItemPrefab; 
    public TextMeshProUGUI playerGoldText;

    [Header("Kho hàng của Shop (Tab Mua)")]
    public List<ShopItem> shopItems; 

    [Header("Cấu hình giá Bán Lại")]
    [Range(0f, 1f)] public float sellPercentage = 0.5f;
    public List<ShopItem> customSellPrices;
    public int defaultSellPrice = 5; 

    private void OnEnable()
    {
        RefreshShop();
    }

    public void RefreshShop()
    {
        ClearPanel(buyContentPanel);
        ClearPanel(sellContentPanel);

        foreach (ShopItem sItem in shopItems)
        {
            GameObject newSlot = Instantiate(shopItemPrefab, buyContentPanel);
            ShopItemUI uiScript = newSlot.GetComponent<ShopItemUI>();
            if (uiScript != null) uiScript.SetupBuySlot(sItem, this);
        }

        if (PlayerInventory.Instance != null)
        {
            foreach (InventorySlot invSlot in PlayerInventory.Instance.inventorySlots)
            {
                if (invSlot.itemData == null || invSlot.itemData.itemName == "Gold" || !invSlot.itemData.isSellable) 
                    continue;

                int sellPrice = CalculateSellPrice(invSlot.itemData);

                GameObject newSlot = Instantiate(shopItemPrefab, sellContentPanel);
                ShopItemUI uiScript = newSlot.GetComponent<ShopItemUI>();
                if (uiScript != null) uiScript.SetupSellSlot(invSlot, sellPrice, this);
            }
        }

        UpdateGoldDisplay();
    }

    private void ClearPanel(Transform panel)
    {
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateGoldDisplay()
    {
        if (playerGoldText != null && PlayerInventory.Instance != null)
        {
            int currentGold = PlayerInventory.Instance.GetTotalGold();
            playerGoldText.text = currentGold.ToString();
        }
    }

    private int CalculateSellPrice(ItemData item)
    {
        if (item != null && item.sellPrice > 0) 
        {
            return item.sellPrice;
        }

        foreach (ShopItem sItem in shopItems)
        {
            if (sItem.item == item) return Mathf.RoundToInt(sItem.price * sellPercentage);
        }

        foreach (ShopItem sItem in customSellPrices)
        {
            if (sItem.item == item) return sItem.price;
        }

        return defaultSellPrice;
    }

    public void TryBuyItem(ShopItem itemToBuy)
    {
        if (PlayerInventory.Instance == null) return;

        int playerGold = PlayerInventory.Instance.GetTotalGold();
        
        if (playerGold >= itemToBuy.price)
        {
            PlayerInventory.Instance.DeductGold(itemToBuy.price);
            PlayerInventory.Instance.AddItem(itemToBuy.item, 1); 
            Debug.Log($"Mua thành công: {itemToBuy.item.itemName}");
            
            RefreshShop();
        }
        else
        {
            Debug.Log($"Không đủ tiền!");
        }
    }

    public void TrySellItem(InventorySlot slotToSell, int sellPrice)
    {
        if (PlayerInventory.Instance == null || slotToSell == null) return;

        ItemData itemToBeSold = slotToSell.itemData;

        PlayerInventory.Instance.RemoveItem(itemToBeSold, 1);

        ItemData goldAsset = PlayerInventory.Instance.allGameItems.Find(x => x.itemName == "Gold");
        if (goldAsset != null)
        {
            PlayerInventory.Instance.AddItem(goldAsset, sellPrice);
        }
        else
        {
            Debug.LogError("LỖI: Chưa có data 'Gold' trong allGameItems của PlayerInventory!");
        }

        RefreshShop();
    }
}