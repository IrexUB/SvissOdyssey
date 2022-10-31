using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoSingleton<ItemManager>
{
	// Listes des differents items en fonction de leur rarete
	public List<Item> ItemRarete1;
	public List<Item> ItemRarete2;
	public List<Item> ItemRarete4;

	// Fonction qui set up les deux items uniques
	private void Start()
	{
		foreach (var item in ItemRarete4)
		{
			item.isUnique = true;
			item.isPicked = false;
		}
	}

	// Fonction qui retourne un item en fonction de sa rarete
	public Item GetItem(int listId, int number)
	{
		switch (listId)
		{
			case 1:
				return ItemRarete1[number];
			case 2:
				return ItemRarete2[number];
			case 4:
				{
					// On verifie si l'item unique a deja etait ramasse
					if (ItemRarete4[number].isPicked == false)
					{
						return ItemRarete4[number];	
					}
					return null;
				}
			default:
				return null;
				break;
		}
	}
}
