using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlaqueToxic : NetworkBehaviour
{
    private bool isTakingDamage = false;
    private float damage = 10f;

    [Server]
    public void Setup(float newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si ce n'est pas le serveur
        if (!isServer) return;

        if (collision.tag == "Player")
            StartCoroutine(ContinousDamage(collision));
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Si ce n'est pas le serveur
        if (!isServer) return;

        if (collision.tag == "Player")
            if (!isTakingDamage)
                StartCoroutine(ContinousDamage(collision));
    }
    IEnumerator ContinousDamage(Collider2D collision)
    {
        isTakingDamage = true;
        for (int i = 0; i < 10; i++)//applique des degats toutes les secondes pendant 10 secondes
        {
            Health pHealth = collision.GetComponent<Health>();
            pHealth.ServerTakeDamage(damage);

            yield return new WaitForSeconds(1f);
        }
    }
}
