using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/Mastodonte/IndestructibleAbility", fileName = "IndestructibleAbility")]
public class IndestructibleAbility : TenacityAbility
{
    public override void Activate(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense *= 2f;
        playerStats.m_physicalAttack *= 0.3f;
        playerStats.m_elementaryAttack *= 0.3f;

        var playerState = parent.GetComponent<PlayerState>();
        playerState.SetImmuneToCrowdControlState(true);
    }

    public override void BeginCooldown(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense /= 2f;
        playerStats.m_physicalAttack /= 0.3f;
        playerStats.m_elementaryAttack /= 0.3f;

        var playerState = parent.GetComponent<PlayerState>();
        playerState.SetImmuneToCrowdControlState(false);
    }
}