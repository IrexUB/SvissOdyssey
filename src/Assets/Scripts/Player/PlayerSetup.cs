using UnityEngine;
using Mirror;

// Script exécuté sur chaque client, il désactive les components que les autres clients n'ont pas à avoir accès sur le joueur crée

public class PlayerSetup : NetworkBehaviour
{
	[SerializeField] private Behaviour[] componentsToDisable;
	[SerializeField] private GameObject[] gameobjectsToDisable;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
		GameManager.instance.RegisterPlayer(netId, this);

		if(isLocalPlayer)
			GameManager.instance.SetLocalPlayerInstance(gameObject);

		// Si le personnage n'appartient pas au client
		if (!isLocalPlayer)
		{
			DisableCollision();
			DisableComponent();
			return;
		}

		// Si c'est un host donc le client et le serveur en même temps
		if(isClient && isServer)
		{
			// On affiche l'ip du joueur car c'est lui qui host
			UIManager.instance.UpdateIpServer(GameUtils.GetLocalIPAddress());
		}
        // Si c'est le client uniquement alors on met l'ip du networkManager
        if (isClientOnly)
        {
			// On affiche l'ip du serveur 
            UIManager.instance.UpdateIpServer(NetworkClient.serverIp);
        }
	}

	private void DisableComponent()
	{
        foreach (var obj in gameobjectsToDisable)
        {
			obj.SetActive(false);
        }

		foreach (var component in componentsToDisable)
		{
			component.enabled = false;
		}
	}

	private void DisableCollision()
    {

		//Physics2D.IgnoreCollision(GameManager.instance.GetLocalPlayer.GetComponent<PlayerController>().GetRealPlayerCollider, transform.GetComponent<PlayerController>().GetRealPlayerCollider);
    }


	private void OnDestroy()
    {
		if (GameManager.instance)
			GameManager.instance.UnRegisterPlayer(netId);
	}
}
