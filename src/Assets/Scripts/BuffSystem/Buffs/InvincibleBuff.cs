using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Buff/InvincibleBuff", fileName = "InvincibleBuff")]
public class InvincibleBuff : ScriptableBuff
{
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedInvincibleBuff(this, obj);
    }
}
