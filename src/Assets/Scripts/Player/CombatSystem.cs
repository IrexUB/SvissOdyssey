using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Cette classe s'occupe de g�rer tout le syst�me de combat du joueur
// Elle est aussi la passerelle qui permet de r�cup�rer les comp�tences �quip�es par le joueur, ainsi que la touche associ�e � chacune des comp�tences...
// Elle ex�cute de plus un comportement diff�rent en fonction du type d'arme �quip�e : (�p�e, Dague, B�ton)
public class CombatSystem : MonoBehaviour
{
    [SerializeField()] private WeaponBehaviour m_currentWeapon;
    [SerializeField()] private Transform m_attackPoint;
    [SerializeField()] Dictionary<int, Ability> m_abilities = new Dictionary<int, Ability>();
    [SerializeField()] List<AbilityHolder> m_holders;

    public delegate void SkillEvent(List<AbilityHolder> holders);
    public static SkillEvent SkillAdded;
    public static SkillEvent SkillUpdate;

    private void Start()
    {
        m_holders = GetComponents<AbilityHolder>().ToList();

        SkillTreeManager.OnSkillUnlocked += TriggerOnSkillUnlock;

        if (SkillAdded != null)
        {
            SkillAdded(m_holders);
        }
    }

    void Update()
    {
        m_currentWeapon.HandleBasicAttack(m_attackPoint);

        if (SkillUpdate != null)
        {
            SkillUpdate(m_holders);
        }
     }

    public Transform GetAttackPoint()
    {
        return m_attackPoint;
    }

    // La m�thode ci-dessous permet le d�blocage ou l'am�lioration d'une comp�tence.
    // Si la comp�tence pass� en param�tre existe dans le Dictionnaire, elle est am�lior�e (�cras� avec la nouvelle comp�tence)
    // Sinon elle est ajout�e.
    // Elle s'occupe en plus de cela, d'�mettre l'�v�nement correspondant � l'ajout d'une nouvelle comp�tence afin de pouvoir mettre � jour la barre de sort.
    void TriggerOnSkillUnlock(Skill unlockedSkill) {
        if (!(unlockedSkill.m_dependencies.Length > 0))
        {
            m_abilities.Add(unlockedSkill.m_id, unlockedSkill.m_abilityToUnlock);
        } else
        {
            UpgradeMajorSkill(unlockedSkill);
        }

        UpdateAbilityHolders();

        if (SkillAdded != null)
        {
            SkillAdded(m_holders);
        }
    }

    void UpdateAbilityHolders()
    {
        var i = 0;
        foreach (var ability in m_abilities)
        {
            m_holders[i].m_ability = ability.Value;
            i++;
        }
    }

    void UpgradeMajorSkill(Skill upgrade)
    {
        ReplaceDictionnaryKey(upgrade.m_dependencies[0], upgrade.m_id);
        m_abilities[upgrade.m_id] = upgrade.m_abilityToUnlock;
    }

    void ReplaceDictionnaryKey(int oldKey, int newKey)
    {
        var value = m_abilities[oldKey];
        m_abilities.Remove(oldKey);

        m_abilities.Add(newKey, value);
    }

    private void OnDrawGizmos()
    {
        Color color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2);
    }
}
