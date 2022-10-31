using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : MonoSingleton<GameManager>
{
	[Header("Manager Settings")]
	[SerializeField] private GameObject[] _managerPrefabs;
	private List<GameObject> _instanciedManagerPrefabs = new List<GameObject>();
	[SerializeField] private GameObject[] _objectPrefabs;
	private List<GameObject> _instanciedObjectPrefabs = new List<GameObject>();

	[Header("GameState")]
	private GameUtils.GameStates _currentGameState = GameUtils.GameStates.WAIT_PLAYER;

	[Header("Game Parameters")]
	private Dictionary<uint, PlayerSetup> players = new Dictionary<uint, PlayerSetup>();
	public List<PlayerSetup> GetPlayers
    {
        get {
			List<PlayerSetup> finalList = new List<PlayerSetup>();
            foreach (var player in players)
            {
				finalList.Add(player.Value);
            }
			return finalList;
		}
    }
	[SerializeField] private int _startGameTimer = 5;
	private bool _gameStart = false;
	public bool gameStart { get { return _gameStart; } }
	private bool nextSceneLoading = false;

	private GameObject localPlayerInstance;
	public void SetLocalPlayerInstance(GameObject instance) {
		Debug.Log("SetLocalPlayerInstance: " + instance.name);
		localPlayerInstance = instance;
	}
	public GameObject GetLocalPlayer { get => localPlayerInstance; }

	[Header("Prefabs")]
	public GameObject damagePopupPrefab;
	public GameObject experiencePrefab;
	public GameObject coinPrefab;

	[Header("Sounds")]
	public List<GameUtils.SoundList> SoundList;
	public List<SourceAudio> SourceAudioListe;
	public SoundManager _soundManager;

    private void Start()
    {
		
    }

    #region Config Gamemanager
    // Création des différents managers
    protected override void Awake()
    {
        base.Awake();

        InstantiateManagerPrefabs();
		InstantiateObjectsPrefabs();

		_soundManager.FillDictionnary();
    }
	// Fonction qui crée les managers présents dans la variable managerPrefabs
	private void InstantiateManagerPrefabs()
    {
        for (int i = 0; i < _managerPrefabs.Length; i++)
        {
            GameObject manager = Instantiate(_managerPrefabs[i], Vector3.zero, Quaternion.identity);
            _instanciedManagerPrefabs.Add(manager);
        }
    }
	private void InstantiateObjectsPrefabs()
    {
        for (int i = 0; i < _objectPrefabs.Length; i++)
        {
            GameObject obj = Instantiate(_objectPrefabs[i], Vector3.zero, Quaternion.identity);
			DontDestroyOnLoad(obj);
            _instanciedObjectPrefabs.Add(obj);
        }
    }
    // Quand l'objet GameManager est détruit alors on détruit chaque manager que le GameManager a créé
    protected override void OnDestroy()
    {
		base.OnDestroy();

		// Destruction de tous les managers générée par le GameManager
		foreach (var manager in _instanciedManagerPrefabs)
        {
            Destroy(manager);
        }

		DestroyAllSpawnsObjects();
	}
	private void DestroyAllSpawnsObjects()
    {
		// Destruction de tous les objets générée par le GameManager
		foreach (var obj in _instanciedObjectPrefabs)
		{
			Destroy(obj);
		}
    }
    #endregion

    #region Delegates
    public delegate void GameStateChangedDelegate(GameUtils.GameStates currentGameState, GameUtils.GameStates previousGameState);
	public event GameStateChangedDelegate EventGameStateChanged;
	#endregion

	#region GameState Management
	// Fonction qui change le GameState
	private void ChangeGameState(GameUtils.GameStates value)
	{
		GameUtils.GameStates previousGameState = _currentGameState;
		_currentGameState = value;
		EventGameStateChanged?.Invoke(_currentGameState, previousGameState);
	}
    #endregion

    // Enregistre un nouveau joueur sur le serveur
	public void RegisterPlayer(uint _netId, PlayerSetup _playerSetup)
	{
		Debug.Log("RegisterPlayer In Gamemanager");
		// On ajoute le joueur dans le liste des joueurs
		players.Add(_netId, _playerSetup);

		// On essaye de démarrer la partie
		TryStartGame();
	}

	// Retire un joueur enregistré
	public void UnRegisterPlayer(uint _netId)
	{
		// On retire le joueur de la liste des joueurs
		players.Remove(_netId);

		// On regarde si la partie est déjà lancé si oui on étant le server
		if(gameStart)
        {
			OnStopServer();
        }
	}

	public void OnStopServer()
    {
		CustomNetworkManager.singleton.StopServer();

		StartCoroutine(IEOnStopServer());
	}

	private IEnumerator IEOnStopServer()
    {
		DestroyAllSpawnsObjects();
		Destroy(CustomNetworkManager.singleton.gameObject);

		NetworkServer.Shutdown();
		NetworkClient.Disconnect();

		AsyncOperation asyncOp = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
		while(!asyncOp.isDone)
        {
			yield return null;
        }

		// Récupération du script MainMenu
		MainMenu mainMenu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<MainMenu>();
		mainMenu.ShowLoseMenu();

		yield return new WaitForSeconds(0.1f);

		// Destruction du GameManager
		Destroy(gameObject);
	}

	#region InitStartGame && Timer

	// Fonction appelé quand un joueur rejoint la partie pour voir si on peut commencer la partie
	private void TryStartGame()
	{
		// Si il n'y a pas assez de joueur
		if (players.Count < NetworkServer.maxConnections) return;

		Debug.Log("La partie peut démarrer");

		// Lancement du chrono de x secondes
		StartCoroutine(EnumStartTimer());
	}

	// Timer de StartGameTimer seconde(s) qui update pour chaque joueur le timer
	private IEnumerator EnumStartTimer()
	{
		ChangeGameState(GameUtils.GameStates.TIMER);

		for (int i = 1; i <= _startGameTimer; i++)
		{
			// Si il n'y a plus assez de joueur pour lancer la partie alors on stop le timer
			if (players.Count < NetworkServer.maxConnections)
			{
				ChangeGameState(GameUtils.GameStates.WAIT_PLAYER);
				yield break;
			}

			// On update de timer pour chaque joueur
			yield return new WaitForSeconds(1f);
			UpdateTimer(_startGameTimer - i);
			
		}

		// On lance la partie car l'IEnumerator ne ce n'est pas arreté
		StartGame();
	}

	// Update du timer pour chaque joueur
	private void UpdateTimer(int timeInSeconds)
	{
		//On update le timer si une instance existe
		if(UIManager.instance)
			UIManager.instance.UpdateTimer(timeInSeconds);
	}

	#endregion

	// Fonction qui démarre la partie
	private void StartGame()
	{
		_gameStart = true;
		ChangeGameState(GameUtils.GameStates.PLAY);
		Debug.Log("Lancement de la partie !");
	}
}
