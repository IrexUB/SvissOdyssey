using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SorciereGlace : PEnemyController
{
	[SerializeField]
	private GameObject glacePrefab;
	bool canLaunch = true;
	[SerializeField] private float distanceattack = 10f;
	[SerializeField]
	private float speedIce = 4f;
	protected override void Start()
    {
		base.Start();
		canLaunch = true;
    }

	protected override void Update()
    {
		base.Update();
		if (playerTarget != null)
		{
			float distance = Vector2.Distance(playerTarget.position, transform.position);
			if (distance <= distanceattack) //si la distance entre le joueur et la sorciere est inferieure a la distance d'attaque de la sorciere, on lance un "flocon"
				StartCoroutine(LaunchIce());
		}
	}
	IEnumerator LaunchIce()
	{
		if (canLaunch)
		{
			canLaunch = false;
			//spawn de la glace
			GameObject glace = Instantiate(glacePrefab, transform.position, Quaternion.identity);
			//spawn de la glace coté serveur
			NetworkServer.Spawn(glace);
			glace.layer = gameObject.layer;

			SpriteRenderer spirteGlace = glace.GetComponent<SpriteRenderer>();
			spirteGlace.sortingLayerName = spriteRenderer.sortingLayerName;

			Vector2 directionIce = playerTarget.position - transform.position;
			glace.GetComponent<Rigidbody2D>().AddForce(directionIce * speedIce, ForceMode2D.Impulse);

			yield return new WaitForSeconds(1f);
			//destruction de la glace
			if (glace)
				NetworkServer.Destroy(glace);
			canLaunch = true;
		}
	}
}
