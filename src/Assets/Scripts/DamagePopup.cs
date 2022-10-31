using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class DamagePopup : NetworkBehaviour
{
	public static DamagePopup Create(Vector2 pos, float damage, bool isCritical, bool _colorChange = false)
	{
		GameObject instance = Instantiate(GameManager.instance.damagePopupPrefab, pos, Quaternion.identity);
		DamagePopup popup = instance.GetComponent<DamagePopup>();
		if (_colorChange)
			popup.SetPopUpColor();
		popup.damage = damage;
		popup.isCritical = isCritical;

		NetworkServer.Spawn(instance);

		return popup;
	}

	[SyncVar] private float damage;
	[SyncVar] private bool isCritical;
	private TextMeshPro _textMesh;
	private Color _textColor;
	private const float _MOVE_Y_SPEED = 0.75f;
	private const float _DISAPPEAR_SPEED = 2f;
	private static int _orderInLayer = 0;

	private float _disappearTime = 1.5f;

	// A la création d'une nouvelle popup de damage
	private void Awake()
	{
		_textMesh = GetComponent<TextMeshPro>();
		_textColor = new Color(255f / 255f, 0f / 255f, 0f / 255f, 125f / 255f);
	}

	private void Start()
	{
		Setup();
	}

	public void Setup()
	{
		
		_orderInLayer++;

		// Si c'est un coup critique
		if (isCritical)
		{
			_textMesh.fontSize += 2;
			_textColor = new Color(255f/255f, 0f/255f, 0f/255f, 255f/255f);
		}

		_textMesh.SetText(((int)damage).ToString());
		_textMesh.color = _textColor;
		_textMesh.sortingOrder = _orderInLayer;
	}

	private void Update()
	{
		transform.position += new Vector3(0, _MOVE_Y_SPEED) * Time.deltaTime;

		_disappearTime -= Time.deltaTime;
		// Si la popup doit commencer à disparaitre
		if(_disappearTime <= 0)
		{
			_textColor.a -= _DISAPPEAR_SPEED * Time.deltaTime;
			_textMesh.color = _textColor;

			if (_textColor.a <= 0)
				NetworkServer.Destroy(gameObject);
		}
	}
	private void SetPopUpColor()
	{
		_textColor = new Color(0f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
	}
}
