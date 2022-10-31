using System.Collections;
using UnityEngine;

public class TimedAllStatsReductionDebuff : TimedBuff
{
    Stats m_targetStats;
    public TimedAllStatsReductionDebuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
        m_targetStats = obj.GetComponent<Stats>();
    }

    protected override void ApplyEffect()
    {
        AllStatsReductionDebuff statsReductionDebuff = (AllStatsReductionDebuff)m_buff;
        m_targetStats.DecreaseAllStats(statsReductionDebuff.m_reductionPercentage);
        Debug.Log("Stats of " + m_obj.name + " reduced by " + statsReductionDebuff.m_reductionPercentage);
    }

    public override void End()
    {
        AllStatsReductionDebuff statsReductionDebuff = (AllStatsReductionDebuff)m_buff;
        m_targetStats.IncreaseAllStats(statsReductionDebuff.m_reductionPercentage);

        Debug.Log("Stats of " + m_obj.name + " restored");
        m_effectStacks = 0;
    }
}