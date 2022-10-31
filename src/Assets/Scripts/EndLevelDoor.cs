using UnityEngine;
using Mirror;

public class EndLevelDoor : NetworkBehaviour
{
	[SyncVar] private bool loadScene = false;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Si la porte a déjà était passé
		if (loadScene) return;

		if (collision.CompareTag("Player"))
        {
			if(collision.TryGetComponent(out PlayerController pController))
            {
				if (pController.GetRealPlayerCollider == collision)
                {
					loadScene = true;
					CustomNetworkManager.singleton.LoadNextScene();
				}
            }
		}
	}

}
