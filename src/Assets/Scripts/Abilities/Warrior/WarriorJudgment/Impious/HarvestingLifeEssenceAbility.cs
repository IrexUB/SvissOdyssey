using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/Impious/HarvestingLifeEssenceAbility", fileName = "HarvestingLifeEssenceAbility")]
public class HarvestingLifeEssenceAbility : CurseAbility
{
    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallHarvestingLifeEssence(parent));
    }

    IEnumerator CallHarvestingLifeEssence(GameObject parent)
    {
        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;

        var physicalAoe = Instantiate(m_aoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        physicalAoe.transform.localScale = new Vector3(1f, 1f, 1f);
        physicalAoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        var hitEnemies = HarvestNearbyEnemy(parent, playerPhysicalAttack * 1.40f);
        parent.GetComponent<BuffableEntity>().AddBuff(m_vampirismBuff.InitializeBuff(parent));
        foreach (var enemy in hitEnemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<BuffableEntity>().AddBuff(m_statsReductionDebuff.InitializeBuff(enemy.gameObject));
            }
        }

        yield return new WaitForSeconds(0.4f);

        Destroy(physicalAoe);
    }

    List<Collider2D> HarvestNearbyEnemy(GameObject parent, float damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(parent.transform.position, m_attackRadius);
        List<Collider2D> filteredEnemies = new List<Collider2D>();

        foreach (var enemy in hitEnemies)
        {   
            if (enemy.CompareTag("Enemy"))
            {
                if (!enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Physical, damage))
                {
                    parent.GetComponent<PlayerStats>().IncreaseVampirismPermanent(0.1f);
                }
                Debug.Log("Deal " + damage + " to " + enemy.name);
                filteredEnemies.Add(enemy);
            }
        }

        return filteredEnemies;
    }
}