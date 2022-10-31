using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Health : NetworkBehaviour
{
	[SerializeField] private float maxHealth = 50f;

	public float MaxHealth
	{
		get
		{
			return this.maxHealth;
		}

		set
		{
			this.maxHealth = value;
		}
	}

	public float CurrentHealth
	{
		get
		{
			return this.currentHealth;
		}

		set
		{
			this.currentHealth = value;
		}
	}

	[SyncVar] [SerializeField] private float currentHealth;
	[SyncVar] private float currentDefense;
	public float GetPercentageOfHealth { get => (currentHealth * 100 / maxHealth); }

	[SyncVar] private bool _isDied = false;
	private Animator animator;
	private PlayerController playerController;

	public bool isDied
	{
		get { 
			return _isDied; 
		}
	}

	[SyncVar] private GameUtils.Effects actualEffect = GameUtils.Effects.NONE;
	[SyncVar] private float effectDuration;
	[SyncVar] private float effectPower;

	[SerializeField] private float _destroyTime = 2f;

	// Réanimation
	private const float WAIT_FOR_REVIVE = 5f;
	[SerializeField] private GameObject ReviveTimerUI;
	[SyncVar] private bool isReanimated = false;
	[SerializeField] private Behaviour[] disableOnDeath;

	private void Start()
	{
		// On regarde si on peut récupérer le component PlayerController
		if (TryGetComponent(out PlayerController pController))
		{
			// Si oui c'est donc un joueur
			playerController = pController;
			// Récupération de l'animator sur la partie GFX du joueur
			if (playerController.GetGFX.TryGetComponent(out Animator anim))
			{
				animator = anim;
			}
		}
	}
	private void Update()
	{
		// Si ce n'est pas un joueur
		if (playerController == null) return;

		if (currentHealth >= maxHealth)
			currentHealth = maxHealth;

		animator.SetBool("dead", _isDied);
		animator.SetBool("isReanimated", isReanimated);
	}

	#region Server

	// Initialisation de différente variable
	public override void OnStartServer() => currentHealth = maxHealth;

	// Fonction qui définie la nouvelle vie à la personne qui possède le script
	[Server]
	private void SetHealth(float newHealth) {

		if((currentHealth - newHealth) > 0)
			DamagePopup.Create(transform.position, (currentHealth - newHealth), newHealth < 20);

		Mathf.Clamp(newHealth, 0, maxHealth);
		currentHealth = newHealth;

		if (currentHealth <= 0 && !_isDied)
		{
			_isDied = true;

			if (CompareTag("Player"))
			{
				// Vérification que tous les joueurs ne sont pas mort
				var players = NetworkServer.connections;
				int nbDeath = 0;
				foreach (var p in players)
				{
					if (p.Value.identity.GetComponent<Health>().isDied)
					{
						nbDeath++;
					}
				}
				if (nbDeath == players.Count) GameManager.instance.OnStopServer();
				else TargetOnDeath();
			}
			else
			{
				EnemyDeath();
			}
		}
	}
	[Server]
	public void SetDefense(float value)
	{
		Mathf.Clamp(value, 0, 100);
		currentDefense = value;
	}
	[Command]
	public void CmdTakeDamage(float damage)
	{
		SetHealth(Mathf.Max(currentHealth - (damage - ((currentDefense / 100) * damage)), 0));
	}
	[Server] 
	public void ServerTakeDamage(float damage)
	{
		SoundManager.Create(GameUtils.SoundList.COUP_RECU, transform.position, transform.rotation);
		SetHealth(currentHealth - (damage - ((currentDefense / 100) * damage)));
	}

	#region Réanimation

	// Fonction qui affiche le texte que l'on est entrain d'être réanimé
	// Elle est exécutée sur le client qui se fait réanimer
	[TargetRpc]
	private void TargetChangeStateReanimationUI(bool state, string resuscitatorPseudo)
	{
		Debug.Log("TargetChangeState..()");
		UIManager.instance.ChangeReanimationStateUI(state, resuscitatorPseudo);
		animator.SetBool("isReanimated", isReanimated);
	}
	// Fonction exécutée par le serveur sur le client qui doit ce faire réanimer
	[Command(requiresAuthority=false)]
	public void CmdTryReanimation(string resuscitatorPseudo)
	{
		Debug.Log(isDied);
		// Vérification que le joueur est bien mort
		if (!isDied) return;
		Debug.Log("tryreanimation");

		isReanimated = true;

		Debug.Log("tryReanimation("+resuscitatorPseudo+")");

		// Activation de l'UI
		TargetChangeStateReanimationUI(true, resuscitatorPseudo);

		// Execution de la coroutine pour le timer
		StartCoroutine(IETryReanimation());
	}
	// Fonction exécutée sur le serveur sur le client qui doit ce arreter de ce faire réanimer
	[Command(requiresAuthority=false)]
	public void CmdStopReanimation()
	{
		// Vérification que le joueur est bien mort ET qu'il est entrain d'être réanimé
		if (!isDied || !isReanimated) return;

		isReanimated = false;
		TargetChangeStateReanimationUI(false, "");
	}
	[ClientRpc]
	private void RpcSetTimerValue(GameObject timerCanvas, float time)
	{
		Transform timer = timerCanvas.transform.GetChild(0).transform;

		Timer timerScript = timer.GetComponent<Timer>();
		timerScript.Setup(time);
	}
	[Server]
	private IEnumerator IETryReanimation()
	{
		// Création d'une instance d'un timer
		GameObject timerCanvas = Instantiate(ReviveTimerUI, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
		Transform timer = timerCanvas.transform.GetChild(0).transform;

		Timer timerScript = timer.GetComponent<Timer>();
		timerScript.Setup(WAIT_FOR_REVIVE);

		// Apparition de l'objet sur le serveur
		NetworkServer.Spawn(timerCanvas);
		RpcSetTimerValue(timerCanvas, WAIT_FOR_REVIVE);

		float time = 0f; 
		// On attend que la réanimation est terminé
		while(time < WAIT_FOR_REVIVE)
		{
			// Si le joueur est encore entrain d'être réanimé
			if (isReanimated)
			{
				time += 0.1f;
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				TargetChangeStateReanimationUI(false, "");
				NetworkServer.Destroy(timerCanvas);
				yield break;
			}
		}

		// Si on arrive ici c'est que le joueur viens d'être réanimé
		TargetChangeStateReanimationUI(false, "");

		currentHealth = 10f;
		_isDied = false;
		isReanimated = false;

		TargetEnableComponents();
	}
	[TargetRpc]
	private void TargetEnableComponents()
	{
		InvertComponentsState();
	}
	#endregion

	[Server]
	public void ApplyEffect(GameUtils.Effects effect, float durationTime = 5, float power = 1)
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;
		Debug.Log(effect.ToString() + " Time " + durationTime + " | power : " + power);
		StartCoroutine(IEApplyEffect(effect, durationTime, power));
	}
	private IEnumerator IEApplyEffect(GameUtils.Effects effect, float durationTime, float power)
	{
		if (!isServer) yield break;

		Debug.Log(effect);

		durationTime = Mathf.Abs(durationTime);
		power = Mathf.Abs(power);

		// Si on applique le même effet
		if (actualEffect == effect)
		{
			// On ajoute simplement le temps ainsi que les dégats à appliquer
			effectDuration += durationTime;
			effectPower += power;
			yield break;
		} else
		{
			effectDuration = durationTime;
			effectPower = power;
		}

		actualEffect = effect;

		switch (effect)
		{
			case GameUtils.Effects.NONE:
				effectDuration = 0;
				effectPower = 0;
				break;
			case GameUtils.Effects.POISON:
				while(effect == actualEffect && effectDuration > 0)
				{
					ServerTakeDamage(effectPower / (durationTime/0.1f));
					yield return new WaitForSeconds(0.1f);
					effectDuration -= 0.1f;

					if (effectDuration <= 0)
					{
						actualEffect = GameUtils.Effects.NONE;
					}
				}
				break;
			case GameUtils.Effects.REGENERATION:
				while (effect == actualEffect && effectDuration > 0)
				{
					SetHealth(currentHealth + (effectPower / (durationTime / 0.1f)));
					yield return new WaitForSeconds(0.1f);
					effectDuration -= 0.1f;

					if (effectDuration <= 0)
					{
						actualEffect = GameUtils.Effects.NONE;
					}
				}
				break;
			case GameUtils.Effects.INSTANT_HEAL:
				SetHealth(currentHealth + effectPower);
				actualEffect = GameUtils.Effects.NONE;
				break;
			case GameUtils.Effects.STUN:
				Debug.Log("Effect Stun");
				if (!playerController)
				{
					actualEffect = GameUtils.Effects.NONE;
					effectDuration = 0;
					effectPower = 0;
				}
				playerController.enabled = false;
				while (effect == actualEffect && effectDuration > 0)
				{
					yield return new WaitForSeconds(0.1f);
					effectDuration -= 0.1f;
					Debug.Log("EffectDuration : " + effectDuration);

					if (effectDuration <= 0)
					{
						if (!isDied) {
							playerController.enabled = true;
						}
						actualEffect = GameUtils.Effects.NONE;
					}
				}
				break;
			default:
				break;
		}
	}

	#endregion

	// Fonction exécutée sur le client qui vient de mourir
	[TargetRpc]
	private void TargetOnDeath()
	{
		Debug.Log("TargetOnDeath()");

		SoundManager.Create(GameUtils.SoundList.MORT, transform.position, transform.rotation);
		// Si c'est un joueur
		playerController.GetRigidbody2D.velocity = Vector2.zero;

		// On change de state tous les scripts mis dans la variable disableOnDeath
		InvertComponentsState();
	}
	// Fonction exécutée sur le serveur, à la mort d'un ennemi
	[Server]
	private void EnemyDeath()
	{
		SoundManager.Create(GameUtils.SoundList.MORT, transform.position, transform.rotation);
		// Si ce n'est pas le joueur
		Destroy(gameObject, _destroyTime);
	}
	// Fonction qui inverse l'etat des components de la liste disableOnDeath 
	private void InvertComponentsState()
	{
		foreach (var item in disableOnDeath)
		{
			item.enabled = !item.enabled;
		}
	}
} 
