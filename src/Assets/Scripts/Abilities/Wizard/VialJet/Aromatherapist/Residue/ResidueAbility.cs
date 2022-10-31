using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/VialJet/Aromatherapist/ResidueAbility", fileName = "ResidueAbility")]
public class ResidueAbility : AntidoteAbility
{
    [SerializeField] protected HealthRegenerationBuff m_regenerationBuff;
    [SerializeField] private PoisonDot m_poisonDot;
    public override void Activate(GameObject parent)
    {
        var targets = SpawnPuddle(parent);

        m_puddle.OnEnemyStayInPuddle += VialJetBase;
        m_puddle.OnAllyStayInPuddle += CallAntidote;
        m_puddle.OnEnemyExitPuddle += ApplyResidueOffensiveDot;
        m_puddle.OnAllyExitPuddle += ApplyResidueDefensiveDot;

        m_puddle.AddTargetsToList(targets);
    }

    public override void BeginCooldown(GameObject parent)
    {
        m_puddle.DeleteTargetsFromList();
        base.BeginCooldown(parent);
    }

    protected void ApplyResidueDefensiveDot(GameObject ally)
    {

        ally.GetComponent<BuffableEntity>().AddBuff(m_regenerationBuff.InitializeBuff(ally));
    }

    protected void ApplyResidueOffensiveDot(GameObject enemy)
    {
        Debug.Log("BUFF APPLIED !");
        TimedPoisonDot timedPoison = (TimedPoisonDot)m_poisonDot.InitializeBuff(enemy);
        timedPoison.m_casterElementaryAttack = m_stats.m_elementaryAttack;

        enemy.GetComponent<BuffableEntity>().AddBuff(timedPoison);
    }
}