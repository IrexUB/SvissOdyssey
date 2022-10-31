using UnityEngine;
using TMPro;
using Mirror;

public class StatusNumberGenerator : NetworkBehaviour
{
	[SerializeField] private TextMeshPro _textMesh;
	[SyncVar] private int number = 0;

	private void Start()
	{
		if(_textMesh == null)
			_textMesh = transform.GetChild(1).GetComponent<TextMeshPro>();

		_textMesh.SetText(number.ToString());
	}

	[Server]
	public void SetNumber(int newNumber)
	{
		number = newNumber;

		RpcSetNumber(number);
	}

	[ClientRpc]
	private void RpcSetNumber(int newNumber)
	{
		_textMesh.SetText(number.ToString());
	}
}

