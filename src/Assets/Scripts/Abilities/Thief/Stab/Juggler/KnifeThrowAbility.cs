using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Stab/Juggler/KnifeThrowAbility", fileName = "KnifeThrowAbility")]
public class KnifeThrowAbility : Ability
{
    [SerializeField] protected GameObject m_knifePrefab;
    [SerializeField] protected float m_knifeVelocity;
    protected float m_playerPhysicalAttack;
    protected ProjectileBehaviour m_knifeThrowBehaviour;

    private void OnEnable()
    {
        // KnifeThrowBehaviour.OnCollisionEnterEvent += CallKnifeThrow;
    }

    private void OnDisable()
    {
        // KnifeThrowBehaviour.OnCollisionEnterEvent -= CallKnifeThrow;
    }

    public override void Activate(GameObject parent)
    {
        InstantiateKnife(parent);
        SoundManager.Create(GameUtils.SoundList.LANCER_COUTEAU, parent.transform.position, parent.transform.rotation);
    }

    public void InstantiateKnife(GameObject parent)
    {
        m_playerPhysicalAttack = parent.GetComponent<PlayerStats>().m_physicalAttack;

        var attackPoint = parent.GetComponent<CombatSystem>().GetAttackPoint();
        float shootAngle = Utility.GetAngleTowardsMouse(attackPoint);
        Quaternion knifeRotation = Quaternion.Euler(new Vector3(0f, 0f, shootAngle - 90f));

        var knife = Instantiate(m_knifePrefab, attackPoint.position, knifeRotation);
        m_knifeThrowBehaviour = knife.AddComponent<ProjectileBehaviour>();
        m_knifeThrowBehaviour.OnCollisionEnterEvent += CallKnifeThrow;

        m_knifeThrowBehaviour.m_projectileVelocity = m_knifeVelocity;
    }

    protected void CallKnifeThrow(GameObject enemy)
    {
        Debug.Log("Called knife !");
        enemy.GetComponent<Stats>().DealDamage(Stats.AttackType.Physical, m_playerPhysicalAttack * 0.5f);
    }
}