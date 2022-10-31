using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab/Assassin/DeepBurnAbility", fileName = "DeepBurnAbility")]
public class StealthAbility : ExecutionerAbility
{
    [SerializeField] protected InvisibilityBuff m_invisibility;
    public override void Activate(GameObject parent)
    {
        CallStealthAbility(parent);
    }

    void CallStealthAbility(GameObject parent)
    {
        var hitEnemy = CallCriticalAttack(parent);
        if (hitEnemy != null && !hitEnemy.CompareTag("Boss"))
        {
            var enemyHealth = hitEnemy.GetComponent<Stats>();
            if (enemyHealth.GetCurrentHealth() <= enemyHealth.GetMaxHealth() * 0.15f)
            {
                Debug.Log("ENEMY ONE SHOT !");
                enemyHealth.Kill();
                parent.GetComponent<BuffableEntity>().AddBuff(m_invisibility.InitializeBuff(parent));
            }
        }
    }
}