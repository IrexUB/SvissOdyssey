using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball/Healer/VitalityAbility", fileName = "VitalityAbility")]
public class VitalityAbility : KindnessAbility
{
    [SerializeField] HealthRegenerationBuff m_healthRegeneration;
    public override void Activate(GameObject parent)
    {
        CastFireballHealer(parent);
        m_fireballOffDefBehaviour.OnCollisionEnterEventOffensive += CallKindnessOffensive;
        m_fireballOffDefBehaviour.OnCollisionEnterEventOffensive += CallVitalityDefensive;
    }

    protected void CallVitalityDefensive(GameObject ally)
    {
        if (ally != m_caster)
        {
            var allyStats = ally.GetComponent<Stats>();
            allyStats.RegenerateHealth(m_casterStats.m_elementaryDefense * 0.2f);
            allyStats.GetComponent<BuffableEntity>().AddBuff(m_healthRegeneration.InitializeBuff(ally));
            Destroy(m_tmpFireball);
        }

    }
}
