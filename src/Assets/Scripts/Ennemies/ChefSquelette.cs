using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChefSquelette : PEnemyController
{
	[SerializeField]
	private GameObject osPrefab;
	[SerializeField]
	private GameObject squelettePrefab;
	[SerializeField] private float distanceattack = 4f;
	[SerializeField]
	private float speedOs = 2f;
	float nextAttack = 0f;//temps d'attente entre l'attaque et la prochaine
	float waitedTime = 0.1f;
	float distance = 0; //distance entre le joueur et l'ennemi 

	protected override void Update()
	{
		base.Update();

		if (!isServer) return;

		if (playerTarget)
			distance = Vector2.Distance(playerTarget.position, transform.position); //changement de la valeur de la distance ennemi-joueur
		else
			distance = 0;

		if (distance <= 10 && distance != 0)
		{
			waitedTime += Time.deltaTime;
            if (waitedTime >= nextAttack)//on vérifie si le mob peut attquer
            {
				int numberAttack = (int)Random.Range(0, 3);//on choisit une attaque au hasard
				switch (numberAttack)
				{
					case 0:
						nextAttack = 1.5f;
						if (distance <= distanceattack)
							if (playerTarget != null)
								StartCoroutine(LaunchBone());
						break;
					case 1:
						nextAttack = Random.Range(2f, 3f);
						spawnSquelette();
						break;
					default:
						break;
				}
				waitedTime = 0.1f;
			}
		}
	}
	private IEnumerator LaunchBone()//lancement d'un os
	{
		GameObject os = Instantiate(osPrefab, transform.position, Quaternion.identity);
		os.layer = gameObject.layer;
		SpriteRenderer spriteOs = os.GetComponent<SpriteRenderer>();
		spriteOs.sortingLayerName = spriteRenderer.sortingLayerName;

		// Récupération du script de l'os
		Os osScript = os.GetComponent<Os>();
		osScript.Setup(stats.m_physicalAttack);
		
		//Fait spawn l'os coté serveur
		NetworkServer.Spawn(os);

		Vector2 directionOs = playerTarget.position - transform.position;
		os.GetComponent<Rigidbody2D>().AddForce(directionOs * speedOs, ForceMode2D.Impulse);
		yield return new WaitForSeconds(nextAttack);
        if (os)
			Destroy(os);
	}
	private void spawnSquelette()//spawn d'un squelette
	{
		GameObject squelette = Instantiate(squelettePrefab, transform.position, Quaternion.identity);
		Squelette pathSquelette = squelette.GetComponent<Squelette>();
		pathSquelette.SetUpPathFinding(pathFinding);
		NetworkServer.Spawn(squelette);
	}
}
