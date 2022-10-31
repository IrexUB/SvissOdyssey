using System.Collections;
using UnityEngine;

public class KnifeTargetManager
{
    [SerializeField] private GameObject m_currentTarget;
    [SerializeField] private uint m_knifeStack;
    private bool m_isTargetKillable;


    public delegate void OnKilledEvent(GameObject enemy);
    public event OnKilledEvent OnKilledWithStack;

    public KnifeTargetManager()
    {
        m_knifeStack = 0;
        m_isTargetKillable = false;
    }

    public void IncreaseKnifeStack(GameObject enemy)
    {
        if (m_currentTarget == null && enemy.CompareTag("Enemy"))
        {
            m_currentTarget = enemy;
            m_knifeStack = 1;
        } else
        {
            if (m_currentTarget != enemy && enemy.CompareTag("Enemy"))
            {
                m_currentTarget = enemy;    
                m_knifeStack = 1;
            } else
            {
                m_knifeStack++;
            }
        }

        if (m_knifeStack == 3)
        {
            m_isTargetKillable = true;
            if (OnKilledWithStack != null)
            {
                OnKilledWithStack(enemy);
            }
        }

    }

    public bool IsTargetKillable()
    {
        return m_isTargetKillable;
    }
}