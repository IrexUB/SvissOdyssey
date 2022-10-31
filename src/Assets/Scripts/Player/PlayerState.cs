using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private bool m_isImmuneToCrowdControl;
    [SerializeField] private bool m_isInvisible;
    [SerializeField] private bool m_isInvincible;

    private void Start()
    {
        m_isInvisible = false;
    }

    public void SetImmuneToCrowdControlState(bool state)
    {
        m_isImmuneToCrowdControl = state;
    }

    public bool IsImmuneToCrowdControl()
    {
        return m_isImmuneToCrowdControl;
    }

    public void SetInvisible(bool state)
    {
        m_isInvisible = state;
    }
    public bool IsInvisible()
    {
        return m_isInvisible;
    }

    public void SetInvincible(bool state)
    {
        m_isInvincible = state;
    }
    public bool IsInvincible()
    {
        return m_isInvincible;
    }
}
