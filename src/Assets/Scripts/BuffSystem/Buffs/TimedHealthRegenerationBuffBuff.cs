using System.Collections;
using UnityEngine;

public class TimedHealthRegenerationBuffBuff : TimedBuff
{

    private IEnumerator m_healthRegeneration;

    public TimedHealthRegenerationBuffBuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
    }

    protected override void ApplyEffect()
    {
        m_healthRegeneration = HealthRegeneration();
        CoroutineHelper.instance.StartCoroutine(m_healthRegeneration);
    }

    public override void End()
    {
        CoroutineHelper.instance.StopCoroutine(m_healthRegeneration);

        m_effectStacks = 0;
    }

    public IEnumerator HealthRegeneration()
    {
        var targetStats = m_obj.GetComponent<Stats>();

        while (true)
        {
            if (m_obj != null)
            {
                HealthRegenerationBuff regenerationBuff = (HealthRegenerationBuff)m_buff;
                targetStats.RegenerateHealth(targetStats.GetMaxHealth() * (regenerationBuff.m_buffPercentage / 100));
            }
            yield return new WaitForSeconds(1f);
        }
    }
}