using System.Collections;
using UnityEngine;
using Mirror;

public class ProjectileBehaviour : ProjectileBase
{
    public event CollisionEnterEvent OnCollisionEnterEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            if (OnCollisionEnterEvent != null)
            {
                OnCollisionEnterEvent(collision.gameObject);
            } else
            {
                Debug.Log("Null event");
            }

             Destroy(gameObject);
        } else if (collision.CompareTag("Utils"))
        {
            ;
        } else
        {
            Destroy(gameObject);
        }
    }
}