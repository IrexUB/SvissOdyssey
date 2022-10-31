using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class CustomNetworkManager : NetworkManager
{
	public static new CustomNetworkManager singleton { get; private set; }

	[Header("Scenes Parameters")]
	[SerializeField] [Scene] private string _bootScene;
	public string GetBootScene
	{
		get => _bootScene;
	}

	public List<Level> _levels;
	public List<string> _serverLoadScenes;
	public int currentLevelIndex = 0;
	public int currentSublevelIndex = 0;

	private bool _defaultSceneLoaded = false;
	private bool _loadingNewScene = false;
	public bool loadingNewScene { get => _loadingNewScene; }

	[Header("Map pathfinding")]
	private PathFinding pathFindingLayer1;
	private PathFinding pathFindingLayer2;
	private PathFinding pathFindingLayer3;

	private List<Vector3> teleportPos = new List<Vector3>();
	[SerializeField] private GameObject[] players;
	private int playerSkinID = 0;

	#region Start Management
	public override void Start()
	{
		singleton = this;
		base.Start();
	}

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
		Transform startPos = GetStartPosition();

		GameObject playerObject = players[playerSkinID];

		GameObject player = startPos != null
			? Instantiate(playerObject, startPos.position, startPos.rotation)
			: Instantiate(playerObject);

		player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
		NetworkServer.AddPlayerForConnection(conn, player);

		playerSkinID++;
        if (playerSkinID >= players.Length)
        {
			playerSkinID = 0;
        }
	}

	// Quand le serveur démarre
	public override void OnStartServer()
	{
		_defaultSceneLoaded = false;

		currentLevelIndex = 0;
		currentSublevelIndex = 0;

		StartCoroutine(ServerLoadDefaultScenes());
	}

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);

		_loadingNewScene = true;

		StartCoroutine(WaitAndTeleport(loadingSceneAsync));
	}

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

		if (conn.identity == null) return;
		// On téléporte le joueur
		NetworkTransform networkT = conn.identity.GetComponent<NetworkTransform>();
		if (networkT)
		{
			networkT.ServerTeleport(teleportPos[0]);
			teleportPos.RemoveAt(0);
		}
	}

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

		Debug.Log("test");
    }

    private IEnumerator WaitAndTeleport(AsyncOperation op)
    {
		if (op == null) yield break;

		while (!op.isDone)
		{
			yield return null;
		}

		// Récupération des différents pathfinding.

		// Récupération des différents points de spawn des joueurs pour 
		var players = GameManager.instance.GetPlayers;
		foreach (var player in players)
		{
			teleportPos.Add(GetStartPosition().position);
		}

		_loadingNewScene = false;
	}

    // On charge les différentes scènes par défaut
    private IEnumerator ServerLoadDefaultScenes()
	{
		yield return new WaitForSeconds(0.2f);
		ServerChangeScene(_levels[currentLevelIndex].traderPlace);

		_defaultSceneLoaded = true;
	}

	public void LoadNextScene()
    {
		// On augmente de 1 le sous level (Un sous level est la scène de shop, la tour, la salle du boss)
		currentSublevelIndex++;
		if (currentSublevelIndex > 2)
		{
			currentSublevelIndex = 0;
			currentLevelIndex++;
		}

		string newSubLevelName;
		switch (currentSublevelIndex)
		{
			case 0:
				newSubLevelName = _levels[currentLevelIndex].traderPlace;
				break;
			case 1:
				newSubLevelName = _levels[currentLevelIndex].tower;
				break;
			case 2:
				newSubLevelName = _levels[currentLevelIndex].boss;
				break;
			default:
				newSubLevelName = _levels[currentLevelIndex].traderPlace;
				break;
		}

		ServerChangeScene(newSubLevelName);
	}
	#endregion

	#region Stop Management

	// Quand le serveur s'arrete
	public override void OnStopServer()
	{
		StartCoroutine(ServerUnloadAllActiveScenes());
	}

	// Fonction qui décharge toute les scènes active que le serveur possède
	private IEnumerator ServerUnloadAllActiveScenes()
	{
		for (int index = 0; index < _serverLoadScenes.Count; index++)
			yield return SceneManager.UnloadSceneAsync(_serverLoadScenes[index]);

		_serverLoadScenes.Clear();
		_defaultSceneLoaded = false;

		yield return Resources.UnloadUnusedAssets();
	}

	// Quand un client quitte un serveur
	public override void OnStopClient()
	{
		if (GameManager.instance)
		{
			// On retire le joueur de la liste des joueurs du GameManager
			GameManager.instance.UnRegisterPlayer(NetworkClient.localPlayer.netId);
		}

		// Si c'est uniquement le client
		if (mode == NetworkManagerMode.ClientOnly)
			StartCoroutine(ClientUnloadAllActiveScenes());
	}

	// Fonction qui décharge toute les scènes active que le client possède
	private IEnumerator ClientUnloadAllActiveScenes()
	{
		for (int index = 0; index < SceneManager.sceneCount; index++)
		{
			if (SceneManager.GetSceneAt(index) != SceneManager.GetActiveScene())
				yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
		}
	}

	#endregion

}
