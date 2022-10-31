using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Chimiste : PEnemyController
{
	[SerializeField]
	private GameObject toxicPrefab;
	private bool canAttack = true;
	protected override void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		StartCoroutine(SpawnToxic());
	}
	private IEnumerator SpawnToxic()
	{
        if (canAttack)//si il peut attaquer
		{
			canAttack = false;

			//on instantie une flaque
			GameObject flaque = Instantiate(toxicPrefab, transform.position, Quaternion.identity);

			// Récupération du script de la flaque
			FlaqueToxic flaqueScript = flaque.GetComponent<FlaqueToxic>();
			flaqueScript.Setup(stats.m_elementaryAttack);

			flaque.layer = gameObject.layer;
			SpriteRenderer flaqueRenderer = flaque.GetComponent<SpriteRenderer>();
			flaqueRenderer.sortingLayerName = spriteRenderer.sortingLayerName;

			//on la fait spawn coté serveur
			NetworkServer.Spawn(flaque);

			yield return new WaitForSeconds(1f);
			canAttack = true;

			//on la detruit
			NetworkServer.Destroy(flaque);
		}
	}
}
