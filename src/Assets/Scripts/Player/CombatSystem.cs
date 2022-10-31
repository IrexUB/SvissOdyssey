using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Cette classe s'occupe de gérer tout le système de combat du joueur
// Elle est aussi la passerelle qui permet de récupérer les compétences équipées par le joueur, ainsi que la touche associée à chacune des compétences...
// Elle exécute de plus un comportement différent en fonction du type d'arme équipée : (Épée, Dague, Bâton)
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

    // La méthode ci-dessous permet le déblocage ou l'amélioration d'une compétence.
    // Si la compétence passé en paramètre existe dans le Dictionnaire, elle est améliorée (écrasé avec la nouvelle compétence)
    // Sinon elle est ajoutée.
    // Elle s'occupe en plus de cela, d'émettre l'événement correspondant à l'ajout d'une nouvelle compétence afin de pouvoir mettre à jour la barre de sort.
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
