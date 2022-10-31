using System.Collections;
using UnityEngine;

public class TimedResistanceDebuff : TimedBuff
{
    private Stats targetStats;

    public TimedResistanceDebuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
        targetStats = obj.GetComponent<Stats>();
    }

    protected override void ApplyEffect()
    {
        ResistanceDebuff resistanceDebuff = (ResistanceDebuff)m_buff;
        float reductionMultiplicator = 1 - (resistanceDebuff.m_reductionPercentage / 100);
        targetStats.m_defense *= reductionMultiplicator;
        targetStats.m_elementaryDefense *= reductionMultiplicator;
    }

    public override void End()
    {
        ResistanceDebuff resistanceDebuff = (ResistanceDebuff)m_buff;
        float reductionMultiplicator = 1 - (resistanceDebuff.m_reductionPercentage / 100);
        targetStats.m_defense /= reductionMultiplicator;
        targetStats.m_elementaryDefense /= reductionMultiplicator;
    }
}