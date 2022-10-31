using System.Collections;
using UnityEngine;

// Cette classe est le "modèle" nous permettant de construire toutes les compétences
// Elle est utilisé par l'arbre de compétences ainsi que l'AbilityHolder
public class Ability : ScriptableObject
{
    public string m_name;
    [TextArea(1, 3)]
    public string m_description;
    public Sprite m_sprite;

    public float m_cooldownTime;
    public float m_activeTime;

    public bool m_isReactivable;
    public uint m_reactivationStack;
    public float m_reactivationCooldownIncrease;


    // On surcharge la fonction Activate() qui va effectuer le comportement défini  
    // Ex: Dash, boule de feu...
    public virtual void Activate(GameObject parent) { }
    public virtual void Reactivate(GameObject parent) { }

    // Cette fonction permet de lancer un comportement à la fin du temps d'activation de l'abilité.
    public virtual void BeginCooldown(GameObject parent) { }
}