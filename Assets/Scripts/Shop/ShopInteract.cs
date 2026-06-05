using UnityEngine;

public class ShopInteract : MonoBehaviour
{
    [Header("Cài đặt UI")]
    public GameObject shopUIPanel; 

    [Header("Nút tương tác")]
    public GameObject interactPrompt;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (PauseMenu.GameIsPaused) return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            
            if (interactPrompt != null && (shopUIPanel == null || !shopUIPanel.activeSelf))
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
            
            if (shopUIPanel != null && shopUIPanel.activeSelf)
            {
                ToggleShop();
            }
        }
    }

    private void ToggleShop()
    {
        if (shopUIPanel != null)
        {
            bool isOpening = !shopUIPanel.activeSelf;
            
            shopUIPanel.SetActive(isOpening);

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(!isOpening); 
            }

            if (isOpening)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        else
        {
            Debug.LogWarning("Chưa kéo thả UI Shop vào script!");
        }
    }
}