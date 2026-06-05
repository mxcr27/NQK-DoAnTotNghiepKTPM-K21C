using UnityEngine;
using System.Collections.Generic;

public class EventBarrier : MonoBehaviour
{
    [Header("Điều Kiện Mở Khóa")]
    public List<string> requiredEventIDs;

    public bool requireAll = true;

    [Header("Thông Báo UI")]
    public string lockedMessage = "Khu vực này đang bị phong ấn!";
    public string uiGroupID = "tut_System";

    private void Start()
    {
        CheckCondition();
    }

    public void CheckCondition()
    {
        if (GameDataManager.Instance == null || requiredEventIDs.Count == 0) return;

        bool isUnlocked = false;

        if (requireAll)
        {
            isUnlocked = true; 
            foreach (string id in requiredEventIDs)
            {
                if (!GameDataManager.Instance.worldFlags.Contains(id))
                {
                    isUnlocked = false; 
                    break;
                }
            }
        }
        else
        {
            isUnlocked = false;
            foreach (string id in requiredEventIDs)
            {
                if (GameDataManager.Instance.worldFlags.Contains(id))
                {
                    isUnlocked = true;
                    break;
                }
            }
        }

        if (isUnlocked)
        {
            UnlockBarrier();
        }
    }

    private void UnlockBarrier()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (TutorialPanel.Instance != null && !string.IsNullOrEmpty(lockedMessage))
            {
                TutorialPanel.Instance.ShowTutorial(uiGroupID, lockedMessage);
            }
        }
    }
}