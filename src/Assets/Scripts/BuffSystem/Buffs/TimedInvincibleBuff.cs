using System.Collections;
using UnityEngine;

public class TimedInvincibleBuff : TimedBuff
{
    private PlayerState m_playerState;

    public TimedInvincibleBuff(ScriptableBuff buff, GameObject obj) : base(buff, obj)
    {
        m_playerState = obj.GetComponent<PlayerState>();
    }

    protected override void ApplyEffect()
    {
        m_playerState.SetInvincible(true);
        Debug.Log("Invincibility on !");
    }

    public override void End()
    {
        m_playerState.SetInvincible(false);
        Debug.Log("Invincibility off !");

        m_effectStacks = 0;
    }
}