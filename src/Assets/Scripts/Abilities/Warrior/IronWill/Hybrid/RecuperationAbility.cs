using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IronWill/Hybrid/RecuperationAbility", fileName = "RecuperationAbility")]
public class RecuperationAbility : FleshPutrefactionAbility
{
    private IEnumerator m_regenerationCoroutine;

    public override void Activate(GameObject parent)
    {
        m_regenerationCoroutine = HealthRegeneration(parent);

        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense *= 1.45f;
        playerStats.m_elementaryDefense *= 1.45f;

        CoroutineHelper.instance.StartCoroutine(m_regenerationCoroutine);
        CoroutineHelper.instance.StartCoroutine(SlowNearbyEnemies(parent));
    }

    public override void BeginCooldown(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        playerStats.m_defense /= 1.45f;
        playerStats.m_elementaryDefense /= 1.45f;

        CoroutineHelper.instance.StopCoroutine(m_regenerationCoroutine);
    }

    public IEnumerator HealthRegeneration(GameObject parent)
    {
        var playerStats = parent.GetComponent<PlayerStats>();
        
        while (true)
        {
            playerStats.RegenerateHealth(playerStats.GetMaxHealth() * 0.05f);
            yield return new WaitForSeconds(1f);
        }
    }
}