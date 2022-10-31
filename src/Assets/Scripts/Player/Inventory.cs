using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Inventory : NetworkBehaviour
{
	//Liste d'items dans l'inventaire
	[SerializeField] private Item[] inventaire;
	//Nombre de pieces du joueur
	[SyncVar] [SerializeField] private int coins;
	private Transform nearItemTransform;
	public PlayerController playerController;
	
	//Fonction initialisant l'inventaire a null et les pieces du joueur a 0
	void Start()
	{
		inventaire = new Item[3];
		for (int i = 0; i < inventaire.Length; i++)
		{
			inventaire[i] = null;
		}
		coins = 155;
	}
	//Fonction retournant le nombre de pieces du joueur
	public int getCoins() => coins;
	//Fonction permettant d'enlever des pieces dans l'inventaire du joueur
	[Server]
	public void subCoins(int _coins) => coins -= _coins;
	//Fonction permettant d'ajouter des pieces dans l'inventaire du joueur
	[Server]
	public void addCoins(int _coins) => coins += _coins;
	// Fonction qui retourne tous les items de l'inventaire du joueur
	public Item[] getItemInventory() => inventaire;


	private void Update()
	{
		// Si il y a un item proche
		if(nearItemTransform)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Debug.Log("Touche Appuyée");
				Item item = nearItemTransform.GetComponent<ItemOptions>().GetItem;
				EquipItemInventory(item, 0);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Debug.Log("Touche Appuyée");
				Item item = nearItemTransform.GetComponent<ItemOptions>().GetItem;
				EquipItemInventory(item, 1);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Item item = nearItemTransform.GetComponent<ItemOptions>().GetItem;
				EquipItemInventory(item, 2);
			}
		}
	}
	// Fonction qui équipe un objet dans l'inventaire du joueur
	public void EquipItemInventory(Item newItem, int slotId)
	{
		// Si il n'y a pas le playerController de renseigné alors on le récupère
		if(!playerController) playerController = GameManager.instance.GetLocalPlayer.GetComponent<PlayerController>();

		// S'il y a déjà un objet dans l'emplacement ou l'on veut mettre le nouvel objet
		if (inventaire[slotId] != null)
		{
			Item tempItem = inventaire[slotId];
			playerController.m_stats.DowngradeStatsItem(tempItem);
			inventaire[slotId] = newItem;
			playerController.m_stats.UpdateStatsItem(newItem);
			CmdDropItemInventory(tempItem.rarete, tempItem.id);
		}
		else
		{
			inventaire[slotId] = newItem;
			playerController.m_stats.UpdateStatsItem(newItem);
		}

		// Destruction de l'objet que l'on vient de récupérer
		CmdDestroyObject(nearItemTransform.gameObject);
		nearItemTransform = null;
	}
	// Fonction qui détruit un objet pour tous les clients
	[Command(requiresAuthority=false)]
	private void CmdDestroyObject(GameObject objectToDestroy) => NetworkServer.Destroy(objectToDestroy);
	// Fonction qui drop un objet au sol
	[Command]
	private void CmdDropItemInventory(int rarete, int id)
	{
		// Vérification si l'object existe
		if (ItemManager.instance.GetItem(rarete, id))
		{
			// On converti les paramètres en objet
			Item item = ItemManager.instance.GetItem(rarete, id);
			
			// Création de l'objet sur la carte
			GameObject itemObj = Instantiate(item.GFX, transform.position, transform.rotation);
			Debug.Log("Item instancié" + itemObj.name);
			itemObj.layer = gameObject.layer;
			
			SpriteRenderer spriteRenderer = itemObj.GetComponent<SpriteRenderer>();
			spriteRenderer.sortingOrder = 2;
			spriteRenderer.sortingLayerName = LayerMask.LayerToName(gameObject.layer);

			// Création de l'objet sur tous les clients et configuration du nouvel objet
			NetworkServer.Spawn(itemObj);
			RpcSetupDropItem(itemObj);
		}
	}
	// Fonction qui configure l'objet que l'on vient de faire apparaitre
	[ClientRpc]
	private void RpcSetupDropItem(GameObject objectSpawn)
	{
		objectSpawn.layer = gameObject.layer;
		SpriteRenderer _itemSprite = objectSpawn.GetComponent<SpriteRenderer>();
		_itemSprite.sortingOrder = 2;
		_itemSprite.sortingLayerName = LayerMask.LayerToName(gameObject.layer);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Si c'est un objet alors on le rajoute dans la variable nearItem
		if(collision.tag == "Items")
		{
			nearItemTransform = collision.transform;
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		// Si c'est un objet et que c'est bien l'objet de la variable nearItem
		if(collision.tag == "Items" && nearItemTransform == collision.transform)
		{
			// On retire la possibilité de prendre l'objet
			nearItemTransform = null;
		}
	}
}