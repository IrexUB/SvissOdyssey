using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/Speedrunner/ThreeForThePriceOfOneAbility", fileName = "ThreeForThePriceOfOneAbility")]
public class ThreeForThePriceOfOneAbility : AccelerationAbility
{
    public override void Activate(GameObject parent)
    {
        CallAcceleration(parent);
    }

    public override void Reactivate(GameObject parent)
    {
        Activate(parent);
    }

}