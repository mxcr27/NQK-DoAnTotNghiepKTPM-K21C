using UnityEngine;

public class TutorialDataSender : MonoBehaviour
{
    [Header("Dữ liệu muốn gửi đi")]
    [Tooltip("Gõ chính xác ID TutorialPanel (VD: Move)")]
    public string groupID = "Move"; 
    
    [TextArea]
    public string message = "Nội dung hướng dẫn...";

    public void SendData()
    {
        if (TutorialPanel.Instance != null)
        {
            TutorialPanel.Instance.ShowTutorial(groupID, message);
        }
    }
}