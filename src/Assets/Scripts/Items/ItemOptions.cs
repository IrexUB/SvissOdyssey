using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ItemOptions : NetworkBehaviour
{
	[SerializeField] private Item _item;
	public Item GetItem { get => _item; }
}
