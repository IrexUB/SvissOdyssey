using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuff/ResistanceDebuff", fileName = "ResistanceDebuff")]
public class ResistanceDebuff : ScriptableBuff
{
    public float m_reductionPercentage;
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedResistanceDebuff(this, obj);
    }
}