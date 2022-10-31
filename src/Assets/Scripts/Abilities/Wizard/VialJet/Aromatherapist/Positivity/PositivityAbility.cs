using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/VialJet/Aromatherapist/PositivityAbility", fileName = "PositivityAbility")]
public class PositivityAbility : ResidueAbility
{
    public override void Activate(GameObject parent)
    {
        var targets = SpawnPuddle(parent);

        m_puddle.OnAllyStayInPuddle += CallPositivity;
        m_puddle.OnAllyExitPuddle += ApplyResidueDefensiveDot;

        m_puddle.AddTargetsToList(targets);
    }

    public override void BeginCooldown(GameObject parent)
    {
        base.BeginCooldown(parent);
    }

    protected void CallPositivity(GameObject ally)
    {
        ally.GetComponent<Stats>().RegenerateHealth(ally.GetComponent<Stats>().GetMaxHealth() * 0.09f);
    }
}