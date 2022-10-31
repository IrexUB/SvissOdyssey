using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Kamikaze : PEnemyController
{
	[SerializeField] private float explodeRadius;
	[SerializeField] private float distanceForExplode = 2f;
	[SerializeField] private float timeExplode = 2f;

	private bool isExplode = false;

	// Update ex�cut�e uniquement c�t� serveur
	protected override void Update()
	{
		base.Update();

		// V�rification si l'ennemi � un joueur en target
		if (playerTarget == null) return;

		// Calcul de la distance entre le joueur et l'ennemi
		float distance = Vector2.Distance(transform.position, playerTarget.position);
		if (distance <= distanceForExplode && !isExplode)
		{
			SoundManager.Create(GameUtils.SoundList.EXPLOSION, transform.position, transform.rotation);
			explode();
		}
	}

    [Server]
	private void explode()
	{
		isExplode = true;
		// R�cup�ration de la liste des objets au niveau de l'explosion

		ContactFilter2D _contactFilter = new ContactFilter2D();
		_contactFilter.SetDepth(0, 0);
		_contactFilter.SetLayerMask(layerDetect);

		List<Collider2D> colliders = new List<Collider2D>();
		Physics2D.OverlapCircle(transform.position, explodeRadius, _contactFilter, colliders);

        foreach (var collider in colliders)
        {
			if (collider.name == name) continue;
            
			if(collider.TryGetComponent(out Health health))
			{
				Debug.Log("Joueur");
				health.ServerTakeDamage(stats.m_physicalAttack);
            }
        }

		NetworkServer.Destroy(gameObject);
	}
}
