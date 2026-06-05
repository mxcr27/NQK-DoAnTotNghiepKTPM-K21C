using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string savedScene;

    public float positionX;
    public float positionY;

    public float currentHealth;
    public float currentMana;

    public string currentWeaponName; 

    public List<string> unlockedSkillNames = new List<string>(); 
    public string equippedSkill1; 
    public string equippedSkill2; 

    public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();

    public List<string> worldFlags = new List<string>();
    public List<WaypointSaveData> unlockedWaypoints = new List<WaypointSaveData>();
}

[System.Serializable]
public struct ItemSaveData
{
    public string itemName;
    public int amount;
}

[System.Serializable]
public struct WaypointSaveData
{
    public string waypointID;
    public string sceneName;
}