using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/Impious/LeechAbility", fileName = "LeechAbility")]
public class LeechAbility : WarriorJudgmentAbility
{
    [SerializeField] protected VampirismBuff m_vampirismBuff;
    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallLeechAbility(parent));
    }

    IEnumerator CallLeechAbility(GameObject parent)
    {
        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;

        var physicalAoe = Instantiate(m_aoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        physicalAoe.transform.localScale = new Vector3(1f, 1f, 1f);
        physicalAoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        WarriorJudgmentBase(parent, Stats.AttackType.Physical, playerPhysicalAttack * 1.20f);
        parent.GetComponent<BuffableEntity>().AddBuff(m_vampirismBuff.InitializeBuff(parent));

        yield return new WaitForSeconds(0.4f);

        Destroy(physicalAoe);
    }

}