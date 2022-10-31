using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Buff/VampirismBuff", fileName = "VampirismBuff")]
public class VampirismBuff : ScriptableBuff
{
    public float m_buffPercentage;

    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedVampirismBuff(this, obj);
    }
}
