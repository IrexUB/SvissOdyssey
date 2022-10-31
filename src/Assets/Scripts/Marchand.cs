using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Marchand : NetworkBehaviour
{
	private readonly SyncList<Item> itemsMarchand = new SyncList<Item>();
	public SyncList<Item> GetInventory { get => itemsMarchand; }

	[SerializeField] private Vector3 spawnPos;
	[SerializeField] private Vector3 baseSpawn;

	[SerializeField] private int numberItemDrop;

	// Fonction qui génère les différents items du marchand côté serveur
	void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Génération 
		for (int i = 0; i < 4; i++)//initialisation de l'inventaire du marchand
		{
			float randomList = Random.Range(0, 1000);
			if (randomList < 650)
			{
				// Récupération du nombre d'item de la rareté 1
				int nbItem = ItemManager.instance.ItemRarete1.Count-1;
				itemsMarchand.Add(ItemManager.instance.GetItem(1, Random.Range(0, nbItem)));
				continue;
			}
			if (randomList < 990)
			{
				// Récupération du nombre d'item de la rareté 2
				int nbItem = ItemManager.instance.ItemRarete2.Count - 1;
				itemsMarchand.Add(ItemManager.instance.GetItem(2, Random.Range(0, nbItem)));
				continue;
			}
			if (990 < randomList)
			{
				// Récupération du nombre d'item de la rareté 4
				int nbItem = ItemManager.instance.ItemRarete4.Count - 1;
				int rdmItem = Random.Range(0, nbItem);
                if (!ItemManager.instance.GetItem(4, rdmItem).isPicked)
				{
					itemsMarchand.Add(ItemManager.instance.GetItem(4, rdmItem));
					continue;
				}
                else
                {
					i--;
                }
			}
		}
		numberItemDrop = 0;
		baseSpawn = spawnPos;
	}
	// Fonction qui fait acheter l'item au joueur
	[Command(requiresAuthority=false)]
	public void CmdBuyItem(NetworkIdentity caller, int idPos)
	{
		// Récupération du script Inventaire du joueur
		Inventory pInventory = caller.GetComponent<Inventory>();
		Debug.Log(pInventory.name);

		if (pInventory.getCoins() >= itemsMarchand[idPos].cost)
		{
			Debug.Log("JE PEUX PRENDRE");
			pInventory.subCoins(itemsMarchand[idPos].cost);
			ServerDropItem(idPos);
		}
	}
	// Fonction qui drop l'item coté serveur
	[Server]
	private void ServerDropItem(int idPos)
    {
		if (ItemManager.instance.GetItem(itemsMarchand[idPos].rarete, itemsMarchand[idPos].id))
		{
			numberItemDrop++;
			if (numberItemDrop > 0)
				spawnPos = new Vector3(spawnPos.x + 1, spawnPos.y , spawnPos.z); 
			GameObject newItem = Instantiate(itemsMarchand[idPos].GFX, spawnPos, Quaternion.identity);
			newItem.layer = gameObject.layer;
			SpriteRenderer _itemSprite = newItem.GetComponent<SpriteRenderer>();
			_itemSprite.sortingOrder = 2;
			_itemSprite.sortingLayerName = LayerMask.LayerToName(gameObject.layer);
			ItemManager.instance.GetItem(itemsMarchand[idPos].rarete, itemsMarchand[idPos].id).isPicked = true;
			NetworkServer.Spawn(newItem);
			RPCSetupDropItem(newItem);
		}
	}
	// Fonction qui drop l'item coté client
	[ClientRpc]
	private void RPCSetupDropItem(GameObject objectSpawn)
    {
		objectSpawn.layer = gameObject.layer;
		SpriteRenderer _itemSprite = objectSpawn.GetComponent<SpriteRenderer>();
		_itemSprite.sortingOrder = 2;
		_itemSprite.sortingLayerName = LayerMask.LayerToName(gameObject.layer);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (!collision.transform.parent.CompareTag("Player")) return;

		// Si c'est bien un joueur
		if (collision.transform.parent.TryGetComponent(out PlayerController pController))
        {
			if (pController.GetDashCollider == collision)
			{
				pController.marchandNear = this;
			}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
		if (!collision.transform.parent.CompareTag("Player")) return;

		// Si c'est bien un joueur
		if (collision.transform.parent.TryGetComponent(out PlayerController pController))
		{
			if (pController.GetDashCollider == collision)
			{
				if(pController.marchandNear == this)
					pController.marchandNear = null;
			}
		}
	}
}
