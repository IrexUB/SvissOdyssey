using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Buff/InvisibilityBuff", fileName = "InvisibilityBuff")]
public class InvisibilityBuff : ScriptableBuff
{
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedInvisiblityBuff(this, obj);
    }
}
