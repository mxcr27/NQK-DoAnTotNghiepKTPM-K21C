using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Items/Weapon")]
public class WeaponData : ItemData
{
    [Header("Chỉ số Vũ khí")]
    public int damage = 10;
    public float range = 0.5f;
    public float attackRate = 2f; 

    private void OnEnable()
    {
        isStackable = false; 
    }

    public override void Use(GameObject player)
    {
        base.Use(player); 

        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.currentWeapon = this; 
            Debug.Log("Đã trang bị vũ khí mới: " + itemName);
        }
    }
}