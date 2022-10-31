using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/Hybrid/FleshPutrefactionAbility", fileName = "FleshPutrefactionAbility")]
public class FleshPutrefactionAbility : VersatilityAbility
{
    [SerializeField] protected GameObject m_slowAoe;
    [SerializeField] protected float m_aoeRadius;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);
        CoroutineHelper.instance.StartCoroutine(SlowNearbyEnemies(parent));
    }

    public override void BeginCooldown(GameObject parent)
    {
        base.BeginCooldown(parent);
    }

    public IEnumerator SlowNearbyEnemies(GameObject parent)
    {
        var aoe = Instantiate(m_slowAoe, parent.transform.position, Quaternion.identity, parent.transform);
        aoe.transform.localScale = new Vector3(m_aoeRadius, m_aoeRadius, 0);
        aoe.AddComponent<CircleCollider2D>().isTrigger = true;
        aoe.GetComponent<SpriteRenderer>().sortingLayerName = parent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        yield return new WaitForSeconds(m_activeTime);

        Destroy(aoe);
    }
}