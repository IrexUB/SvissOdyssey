using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab/Juggler/PTAAbility", fileName = "PTAAbility")]
public class PTAAbility : KnifeThrowAbility
{
    [SerializeField] protected KnifeTargetManager m_knifeStackManager = new KnifeTargetManager();

    private void OnEnable()
    {
        // KnifeThrowBehaviour.OnCollisionEnterEvent += CallPTA;
        m_knifeStackManager.OnKilledWithStack += TriggerKillEnemyWithStack;
    }

    private void OnDisable()
    {
        // KnifeThrowBehaviour.OnCollisionEnterEvent -= CallPTA;
        m_knifeStackManager.OnKilledWithStack -= TriggerKillEnemyWithStack;
    }

    public override void Activate(GameObject parent)
    {
        InstantiateKnife(parent);
        m_knifeThrowBehaviour.OnCollisionEnterEvent += CallPTA;
    }

    protected void CallPTA(GameObject enemy)
    {
        m_knifeStackManager.IncreaseKnifeStack(enemy);
    }

    protected void TriggerKillEnemyWithStack(GameObject enemy)
    {
        Debug.Log("TRIGGER CALL (DEAD)");
        enemy.GetComponent<Stats>().Kill();
    }
}