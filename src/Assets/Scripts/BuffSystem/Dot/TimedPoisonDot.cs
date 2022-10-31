using System.Collections;
using UnityEngine;

public class TimedPoisonDot : TimedBuff
{
    private IEnumerator m_poisonDotCoroutine;
    public float m_casterElementaryAttack;

    public TimedPoisonDot(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
    }

    protected override void ApplyEffect()
    {
        m_poisonDotCoroutine = PoisonDot();

        CoroutineHelper.instance.StartCoroutine(m_poisonDotCoroutine);
    }

    public override void End()
    {
        CoroutineHelper.instance.StopCoroutine(m_poisonDotCoroutine);

        m_effectStacks = 0;
    }

    public IEnumerator PoisonDot()
    {
        var targetStats = m_obj.GetComponent<Stats>();

        while (true)
        {
            if (m_obj != null)
            {
                PoisonDot poisonDot = (PoisonDot)m_buff;
                targetStats.DealDamage(Stats.AttackType.Elementary, m_casterElementaryAttack * (poisonDot.m_dotPercentage / 100));
            }
            yield return new WaitForSeconds(1f);
        }
    }
}