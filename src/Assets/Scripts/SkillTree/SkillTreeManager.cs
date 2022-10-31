using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public delegate void SkillUnlockedEvent(Skill unlockedSkill);
    public static event SkillUnlockedEvent OnSkillUnlocked;

    [SerializeField] List<Skill> m_skillTree;

    private Skill m_currentSkill;
    [SerializeField] private Text m_currentSkillDescription;
    [SerializeField] private Text m_currentSkillCost;
    [SerializeField] private Image m_currentSkillImage;
    [SerializeField] private Button m_buyButton;

    [SerializeField] private Sprite m_unlockState;
    [SerializeField] private Sprite m_lockState;

    [SerializeField] private Text m_lvlText;
    [SerializeField] private Slider m_xpBar;

    [SerializeField] private Text m_currentSkillsPointsText;
    [SerializeField] private int m_skillsPoints;

    private void Start()
    {
        LevelingSystem.OnLevelIncrease += IncreaseSkillsPoints;
        LevelingSystem.OnXpAdd += UpdateXpBar;
        LevelingSystem.OnLevelUp += UpdateLevelState;

        UpdateLevelState(1);
        UpdateXpBar(0);

        m_skillsPoints = 1;
        UpdateSkillsPointsState();
    }

    public void OnSkillClick(int skillId)
    {
        m_currentSkill = m_skillTree[skillId];

        m_currentSkillDescription.text = m_currentSkill.m_abilityToUnlock.m_description;
        m_currentSkillImage.sprite = m_currentSkill.m_abilityToUnlock.m_sprite;
        m_currentSkillCost.text = "Coût : " + m_currentSkill.m_cost.ToString() + " points";

        UpdateBuyButtonState();
    }

    public void TryToUnlockSkill()
    {
        if (m_buyButton != null)
        {
            if (CanSkillBeUnlocked())
            {
                m_skillsPoints -= m_currentSkill.m_cost;
                m_currentSkill.m_unlocked = true;

                UpdateSkillsPointsState();

                if (OnSkillUnlocked != null)
                {

                    OnSkillUnlocked(m_currentSkill);
                }
                UpdateBuyButtonState();
            }
        }
    }

    private bool CanSkillBeUnlocked()
    {
        if (m_currentSkill != null)
        {
            if (m_currentSkill.m_unlocked)
            {
                return false;
            }

            var dependencies = m_currentSkill.m_dependencies;
            foreach (var dependency in dependencies)
            {
                if (!IsSkillUnlocked(dependency))
                {
                    return false;
                }
            }

            var constraints = m_currentSkill.m_constraints;
            foreach (var constraint in constraints)
            {
                if (IsSkillUnlocked(constraint))
                {
                    return false;
                }
            }

            if (m_skillsPoints < m_currentSkill.m_cost)
            {
                return false;
            }

            return true;
        }

        return false;
        
    }

    private bool IsSkillUnlocked(int skillId)
    {
        return (m_skillTree[skillId].m_unlocked) ? true : false;
    }

    private void UpdateBuyButtonState()
    {
        if (m_currentSkill.m_unlocked)
        {
            m_buyButton.image.sprite = m_unlockState;
        } else
        {
            m_buyButton.image.sprite = m_lockState;
        }
    }

    private void IncreaseSkillsPoints()
    {
        m_skillsPoints++;
        UpdateSkillsPointsState();
    }

    private void UpdateSkillsPointsState()
    {
        m_currentSkillsPointsText.text = m_skillsPoints.ToString();
    }

    private void UpdateXpBar(float currentPercentage)
    {
        m_xpBar.value = currentPercentage;
    }

    private void UpdateLevelState(int level)
    {
        m_lvlText.text = "Lvl: " + level.ToString();
    }
}
