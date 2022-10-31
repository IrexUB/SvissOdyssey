using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCollisionManager : MonoBehaviour
{
    [SerializeField] private List<Collider2D> m_colliders = new List<Collider2D>();
    public delegate void DashCollidingEvent(GameObject a, GameObject b);
    public static event DashCollidingEvent OnDashCollidingWithEnemy;

    public List<Collider2D> GetColliders()
    {
        return m_colliders;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_colliders.Contains(collision))
        {
            if (collision.CompareTag("Enemy"))
            {
                if (OnDashCollidingWithEnemy != null)
                {
                    OnDashCollidingWithEnemy(transform.parent.gameObject, collision.gameObject);
                }
                m_colliders.Add(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_colliders.Remove(collision);
    }
}
