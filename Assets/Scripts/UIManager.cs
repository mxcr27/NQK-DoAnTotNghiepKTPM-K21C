using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Giao diện Vàng")]
    public TextMeshProUGUI goldText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateGoldUI(int currentGold)
    {
        goldText.text = currentGold.ToString(); 
    }
}