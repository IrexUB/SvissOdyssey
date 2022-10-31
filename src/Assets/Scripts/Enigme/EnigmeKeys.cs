using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnigmeKeys : NetworkBehaviour
{
	[SerializeField] private List<GameObject> keys = new List<GameObject>();
	[SerializeField] private List<Transform> pos = new List<Transform>();

	private int keyLeft = 0;

	[SerializeField] private GameObject doorToDisable;

	private void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Si on a pas le m�me nombre de cle que de position
		if (keys.Count != pos.Count) return;

		foreach (var _pos in pos)
		{
			_pos.position = new Vector3(_pos.position.x, _pos.position.y, 0);
		}

		for (int i = 0; i < keys.Count; i++)
		{
			// Apparition de la cl�
			Key key = Instantiate(keys[i], pos[i].position, Quaternion.identity).GetComponent<Key>();
			SpriteRenderer _keyRenderer = key.GetComponent<SpriteRenderer>();
			_keyRenderer.sortingOrder = 50;

			key.SetEnigmeKeys(this);

			RpcSpawnKey(i);

			keyLeft++;
		}
	}

	// Fonction ex�cut�e en cas de r�cup�ration de cl� 
	public void RecupKey(GameObject obj)
	{
		Debug.Log("Le joueur a recupere la clef");
		// Si ce n'est pas le serveur
		if (!isServer) return;
		Debug.Log("Condition if pass�e");

		RpcDestroyKey(obj);

		keyLeft--;
		// Si il n'y a plus de cl�
		if (keyLeft <= 0) {
			RpcDestroyDoor();
			Destroy(doorToDisable); 
		}
	}

	// Fonction ex�cut�e sur chaque client pour supprimer la porte
	[ClientRpc]
	private void RpcDestroyDoor()
	{
		Destroy(doorToDisable);
	}

	// Fonction qui permet de faire spawn une cl� sur tout les clients
	[ClientRpc]
	private void RpcSpawnKey(int i)
	{
		if (!isClientOnly) return;

		Debug.Log("condition if pour la cle passee ");
		GameObject key = Instantiate(keys[i], pos[i].position, Quaternion.identity);
		SpriteRenderer _keyRenderer = key.GetComponent<SpriteRenderer>();
		_keyRenderer.sortingOrder = 50;
	}

	// Fonction qui permet de supprimer une cl� sur tout les clients
	[ClientRpc]
	private void RpcDestroyKey(GameObject obj)
	{
		Destroy(obj);
	}
}
