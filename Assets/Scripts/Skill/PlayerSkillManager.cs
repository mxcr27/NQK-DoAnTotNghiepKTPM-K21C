using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public static PlayerSkillManager Instance; 

    [Header("Kỹ Năng (Toàn bộ chiêu trong game)")]
    public List<SkillData> allGameSkills = new List<SkillData>(); 
    
    [Header("Chiêu đã mở khóa")]
    public List<SkillData> unlockedSkills = new List<SkillData>(); 

    [Header("Trang bị Kỹ năng")]
    public SkillData skillSlot1;
    public SkillData skillSlot2;

    [Header("Cài đặt bắn")]
    public Transform castPoint;  

    private PlayerMana playerMana; 
    
    private PlayerHealth playerHealth;
    private float[] nextSkillTimes = new float[2]; 

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.isDead) return;

        if (Time.timeScale == 0f) return;

        if (Input.GetKeyDown(KeyCode.K)) TryCastSkill(0, skillSlot1);
        if (Input.GetKeyDown(KeyCode.L)) TryCastSkill(1, skillSlot2);
    }

    public bool EquipSkill(SkillData newSkill, int slotIndex)
    {
        if (slotIndex == 1 && skillSlot2 == newSkill)
        {
            return false;
        }
        if (slotIndex == 2 && skillSlot1 == newSkill)
        {
            return false;
        }

        if (slotIndex == 1)
        {
            skillSlot1 = newSkill;
        }
        else if (slotIndex == 2)
        {
            skillSlot2 = newSkill;
        }

        return true;
    }

    public void UnequipSkill(int slotIndex)
    {
        if (slotIndex == 1)
        {
            skillSlot1 = null;
        }
        else if (slotIndex == 2)
        {
            skillSlot2 = null;
        }
    }

    void TryCastSkill(int slotIndex, SkillData skillToCast)
    {
        if (skillToCast == null) return;

        if (Time.time < nextSkillTimes[slotIndex])
        {
            float timeRemaining = nextSkillTimes[slotIndex] - Time.time;
            return;
        }

        if (playerMana != null && playerMana.UseMana(skillToCast.manaCost))
        {
            bool success = skillToCast.Activate(gameObject, castPoint);

            if (success)
            {
                float sharedCooldownTime = Time.time + skillToCast.cooldown;
                
                nextSkillTimes[0] = sharedCooldownTime; 
                nextSkillTimes[1] = sharedCooldownTime; 
            }
            else
            {
                playerMana.currentMana += skillToCast.manaCost;
            }
        }
    }
}