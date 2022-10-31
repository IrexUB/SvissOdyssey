using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody2D))]
[System.Serializable]
public class PEnemyController : NetworkBehaviour
{
	[Header("Ennemy Stats")]
	[SerializeField] protected Stats stats;
	[SerializeField] protected Rigidbody2D rb;

	[Header("Target Informations")]
	protected Vector3 targetPosAlea;
	protected Transform playerTarget;
	protected bool canDetectPlayer = true;

	[Header("Player detector")]
	[SerializeField] protected LayerMask layerDetect;
	[SerializeField] protected float detectDistance = 3.5f;
	[SerializeField] protected float bonusToDetect = 1f;
	[SerializeField] protected float speedBonus = 1f;

	[Header("Pathfinding")]
	[SerializeField] protected PathFinding pathFinding;
	protected List<Node> path;
	protected Node currentNode;
	protected int currentWaypoint;
	[SerializeField] protected float distanceForPassWaypoint;
	[SerializeField] protected float stopDistanceWithPlayer = 1f;
	// Distance pour générer une position aléatoire autour du joueur
	[SerializeField] protected float wanderDistance;
	[SerializeField] protected Collider2D enemyZone;

	[Header("Graphics")]
	[SerializeField] protected GameObject GFX;
	[SerializeField] protected SpriteRenderer spriteRenderer;
	[SerializeField] protected Animator animator;
	[SerializeField] protected bool facingRight = true;
	[SerializeField] protected ParticleSystem interrogationPoint;

	protected virtual void Start()
	{
		// Récupération du spriteRenderer de l'ennemi
		spriteRenderer = GFX.GetComponent<SpriteRenderer>();
		animator = GFX.GetComponent<Animator>();
		if (!stats)
			stats = GetComponent<Stats>();

		layerDetect = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

		// Si ce n'est pas le serveur alors return
		if (!isServer) return;

		if (!pathFinding)
			Debug.LogError("Attention la variable PathFinding de l'ennemi est vide !", this);

		// Initialisation des components
		if (!rb)
			rb = GetComponent<Rigidbody2D>();

		targetPosAlea = transform.position;
		targetPosAlea += wanderDistance * new Vector3(
													  Random.Range(-1f, 1f),
													  Random.Range(-1f, 1f),
													  0
													 ).normalized;

		// Appel de la fonction PathUpdate 0.6 fois par seconde (Temps par defaut)
		InvokeRepeating("PathUpdate", 0, 1f);
	}

	[Server]
	protected virtual void PathUpdate()
	{
		if (playerTarget != null)
		{
			path = pathFinding.GetPath(transform.position, playerTarget.position);
			currentWaypoint = 0;
		}
		else
		{
			// Génération d'un point proche de l'ennemi
			targetPosAlea = transform.position;
			targetPosAlea += wanderDistance * new Vector3(
														  Random.Range(-1f, 1f),
														  Random.Range(-1f, 1f),
														  0
														 ).normalized;

			path = pathFinding.GetPath(transform.position, targetPosAlea);
			currentWaypoint = 0;
		}
	}

	// Fonction exécutée uniquement sur le serveur
	// Actualise la zone autour de l'ennemi
	protected virtual void Update()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;
		speedBonus = 1f;

		// Si on peut récupérer une nouvelle cible
		if (canDetectPlayer)
		{
			// Récupération du joueur le plus proche
			float minDis = float.MaxValue;
			Transform target = null;
			var players = NetworkServer.connections;
			foreach (var p in players)
			{
				if (p.Value.identity == null) continue;

				// Vérification qu'ils sont sur le même layer
				if ((layerDetect & (1 << p.Value.identity.gameObject.layer)) <= 0) continue;

				// Vérification si le joueur n'est pas mort
				Health pHealth = p.Value.identity.GetComponent<Health>();
				if (pHealth.isDied) continue;

				float dist = Vector2.Distance(p.Value.identity.transform.position, transform.position);
				if (dist <= detectDistance && dist <= minDis)
				{
					target = p.Value.identity.transform;
					minDis = dist;
				}
			}

			if (playerTarget == null && target != null)
				RpcPlayInterrogationParticle();

			playerTarget = target;
			if (playerTarget)
				speedBonus = 2f;
		} else
        {
			playerTarget = null;
        }

		// On récupère la node au position de l'ennemi
		Node tempNode = pathFinding.grid.WorldPositionToNode(transform.position);
		// Si elle est différente de la node de l'ancienne position
		if (currentNode != tempNode)
		{
			//UpdateNearZone();
			currentNode = tempNode;
		}
	}

	[Server]
	protected virtual void UpdateNearZone()
    {
		Bounds bounds = enemyZone.bounds;
		Vector2 boundsBottomLeft = new Vector2((bounds.center.x - bounds.size.x / 2) - 1, (bounds.center.y - bounds.size.y / 2) - 1);
		Vector2 boundsTopRight = new Vector2((bounds.center.x + bounds.size.x / 2) + 1, (bounds.center.y + bounds.size.y / 2) + 1);
		pathFinding.grid.UpdateZone(boundsBottomLeft, boundsTopRight);
	}

	// Fonction exécutée côté serveur
	protected virtual void FixedUpdate()
	{
		// Si ce n'est pas le serveur
		if (!isServer) return;

		// On regarde si il y a un Path
		if (path == null) return;

		// Si il n'y a pas d'autre waypoint disponible sur le path defini (Fin du chemin)
		if (currentWaypoint >= path.Count)
		{
			if (path.Count == 1 && currentWaypoint == 1 && playerTarget)
			{
				float dis = Vector2.Distance(transform.position, path[currentWaypoint-1].pos);
				if (dis < stopDistanceWithPlayer)
				{
					rb.velocity = Vector2.zero;

					return;
				}
				else
				{
					currentWaypoint--;
				}
			} else {
				rb.velocity = Vector2.zero;

				return;
			}
		}

		// Calcul de la direction entre la position de l'ennemi et la position du waypoint pointé
		Vector2 direction = (path[currentWaypoint].pos - (Vector2)transform.position);
		// On applique la nouvelle velocity à l'ennemi
		rb.velocity = direction * stats.m_speed * speedBonus * Time.fixedDeltaTime;

		// Calcul de la distance entre la position de l'ennemi et la position du waypoint pointé
		float distance = Vector2.Distance(transform.position, path[currentWaypoint].pos);

		// Si la distance est inférieur à la distance défini par la variable distanceForPassWaypoint alors on switch de waypoint
		if (!playerTarget)
		{
			if(distance < distanceForPassWaypoint)
				currentWaypoint++;
		} else
        {
			if (distance < stopDistanceWithPlayer)
				currentWaypoint++;
        }

		if (rb.velocity.y > 0 && (rb.velocity.y > Mathf.Abs(rb.velocity.x)))
			Flip(0);
		if (rb.velocity.y < 0 && (Mathf.Abs(rb.velocity.y) > Mathf.Abs(rb.velocity.x)))
			Flip(1);
		if (rb.velocity.x > 0 && (rb.velocity.x > Mathf.Abs(rb.velocity.y)))
			Flip(2);
		if (rb.velocity.x < 0 && (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y)))
			Flip(3);
		if (rb.velocity.y == 0 && rb.velocity.x == 0)
			Flip(4);
	}

	// Fonction qui flip le graphique de l'ennemi
	// Fonction exécutée côté serveur
	[Server]
	protected virtual void Flip(int _movement)
	{

		animator.SetInteger("Movement", _movement);

		RPCFlip(_movement);
	}

	// Tourne l'ennemi pour les clients
	// Fonction exécutée sur chaque client connecté au serveur
	[ClientRpc]
	protected virtual void RPCFlip(int _movement)
	{
		// Si c'est le serveur et un joueur on return
		if (isServer && isClient) return;

		animator.SetInteger("Movement", _movement);
	}

	[ClientRpc]
	private void RpcPlayInterrogationParticle()
    {
		interrogationPoint.Play();
	}

    private void OnDrawGizmosSelected()
    {
        if(path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
				if (i == 0) Debug.DrawLine(transform.position, path[i].pos);
				if (i + 1 < path.Count) Debug.DrawLine(path[i].pos, path[i + 1].pos);
				if (i + 1 == path.Count) {
					if (playerTarget)
						Debug.DrawLine(path[i].pos, playerTarget.position);
					else
						Debug.DrawLine(path[i].pos, targetPosAlea);
				}
			}
		}
    }

    // Fonction qui update la zone autour de l'ennemi à sa mort
    // Fonction exécutée côté serveur
    protected virtual void OnDestroy()
    {
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (CustomNetworkManager.singleton.loadingNewScene) return;

		// Drop de l'experience
		Experience.Create(transform.position, Random.Range(2, 15));
		Coin.Create(transform.position, Random.Range(2, 12));

		//UpdateNearZone();
	}

	public void SetUpPathFinding(PathFinding _pathFinding)
    {
		pathFinding = _pathFinding;
    }

}
