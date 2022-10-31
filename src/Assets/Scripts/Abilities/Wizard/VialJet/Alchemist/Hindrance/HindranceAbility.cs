using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/VialJet/Alchemist/HindranceAbility", fileName = "HindranceAbility")]
public class HindranceAbility : VialJetAbility
{
    [SerializeField] protected SpeedBuff m_hindranceDebuff;
    public override void Activate(GameObject parent)
    {
        var enemies = SpawnPuddle(parent);

        m_puddle.OnEnemyEnterInPuddle += HindranceEnter;
        m_puddle.OnEnemyStayInPuddle += HindranceStay;
        m_puddle.OnEnemyExitPuddle += HindranceExit;

        m_puddle.AddTargetsToList(enemies);
    }

    public override void BeginCooldown(GameObject parent)
    {
        m_puddle.DeleteTargetsFromList();
        base.BeginCooldown(parent);
    }

    void HindranceEnter(GameObject enemy)
    {
        enemy.GetComponent<Stats>().m_speed *= 0.65f;
    }

    protected void HindranceStay(GameObject enemy)
    {
        enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Elementary, m_stats.m_elementaryAttack * 0.85f);
    }

    void HindranceExit(GameObject enemy)
    {
        enemy.GetComponent<Stats>().m_speed /= 0.65f;
    }
}