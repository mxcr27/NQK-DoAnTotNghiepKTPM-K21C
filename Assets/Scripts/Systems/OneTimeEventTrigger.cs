using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class OneTimeEventTrigger : MonoBehaviour
{
    [Header("Định danh Sự kiện")]
    public string eventID;

    [Header("Hành động khi người chơi chạm vào")]
    public UnityEvent onFirstTimeTrigger;

    [Header("Hành động khi Load Game")]
    public UnityEvent onAlreadyCompleted;

    private bool hasTriggered = false;

    void Start()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.worldFlags.Contains(eventID))
        {
            hasTriggered = true;
            GetComponent<Collider2D>().enabled = false;
            onAlreadyCompleted.Invoke(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            GetComponent<Collider2D>().enabled = false;

            if (GameDataManager.Instance != null && !GameDataManager.Instance.worldFlags.Contains(eventID))
            {
                GameDataManager.Instance.worldFlags.Add(eventID);
            }
            onFirstTimeTrigger.Invoke();
        }
    }
}