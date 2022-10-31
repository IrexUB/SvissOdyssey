using UnityEngine;
using Mirror;

public class SyncMe : NetworkBehaviour
{
	public void Setup(NetworkConnectionToClient conn, float disappearTime)
    {
		// Si c'est le serveur on le fait simplement spawn
		if (isServer)
		{
			NetworkServer.Spawn(gameObject, conn);
			return;
		}

		// Si ce n'est pas le serveur
		// On envoie une commande au serveur pour faire spawn l'objet sur les clients
		CmdSpawnMe();
	}

	[Command(requiresAuthority=false)]
	private void CmdSpawnMe()
    {
		NetworkServer.Spawn(gameObject);
    }
}
