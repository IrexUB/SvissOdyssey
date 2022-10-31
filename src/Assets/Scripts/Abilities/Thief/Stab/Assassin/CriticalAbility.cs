using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab/Assassin/CriticalAbility", fileName = "CriticalAbility")]
public class CriticalAbility : StabAbility
{
    public override void Activate(GameObject parent)
    {
        CallCriticalAttack(parent);
    }

    protected Collider2D CallCriticalAttack(GameObject parent)
    {
        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;
        float attackMultiplicator;

        if (Random.value <= 0.1f)
        {
            attackMultiplicator = 0.75f;
        } else
        {
            attackMultiplicator = 0.5f;
        }

        var hitEnemy = BasicDaggerAttack(parent, playerPhysicalAttack * attackMultiplicator);
        return hitEnemy;
    }

    public override void BeginCooldown(GameObject parent)
    {
    }
}