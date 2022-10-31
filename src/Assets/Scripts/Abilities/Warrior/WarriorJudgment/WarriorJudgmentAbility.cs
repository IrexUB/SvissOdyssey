using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/WarriorJudgmentAbility", fileName = "WarriorJudgmentAbility")]
public class WarriorJudgmentAbility : Ability
{
    [SerializeField] protected float m_attackRadius;
    [SerializeField] protected GameObject m_aoePrefab;

    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(WarriorJudgment(parent));
    }

    protected virtual List<Collider2D> WarriorJudgmentBase(GameObject parent, Stats.AttackType type, float damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(parent.transform.position, m_attackRadius);
        List<Collider2D> filteredEnemies = new List<Collider2D>();

        foreach (var enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss"))
            {
                enemy.GetComponent<Stats>().DealDamage(type, damage);
                Debug.Log("Deal " + damage + " to " + enemy.name);
                filteredEnemies.Add(enemy);
            }
        }

        return filteredEnemies;
    }

    private IEnumerator WarriorJudgment(GameObject parent)
    {
        var aoe = Instantiate(m_aoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        aoe.transform.localScale = new Vector3(1f, 1f, 1f);
        aoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        var playerStats = parent.GetComponent<PlayerStats>();

        WarriorJudgmentBase(parent, Stats.AttackType.Physical, playerStats.m_physicalAttack);

        yield return new WaitForSeconds(0.4f);

        Destroy(aoe);
    }
}
