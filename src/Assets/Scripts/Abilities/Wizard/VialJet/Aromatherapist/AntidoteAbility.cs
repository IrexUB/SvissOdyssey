using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/VialJet/Aromatherapist/AntidoteAbility", fileName = "AntidoteAbility")]
public class AntidoteAbility : VialJetAbility
{
    public override void Activate(GameObject parent)
    {
        var targets = SpawnPuddle(parent);
        m_puddle.OnEnemyStayInPuddle += VialJetBase;
        m_puddle.OnAllyStayInPuddle += CallAntidote;
        m_puddle.AddTargetsToList(targets);
    }

    protected void CallAntidote(GameObject ally)
    {
        ally.GetComponent<Stats>().RegenerateHealth(ally.GetComponent<Stats>().GetMaxHealth() * 0.05f);
    }
}