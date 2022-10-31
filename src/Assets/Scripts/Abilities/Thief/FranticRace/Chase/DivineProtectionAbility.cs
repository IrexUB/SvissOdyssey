using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/Chase/DivineProtectionAbility", fileName = "DivineProtectionAbility")]
public class DivineProtectionAbility : ChronoMaitreAbility
{
    public static event CooldownReductionEvent OnRestoreCooldown;
    [SerializeField] private InvincibleBuff m_invicibilityBuff;

    public override void Activate(GameObject parent)
    {
        DashCollisionManager.OnDashCollidingWithEnemy += CallDivineProtection;

        CoroutineHelper.instance.StartCoroutine(CallFranticRace(parent));
        parent.GetComponent<BuffableEntity>().AddBuff(m_invicibilityBuff.InitializeBuff(parent));

        if (OnRestoreCooldown != null)
        {
            OnRestoreCooldown(100);
        }
    }
    public override void BeginCooldown(GameObject parent)
    {
        DashCollisionManager.OnDashCollidingWithEnemy -= CallDivineProtection;
    }

    void CallDivineProtection(GameObject parent, GameObject enemy)
    {
        CallHustleAbility(parent, enemy);
    }
}