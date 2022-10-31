using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab/Juggler/TrillOfTheHuntAbility", fileName = "TrillOfTheHuntAbility")]
public class TrillOfTheHuntAbility : PTAAbility
{
    private BuffableEntity m_buffableEntity;
    [SerializeField] private SpeedBuff m_speedBuff;

    public delegate void ReloadCooldownEvent(float percentage);
    public static event ReloadCooldownEvent ReloadCooldown;

    private void OnEnable()
    {
        m_knifeStackManager.OnKilledWithStack += CallTrillOfTheHunt;
    }

    public override void Activate(GameObject parent)
    {
        m_buffableEntity = parent.GetComponent<BuffableEntity>();
        InstantiateKnife(parent);
        m_knifeThrowBehaviour.OnCollisionEnterEvent += CallPTA;
    }

    protected void CallTrillOfTheHunt(GameObject enemy)
    {
        TriggerKillEnemyWithStack(enemy);
        m_buffableEntity.AddBuff(m_speedBuff.InitializeBuff(m_buffableEntity.gameObject));
        Debug.Log("TRILL OF THE HUNT CALLED !");
        if (ReloadCooldown != null)
        {
            ReloadCooldown(100);
        }
    }
}