using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))] 
public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI priceText; 

    [Header("Cấu hình Giao diện")]
    public float customFontSize = 20f; 

    private Button slotButton;
    private ShopManager shopManager;
    
    private ShopItem currentShopItem; 
    private InventorySlot currentInventorySlot; 
    private bool isSellSlot = false;

    private void Awake()
    {
        slotButton = GetComponent<Button>();

        if (priceText != null)
        {
            priceText.fontSize = customFontSize;
        }
    }

    public void SetupBuySlot(ShopItem sItem, ShopManager manager)
    {
        isSellSlot = false;
        currentShopItem = sItem;
        shopManager = manager;

        iconImage.sprite = sItem.item.icon;
        priceText.text = sItem.price.ToString();

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    public void SetupSellSlot(InventorySlot invSlot, int sellPrice, ShopManager manager)
    {
        isSellSlot = true;
        currentInventorySlot = invSlot;
        shopManager = manager;
        
        currentShopItem = new ShopItem { item = invSlot.itemData, price = sellPrice };

        iconImage.sprite = invSlot.itemData.icon;
        
        priceText.text = $"{sellPrice} (x{invSlot.amount})"; 

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        if (shopManager == null || currentShopItem == null) return;

        if (!isSellSlot)
        {
            shopManager.TryBuyItem(currentShopItem);
        }
        else
        {
            shopManager.TrySellItem(currentInventorySlot, currentShopItem.price);
        }
    }
}