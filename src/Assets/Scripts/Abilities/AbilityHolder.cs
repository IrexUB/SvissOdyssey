using System.Collections;
using UnityEngine;

// Cette classe gère l'état d'une abilité
// Son comportement est simple, une touche est associé à cet AbilityHolder ainsi qu'une Ability (ScriptableObject)
// Lorsque la touche est pressée l'abilité passe à travers différents états et exécute le comportement correspondant.
public class AbilityHolder : MonoBehaviour
{
    public Ability m_ability;
    [SerializeField] private float m_cooldownTime;
    private float m_cooldownIncrease = 0f;
    private float m_activeTime;
    private int m_currentReactivableStack;

    public enum AbilityState
    {
        Ready,
        Active,
        Cooldown
    };

    private AbilityState m_currentState = AbilityState.Ready;
        
    [SerializeField()] private KeyCode m_keybind;
    public KeyCode Key
    {
        get { return m_keybind; }
    }

    public float CooldownPercentage
    {
        get { return 1 - (m_cooldownTime / (m_ability.m_cooldownTime + m_cooldownIncrease)); }
    }

    public delegate void AbilityAction();
    public static event AbilityAction AbilityCalledEvent;


    private void Start()
    {
        m_currentReactivableStack = 0;
        ChronoMaitreAbility.OnCooldownReduction += RestoreCooldown;
        DivineProtectionAbility.OnRestoreCooldown += RestoreCooldown;
        TrillOfTheHuntAbility.ReloadCooldown += RestoreCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ability != null)
        {
            switch (m_currentState)
            {
                case AbilityState.Ready:
                    if (Input.GetKeyDown(m_keybind))
                    {
                        if (AbilityCalledEvent != null)
                        {
                            AbilityCalledEvent?.Invoke();
                        }

                        m_ability.Activate(gameObject);
                        m_currentState = AbilityState.Active;
                        m_activeTime = m_ability.m_activeTime;
                    }
                    break;
                case AbilityState.Active:
                    if (m_activeTime > 0)
                    {
                        m_activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        m_ability.BeginCooldown(gameObject);
                        m_currentState = AbilityState.Cooldown;
                        m_cooldownTime = m_ability.m_cooldownTime * (1 - (GetComponent<PlayerStats>().m_cooldownReduction / 100));
                        m_activeTime = 0;
                    }
                    break;
                case AbilityState.Cooldown:
                    if (m_cooldownTime > 0)
                    {
                        if (m_ability.m_isReactivable && Input.GetKeyDown(m_keybind))
                        {
                            if (m_currentReactivableStack < m_ability.m_reactivationStack)
                            {
                                m_ability.Reactivate(gameObject);
                                m_currentReactivableStack++;

                                m_cooldownTime += m_ability.m_reactivationCooldownIncrease;
                                m_cooldownIncrease += m_ability.m_reactivationCooldownIncrease;
                            }
                        }
                        m_cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        m_currentState = AbilityState.Ready;
                        m_cooldownTime = 0;
                        m_currentReactivableStack = 0;
                        m_cooldownIncrease = 0f;
                    }
                    break;
            }
        }
    }

    public void RestoreCooldown(float restorePercentage)
    {
        if (m_currentState == AbilityState.Cooldown)
        {
            Debug.Log("Restore cooldown by " + restorePercentage + "%");
            m_cooldownTime -= m_cooldownTime * (restorePercentage / 100);
        }
    }
}