using System.Collections;
using UnityEngine;

public class TimedInvisiblityBuff : TimedBuff
{
    private PlayerState m_playerState;

    public TimedInvisiblityBuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
        m_playerState = obj.GetComponent<PlayerState>();
        AbilityHolder.AbilityCalledEvent += CancelInvisiblity;
    }

    protected override void ApplyEffect()
    {
        m_playerState.SetInvisible(true);
        Debug.Log("Invisibility on !");
    }

    void CancelInvisiblity()
    {
        Debug.Log("Invisibility canceled...");
        m_activeTime = -1;

        AbilityHolder.AbilityCalledEvent -= CancelInvisiblity;
    }

    public override void End()
    {
        m_playerState.SetInvisible(false);
        Debug.Log("Invisibility off !");

        m_effectStacks = 0;
    }
}