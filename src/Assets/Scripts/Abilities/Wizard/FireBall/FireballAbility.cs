using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu]
public class FireballAbility : Ability
{
	[SerializeField] protected GameObject m_fireballPrefab;
	[SerializeField] protected float m_fireballVelocity;
	protected float m_elementaryAttack;
	protected ProjectileBehaviour m_fireballBehaviour;
	protected GameObject m_tmpParent;

	public override void Activate(GameObject parent)
	{
		CastFireball(parent);
		SoundManager.Create(GameUtils.SoundList.FIREBALL, parent.transform.position, parent.transform.rotation);
	}

	protected void FireballEffect(GameObject enemy)
	{
		enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Elementary, m_elementaryAttack);
		Debug.Log("Enemy hit :" + enemy.name);
	}

	protected GameObject CastFireball(GameObject parent)
	{
		m_tmpParent = parent;

		m_elementaryAttack = parent.GetComponent<PlayerStats>().m_elementaryAttack;

		var attackPoint = parent.GetComponent<CombatSystem>().GetAttackPoint();
		float shootAngle = Utility.GetAngleTowardsMouse(attackPoint);

		Quaternion fireballAngle = Quaternion.Euler(new Vector3(0f, 0f, shootAngle - 90f));

		var fireball = Instantiate(m_fireballPrefab, attackPoint.position, fireballAngle);

		m_fireballBehaviour = fireball.AddComponent<ProjectileBehaviour>();

		m_fireballBehaviour.m_projectileVelocity = m_fireballVelocity;
		m_fireballBehaviour.OnCollisionEnterEvent += FireballEffect;
	   
		return fireball;
	}
}
