using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/Hybrid/VersatilityAbility", fileName = "VersatilityAbility")]
public class VersatilityAbility : Ability
{
    public override void Activate(GameObject parent)
    { 
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense *= 1.25f;
        playerStats.m_elementaryDefense *= 1.25f;
    }

    public override void BeginCooldown(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense /= 1.25f;
        playerStats.m_elementaryDefense /= 1.25f;
    }
}