using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/Chase/ChronoMaitreAbility", fileName = "ChronoMaitreAbility")]
public class ChronoMaitreAbility : HustleAbility
{
    public delegate void CooldownReductionEvent(float percentage);
    public static event CooldownReductionEvent OnCooldownReduction;

    public override void Activate(GameObject parent)
    {
        DashCollisionManager.OnDashCollidingWithEnemy += CallChronoMaitre;
        CoroutineHelper.instance.StartCoroutine(CallFranticRace(parent));
    }
    public override void BeginCooldown(GameObject parent)
    {
        DashCollisionManager.OnDashCollidingWithEnemy -= CallChronoMaitre;
    }

    void CallChronoMaitre(GameObject parent, GameObject enemy)
    {
        CallHustleAbility(parent, enemy);
        if (OnCooldownReduction != null)
        {
            OnCooldownReduction(20);
        }
    }
}