using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUIManager : MonoSingleton<SkillTreeUIManager>
{
	[Header("Ability Tree UI")]
	[SerializeField] private GameObject abilityTreeUI;
	public bool GetAbilityTreeState { get => abilityTreeUI.activeSelf; }

    private void Start()
    {
		abilityTreeUI.SetActive(false);
    }

    #region Arbre de compétence
    public void ChangeStateTreeAbility()
	{
		UIManager.instance.AbilityTreeChangeState(!abilityTreeUI.activeSelf);

		abilityTreeUI.SetActive(!abilityTreeUI.activeSelf);
	}

	#endregion
}
