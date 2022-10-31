using System.Collections;
using UnityEngine;

public class TimedVampirismBuff : TimedBuff
{
    private PlayerStats m_playerStats;
    public TimedVampirismBuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
        m_playerStats = obj.GetComponent<PlayerStats>();
    }

    protected override void ApplyEffect()
    {
        VampirismBuff vampirismBuff = (VampirismBuff)m_buff;    
        m_playerStats.m_vampirism *= (1 + (vampirismBuff.m_buffPercentage / 100));
        Debug.Log("VAMPIRISM BUFF : " + "+" + vampirismBuff.m_buffPercentage + "%");
    }

    public override void End()
    {
        VampirismBuff vampirismBuff = (VampirismBuff)m_buff;
        m_playerStats.m_vampirism /= (1 + (vampirismBuff.m_buffPercentage / 100));
        Debug.Log("VAMPIRISM RESTORED : " + "-" + vampirismBuff.m_buffPercentage + "%");

        m_effectStacks = 0;
    }
}