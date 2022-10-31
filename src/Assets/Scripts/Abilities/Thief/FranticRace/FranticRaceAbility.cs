using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FranticRace/FranticRaceAbility", fileName = "FranticRaceAbility")]
public class FranticRaceAbility : Ability
{
    [SerializeField] protected float m_dashVelocity;
    protected GameObject m_tmpParent;

    protected NearbyCollidersDetector m_detector;
    List<Collider2D> m_previousCollidersDetected;

    private void OnEnable()
    {
        m_detector = FindObjectOfType<NearbyCollidersDetector>();
    }

    public override void Activate(GameObject parent)
    {
        CoroutineHelper.instance.StartCoroutine(CallFranticRace(parent));
    }

    protected IEnumerator CallFranticRace(GameObject parent)
    {
        m_tmpParent = parent;

        PlayerStats playerStats = parent.GetComponent<PlayerStats>();
        float normalSpeed = playerStats.m_speed;
        playerStats.m_speed = m_dashVelocity;

        CancelCollision();

        yield return new WaitForSeconds(m_activeTime);

        RetrievePreviousCollisionState();

        playerStats.m_speed = normalSpeed;

    }

    void CancelCollision()
    {
        m_previousCollidersDetected = NearbyCollidersDetector.GetNearbyColliders();

        foreach(var collider in m_previousCollidersDetected)
        {
            Physics2D.IgnoreCollision(m_tmpParent.GetComponent<Collider2D>(), collider, true);
        }

    }

    void RetrievePreviousCollisionState()
    {
        foreach (var collider in m_previousCollidersDetected)
        {
            Physics2D.IgnoreCollision(m_tmpParent.GetComponent<Collider2D>(), collider, false);
        }
    }
}