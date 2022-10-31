using System.Collections;
using UnityEngine;

public class TimedSpeedBuff : TimedBuff
{
    private PlayerStats m_stats;
    public TimedSpeedBuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
        m_stats = obj.GetComponent<PlayerStats>();
    }

    protected override void ApplyEffect()
    {
        SpeedBuff speedBuff = (SpeedBuff)m_buff;
        m_stats.m_speed *= (1 + (speedBuff.m_buffPercentage / 100));
    }

    public override void End()
    {
        SpeedBuff speedBuff = (SpeedBuff)m_buff;
        m_stats.m_speed /= (1 + (speedBuff.m_buffPercentage / 100));

        m_effectStacks = 0;
    }
}