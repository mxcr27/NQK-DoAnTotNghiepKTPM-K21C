using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    [Header("Chỉ số Mana")]
    public float maxMana = 100f; 
    public float currentMana;
    public float manaRegenRate = 5f;

    [Header("Giao diện UI")]
    public Slider manaSlider;
    public float drainSpeed = 5f; 

    void Start()
    {
        currentMana = maxMana;
        
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = maxMana;
        }
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana); 
        }

        if (manaSlider != null)
        {
            if (manaSlider.maxValue != maxMana)
            {
                manaSlider.maxValue = maxMana;
            }

            if (manaSlider.value != currentMana)
            {
                manaSlider.value = Mathf.Lerp(manaSlider.value, currentMana, drainSpeed * Time.deltaTime);

                if (Mathf.Abs(manaSlider.value - currentMana) < 0.1f)
                {
                    manaSlider.value = currentMana;
                }
            }
        }
    }

    public bool UseMana(float cost)
    {
        if (currentMana >= cost)
        {
            currentMana -= cost;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void IncreaseMaxMana(float boostAmount)
    {
        maxMana += boostAmount;
        currentMana += boostAmount;
        
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
        }
    }
}