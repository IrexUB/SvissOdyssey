using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball/Healer/BouleMerangAbility", fileName = "BouleMerangAbility")]
public class BouleMerangAbility : KindnessAbility
{
    [SerializeField] HealthRegenerationBuff m_healthRegeneration;

    public override void Activate(GameObject parent)
    {
        CastFireballHealer(parent);

        m_fireballOffDefBehaviour.OnCollisionEnterEventOffensive += CallBouleMerangOffensive;
        m_fireballOffDefBehaviour.OnCollisionEnterEventDefensive += CallBouleMerangDefensive;
    }

    public override void BeginCooldown(GameObject parent)
    {
        m_fireballOffDefBehaviour.m_playerSpeed = m_casterStats.m_speed;
        m_fireballOffDefBehaviour.casterPosition = parent.gameObject.transform;
        m_fireballOffDefBehaviour.m_recall = true;
    }

    protected void CallBouleMerangOffensive(GameObject enemy)
    {
        enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Elementary, m_casterStats.m_elementaryAttack);
        enemy.GetComponent<BuffableEntity>().AddBuff(m_burnDot.InitializeBuff(enemy));
    }
    protected void CallBouleMerangDefensive(GameObject ally)
    {
        if (ally != m_caster)
        {
            var allyStats = ally.GetComponent<Stats>();
            allyStats.RegenerateHealth(m_casterStats.m_elementaryDefense * 0.2f);
            allyStats.GetComponent<BuffableEntity>().AddBuff(m_healthRegeneration.InitializeBuff(ally));
        }

    }
}
