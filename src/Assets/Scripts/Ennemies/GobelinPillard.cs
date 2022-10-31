using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GobelinPillard : PEnemyController
{
	[SerializeField] private float distanceToAttack = 1f;
	[SerializeField] private const float MAX_ATTACK_TIME = 2f;
	[SerializeField] private float attackTime = MAX_ATTACK_TIME;
	private int nbCoin = 0;

	[SerializeField] private float disappearTime = 3f;


	protected override void Update()
    {
        base.Update();

		// Si on a une target
		if (playerTarget)
		{
			float distance = Vector2.Distance(transform.position, playerTarget.position);
			Debug.Log(distance);
			if (distance <= distanceToAttack)
			{
				if (attackTime <= 0)
				{
					// Attaque du joueur
					StealCoin();
				}
			}

			attackTime -= Time.deltaTime;
		}
	}

	protected override void FixedUpdate()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (nbCoin > 0)
		{
			playerTarget = null;

			// Génération d'une position aléatoire loin
			targetPosAlea = new Vector3(transform.position.x * 2, transform.position.y * 2, transform.position.z * 2);

			return;
		}

		base.FixedUpdate();
	}

	private void StealCoin()
	{
		// Récupération du script de vie de la target
		if (playerTarget.TryGetComponent(out Health pHealth))
		{
			pHealth.ServerTakeDamage(stats.m_physicalAttack);

			// Récupération des pieces
			Inventory inventory = playerTarget.GetComponent<Inventory>();
			int nbPiece = Random.Range(0, inventory.getCoins());

			nbCoin = nbPiece;
			inventory.subCoins(nbPiece);

			attackTime = MAX_ATTACK_TIME;
		}

		Invoke("Disappear", disappearTime);
	}

	private void Disappear()
    {
		NetworkServer.Destroy(gameObject);
    }
}
