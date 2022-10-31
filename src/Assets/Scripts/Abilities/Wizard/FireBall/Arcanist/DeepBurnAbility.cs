using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball/Arcanist/DeepBurnAbility", fileName = "DeepBurnAbility")]
public class DeepBurnAbility : ExplosivenessAbility
{
    [SerializeField] private ResistanceDebuff m_resistanceDebuff;

    public override void Activate(GameObject parent)
    {
        m_elementaryAttack *= 1.5f;
        Debug.Log("Elementary damage dealt :" + m_elementaryAttack);
        CastFireball(parent);
        m_fireballBehaviour.OnCollisionEnterEvent += CallDeepBurn;
    }

    void CallDeepBurn(GameObject firstEnemyHit)
    {
        var explosionAoe = Instantiate(m_explosionAoe, firstEnemyHit.transform.position, Quaternion.identity);
        explosionAoe.transform.localScale = new Vector3(2 * m_explosionRadius, 2 * m_explosionRadius, 1f);
        explosionAoe.GetComponent<SpriteRenderer>().sortingLayerName = m_tmpParent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        Collider2D[] enemyHit = Physics2D.OverlapCircleAll(explosionAoe.transform.position, m_explosionRadius);
        foreach (var enemy in enemyHit)
        {
            if (enemy != firstEnemyHit && (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss")))
            {
                Debug.Log("Affected enemy : " + enemy.name);
                enemy.GetComponent<BuffableEntity>().AddBuff(m_burnDot.InitializeBuff(enemy.gameObject));
                enemy.GetComponent<BuffableEntity>().AddBuff(m_resistanceDebuff.InitializeBuff(enemy.gameObject));
            }
        }

        Destroy(explosionAoe, 0.4f);

        m_fireballBehaviour.OnCollisionEnterEvent -= CallDeepBurn;
    }
}
