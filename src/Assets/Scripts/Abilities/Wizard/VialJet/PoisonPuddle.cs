using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoisonPuddle : MonoBehaviour
{
    [SerializeField] private List<Collider2D> m_targetsInPuddle = new List<Collider2D>();
    public delegate void OnTargetStay(GameObject enemy);
    public event OnTargetStay OnEnemyStayInPuddle;
    public event OnTargetStay OnAllyStayInPuddle;

    public delegate void OnTargetIO(GameObject enemy);
    public event OnTargetIO OnEnemyEnterInPuddle;
    public event OnTargetIO OnEnemyExitPuddle;

    public event OnTargetIO OnAllyExitPuddle;

    private void Start()
    {
        StartCoroutine(AffectTargetInPuddle());
    }

    IEnumerator AffectTargetInPuddle()
    {
        foreach (var target in m_targetsInPuddle)
        {
            if (OnEnemyStayInPuddle != null && (target.CompareTag("Enemy") || target.CompareTag("Boss")))
            {
                OnEnemyStayInPuddle(target.gameObject);
            }

            if (OnAllyStayInPuddle != null && target.CompareTag("Player"))
            {
                OnAllyStayInPuddle(target.gameObject);
            }
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(AffectTargetInPuddle());
    }

    public void AddTargetsToList(Collider2D[] targets)
    {
        foreach (var target in targets)
        {
            if (target.CompareTag("Player") || target.CompareTag("Enemy") || target.CompareTag("Boss"))
            {
                OnTriggerEnter2D(target);
            }
        }
    }

    public void DeleteTargetsFromList()
    {
        foreach (var target in m_targetsInPuddle.ToList())
        {
            if (target.CompareTag("Enemy") || target.CompareTag("Boss") || target.CompareTag("Player") )
            {
                OnTriggerExit2D(target);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_targetsInPuddle.Contains(collision) && (collision.CompareTag("Player") || (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))))
        {
            Debug.Log("Enter collision : " + collision.name);
            if (OnEnemyEnterInPuddle != null && (collision.CompareTag("Enemy") || collision.CompareTag("Boss")))
            {
                OnEnemyEnterInPuddle(collision.gameObject);
            }
            m_targetsInPuddle.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnEnemyExitPuddle != null && (collision.CompareTag("Enemy") || collision.CompareTag("Boss")))
        {
            OnEnemyExitPuddle(collision.gameObject);
        }

        if (OnAllyExitPuddle != null && collision.CompareTag("Player"))
        {
            OnAllyExitPuddle(collision.gameObject);
        }

        m_targetsInPuddle.Remove(collision);
    }
}
