using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/Mastodonte/TenacityAbility", fileName = "TenacityAbility")]
public class TenacityAbility : StrengtheningAbility
{
    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        var playerState = parent.GetComponent<PlayerState>();
        playerState.SetImmuneToCrowdControlState(true);
    }

    public override void BeginCooldown(GameObject parent)
    {
        base.BeginCooldown(parent);

        var playerState = parent.GetComponent<PlayerState>();
        playerState.SetImmuneToCrowdControlState(false);
    }
}