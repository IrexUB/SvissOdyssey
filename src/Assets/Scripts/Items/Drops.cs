using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drops : MonoBehaviour
{
	[Header("Drops")]
	public GameObject[] drops;
	public int[] dropsLuck;

	public bool drop = false;

	// V�rification d'un erreur de compatibilit� entre les 2 variables
	void Start()
	{
		if (drops.Length != dropsLuck.Length)
		{
			Debug.Log("[Erreur] Un probl�me avec les variables drops et dropsLuck ! [Destruction de l'objet]", gameObject);
			Destroy(gameObject);
		}

		drop = false;
	}

	public void dropItem(Vector3 _pos)
    {
		// Si le script n'a jamais drop d'item alors il peut drop
		if (!drop)
		{
			drop = true;

			// Si l'objet poss�de des objets � faire spawn
			if (drops.Length > 0)
			{
				// G�n�ration d'un nombre al�atoire
				int rdm = Random.Range(1, 100);

				// V�rification et apparition de chaque item
				for (int i = 0; i < drops.Length; i++)
				{
					if (rdm <= dropsLuck[i])
					{
						StartCoroutine(spawnItem(drops[i], _pos, (0.5f + (i * 0.25f))));
					}
				}
			}
		}
	}

	private IEnumerator spawnItem(GameObject _obj, Vector3 _pos, float _time)
    {
		yield return new WaitForSeconds(_time);
		GameObject dropItem = Instantiate(_obj, _pos, Quaternion.identity);
	}
}
