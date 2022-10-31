using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using State = BossUtils.State;
using Attack = BossUtils.Attack;

public class BossONE : NetworkBehaviour
{

	[System.Serializable]
	private struct Options
	{
		public float handUnusedCircleRadius;

		public int ExperienceDrop;
		public List<Item> ItemsDrop;
	}

	[SerializeField] private GameObject activeOnDeath;

	private State state;
	private Attack attackState;

	private delegate void OnStateChangeDelegate(State _state);
	private event OnStateChangeDelegate OnStateChange;
	private delegate void OnAttackStateChangeDelegate();
	private event OnAttackStateChangeDelegate OnAttackStateChange;

	[Header("Components")]
	[SerializeField] private Health health;
	private List<BossONEHand> usedHands = new List<BossONEHand>();
	private List<BossONEHand> unusedHands = new List<BossONEHand>();
	private int nbOfHands { get => usedHands.Count + unusedHands.Count; }

	[Header("Stats")]
	[SerializeField] private Options options;
	[SerializeField] private Stats stats;

	[Header("Spawnable")]
	[SerializeField] private GameObject loadingHealthTowerPrefab;
	[SerializeField] private GameObject healthTowerPrefab;
	[SerializeField] private GameObject handPrefab;

	#region Handle

	// Quand l'on change de state
	[Server]
	private void HandleStateChange(State _state)
	{
		if (!isServer) return;

		UpdateHandList();

		state = _state;

		switch (state)
		{
			// Au démarrage du bosss
			case State.BEGIN:
				{
					// Récupération des différents components
					if (!health) health = GetComponent<Health>();

					// Apparition des tours
					//StartCoroutine(SpawnHealthTower());

					// Apparition de 2 mains
					SpawnHand();

					// Initialisation des points de vie / defense
					health.ApplyEffect(GameUtils.Effects.REGENERATION, 10, 2);
				}
				break;
			case State.ATTACK:
				{
					// Génération d'une attaque aléatoire
					int randId = Random.Range(1, System.Enum.GetValues(typeof(Attack)).Length);
					attackState = (Attack)randId;

					OnAttackStateChange?.Invoke();
				}
				break;
			case State.END:
				{
					// Apparition des items / XP
					Experience.Create(transform.position, options.ExperienceDrop);

					NetworkServer.Destroy(gameObject);
				}
				break;
			default:
				break;
		}
	}

	private void HandleAttackStateChange()
	{
		switch (attackState)
		{
			case Attack.NONE:
				break;
			case Attack.HIT:
				// Si on a une main disponible
				if (unusedHands.Count > 0)
				{
					// On récupère une main aléatoire de la liste
					int randId = Random.Range(0, unusedHands.Count);
					BossONEHand selectHand = unusedHands[randId];

					// On retire la main de la liste des mains non utilisé pour la mettre dans l'autre
					unusedHands.RemoveAt(randId);
					usedHands.Add(selectHand);

					// Récupération d'un joueur aléatoire
					var players = GameManager.instance.GetPlayers;
					int randPlayer = Random.Range(0, players.Count);
					Debug.Log(players.Count);
					Debug.Log(players.Count-1);
					Debug.Log(randPlayer);
					Debug.Log(players[randPlayer].name);

					Debug.Log("Nom de la cible : " + players[randPlayer].name);

					// On affecte l'action sur la main choisie
					selectHand.Action(state, players[randPlayer].transform, (int)Attack.HIT);
				}
				break;
			case Attack.GRAB:
				break;

				// Si il y a 2 mains disponible
				if (unusedHands.Count > 1)
				{
					// On récupère une main aléatoire de la liste
					int randId = Random.Range(0, unusedHands.Count);
					BossONEHand selectHand01 = unusedHands[randId];
					unusedHands.RemoveAt(randId);

					randId = Random.Range(0, unusedHands.Count);
					BossONEHand selectHand02 = unusedHands[randId];
					unusedHands.RemoveAt(randId);

					usedHands.Add(selectHand01);
					usedHands.Add(selectHand02);

					// Récupération d'un joueur aléatoire
					var players = GameManager.instance.GetPlayers;
					int randPlayer = Random.Range(0, players.Count);

					// On affecte l'action sur la main choisie
					selectHand01.Action(state, players[randPlayer].transform, (int)Attack.GRAB, false);
					selectHand02.Action(state, players[randPlayer].transform, (int)Attack.GRAB, true);
				}
				break;
			case Attack.STUN:
				// Si on a une main disponible
				if (unusedHands.Count > 0)
				{
					// On récupère une main aléatoire de la liste
					int randId = Random.Range(0, unusedHands.Count);
					BossONEHand selectHand = unusedHands[randId];

					// On retire la main de la liste des mains non utilisé pour la mettre dans l'autre
					unusedHands.RemoveAt(randId);
					usedHands.Add(selectHand);

					// Récupération d'un joueur aléatoire
					var players = GameManager.instance.GetPlayers;
					int randPlayer = Random.Range(0, players.Count);

					Debug.Log("Nom de la cible : " + players[randPlayer].name);

					// On affecte l'action sur la main choisie
					selectHand.Action(state, players[randPlayer].transform, (int)Attack.STUN);
				}

				break;
			default:
				break;
		}
	}

	#endregion

	// Au démarrare du boss
	private void Start()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (!stats) stats = GetComponent<Stats>();

		activeOnDeath.SetActive(false);

		// Configuration des différents Handles
		OnStateChange += HandleStateChange;
		OnAttackStateChange += HandleAttackStateChange;

		// On change de state à BEGIN pour démarrer le boss
		ChangeState(State.BEGIN);

		InvokeRepeating("RepeatAttack", 2f, 2f);
	}

	private void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (nbOfHands >= 3) return;
		else
		{
			if (health.GetPercentageOfHealth < 75 && nbOfHands == 1) SpawnHand();
			if (health.GetPercentageOfHealth < 50 && nbOfHands == 2) SpawnHand();
		}
	}

	private void RepeatAttack() => ChangeState(State.ATTACK);
	// Fonction qui permet de change l'état de la variable state
	private void ChangeState(State _newState)
	{
		state = _newState;
		OnStateChange?.Invoke(state);
	}
	// Fonction qui fait spawn les différentes tours de régénération du boss
	//private IEnumerator SpawnHealthTower()
	//{
	//	if (!isServer) yield break;

	//	List<Transform> allSpawnPos = options.AllHealthTowerPosition;
	//	List<Vector3> spawnedLoadingHealthTowerPosition = new List<Vector3>();

	//	for (int i = 0; i < options.NbOfHealthTower; i++)
	//	{
	//		// Récupération d'une position aléatoire
	//		int randPos = Random.Range(0, allSpawnPos.Count);

	//		// Apparition de la tour en mode chargement
	//		GameObject tower = Instantiate(loadingHealthTowerPrefab, allSpawnPos[randPos].position, Quaternion.identity);

	//		// Configuration du temps d'apparition de la tour
	//		Timer timerScript = tower.transform.GetChild(0).GetComponent<Timer>();
	//		timerScript.Setup(options.timerHealthTowerSpawn);

	//		// Spawn de la tour pour tous les clients
	//		NetworkServer.Spawn(tower);
	//		RpcConfigLoadingTower(tower, options.timerHealthTowerSpawn);

	//		// On retire la position choisi, et on ajoute la nouvelle tour dans la liste loadingHealthTower
	//		allSpawnPos.RemoveAt(randPos);
	//		spawnedLoadingHealthTowerPosition.Add(tower.transform.position);
	//	}

	//	// On attend que toutes les tours soient apparue
	//	yield return new WaitForSeconds(options.timerHealthTowerSpawn);

	//	// On remplace toutes les tours de chargement par des tours fixent
	//	for (int i = 0; i < spawnedLoadingHealthTowerPosition.Count; i++)
	//	{
	//		GameObject finalTower = Instantiate(healthTowerPrefab, spawnedLoadingHealthTowerPosition[i], Quaternion.identity);

	//		// Apparition de la tour finale
	//		NetworkServer.Spawn(finalTower);
	//	}
	//}

	[Server]
	private void SpawnHand()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		GameObject hand = Instantiate(handPrefab, transform.position, Quaternion.identity);
		BossONEHand handScript = hand.GetComponent<BossONEHand>();
		unusedHands.Add(handScript);

		NetworkServer.Spawn(hand);

		// On déplace la main sur une position aléatoire autour du boss
		Vector2 randCirclePos = GameUtils.GetRandomPosOnCirclePerimeter(transform.position, options.handUnusedCircleRadius);
		handScript.SetEndPoint(randCirclePos);

		// Lancement de la configuration de la main
		handScript.Setup(health, stats.m_physicalAttack);
	}
	[Server]
	private void UpdateHandDefense()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		health.SetDefense(15 * unusedHands.Count);
	}

	// Fonction qui actualise les mains du boss
	[Server]
	private void UpdateHandList()
	{
		List<BossONEHand> tempsUsedHands = new List<BossONEHand>(usedHands);
		foreach (var hand in usedHands)
		{
			// Si la main n'existe plus
			if (!hand)
			{
				tempsUsedHands.Remove(hand);
				continue;
			}

			// Si la main n'est pas utilisé
			if(!hand.isUsed)
			{
				// On la rajoute dans la liste des mains non utilisé
				tempsUsedHands.Remove(hand);
				unusedHands.Add(hand);

				// On déplace la main sur une position aléatoire autour du boss
				Vector2 randCirclePos = GameUtils.GetRandomPosOnCirclePerimeter(transform.position, options.handUnusedCircleRadius);
				hand.SetEndPoint(randCirclePos);

				continue;
			}
		}

		usedHands = tempsUsedHands;

		UpdateHandDefense();
	}

	[ClientRpc]
	private void RpcConfigLoadingTower(GameObject loadingTower, float time)
	{
		Transform timer = loadingTower.transform.GetChild(0).transform;

		Timer timerScript = timer.GetComponent<Timer>();
		timerScript.Setup(time);
	}

	private void OnDestroy()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// On retire toute les mains
		foreach (var hand in usedHands)
		{
			NetworkServer.Destroy(hand.gameObject);
		}
		foreach (var hand in unusedHands)
		{
			NetworkServer.Destroy(hand.gameObject);
		}

		RpcActiveOnDeath();

		activeOnDeath.SetActive(true);
	}

	[ClientRpc]
	private void RpcActiveOnDeath()
	{
		activeOnDeath.SetActive(true);
	}
}
