using UnityEngine;
using TMPro;

public class LootPopup : MonoBehaviour
{
    [Header("Cài đặt bay")]
    public float moveSpeed = 2f;      
    public float lifeTime = 1.5f;     

    [Header("Cài đặt Màu")]
    [Tooltip("Màu chữ khi nhặt được Vàng")]
    public Color goldColor = Color.yellow;
    [Tooltip("Màu chữ khi nhặt được Trang bị/Vật phẩm thường")]
    public Color defaultColor = Color.white;

    [Header("UI References")]
    public TextMeshProUGUI itemText;
    public CanvasGroup canvasGroup;

    private Vector3 spawnPosition;
    private float timer = 0f;

    public void Setup(ItemData item, int amount)
    {
        if (itemText != null) 
        {
            itemText.text = $"+{amount} {item.itemName}";
            if (item.itemName == "Gold")
            {
                itemText.color = goldColor;
            }
            else
            {
                itemText.color = defaultColor; 
            }
        }
        
        spawnPosition = transform.position;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = new Vector3(spawnPosition.x, transform.position.y + moveSpeed * Time.deltaTime, transform.position.z);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
        }
    }
}