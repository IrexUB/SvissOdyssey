using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Stats : MonoBehaviour
{
	[SerializeField] Health m_healthSystem;
	public Health GetHealth { get => m_healthSystem; }

	public float m_physicalAttack;
	public float m_elementaryAttack;
	public float m_defense;
	public float m_elementaryDefense;
	public float m_speed;


	private void Start()
	{
		m_healthSystem = GetComponent<Health>();
		m_healthSystem.CurrentHealth = m_healthSystem.MaxHealth;
	}

	public void RegenerateHealth(float healthAmount)
	{
		if (m_healthSystem.CurrentHealth + healthAmount >= m_healthSystem.MaxHealth)
		{
			m_healthSystem.CurrentHealth = m_healthSystem.MaxHealth;
		}
		else
		{
			m_healthSystem.CurrentHealth += healthAmount;
		}
	}

	public enum AttackType
	{
		Elementary,
		Physical
	};

	public bool DealDamage(AttackType type, float damage)
	{
		var casterStats = GameManager.instance.GetLocalPlayer.GetComponent<PlayerStats>();
		casterStats.RegenerateHealth((casterStats.m_vampirism / 100) * damage);

		Debug.Log("Caster : " + casterStats.name);
		Debug.Log("Regeneration : " + (casterStats.m_vampirism / 100) * damage);
		
		if (m_healthSystem.CurrentHealth - damage > 0)
		{
			switch (type)
			{
				case AttackType.Elementary:
					m_healthSystem.CurrentHealth -= (damage * (1 - (m_elementaryDefense / 100)));
					//DamagePopup.Create(transform.position, m_elementaryAttack, false, true);
					break;
				case AttackType.Physical:
					m_healthSystem.CurrentHealth -= (damage * (1 - (m_defense / 100)));
					//DamagePopup.Create(transform.position, m_physicalAttack, false, true);
					break;
			}
			return true;
		}
		else
		{
			Kill();
			return false;
		}
	}

	public void DecreaseAllStats(float percentage) {
		float percentageMultiplicator = (1 - (percentage / 100));

		m_healthSystem.MaxHealth *= percentageMultiplicator;
		m_healthSystem.CurrentHealth *= percentageMultiplicator;
		m_physicalAttack *= percentageMultiplicator;
		m_elementaryAttack *= percentageMultiplicator;
		m_defense *= percentageMultiplicator;
		m_elementaryDefense *= percentageMultiplicator;
		m_speed *= percentageMultiplicator;
	}

	public void IncreaseAllStats(float percentage)
	{
		float percentageMultiplicator = (1 - (percentage / 100));

		m_healthSystem.MaxHealth /= percentageMultiplicator;
		m_healthSystem.CurrentHealth /= percentageMultiplicator;
		m_physicalAttack /= percentageMultiplicator;
		m_elementaryAttack /= percentageMultiplicator;
		m_defense /= percentageMultiplicator;
		m_elementaryDefense /= percentageMultiplicator;
		m_speed /= percentageMultiplicator;
	}

	public bool DecreaseMaxHealth(float amount)
	{
		if (m_healthSystem.MaxHealth - amount > 0)
		{
			m_healthSystem.MaxHealth -= amount;
			return true;
		}
		else
		{
			Kill();
			return false;
		}
	}

	public void Kill()
	{
		m_healthSystem.CurrentHealth = 0f;
		NetworkServer.Destroy(gameObject);
	}

	public float GetCurrentHealth()
	{
		return m_healthSystem.CurrentHealth;
	}

	public float GetMaxHealth()
	{
		return m_healthSystem.MaxHealth;
	}

	//fonction pour augmenter les stats d'un joueur apres qu'il ai pris un item
	public void UpdateStatsItem(Item item)
	{
		var otherStats = GameManager.instance.GetLocalPlayer.GetComponent<PlayerStats>();

		m_healthSystem.MaxHealth *= (1 + (item.HP/100));
		m_physicalAttack *= (1 + (item.AP / 100));
		m_elementaryAttack *= (1 + (item.AE / 100));
		m_defense *= (1 + (item.DEF / 100));
		m_elementaryDefense *= (1 + (item.DEF_ELE / 100));
		m_speed *= (1 + (item.SPEED / 100));
		otherStats.m_vampirism += item.VAMP;
		otherStats.m_cooldownReduction *= (1 + (item.CDR / 100));
	}

	//fonction pour baisser les stats d'un joueur apres qu'il ai laché un item
	public void DowngradeStatsItem(Item item)
	{
		var otherStats = GameManager.instance.GetLocalPlayer.GetComponent<PlayerStats>();

		m_healthSystem.MaxHealth /= (1 + (item.HP / 100));
		m_physicalAttack /= (1 + (item.AP / 100));
		m_elementaryAttack /= (1 + (item.AE / 100));
		m_defense /= (1 + (item.DEF / 100));
		m_elementaryDefense /= (1 + (item.DEF_ELE / 100));
		m_speed /= (1 + (item.SPEED / 100));
		otherStats.m_vampirism -= item.VAMP;
		otherStats.m_cooldownReduction /= (1 + (item.CDR / 100));
	}
}
