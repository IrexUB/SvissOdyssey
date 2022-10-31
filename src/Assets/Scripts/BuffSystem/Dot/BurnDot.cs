using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Dot/BurnDot", fileName = "BurnDot")]
public class BurnDot : ScriptableBuff
{
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedBurnDot(this, obj);
    }
}