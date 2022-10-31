using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PressurePlateNumber : NetworkBehaviour
{
    [SerializeField] private int representValue = 1;
    public int GetRepresentValue { get => representValue; }

    private bool isGoodPlate = false;
    private int id;

    private EnigmeNumber responseComponent;

    public void SetResponseComponent(EnigmeNumber enigmeNumber) => responseComponent = enigmeNumber;
    public void SetIsGoodPlate() => isGoodPlate = true;
    public void SetId(int newId) => id = newId;

    private List<Transform> allTriggers = new List<Transform>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si ce n'est pas le serveur
        if (!isServer) return;

        if (!isGoodPlate) return;

        // On regarde si c'est un joueur
        if (collision.TryGetComponent(out PlayerController pController))
        {
            // Si ce n'est pas le vrai collider du joueur
            if (pController.GetRealPlayerCollider != collision) return;
        }

        allTriggers.Add(collision.transform);
        responseComponent.UpdateActivatePlate(true, id);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si ce n'est pas le serveur
        if (!isServer) return;

        if (!isGoodPlate) return;

        allTriggers.Remove(collision.transform);
        if(allTriggers.Count <= 0)
            responseComponent.UpdateActivatePlate(false, id);
    }
}
