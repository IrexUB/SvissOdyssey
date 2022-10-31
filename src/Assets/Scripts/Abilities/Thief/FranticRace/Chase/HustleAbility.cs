using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/Chase/HustleAbility", fileName = "HustleAbility")]
public class HustleAbility : FranticRaceAbility
{

    public override void Activate(GameObject parent)
    {
        DashCollisionManager.OnDashCollidingWithEnemy += CallHustleAbility;
        CoroutineHelper.instance.StartCoroutine(CallFranticRace(parent));
    }


    public override void BeginCooldown(GameObject parent)
    {
        DashCollisionManager.OnDashCollidingWithEnemy -= CallHustleAbility;
    }


    protected void CallHustleAbility(GameObject parent, GameObject enemy)
    {
        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;
        enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Physical, playerPhysicalAttack * 0.8f);
        Debug.Log("Dealt " + playerPhysicalAttack + " to " + enemy.name);
    }
}