using UnityEngine;
using Mirror;

public class BossHealthTower : NetworkBehaviour
{
	private Health bossHealth;
	private const float WAIT_FOR_HEALTH = 5f;
	private float timerForHealth = WAIT_FOR_HEALTH;

	[Header("Options")]
	[SerializeField] private float healthPower = 3;
	
	[Server]
	private void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Récupération du script du boss
		bossHealth = GameObject.FindGameObjectWithTag("Boss").GetComponent<Health>();
	}

	// Toute les x secondes on régénère le boss;
	private void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		timerForHealth -= Time.deltaTime;
		if(timerForHealth < 0)
        {
			timerForHealth = WAIT_FOR_HEALTH;

			// Application d'un effet de régénération sur le boss
			bossHealth.ApplyEffect(GameUtils.Effects.INSTANT_HEAL, 0, healthPower);
		}
	}
}
