using System.Collections;
using UnityEngine;

public class FireballBehaviour : ProjectileBase
{
    public event CollisionEnterEvent OnCollisionEnterEventOffensive;
    public event CollisionEnterEvent OnCollisionEnterEventDefensive;

    public bool m_recall;
    public float m_playerSpeed;
    public Transform casterPosition;

    private float epsilon = 2f;

    private void Update()
    {
        // Le bout de programme ci-dessous n'est pas à sa place, cependant il était bien plus simple de le mettre ici.
        // Celui-ci permet à la Boule'Merang d'être rappelée.
        if (m_recall)
        {
            transform.position = Vector3.Lerp(transform.position, casterPosition.position, (m_playerSpeed * 1.2f) * Time.deltaTime);
            m_projectileVelocity = 0;
            m_recall = (transform.position - casterPosition.position).magnitude >= epsilon;

            if (m_recall == false)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (OnCollisionEnterEventDefensive != null)
            {
                OnCollisionEnterEventDefensive(collision.gameObject);
            }

        }
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            if (OnCollisionEnterEventOffensive != null)
            {
                OnCollisionEnterEventOffensive(collision.gameObject);
            }
        }
    }
}