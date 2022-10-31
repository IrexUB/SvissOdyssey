using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Key : NetworkBehaviour
{
    private EnigmeKeys enigmeKeys;

    public void SetEnigmeKeys(EnigmeKeys enigme) => enigmeKeys = enigme;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si ce n'est pas le serveur
        if (isServer) return;

        if(collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out PlayerController pController))
            {
                if (pController.GetRealPlayerCollider == collision)
                {
                    enigmeKeys.RecupKey(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
