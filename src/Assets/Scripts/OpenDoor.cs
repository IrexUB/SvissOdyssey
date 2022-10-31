using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	private void HandleGameStateChange(GameUtils.GameStates currentGameState, GameUtils.GameStates previousGameState)
	{
		if (currentGameState == GameUtils.GameStates.PLAY)
			gameObject.SetActive(false);
	}

	void Start()
	{
		if (GameManager.instance.gameStart) gameObject.SetActive(false);
		else GameManager.instance.EventGameStateChanged += HandleGameStateChange;
	}

    private void OnDisable()
	{
		// Si il y a encore le gamemanager
		if(GameManager.instance)
			GameManager.instance.EventGameStateChanged -= HandleGameStateChange;
	}
}
