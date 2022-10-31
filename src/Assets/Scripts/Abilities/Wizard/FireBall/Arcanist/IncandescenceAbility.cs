using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball/Arcanist/IncandescenceAbility", fileName = "IncandescenceAbility")]
public class IncandescenceAbility : FireballAbility
{
    [SerializeField] protected BurnDot m_burnDot;

    public override void Activate(GameObject parent)
    {
        CastFireball(parent);
        m_fireballBehaviour.OnCollisionEnterEvent += IncandescenceEffect;
    }

    protected void IncandescenceEffect(GameObject enemy)
    {
        enemy.GetComponent<BuffableEntity>().AddBuff(m_burnDot.InitializeBuff(enemy));
    }
}
