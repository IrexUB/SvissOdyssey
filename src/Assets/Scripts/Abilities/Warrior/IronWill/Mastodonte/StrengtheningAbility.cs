using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/Mastodonte/StrengtheningAbility", fileName = "StrengtheningAbility")]
public class StrengtheningAbility : Ability
{
    public override void Activate(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense *= 1.45f;
    }

    public override void BeginCooldown(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense /= 1.45f;
    }
}