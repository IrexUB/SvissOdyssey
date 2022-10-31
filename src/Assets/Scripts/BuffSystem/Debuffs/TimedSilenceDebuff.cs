using System.Collections;
using UnityEngine;

public class TimedSilenceDebuff : TimedBuff
{

    public TimedSilenceDebuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
    }

    protected override void ApplyEffect()
    {
        Debug.Log("Silenced..." + m_obj.name);
    }

    public override void End()
    {
        Debug.Log("Silenced end..." + m_obj.name);
        m_effectStacks = 0;
    }
}