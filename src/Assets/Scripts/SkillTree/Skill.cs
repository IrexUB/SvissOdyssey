using System.Collections;
using UnityEngine;

[System.Serializable]

// L'arbre de compétence repose sur cette classe
// Effectivement, elle permet de fixer toutes les conditions de déblocage d'une certaine abilité
// Les dépendances qu'elle doit remplir ainsi que les contraintes :
// Si une certaine abilité est déjà débloquée, le déblocage de la capacité possédant cette contrainte est impossible.
public class Skill
{
    public int m_id;

    public int[] m_dependencies;
    public int[] m_constraints;

    public bool m_unlocked;
    public int m_cost;

    public Ability m_abilityToUnlock;
}