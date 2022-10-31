using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Cette classe doit-être équipé sur toute entité susceptible de subir des buff/debuff/dot
// Elle repose sur un dictionnaire stockant le type d'effet ainsi que la classe gérant l'évolution de cet effet dans le temps.
public class BuffableEntity : MonoBehaviour
{
    [SerializeField] private Dictionary<ScriptableBuff, TimedBuff> m_buffs = new Dictionary<ScriptableBuff, TimedBuff>();
    void Update()
    {
        foreach (var buff in m_buffs.Values.ToList())
        {
            buff.Tick(Time.deltaTime);
            if (buff.m_isFinished)
            {
                m_buffs.Remove(buff.m_buff);
            }
        }
    }

    public void AddBuff(TimedBuff buff)
    {
        if (m_buffs.ContainsKey(buff.m_buff))
        {
            m_buffs[buff.m_buff].Activate();
        }
        else
        {
            m_buffs.Add(buff.m_buff, buff);
            buff.Activate();
        }
    }
}