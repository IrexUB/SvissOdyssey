using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuff/SilenceDebuff", fileName = "SilenceDebuff")]
public class SilenceDebuff : ScriptableBuff
{ 
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedSilenceDebuff(this, obj);
    }
}