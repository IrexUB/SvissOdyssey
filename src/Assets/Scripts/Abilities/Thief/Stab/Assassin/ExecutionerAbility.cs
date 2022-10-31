using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab/Assassin/ExecutionerAbility", fileName = "ExecutionerAbility")]
public class ExecutionerAbility : CriticalAbility
{
    public override void Activate(GameObject parent)
    {
        CallExecutionerAbility(parent);
    }

    void CallExecutionerAbility(GameObject parent)
    {
        var hitEnemy = CallCriticalAttack(parent);
        if (hitEnemy != null && !hitEnemy.CompareTag("Boss"))
        {
            var enemyStats = hitEnemy.GetComponent<Stats>();
            if (enemyStats.GetCurrentHealth() <= enemyStats.GetMaxHealth() * 0.15f)
            {
                Debug.Log("ENEMY ONE SHOT !");
                enemyStats.Kill();
            }  
        }
    }

}