using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab/StabAbility", fileName = "StabAbility")]
public class StabAbility : Ability
{
    [SerializeField] protected float m_attackRadius;
    public override void Activate(GameObject parent)
    {
        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;
        BasicDaggerAttack(parent, playerPhysicalAttack * 0.5f);
    }

    protected Collider2D BasicDaggerAttack(GameObject parent, float damage)
    {
        var attackPoint = parent.GetComponent<CombatSystem>().GetAttackPoint();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, m_attackRadius);
        Collider2D hitEnemy = null;

        if (hitEnemies != null)
        {
            foreach (var enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss"))
                {
                    enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Physical, damage);
                    Debug.Log("Damage dealt to " + enemy.name + ": " + damage);

                    hitEnemy = enemy;
                }
            }
        }

        return hitEnemy;
    }
}