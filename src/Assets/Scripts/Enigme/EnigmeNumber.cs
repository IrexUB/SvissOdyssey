using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnigmeNumber : NetworkBehaviour
{
	[SerializeField] private List<StatusNumberGenerator> status = new List<StatusNumberGenerator>();
	[SerializeField] private List<PressurePlateNumber> pressurePlates = new List<PressurePlateNumber>();

	private List<int> validNumber = new List<int>();
	private List<bool> isPressed = new List<bool>();

	[SerializeField] private GameObject doorToDisable;

	private void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if(!doorToDisable) { Debug.LogError("Attention, la variable doorToDisable n'est pas assigné !", this); return; }

		// On génère x nombre aléatoire par rapport au nombre de status
		for (int i = 0; i < status.Count; i++)
		{
			int rdm = Random.Range(1, 9);
			while (validNumber.Contains(rdm))
			{
				rdm = Random.Range(1, 9);
			}

			validNumber.Add(rdm);
			isPressed.Add(false);
		}

		// On affiche les numéros sur les status
		for (int i = 0; i < status.Count; i++)
		{
			status[i].SetNumber(validNumber[i]);
		}

		// Configuration des plaques
		int id = 0;
		foreach (var plate in pressurePlates)
        {
			// Si la status possède un bon nombre
			if(validNumber.Contains(plate.GetRepresentValue))
            {
				plate.SetIsGoodPlate();
				plate.SetId(id);
				plate.SetResponseComponent(this);

				id++;
            }
        }
	}

	[Server]
	public void UpdateActivatePlate(bool state, int id)
    {
		isPressed[id] = state;

		// Vérification que si toute les plates sont appuyé
		int check = 0;
        foreach (var item in isPressed)
        {
			if (!item) check++;
        }
		
		if(check == 0)
        {
			//[UNCOMPLETE] [SOUND] Mettre un son d'ouverture de porte / résolution d'enigme
			RpcDestroyDoor();
			Destroy(doorToDisable);
		}
    }

	[ClientRpc]
	private void RpcDestroyDoor()
    {
		//[UNCOMPLETE] [SOUND] Mettre un son d'ouverture de porte / résolution d'enigme

		Destroy(doorToDisable);
    }
}
