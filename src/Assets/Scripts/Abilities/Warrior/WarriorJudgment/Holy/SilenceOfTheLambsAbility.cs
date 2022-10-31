using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WarriorJudgment/Holy/SilenceOfTheLambsAbility", fileName = "SilenceOfTheLambsAbility")]
public class SilenceOfTheLambsAbility : EnchantedBladeAbility
{
    [SerializeField] private SilenceDebuff m_silenceDebuff;
    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallSilenceOfTheLambs(parent));
    }

    IEnumerator CallSilenceOfTheLambs(GameObject parent)
    {
        yield return CoroutineHelper.instance.StartCoroutine(CallSharpeBlade(parent));

        var playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;

        var magicalAoe = Instantiate(m_secondAoePrefab, parent.transform.position, Quaternion.identity, parent.transform);
        magicalAoe.transform.localScale = new Vector3(1f, 1f, 1f);
        magicalAoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        var hitEnemies = WarriorJudgmentBase(parent, Stats.AttackType.Elementary, playerPhysicalAttack * 0.75f);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<BuffableEntity>().AddBuff(m_silenceDebuff.InitializeBuff(enemy.gameObject));
        }

        yield return new WaitForSeconds(0.4f);

        Destroy(magicalAoe);
    }

}