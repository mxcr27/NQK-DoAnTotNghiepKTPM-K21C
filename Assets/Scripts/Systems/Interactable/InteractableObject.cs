using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("Hiển thị nút bấm (Nút E)")]
    public GameObject promptUI;

    [Header("Hành động khi tương tác")]
    public UnityEvent onInteract;

    [Header("Hành động khi rời đi")]
    public UnityEvent onExit; 

    private bool isPlayerInRange = false;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {           
            if (promptUI != null) promptUI.SetActive(false); 
            onInteract.Invoke(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
            isPlayerInRange = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);

            onExit.Invoke(); 
        }
    }
}