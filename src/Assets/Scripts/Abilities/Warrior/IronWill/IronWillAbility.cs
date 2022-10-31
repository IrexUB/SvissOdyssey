using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/IronWilAbility",fileName = "IronWillAbility")]
public class IronWillAbility : Ability
{
    public override void Activate(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense *= 1.25f;
    }

    public override void BeginCooldown(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense /= 1.25f;
    }
}
