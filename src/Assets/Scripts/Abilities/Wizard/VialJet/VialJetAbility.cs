using System.Collections;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Abilities/VialJet/VialJetAbility", fileName = "VialJetAbility")]
public class VialJetAbility : Ability
{
	[SerializeField] protected float m_puddleRadius;
	[SerializeField] protected GameObject m_poisonPuddleAoe;
	protected PoisonPuddle m_puddle;
	protected PlayerStats m_stats;

	private GameObject tmpPoisonPuddle;
	public override void Activate(GameObject parent)
	{
		var enemies = SpawnPuddle(parent);
		m_puddle.OnEnemyStayInPuddle += VialJetBase;
		m_puddle.AddTargetsToList(enemies);
	}

	protected Collider2D[] SpawnPuddle(GameObject parent)
	{
		m_stats = parent.GetComponent<PlayerStats>();

		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 10;

		Vector3 poisonPuddlePos = Camera.main.ScreenToWorldPoint(mousePos);

		tmpPoisonPuddle = Instantiate(m_poisonPuddleAoe, poisonPuddlePos, Quaternion.identity);
		tmpPoisonPuddle.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;
		m_puddle = tmpPoisonPuddle.AddComponent<PoisonPuddle>();
		m_puddle.transform.localScale = new Vector3(2f * m_puddleRadius, 2f * m_puddleRadius, 1f);

		Collider2D[] enemiesHitOnImpact = Physics2D.OverlapCircleAll(tmpPoisonPuddle.transform.position, m_puddleRadius);
		return enemiesHitOnImpact;
	}

	protected void VialJetBase(GameObject enemy)
	{
		enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Elementary, m_stats.m_elementaryAttack * 0.75f);
	}

	public override void BeginCooldown(GameObject parent)
	{
		Destroy(tmpPoisonPuddle);
	}
}