using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Dot/PoisonDot", fileName = "PoisonDot")]
public class PoisonDot : ScriptableBuff
{
    [SerializeField] public float m_dotPercentage;
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedPoisonDot(this, obj);
    }
}