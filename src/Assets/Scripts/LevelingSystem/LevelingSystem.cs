using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingSystem : MonoSingleton<LevelingSystem>
{
    [SerializeField] private int m_currentLevel;
    [SerializeField] private int m_currentXp;
    [SerializeField] private int m_xpToNextLevel;

    public delegate void LevelIncreaseEvent();
    public static event LevelIncreaseEvent OnLevelIncrease;

    public delegate void LevelUpEvent(int level);
    public static event LevelUpEvent OnLevelUp;

    public delegate void ExperienceIncreaseEvent(float currentXp);
    public static event ExperienceIncreaseEvent OnXpAdd;

    private void Start()
    {
        m_currentLevel = 1;
        SetLevel(m_currentLevel);
    }

    public bool AddExperience(int experienceToAdd)
    {
        m_currentXp += experienceToAdd;

        var xpPercentage = m_currentXp / (float)m_xpToNextLevel;

        if (OnXpAdd != null)
        {
            OnXpAdd(xpPercentage);
        }

        if (m_currentXp >= m_xpToNextLevel)
        {
            IncreaseLevel();

            if (OnXpAdd != null)
            {
                OnXpAdd(0);
            }

            if (OnLevelUp != null)
            {
                OnLevelUp(m_currentLevel);
            }

            return true;
        }

        return false;
    }

    public void IncreaseLevel()
    {
        m_currentLevel++;
        SetLevel(m_currentLevel);
        if (OnLevelIncrease != null)
        {
            OnLevelIncrease();
        }
    }

    private void SetLevel(int level)
    {
        this.m_currentLevel = level;

        m_currentXp -= m_xpToNextLevel;
        m_xpToNextLevel = (int)(50f * (Mathf.Pow(level + 1, 2) - (5 * (level + 1)) + 8));
    }
}
