using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/Holy/SharpBladeAbility", fileName = "SharpBladeAbility")]
public class SharpBladeAbility : WarriorJudgmentAbility
{
    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallSharpeBlade(parent));
    }

    protected IEnumerator CallSharpeBlade(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();

        var physicalAoe = Instantiate(m_aoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        physicalAoe.transform.localScale = new Vector3(1f, 1f, 1f);
        physicalAoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;
        WarriorJudgmentBase(parent, Stats.AttackType.Physical, playerStats.m_physicalAttack * 1.5f);

        yield return new WaitForSeconds(0.4f);

        Destroy(physicalAoe);
    }
}