using System.Collections;
using UnityEngine;
using Mirror;

public class Experience : NetworkBehaviour
{
	public static Experience Create(Vector2 pos, int numberOfEXP)
	{
		// Cr�ation des numberOFExp-1 restante
		for (int i = 0; i < numberOfEXP - 1; i++)
		{
			Create(pos, 0);
		}

		GameObject instance = Instantiate(GameManager.instance.experiencePrefab, pos, Quaternion.identity);
		Experience exp = instance.GetComponent<Experience>();

		NetworkServer.Spawn(instance);

		return exp;
	}

	private float speed = 1f;
	private float timeForRecover = 2f;
	private float canRecoverDistance = 0.2f;

	private Transform playerTarget;
	private bool isRecovered = false;

	// A la cr�ation d'une boule d'exp�rience
	private void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Application d'une rotation al�atoire
		transform.Rotate(new Vector3(0, 0, Random.Range(0.0f, 360.0f)));
	}

	private void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Si l'orb d'exp�rience a �t� r�cup�r�
		if (isRecovered) return;

		// Si on peut r�cup�rer l'exp�rience ET que l'on n'a pas de target
		if (timeForRecover < 0 && !playerTarget)
		{
			float distance = float.MaxValue;

			var players = GameManager.instance.GetPlayers;
			foreach (var player in players)
			{
				float dis = Vector2.Distance(transform.position, player.transform.position);
				if(dis < distance)
				{
					distance = dis;
					playerTarget = player.transform;
				}
			}
		} else
		{
			timeForRecover -= Time.deltaTime;
		}

		// Si on a un joueur
		if(playerTarget)
		{
			speed += Time.deltaTime;

			// Si la distance entre le joueur en target et la boule d'exp�rience
			float distance = Vector2.Distance(transform.position, playerTarget.position);
			if(distance <= canRecoverDistance)
			{
				StartCoroutine(TargetCanTakeExp());
			}

			// On ce dirige vers le joueur
			Vector2 dir = playerTarget.position - transform.position;
			dir.Normalize();
			Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, dir);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * 2);
		}
		
		transform.Translate(Vector3.up * speed * Time.deltaTime);
	}

	[Server]
	private IEnumerator TargetCanTakeExp()
    {
		// Si ce n'est pas le serveur
		if (!isServer) yield break;

		isRecovered = true;

		// Envoie au client qu'il a r�cup�r� de l'exp�rience
		NetworkIdentity targetIdentity = playerTarget.GetComponent<NetworkIdentity>();
		TargetRecoverEXP(targetIdentity.connectionToClient);

		// D�sactivation du spriteRenderer
		GetComponent<SpriteRenderer>().enabled = false;
		RpcDisableSpriteRenderer();

		// On attend 2 secondes
		yield return new WaitForSeconds(2);

		// Destruction de la boule d'exp�rience
		NetworkServer.Destroy(gameObject);
	}

	[ClientRpc]
	private void RpcDisableSpriteRenderer()
    {
		GetComponent<SpriteRenderer>().enabled = false;
    }

	[TargetRpc]
	private void TargetRecoverEXP(NetworkConnection target) 
	{
		// [UNCOMPLETE]
		var experienceGivedByBubble = 4;
		LevelingSystem.instance.AddExperience(experienceGivedByBubble);
		Debug.Log("R�cup�ration d'experience pour le joueur : " + target.identity.name);
	}
}
