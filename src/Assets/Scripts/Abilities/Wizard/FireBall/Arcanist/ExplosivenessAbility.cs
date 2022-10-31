using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball/Arcanist/ExplosivenessAbility", fileName = "ExplosivenessAbility")]
public class ExplosivenessAbility : IncandescenceAbility
{
    [SerializeField] protected GameObject m_explosionAoe;
    [SerializeField] protected float m_explosionRadius;

    public override void Activate(GameObject parent)
    {
        CastFireball(parent);
        m_fireballBehaviour.OnCollisionEnterEvent += CallExplosiveness;
    }

    void CallExplosiveness(GameObject firstEnemyHit)
    {
        var explosionAoe = Instantiate(m_explosionAoe, firstEnemyHit.transform.position, Quaternion.identity);
        explosionAoe.transform.localScale = new Vector3(2 * m_explosionRadius, 2 * m_explosionRadius, 1f);
        explosionAoe.GetComponent<SpriteRenderer>().sortingLayerName = m_tmpParent.GetComponent<PlayerController>().GetGFX.sortingLayerName;

        Collider2D[] enemyHit = Physics2D.OverlapCircleAll(explosionAoe.transform.position, m_explosionRadius);
        foreach (var enemy in enemyHit)
        {   
            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss"))
            {
                if (enemy != firstEnemyHit)
                {
                    Debug.Log("Affected enemy : " + enemy.name);
                    enemy.GetComponent<BuffableEntity>().AddBuff(m_burnDot.InitializeBuff(enemy.gameObject));
                }
            }
        }

        Destroy(explosionAoe, 0.4f);

        m_fireballBehaviour.OnCollisionEnterEvent -= CallExplosiveness;
    }
}
