using System.Collections.Generic;
using UnityEngine;
using Mirror;

using State = BossUtils.State;
using Attack = BossUtils.Attack;

public class BossONEHand : NetworkBehaviour
{
	private State currentState = State.END; // State.END = non utilisé
	private Attack currentAttack = Attack.NONE;

	public float selectTargetRadius = 1f;
	private Transform playerTarget;
	private Health targetHealth;
	private NetworkIdentity targetIdentity;

	private Vector2 endTarget;
	private bool grabOrient = false; // False = gauche || True = droite

	public bool isUsed { 
		get {
			return (currentState != State.END && playerTarget != null);
		}
	}

	[Header("Options")]
	[SerializeField] private float endDistance = 0.5f;
	[SerializeField] private float hitDistance = 0.5f;
	[SerializeField] private float grabDistance = 0.4f;
	[SerializeField] private float damage = 10f;
	[SerializeField] private float speed = 5f;
	[SerializeField] private float percentageHealthOfBossOnAttack = -1f;

	[Header("Components")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Health bossHealth;

	[Server]
	public void Setup(Health _bossHealth, float newDamage)
    {
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// On récupère le spriteRenderer que si il n'est pas initialisé
		if (!spriteRenderer)
			spriteRenderer = GetComponent<SpriteRenderer>();

		bossHealth = _bossHealth;
		damage = newDamage;

		Action(State.END);
	}

	// Fonction qui permet de définir une action sur une main du boss
	[Server]
	public void Action(State state, Transform newTarget = null, int action = 0, bool newGrabOrient = false, float percentageHealth = -1f)
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Vérification que l'ancinne attaque de la main soit un GRAB
		if(currentAttack == Attack.GRAB)
		{
			if (playerTarget)
			{
				if (playerTarget.TryGetComponent(out Health health))
				{
					// Si c'est le cas on retire enleve les effets que l'on a appliqué sur la target
					health.ApplyEffect(GameUtils.Effects.NONE, 0, 0);

					// Réactivation des mouvements du joueur 
					NetworkIdentity targetIdentity = playerTarget.GetComponent<NetworkIdentity>();
					TargetUpdateMovementState(targetIdentity.connectionToClient, true);
				}
			}
		}

		currentState = state;
		playerTarget = newTarget;
		targetHealth = null;
		targetIdentity = null;

		// Si on a une target
		if (playerTarget)
		{
			targetHealth = playerTarget.GetComponent<Health>();
			targetIdentity = playerTarget.GetComponent<NetworkIdentity>();
		}

		// On réinitialise les différentes états que la main peut avoir
		currentAttack = Attack.NONE;
		Debug.Log(((Attack)action).ToString());
		switch (currentState)
		{
			case State.ATTACK:
				currentAttack = (Attack)action;

				if (currentAttack == Attack.GRAB)
				{
					grabOrient = newGrabOrient;
					spriteRenderer.flipX = newGrabOrient;

					if(percentageHealth > 0)
						percentageHealthOfBossOnAttack = percentageHealth;
				} else
				{
					spriteRenderer.flipX = false;
				}
				break;
			default:
				break;
		}
	}
	public void SetEndPoint(Vector2 pos)
	{
		endTarget = pos;
	}

	private void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// Si le boss est mort alors on s'autodétruit
		if (!bossHealth) NetworkServer.Destroy(gameObject);

		// Si on a pas de target OU alors que la main est inactive et qu'elle n'a pas de déplacement à faire
		if (playerTarget == null && (currentState == State.END && endTarget == Vector2.zero)) return;

		// Si ce n'est pas une attaque de grab OU que la main est inutilisé alors on ce déplace vers le joueur ciblé
		if (currentAttack != Attack.GRAB && currentState != State.END)
			transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);

		switch (currentState)
		{
			case State.ATTACK:
				//Debug.Log("State.ATTACK");
				switch (currentAttack)
				{
					case Attack.NONE:
						currentState = State.END;
						break;
					case Attack.HIT:
						float distance = Vector2.Distance(transform.position, playerTarget.position);
						if (distance < hitDistance)
						{
							// [UNCOMPLETE] Mettre en place les animations
							// On applique les dégats au joueur
							if (targetHealth)
							{
								targetHealth.ServerTakeDamage(damage);

								// Fin de l'attaque car on a touché le joueur
								currentState = State.END;
							}
						}
						break;
					case Attack.GRAB:
						Vector3 diff;
						if (grabOrient)
							diff = new Vector3(0.25f, 0);
						else
							diff = new Vector3(-0.25f, 0);

						TargetUpdateMovementState(targetIdentity.connectionToClient, false);

						// On regarde si le joueur cible à moins de 15% de sa vie
						if(targetHealth.GetPercentageOfHealth <= 15 || (percentageHealthOfBossOnAttack - bossHealth.GetPercentageOfHealth) > 10)
                        {
							currentState = State.END;
							targetHealth.ApplyEffect(GameUtils.Effects.NONE, 0, 0);
							TargetUpdateMovementState(targetIdentity.connectionToClient, true);
						}

						transform.position = Vector2.MoveTowards(transform.position, playerTarget.position + diff, speed * Time.deltaTime);

						float dist = Vector2.Distance(transform.position, playerTarget.position + diff);
						if (dist < grabDistance)
						{
							// On regarde si le joueur est mort
							if (targetHealth.isDied)
							{
								currentState = State.END;
								targetHealth.ApplyEffect(GameUtils.Effects.NONE, 0, 0);
								TargetUpdateMovementState(targetIdentity.connectionToClient, true);
							}
							else
							{
								targetHealth.ApplyEffect(GameUtils.Effects.POISON, 5f, 0.2f);
							}
						}
						break;
					case Attack.STUN:
						float dista = Vector2.Distance(transform.position, playerTarget.position);
						if (dista < hitDistance)
						{
							// [UNCOMPLETE] Mettre en place les animations
							// On applique les dégats au joueur
							if (targetHealth)
							{
								targetHealth.ServerTakeDamage(damage);
								targetHealth.ApplyEffect(GameUtils.Effects.STUN, 3);

								// Fin de l'attaque car on a touché le joueur
								currentState = State.END;
							}
						}
						break;
					default:
						break;
				}			
				break;
			case State.END:
				transform.position = Vector2.MoveTowards(transform.position, endTarget, speed * Time.deltaTime);
				float dis = Vector2.Distance(transform.position, endTarget);
				if(dis < endDistance)
				{
					endTarget = Vector2.zero;
					playerTarget = null;
				}
				break;
		}
	}

	[TargetRpc]
	private void TargetUpdateMovementState(NetworkConnection targetConnection, bool state)
	{
		Debug.Log(targetConnection.identity.name);
		Debug.Log("New State : " + state);
		targetConnection.identity.GetComponent<PlayerController>().enabled = state;
	}

    private void OnDestroy()
    {
		// Si on a une cible et que l'on est entrain de le grab
        if(playerTarget && currentAttack == Attack.GRAB)
        {
			currentState = State.END;
			targetHealth.ApplyEffect(GameUtils.Effects.NONE, 0, 0);
			TargetUpdateMovementState(targetIdentity.connectionToClient, true);
		}
    }
}

