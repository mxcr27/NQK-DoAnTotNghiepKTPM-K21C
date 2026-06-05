using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance; 

    [Header("Bảng Trạng Thái")]
    public TextMeshProUGUI statusText; 

    [Header("Dữ liệu Nhân vật")]
    public PlayerHealth playerHealth; 
    public PlayerMana playerMana;    
    public PlayerInventory playerInventory;
    public PlayerCombat playerCombat;

    [Header("Bảng Vũ Khí")]
    public Image weaponIcon;           
    public TextMeshProUGUI weaponName; 

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (playerHealth != null && playerMana != null && playerInventory != null)
        {
            UpdateStatus(
                playerHealth.currentHealth, 
                playerHealth.maxHealth, 
                (int)playerMana.currentMana, 
                (int)playerMana.maxMana, 
                playerInventory.GetTotalGold()
            );
        }

        UpdateWeaponDisplay();
    }

    private void UpdateWeaponDisplay()
    {
        if (playerCombat != null && playerCombat.currentWeapon != null)
        {
            weaponIcon.sprite = playerCombat.currentWeapon.icon; 
            weaponName.text = $"Vũ Khí : {playerCombat.currentWeapon.itemName}"; 
            weaponIcon.color = Color.white;
        }
        else
        {
            if (weaponName != null) weaponName.text = "Equipped Weapon : Trống";
            if (weaponIcon != null) weaponIcon.color = new Color(1, 1, 1, 0); 
        }
    }

    public void UpdateStatus(int currentHP, int maxHP, int currentMP, int maxMP, int gold)
    {
        if (statusText != null)
        {
            statusText.text = $"HP : {currentHP}/{maxHP}\nMP : {currentMP}/{maxMP}\nGold : {gold}";
        }
    }
}