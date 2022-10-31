using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEffect : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enemiesInRange.Contains(collision.gameObject) && (collision.CompareTag("Enemy") || collision.CompareTag("Boss")))
        {
            enemiesInRange.Add(collision.gameObject);
            collision.GetComponent<Stats>().m_speed *= 0.85f;
            Debug.Log("Effect on : " + collision.name + " applied ");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enemiesInRange.Contains(collision.gameObject))
        {
            enemiesInRange.Remove(collision.gameObject);
            collision.GetComponent<Stats>().m_speed /= 0.85f;
            Debug.Log("Effect on : " + collision.name + " removed ");
        }
    }
}
