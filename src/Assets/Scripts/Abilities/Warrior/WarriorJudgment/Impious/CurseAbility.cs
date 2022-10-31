using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/Impious/CurseAbility", fileName = "CurseAbility")]
public class CurseAbility : LeechAbility
{
    [SerializeField] protected AllStatsReductionDebuff m_statsReductionDebuff;
    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallCurseAbility(parent));
    }

    IEnumerator CallCurseAbility(GameObject parent)
    {
        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;

        var physicalAoe = Instantiate(m_aoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        physicalAoe.transform.localScale = new Vector3(1f, 1f, 1f);
        physicalAoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        var hitEnemies = WarriorJudgmentBase(parent, Stats.AttackType.Physical, playerPhysicalAttack * 1.40f);
        parent.GetComponent<BuffableEntity>().AddBuff(m_vampirismBuff.InitializeBuff(parent));
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<BuffableEntity>().AddBuff(m_statsReductionDebuff.InitializeBuff(enemy.gameObject));
        }

        yield return new WaitForSeconds(0.4f);

        Destroy(physicalAoe);
    }

}