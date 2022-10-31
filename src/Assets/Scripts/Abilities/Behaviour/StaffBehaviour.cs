using System.Collections;
using UnityEngine;

// Classe qui permet l'instanciation d'un nouveau bâton en tant qu'arme principale...
[CreateAssetMenu(fileName = "New Staff", menuName = "ScriptableObjects/Weapons/New Staff")]
public class StaffBehaviour : WeaponBehaviour
{
    [SerializeField] private GameObject m_projectile;

    [SerializeField] private float m_fireCooldown;
    [SerializeField] private float m_staffProjectileVelocity;
    private float m_currentWait;

    private bool m_canFire = true;

    public override void HandleBasicAttack(Transform attackPoint)
    {
        if (Input.GetMouseButtonDown(0) && m_canFire)
        {
            Fire(attackPoint);
        } else
        {
            if (m_currentWait > 0f)
            {
                m_currentWait -= Time.deltaTime;
            } else
            {
                m_currentWait = 0f;
                m_canFire = true;
            }
        }
    }

    private void Fire(Transform attackPoint)
    {
        float shootAngle = Utility.GetAngleTowardsMouse(attackPoint);
        Quaternion projectileAngle = Quaternion.Euler(new Vector3(0f, 0f, shootAngle - 90f));

        var staffProjectile = Instantiate(m_projectile, attackPoint.position, projectileAngle);
        var behavior = staffProjectile.AddComponent<ProjectileBehaviour>();
        behavior.OnCollisionEnterEvent += DealDamageToEnemy;
        behavior.m_projectileVelocity = m_staffProjectileVelocity;

        m_currentWait = m_fireCooldown;

        m_canFire = false;
    }

    void DealDamageToEnemy(GameObject enemy)
    {
        enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Elementary, m_attackDamage);
    }
}