using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[DefaultExecutionOrder(-50)]
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("World Flag")]
    public List<string> worldFlags = new List<string>();
    public List<WaypointSaveData> savedWaypoints = new List<WaypointSaveData>();

    [Header("Object Player")]
    [SerializeField] private GameObject player;

    [Header("Player Component:")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMana playerMana;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerSkillManager playerSkill;
    [SerializeField] private PlayerInventory playerInventory;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public PlayerData CapturePlayerData()
{
    PlayerData data = new PlayerData();
    data.savedScene = SceneManager.GetActiveScene().name;

    if (player != null)
    {
        // 1. Tọa độ và Chỉ số
        data.positionX = playerHealth.checkpointPosition.x;
        data.positionY = playerHealth.checkpointPosition.y;
        data.currentHealth = playerHealth.currentHealth;
        data.currentMana = playerMana.currentMana;

        // 2. Lưu Vũ khí
        if (playerCombat.currentWeapon != null)
            data.currentWeaponName = playerCombat.currentWeapon.name;

        // 3. Lưu Kỹ năng (Skill)
        foreach (var skill in playerSkill.unlockedSkills)
        {
            data.unlockedSkillNames.Add(skill.name);
        }
        if (playerSkill.skillSlot1 != null) data.equippedSkill1 = playerSkill.skillSlot1.name;
        if (playerSkill.skillSlot2 != null) data.equippedSkill2 = playerSkill.skillSlot2.name;

        // 4. Lưu Túi đồ (Inventory)
        foreach (var slot in playerInventory.inventorySlots)
        {
            ItemSaveData itemData = new ItemSaveData();
            itemData.itemName = slot.itemData.name;
            itemData.amount = slot.amount;
            data.inventoryItems.Add(itemData);
        }
    }
    data.worldFlags = new List<string>(this.worldFlags);
    data.unlockedWaypoints = new List<WaypointSaveData>(this.savedWaypoints);
    return data;
}

public void ApplyPlayerData(PlayerData data, bool ignorePosition = false)
    {
        if (data == null) return;

        if (player != null)
        {
            // 1. Phục hồi Vị trí & Máu
            if (!ignorePosition)
            {
                player.transform.position = new Vector2(data.positionX, data.positionY);
                if (playerHealth != null) 
                {
                    playerHealth.checkpointPosition = new Vector2(data.positionX, data.positionY);
                }
            }
            
            if (playerHealth != null) 
            {
                playerHealth.currentHealth = (int)data.currentHealth;
            }
            if (playerMana != null) playerMana.currentMana = (int)data.currentMana;

            // 2. Phục hồi Kỹ năng
            if (playerSkill != null)
            {
                playerSkill.unlockedSkills.Clear(); 
                foreach (string sName in data.unlockedSkillNames)
                {
                    SkillData foundSkill = playerSkill.allGameSkills.Find(s => s.name == sName);
                    if (foundSkill != null) playerSkill.unlockedSkills.Add(foundSkill);
                }
                playerSkill.skillSlot1 = playerSkill.allGameSkills.Find(s => s.name == data.equippedSkill1);
                playerSkill.skillSlot2 = playerSkill.allGameSkills.Find(s => s.name == data.equippedSkill2);
            }

            // 3. Phục hồi Túi đồ
            if (playerInventory != null)
            {
                playerInventory.inventorySlots.Clear(); 
                foreach (var savedItem in data.inventoryItems)
                {
                    ItemData foundItem = playerInventory.allGameItems.Find(i => i.name == savedItem.itemName);
                    if (foundItem != null)
                    {
                        playerInventory.inventorySlots.Add(new InventorySlot(foundItem, savedItem.amount));
                    }
                }
                if (UIManager.Instance != null)
                {
                    int goldOnLoad = playerInventory.GetTotalGold();
                    UIManager.Instance.UpdateGoldUI(goldOnLoad);
                }
            }

            // 4. Phục hồi Vũ khí
            if (playerCombat != null && !string.IsNullOrEmpty(data.currentWeaponName))
            {
                ItemData foundWeaponItem = playerInventory.allGameItems.Find(i => i.name == data.currentWeaponName);
                if (foundWeaponItem != null && foundWeaponItem is WeaponData)
                {
                    WeaponData weaponToEquip = (WeaponData)foundWeaponItem;
                    weaponToEquip.Use(playerCombat.gameObject); 
                }
            }

            if (data.worldFlags != null)
            {
                this.worldFlags = new List<string>(data.worldFlags);
            }
            if (data.unlockedWaypoints != null)
            {
                this.savedWaypoints = new List<WaypointSaveData>(data.unlockedWaypoints);
            }
        }
    }

    void Start()
    {
        if (SaveSystem.HasSaveFile())
        {
            PlayerData savedData = SaveSystem.LoadGame();
            
            if (savedData != null)
            {
                if (PlayerPrefs.HasKey("TransitionSpawnID"))
                {
                    string targetID = PlayerPrefs.GetString("TransitionSpawnID");
                    
                    ApplyPlayerData(savedData, ignorePosition: true);
                    
                    SceneSpawnPoint[] spawnPoints = Object.FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);
                    bool foundSpawn = false;
                    
                    foreach (SceneSpawnPoint sp in spawnPoints)
                    {
                        if (sp.spawnPointID == targetID)
                        {
                            player.transform.position = sp.transform.position;
                            
                            if (playerHealth != null)
                            {
                                playerHealth.checkpointPosition = sp.transform.position;
                            }
                            foundSpawn = true;
                            break;
                        }
                    }
                    
                    if (!foundSpawn)
                    {
                        player.transform.position = new Vector2(savedData.positionX, savedData.positionY);
                    }
                    
                    PlayerPrefs.DeleteKey("TransitionSpawnID");
                    PlayerPrefs.Save();
                }
                else if (savedData.savedScene == SceneManager.GetActiveScene().name)
                {
                    ApplyPlayerData(savedData, ignorePosition: false);
                }
            }
        }
    }
}