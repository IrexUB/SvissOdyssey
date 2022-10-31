using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> m_skills;

    private void Start()
    {
        Debug.Log("Start !");
        CombatSystem.SkillAdded += OnNewSkillAdded;
        CombatSystem.SkillUpdate += OnSkillUpdate;
    }

    private void OnNewSkillAdded(List<AbilityHolder> holders)
    {
        var i = 0;
        foreach (var holder in holders)
        {
            if (holder.m_ability != null)
            {
                m_skills[i].GetComponent<Image>().sprite = holder.m_ability.m_sprite;
                m_skills[i].transform.GetChild(0).GetComponent<Image>().sprite = holder.m_ability.m_sprite;
            }
            m_skills[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = holder.Key.ToString();

            ++i;
        }
    }

    private void OnSkillUpdate(List<AbilityHolder> holders)
    {
        var i = 0;
        foreach (var holder in holders)
        {
            if (holder.m_ability != null)
            {
                m_skills[i].GetComponent<Image>().fillAmount = holder.CooldownPercentage;
            }

            ++i;
        }
    }
}
