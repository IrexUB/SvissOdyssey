using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public float m_cooldownReduction; // In percentage
    public float m_vampirism; // In percentage

    public void IncreaseVampirismPermanent(float percentage)
    {
        m_vampirism *= (1f + percentage);
        Debug.Log("Vampirism increased of : " + m_vampirism);
    }
}
