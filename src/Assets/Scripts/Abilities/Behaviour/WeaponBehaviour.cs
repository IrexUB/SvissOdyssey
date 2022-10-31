using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponBehaviour : ScriptableObject
{
    public string m_name;
    public float m_attackRadius;
    public float m_attackDamage;
    public virtual void HandleBasicAttack(Transform attackPoint) { }
}
