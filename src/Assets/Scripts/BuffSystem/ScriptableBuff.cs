using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cette classe sert de patron à tout les buff/debuf/dot
// Elle contient certaines indications à donner au TimedBuff, comme par exemple le fait de pouvoir "refresh" le buff/debuff/dot
public abstract class ScriptableBuff : ScriptableObject
{
    public string m_name;
    public float m_duration;
    public bool m_isDurationStacked;
    public bool m_isEffectStacked;
    public bool m_isDurationRefreshable;

    public abstract TimedBuff InitializeBuff(GameObject obj);
}
