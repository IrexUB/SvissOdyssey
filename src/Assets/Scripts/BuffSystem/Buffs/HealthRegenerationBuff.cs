using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Buff/HealthRegenerationBuff", fileName = "HealthRegenerationBuff")]
public class HealthRegenerationBuff : ScriptableBuff
{
    public float m_buffPercentage;

    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedHealthRegenerationBuffBuff(this, obj);
    }
}
