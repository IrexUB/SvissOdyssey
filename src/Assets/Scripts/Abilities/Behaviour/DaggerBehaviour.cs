using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dagger", menuName = "ScriptableObjects/Weapons/New Dagger")]
public class DaggerBehaviour : WeaponBehaviour
{
    private bool m_canAttack;
    [SerializeField] private float m_cooldown;
    private float m_currentWait;
    public override void HandleBasicAttack(Transform attackPoint)
    {
        if (Input.GetMouseButtonDown(0) && m_canAttack)
        {
            CallDaggerAttack(attackPoint);
            SoundManager.Create(GameUtils.SoundList.COUTEAU, attackPoint.transform.position, attackPoint.transform.rotation);
        }
        else
        {
            if (m_currentWait > 0f)
            {
                m_currentWait -= Time.deltaTime;
            }
            else
            {
                m_currentWait = 0f;
                m_canAttack = true;
            }
        }
    }

    void CallDaggerAttack(Transform attackPoint)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, m_attackRadius);
        if (hitEnemies != null)
        {
            foreach (var enemy in hitEnemies)
            {
                // Ici nous sommes obligé de filtrer à la "main" le fait que le Collider2D soit associé à un ennemi ou un boss
                // Une autre approche était de faire cela avec un LayerMask, cependant les Layers sont déjà utilisé pour gérer le concept de verticalité dans le jeu.
                if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss"))
                {
                    enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Physical, m_attackDamage);
                    Debug.Log("Hit enemy: " + enemy.name);

                    m_currentWait = m_cooldown;
                    m_canAttack = false;

                    break;
                }
            }
        }
    }
}