using UnityEngine;

public class EnemyAnimEvents : MonoBehaviour
{
    private EnemyAI aiCore;

    void Start()
    {
        aiCore = GetComponentInParent<EnemyAI>(); //[cite: 2]
    }

    public void TriggerDamage()
    {
        if (aiCore != null) //[cite: 2]
        {
            aiCore.DealDamage(); //[cite: 2]
        }
    }

    public void TriggerResetAttack()
    {
        if (aiCore != null)
        {
            aiCore.ResetAttack();
        }
    }
}