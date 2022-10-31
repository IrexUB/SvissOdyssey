using System.Collections;
using UnityEngine;

public class TimedBurnDot : TimedBuff
{
    private IEnumerator m_burnDotCoroutine;

    public TimedBurnDot(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
    }

    protected override void ApplyEffect()
    {
        m_burnDotCoroutine = DotBurn();

        CoroutineHelper.instance.StartCoroutine(m_burnDotCoroutine);
    }

    public override void End()
    {
        CoroutineHelper.instance.StopCoroutine(m_burnDotCoroutine);

        m_effectStacks = 0;
    }

    public IEnumerator DotBurn()
    {
        var targetStats = m_obj.GetComponent<Stats>();

        while (true)
        {
            if (m_obj != null)
            {
                targetStats.DealDamage(Stats.AttackType.Elementary, targetStats.GetMaxHealth() * 0.05f);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}