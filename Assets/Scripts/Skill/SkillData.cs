using UnityEngine;

public enum SkillType { Attack, Heal }

[CreateAssetMenu(fileName = "New Skill", menuName = "Game Data/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string skillName;
    
    public SkillType skillType = SkillType.Attack; 
    
    [TextArea] public string description;
    public Sprite icon;

    [Header("Chỉ số Chi phí")]
    public float manaCost = 20f;
    public float cooldown = 2f;
    public int goldCost = 150;

    [Header("CHIÊU TẤN CÔNG")]
    public int damage = 30;

    [Header("CHIÊU HỒI MÁU")]
    public int healAmount = 50;

    [Header("Hiển thị (Prefab)")]
    public GameObject skillPrefab; 

    public bool Activate(GameObject caster, Transform castPoint)
    {
        if (skillPrefab != null)
        {
            GameObject spawnedVFX = Instantiate(skillPrefab, castPoint.position, castPoint.rotation);

            if (skillType == SkillType.Attack)
            {
                spawnedVFX.transform.SetParent(null); 
                SkillEntity entity = spawnedVFX.GetComponent<SkillEntity>();
                if (entity != null)
                {
                    entity.damage = this.damage;
                    entity.caster = caster;
                }
            }
            else if (skillType == SkillType.Heal)
            {
                spawnedVFX.transform.SetParent(caster.transform);
            }
        }

        if (skillType == SkillType.Attack)
        {
            Debug.Log("Đã kích hoạt: " + skillName);
        }
        else if (skillType == SkillType.Heal)
        {
            PlayerHealth hp = caster.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.Heal(healAmount);
            }
        }

        return true; 
    }
}