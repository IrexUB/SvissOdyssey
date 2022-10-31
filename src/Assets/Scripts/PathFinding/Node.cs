using UnityEngine;

public class Node
{
	public Vector2 pos;
	public bool walkable = true;
	public int landPenality = 0;
	public int enemyZonePenality = 0;

	public Vector2 gridXY;

	// Distance depuis le départ
	public int gCost = 0;
	// Distance par rapport à l'arrivé
	public int hCost = 0;

	// Parent de la node qui permet de retracer le chemin à la fin
	public Node parent;

	// Constructeur
	public Node(Vector2 _pos, bool _walkable, int _landPenality, int _enemyZonePenality, Vector2 _gridXY)
    {
		pos = _pos;
		walkable = _walkable;
		landPenality = _landPenality;
		enemyZonePenality = _enemyZonePenality;
		gridXY = _gridXY;
    }

	// Addition du gCost et du hCost avec la penalité si on passe sur cette case
	public int fCost
    {
		get
        {
			return gCost + hCost + (enemyZonePenality + landPenality);
        }
    }
}
