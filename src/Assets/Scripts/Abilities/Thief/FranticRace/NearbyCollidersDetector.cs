using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyCollidersDetector : MonoBehaviour
{
    private static List<Collider2D> m_validColliders = new List<Collider2D>();
    private GameObject m_carrier;

    private void Start()
    {
        m_carrier = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player") && collision.gameObject != m_carrier)
        {
            if (!m_validColliders.Contains(collision))
            {
                m_validColliders.Add(collision);
            }
        }
    }

    public static List<Collider2D> GetNearbyColliders()
    {
        return m_validColliders;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_validColliders.Contains(collision))
        {
            m_validColliders.Remove(collision);
        }
    }
}
