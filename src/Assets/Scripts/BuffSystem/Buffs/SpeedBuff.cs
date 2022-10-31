using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Buff/SpeedBuff", fileName = "SpeedBuff")]
public class SpeedBuff : ScriptableBuff
{
    public float m_buffPercentage;

    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedSpeedBuff(this, obj);
    }
}
