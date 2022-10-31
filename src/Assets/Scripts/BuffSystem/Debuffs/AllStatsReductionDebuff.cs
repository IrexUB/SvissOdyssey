using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuff/AllStatsReductionDebuff", fileName = "AllStatsReductionDebuff")]
public class AllStatsReductionDebuff : ScriptableBuff
{
    public float m_reductionPercentage;
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedAllStatsReductionDebuff(this, obj);
    }
}