using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/Holy/EnchantedBladeAbility", fileName = "EnchantedBladeAbility")]
public class EnchantedBladeAbility : SharpBladeAbility
{
    [SerializeField] protected GameObject m_secondAoePrefab;
    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallEnchantedBlade(parent));
    }

    protected IEnumerator CallEnchantedBlade(GameObject parent)
    {
        yield return CoroutineHelper.instance.StartCoroutine(CallSharpeBlade(parent));

        var playerStats = parent.GetComponent<PlayerStats>();

        var magicalAoe = Instantiate(m_secondAoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        magicalAoe.transform.localScale = new Vector3(1f, 1f, 1f);
        magicalAoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        WarriorJudgmentBase(parent, Stats.AttackType.Elementary, playerStats.m_physicalAttack * 0.75f);

        yield return new WaitForSeconds(0.4f);

        Destroy(magicalAoe);
    }
}