using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Os : NetworkBehaviour
{
	private float damage = 10f;

	[Server]
	public void Setup(float newDamage)
	{
		damage = newDamage;
	}

	void Update()
	{
		transform.Rotate(new Vector3(0,0,5));
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
}
