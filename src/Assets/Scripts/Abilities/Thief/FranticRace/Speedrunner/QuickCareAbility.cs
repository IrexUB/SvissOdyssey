using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/Speedrunner/QuickCareAbility", fileName = "QuickCareAbility")]
public class QuickCareAbility : FranticRaceAbility
{
    public override void Activate(GameObject parent)
    {
        CallQuickCare(parent);
    }

    protected void CallQuickCare(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallFranticRace(parent));
        PlayerStats playerStats = parent.GetComponent<PlayerStats>();

        playerStats.RegenerateHealth(playerStats.GetMaxHealth() * 0.10f);
    }
}