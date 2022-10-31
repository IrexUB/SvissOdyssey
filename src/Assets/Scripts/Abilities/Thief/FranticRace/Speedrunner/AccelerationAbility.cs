using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/Speedrunner/AccelerationAbility", fileName = "AccelerationAbility")]
public class AccelerationAbility : FranticRaceAbility
{
    public override void Activate(GameObject parent)
    {
        CallAcceleration(parent);
    }

    protected void CallAcceleration(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallFranticRace(parent));
        PlayerStats playerStats = parent.GetComponent<PlayerStats>();

        playerStats.RegenerateHealth(playerStats.GetMaxHealth() * 0.05f);
    }
}