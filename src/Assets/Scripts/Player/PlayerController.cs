using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Script exécuté uniquement sur le client à qui appartient le joueur

public class PlayerController : NetworkBehaviour
{
	[SerializeField()] private bool m_isFacingRight = true;

	[Header("Stats")]
	public PlayerStats m_stats;

	[Header("Déplacement")]
	private Vector2 m_direction;

	[Header("Components")]
	[SerializeField] private Rigidbody2D m_rb2d;
	public Rigidbody2D GetRigidbody2D { get => m_rb2d; }
	[SerializeField] private SpriteRenderer m_spriteRenderer;
	public SpriteRenderer GetGFX { get => m_spriteRenderer; }
	[SerializeField] private Collider2D m_collider2D;
	public Collider2D GetRealPlayerCollider { get => m_collider2D; }
	[SerializeField] private Collider2D m_DashCollider;
	public Collider2D GetDashCollider { get => m_DashCollider; }
	[SerializeField] private Animator m_animator;

	[Header("Object")]
	[SerializeField] private Transform m_swordPivot;
	[SerializeField] private Camera m_camera;
	[SerializeField] public Inventory inventory;

	[Header("Other Variables")]
	public Marchand marchandNear;

	[Header("Reanimation")]
	[SerializeField] private float reanimationDistance = 2f;
	private Health playerToRevive;
	private bool isReanimating = false;

	void Start()
	{
		m_isFacingRight = true;
		m_stats = GetComponent<PlayerStats>();

		if (!m_rb2d || !m_swordPivot || !m_spriteRenderer)
			Debug.LogError("Attention un component/objet est null !", gameObject);
	}

	void Update()
	{
		// Si le manager existe
		if(UIManager.instance)
			// On actualise les stats du joueur
			UIManager.instance.UpdateStatsText(m_stats);

		//
		if (Input.GetKeyDown(KeyCode.L))
		{
			SkillTreeUIManager.instance.ChangeStateTreeAbility();
        }

		// Si on appuye sur la touche E
		if (Input.GetKeyDown(KeyCode.M))
		{
			// Si il y a une marchand proche
			if(marchandNear)
			{
				// Ouverture du marchand
				if(!UIManager.instance.GetTraderUIState)
					UIManager.instance.OpenTraderUI(marchandNear.GetInventory);
				// Fermeture du marchand
				else
					UIManager.instance.CloseTraderUI();
			}
		}

		UIManager.instance.ChangeInventoryInterface(netIdentity);

		// Si l'application n'est pas focus alors on ne fait rien ou qu'une interface est ouverte
		if (!Application.isFocused || SkillTreeUIManager.instance.GetAbilityTreeState || UIManager.instance.GetTraderUIState)
		{
			m_direction.x = 0;
			m_direction.y = 0;
			return;
		}

		m_direction.x = Input.GetAxisRaw("Horizontal");
		m_direction.y = Input.GetAxisRaw("Vertical");

		// Rotation du personnage
		if (m_rb2d.velocity.y > 0 && (m_rb2d.velocity.y > Mathf.Abs(m_rb2d.velocity.x)))
			Flip(0);
		if (m_rb2d.velocity.y < 0 && (Mathf.Abs(m_rb2d.velocity.y) > Mathf.Abs(m_rb2d.velocity.x)))
			Flip(1);
		if (m_rb2d.velocity.x > 0 && (m_rb2d.velocity.x > Mathf.Abs(m_rb2d.velocity.y)))
			Flip(2);
		if (m_rb2d.velocity.x < 0 && (Mathf.Abs(m_rb2d.velocity.x) > Mathf.Abs(m_rb2d.velocity.y)))
			Flip(3);
		if (m_rb2d.velocity.y == 0 && m_rb2d.velocity.x == 0)
			Flip(4);



		bool finalUIState = false;
		var listOfPlayerNext = NetworkServer.connections;
		// Vérification si de tous les joueurs autour du joueur pour savoir si ils ont besoin de réanimation
		foreach (var player in listOfPlayerNext)
		{
			Transform p = player.Value.identity.transform;
			float distance = Vector2.Distance(transform.position, p.position);
			if (distance > reanimationDistance) continue;

			Health pHealth = p.GetComponent<Health>();

			if (pHealth.isDied)
			{
				finalUIState = true;
				playerToRevive = pHealth;
				break;
			}
		}
		if (!finalUIState) playerToRevive = null;
		UIManager.instance.ChangeCanReviveStateUI(finalUIState);

		//////////////// Fonctionnalité par forcément fonctionnelle
		// Si on peut réanimer un joueur
		if (Input.GetKeyDown(KeyCode.X) && playerToRevive)
		{
			if (!isReanimating)
			{
				playerToRevive.CmdTryReanimation(PlayerInfo.instance.pseudo);
				m_animator.SetBool("IsReviveur", true);
				isReanimating = true;
			}
		}
		else
		{
			if (isReanimating)
			{
				m_animator.SetBool("IsReviveur", false);
				playerToRevive.CmdStopReanimation();
				isReanimating = false;
			}
		}
		////////////////

		RotateSword();
	}

	private void FixedUpdate()
	{
		m_rb2d.velocity = m_direction.normalized * (m_stats.m_speed);
	}

	private void Flip(int movement)
	{
		m_animator.SetInteger("Movement", movement);
	}

	private void RotateSword()
	{
		Vector3 diff = m_camera.ScreenToWorldPoint(Input.mousePosition) - m_swordPivot.position;
		diff.Normalize();

		float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		m_swordPivot.rotation = Quaternion.Euler(
			m_swordPivot.rotation.eulerAngles.x,
			0,
			rotZ
		);
	}

	// Fonction exécutée sur le joueur pour en cas de tentative achat d'un objet
	public void callBuyItem(int slotID)
	{
		if(marchandNear)
		{
			marchandNear.CmdBuyItem(netIdentity, slotID);
		}
	}
}
