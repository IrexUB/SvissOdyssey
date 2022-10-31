using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BossTWO : NetworkBehaviour
{
	[SerializeField] private Stats stats;

	[SerializeField] private const float WAIT_FOR_SHOOT = 5f;
	private float timerForShoot = WAIT_FOR_SHOOT;
	
	[SerializeField] private List<Transform> teleportPos = new List<Transform>();
	[SerializeField] private float teleportTime = 10f;

	[SerializeField] private GameObject projectile;
	[SerializeField] private GameObject activeOnDeath;

	private void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (!stats) stats = GetComponent<Stats>();

		// Mise en place de la téléportation toute les teleportTime secondes
		InvokeRepeating("Teleport", teleportTime, teleportTime);
	}

	private void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		timerForShoot -= Time.deltaTime;

		if(timerForShoot <= 0)
		{
			timerForShoot = WAIT_FOR_SHOOT;

			// Récupération d'un joueur aléatoire
			var players = NetworkServer.connections;
			int randPlayer = Random.Range(0, players.Count);
			Transform player = players[randPlayer].identity.transform;

			Vector2 dir = player.position - transform.position;
			GameObject sProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
			Rigidbody2D rbProjectile = sProjectile.GetComponent<Rigidbody2D>();
			rbProjectile.AddForce(dir * stats.m_speed, ForceMode2D.Impulse);

			// Récupération du script du projectile
			BossTwoProjectile projectileScript = sProjectile.GetComponent<BossTwoProjectile>();
			projectileScript.Setup(stats.m_elementaryAttack);

			NetworkServer.Spawn(sProjectile);
		}
	}

	private void Teleport()
	{
		int randPos = Random.Range(0, teleportPos.Count);
		transform.position = teleportPos[randPos].position;
	}

	private void OnDestroy()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		RpcActiveOnDeath();

		activeOnDeath.SetActive(true);
	}

	[ClientRpc]
	private void RpcActiveOnDeath()
	{
		activeOnDeath.SetActive(true);
	}
}
