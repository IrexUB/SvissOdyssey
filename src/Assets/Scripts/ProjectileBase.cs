using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public float m_projectileVelocity;
    private Rigidbody2D m_rb2d;

    public delegate void CollisionEnterEvent(GameObject obj);
    void Start()
    {
        m_rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rb2d.velocity = transform.up * m_projectileVelocity;
    }
}