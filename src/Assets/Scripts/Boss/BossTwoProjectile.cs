using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BossTwoProjectile : NetworkBehaviour
{
	private float damage = 10f;
	private const float DISAPPEAR_TIME = 2f;

	[Server]
	public void Setup(float newDamage)
	{
		damage = newDamage;

		Invoke("Despawn", DISAPPEAR_TIME);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (collision.tag == "Player")
		{
			Health pHealth = collision.GetComponent<Health>();
			pHealth.ServerTakeDamage(damage);
			NetworkServer.Destroy(gameObject);
		}
	}

	private void Despawn()
    {
		NetworkServer.Destroy(gameObject);
    }
}
