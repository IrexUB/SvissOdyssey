using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball/Healer/KindnessAbility", fileName = "KindnessAbility")]
public class KindnessAbility : FireballAbility
{
    [SerializeField] protected BurnDot m_burnDot;

    // Ici le FireballBehaviour permet de pouvoir surcharger les comportements selon le layer touché
    protected FireballBehaviour m_fireballOffDefBehaviour;
    // Le tmpFireball permet de pouvoir gérer la durée de vie de l'objet
    protected GameObject m_tmpFireball;
    protected PlayerStats m_casterStats;
    protected GameObject m_caster;

    public override void Activate(GameObject parent)
    {

        CastFireballHealer(parent);
        m_fireballOffDefBehaviour.OnCollisionEnterEventDefensive += CallKindnessDefensive;
        m_fireballOffDefBehaviour.OnCollisionEnterEventOffensive += CallKindnessOffensive;
    }

    protected void CastFireballHealer(GameObject parent)
    {
        m_casterStats = parent.GetComponent<PlayerStats>();
        m_caster = parent;
        m_elementaryAttack = m_casterStats.m_elementaryAttack;

        var attackPoint = parent.GetComponent<CombatSystem>().GetAttackPoint();
        float shootAngle = Utility.GetAngleTowardsMouse(attackPoint);

        Quaternion fireballAngle = Quaternion.Euler(new Vector3(0f, 0f, shootAngle - 90f));

        m_tmpFireball = Instantiate(m_fireballPrefab, attackPoint.position, fireballAngle);
        m_fireballOffDefBehaviour = m_tmpFireball.AddComponent<FireballBehaviour>();

        m_fireballOffDefBehaviour.m_projectileVelocity = m_fireballVelocity;
    }

    protected void CallKindnessOffensive(GameObject enemy)
    {
        enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Elementary, m_casterStats.m_elementaryAttack);
        enemy.GetComponent<BuffableEntity>().AddBuff(m_burnDot.InitializeBuff(enemy));
        Destroy(m_tmpFireball);
    }

    protected void CallKindnessDefensive(GameObject ally)
    {
        if (ally != m_caster)
        {
            var allyStats = ally.GetComponent<Stats>();
            allyStats.RegenerateHealth(m_casterStats.m_elementaryDefense * 0.2f);
            Destroy(m_tmpFireball);
        }

    }
}
