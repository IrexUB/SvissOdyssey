using System.Collections;
using UnityEngine;

// Cette classe gère l'évolution du buff/debuff/dot appliqué sur une BuffableEntity
public abstract class TimedBuff
{
    [SerializeField] protected float m_activeTime;

    // Cet variable permet de gérer le fait de pouvoir "stacker" les effets
    // Quand un certain effet ce stack, il devient plus "puissant"
    protected uint m_effectStacks;
    public ScriptableBuff m_buff { get; }

    protected readonly GameObject m_obj;
    public bool m_isFinished;

    public TimedBuff(ScriptableBuff buff, GameObject obj)
    {
        m_buff = buff;
        m_obj = obj;
    }

    public void Tick(float deltaTime)
    {
        m_activeTime -= deltaTime;
        if (m_activeTime < 0)
        {
            End();
            m_isFinished = true;
        }
    }

    public void Activate()
    {
        if (m_buff.m_isEffectStacked || m_activeTime <= 0)
        {
            ApplyEffect();
            m_effectStacks++;
        }

        if (m_buff.m_isDurationStacked || m_activeTime <= 0)
        {
            m_activeTime += m_buff.m_duration;
        }

        if (m_buff.m_isDurationRefreshable || m_activeTime <= 0)
        {
            m_activeTime = m_buff.m_duration;
        }
    }

    protected abstract void ApplyEffect();
    public abstract void End();
}