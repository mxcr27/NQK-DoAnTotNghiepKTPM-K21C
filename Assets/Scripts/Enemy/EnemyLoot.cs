using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootItem
{
    public ItemData item;              
    [Range(0f, 100f)] public float dropChance = 100f;    
    public int minAmount = 1;          
    public int maxAmount = 1;          
}

public class EnemyLoot : MonoBehaviour
{
    [Header("Bảng Rớt Đồ")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Hiệu ứng Popup")]
    public GameObject lootPopupPrefab; 
    public float popupDelay = 0.3f;

    private bool hasDropped = false; 

    public void DropLoot()
    {
        if (hasDropped) return; 
        
        hasDropped = true; 

        StartCoroutine(DropLootRoutine());
    }

    private IEnumerator DropLootRoutine()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        foreach (LootItem loot in lootTable)
        {
            float randomRoll = Random.Range(0f, 100f);

            if (randomRoll <= loot.dropChance)
            {
                int dropAmount = Random.Range(loot.minAmount, loot.maxAmount + 1);

                if (PlayerInventory.Instance != null && dropAmount > 0)
                {
                    PlayerInventory.Instance.AddItem(loot.item, dropAmount);
                    if (lootPopupPrefab != null && playerTransform != null)
                    {
                        Vector3 spawnPos = playerTransform.position + Vector3.up * 1.5f; 
                        GameObject popup = Instantiate(lootPopupPrefab, spawnPos, Quaternion.identity);
                        LootPopup popupScript = popup.GetComponent<LootPopup>();
                        if (popupScript != null)
                        {
                            popupScript.Setup(loot.item, dropAmount);
                        }

                        yield return new WaitForSeconds(popupDelay);
                    }
                }
            }
        }
    }

    public void ResetLoot()
    {
        hasDropped = false;
    }
}