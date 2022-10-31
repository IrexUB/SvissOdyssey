using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using Mirror;
using System.Net;

public class MainMenu : MonoBehaviour
{
	[Header("MainScreen")]
	[SerializeField] private GameObject mainScreen;
	[Header("GameOverScreen")]
	[SerializeField] private GameObject gameOverScreen;

	[Header("Start Menu")]
	[SerializeField] private GameObject startMenu;

	[Header("Options Menu")]
	[SerializeField] private GameObject optionsMenu;

	[Header("Story Menu")]
	[SerializeField] private GameObject storyMenu;
	[SerializeField] private PlayableDirector directorCut;

	[Header("Network Menu")]
	[SerializeField] private GameObject networkHUD;
	[SerializeField] private InputField IPInputField;
	[SerializeField] private Text IpText;
	[SerializeField] private Text localIP;
	private bool correctIP = false;

	private bool loadingScene = false;

	private void Start()
	{
		// Activation du menu principal
		mainScreen.SetActive(true);
		// Désactivation et configuration du menu start
		startMenu.SetActive(false);
		// Désactivation et configuration du menu options
		optionsMenu.SetActive(false);
		// Désactivation et configuration du menu lose
		gameOverScreen.SetActive(false);

		storyMenu.SetActive(false);

		// Désactivation et configuration du networkHUD
		networkHUD.SetActive(false);
		IpText.text = CustomNetworkManager.singleton.networkAddress;
		UpdateLocalIPText();
	}

	public void ShowLoseMenu()
    {
		mainScreen.SetActive(false);
		gameOverScreen.SetActive(true);
    }

    #region MainMenu | LoseMenu
    private void Update()
    {
		if (!mainScreen.activeSelf && !gameOverScreen.activeSelf) return;
        if(Input.anyKeyDown)
        {
			mainScreen.SetActive(false);
			gameOverScreen.SetActive(false);
			startMenu.SetActive(true);
        }
    }
    #endregion

    #region Start Menu

    public void StartButton()
	{
		startMenu.SetActive(false);

		networkHUD.SetActive(true);
	}

	public void OptionsButton()
	{
		// Affichage du menu option uniquement
		startMenu.SetActive(false);
		optionsMenu.SetActive(true);
	}

	public void QuitButton()
	{
		Application.Quit();
	}

	public void StoryButton()
    {
		startMenu.SetActive(false);
		storyMenu.SetActive(true);
		directorCut.Play();
    }
	#endregion

	#region Options Menu
	#endregion

	#region Network Menu

	public void HostButton()
	{
		if (loadingScene) return;

		loadingScene = true;

		StartCoroutine(IEHostButton());
	}
	private IEnumerator IEHostButton()
	{
		// Changement de la scène de boot
		yield return SceneManager.LoadSceneAsync(CustomNetworkManager.singleton.GetBootScene, LoadSceneMode.Additive);

		CustomNetworkManager.singleton.StartHost();
	}
	public void JoinWithIPButton()
	{
		if (loadingScene) return;

		// Si l'ip n'est pas valide on return
		if (!correctIP) return;

		loadingScene = true;

		StartCoroutine(IEJoinWithIPButton());
	}
	private IEnumerator IEJoinWithIPButton()
    {
		// Changement de la scène de boot
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(CustomNetworkManager.singleton.GetBootScene, LoadSceneMode.Additive);
		while (!asyncLoad.isDone)
		{
			yield return null;
		}

		// On change l'ip du NetworkManager par l'ip du champs
		CustomNetworkManager.singleton.networkAddress = IpText.text;
		// Connexion au serveur
		CustomNetworkManager.singleton.StartClient();
	}

	public void OnChangeIpField()
    {
		// Vérification de la validité de l'adresse rentré dans le champs
		IPAddress ip;
		bool validIP = IPAddress.TryParse(IPInputField.text, out ip);
		
		// Si l'adresse est valide
		if (validIP)
		{
			correctIP = true;
			IpText.color = Color.green;
		} else
        {
			correctIP = false;
			IpText.color = Color.red;
		}
	}
	public void UpdateLocalIPText()
    {
		localIP.text = GameUtils.GetLocalIPAddress();
	}

	#endregion

	#region Utils button function

	public void ReturnButton()
    {
		// Désactivation de tout les menus
		networkHUD.SetActive(false);
		optionsMenu.SetActive(false);
		storyMenu.SetActive(false);

		// Activation menu de départ
		startMenu.SetActive(true);
    }

	#endregion
}
