using UnityEngine;

public enum ItemType 
{ 
    VatPhamTieuHao, 
    VuKhi,          
    Giap,           
    KyNang          
}

[CreateAssetMenu(fileName = "New Basic Item", menuName = "Game Data/Items/Basic Item")]
public class ItemData : ScriptableObject
{
    [Header("Phân loại")]
    public ItemType loaiTrangBi = ItemType.VatPhamTieuHao; 

    [Header("Thông tin cơ bản")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public bool isStackable; 

    [Header("Kinh tế")]
    public bool isSellable = true;
    
    public int sellPrice = 0; 

    public virtual void Use(GameObject player)
    {
        Debug.Log("Đang tương tác với: " + itemName);
    }
}