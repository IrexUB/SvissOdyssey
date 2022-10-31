using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/VialJet/Alchemist/BossKillerAbility", fileName = "BossKillerAbility")]
public class BossKillerAbility : HindranceAbility
{
    public override void Activate(GameObject parent)
    {
        var enemies = SpawnPuddle(parent);
        m_puddle.OnEnemyStayInPuddle += BossKillerStay;
        m_puddle.OnEnemyExitPuddle += BossKillerExit;
        m_puddle.OnEnemyEnterInPuddle += BossKillerEnter;
        m_puddle.AddTargetsToList(enemies);
    }

    public override void BeginCooldown(GameObject parent)
    {
        base.BeginCooldown(parent);
    }

    protected void BossKillerStay(GameObject enemy)
    {
        HindranceStay(enemy);   
        enemy.GetComponent<Stats>().DecreaseMaxHealth(enemy.GetComponent<Stats>().GetMaxHealth() * 0.01f);
    }

    void BossKillerExit(GameObject enemy)
    {
        Debug.Log("Exit : " + enemy.name);
        enemy.GetComponent<Stats>().m_speed /= 0.65f;
    }

    void BossKillerEnter(GameObject enemy)
    {
        enemy.GetComponent<Stats>().m_speed *= 0.65f;
    }
}