using UnityEngine;
using System.Collections.Generic;

public class AGrid : MonoBehaviour
{
	private Node[,] grid;

	[SerializeField] private Vector2 gridWorldSize = new Vector2(0, 0);
	[SerializeField] private float nodeDiameter = 1f;

	[Header("Parameters")]
	[SerializeField] private LayerMask noWalkableLayer;
	[SerializeField] private LayerMask test;
	[SerializeField] private int penalityEnemyZone = 10;

	// Cette variable nous donne le nombre de node par x / y (int);
	private Vector2 gridSize;
	private Vector2 bottomLeft;

	void Start()
	{
		gridSize = new Vector2(Mathf.RoundToInt(gridWorldSize.x / nodeDiameter), Mathf.RoundToInt(gridWorldSize.y / nodeDiameter));

		CreateGrid();
	}

	// Fonction exécutée au démarrage pour créer la grille pour le pathfinding
	public void CreateGrid()
	{
		grid = new Node[(int)gridSize.x, (int)gridSize.y];
		bottomLeft = new Vector2(transform.position.x - (gridWorldSize.x / 2), transform.position.y - (gridWorldSize.y / 2));

		for (int y = 0; y < gridSize.y; y++)
		{
			for (int x = 0; x < gridSize.x; x++)
			{
				Vector2 pos = bottomLeft + Vector2.right * (x * nodeDiameter + nodeDiameter / 2) + Vector2.up * (y * nodeDiameter + nodeDiameter / 2);
				bool walkable = !(Physics2D.OverlapCircle(pos, nodeDiameter / 2, noWalkableLayer));
				
				int enemyZonePenality = Physics2D.OverlapCircle(pos, nodeDiameter / 2, (int)Mathf.Pow(2, LayerMask.NameToLayer("EnemyZone"))) ? penalityEnemyZone : 0;
				grid[x,y] = new Node(pos, walkable, 0, enemyZonePenality, new Vector2(x, y));
			}
		}

	}

	// Fonction qui rafraichie une zone entre 2 positions
	public void UpdateZone(Vector2 _bottomLeftPos, Vector2 _topRightPos)
	{
        for (float y = _bottomLeftPos.y; y <= _topRightPos.y; y += nodeDiameter)
        {
            for (float x = _bottomLeftPos.x; x < _topRightPos.x; x += nodeDiameter)
            {
				Node node = WorldPositionToNode(new Vector2(x, y));

				bool walkable = !(Physics2D.OverlapCircle(node.pos, nodeDiameter / 2, noWalkableLayer));
				int enemyZonePenality = 0;
				if(Physics2D.OverlapCircle(node.pos, nodeDiameter / 2, (int)Mathf.Pow(2, LayerMask.NameToLayer("EnemyZone"))))
                {
					//Debug.Log("EnemyZone");
					enemyZonePenality = penalityEnemyZone;
				}

				if(Physics2D.OverlapCircle(node.pos, nodeDiameter / 2, (int)Mathf.Pow(2, LayerMask.NameToLayer("Enemy"))))
                {
					//Debug.Log("Enemy");
					enemyZonePenality = 50000;
                }

				//Debug.Log("EnemyZonePenality : " + enemyZonePenality);

				node.walkable = walkable;
				node.enemyZonePenality = enemyZonePenality;
            }
        }
	}

	// Retourne les 8 voisins (si possible) de la node mis en paramètre
	public List<Node> GetNeighbour(Node node)
	{
		if (node == null)
			return null;

		// Création d'une liste de voisins
		List<Node> neighbour = new List<Node>();

		// On boucle d'en bas à gauche à en haut à gauche
		for (int y = -1; y < 2; y++)
		{
			for (int x = -1; x < 2; x++)
			{
				// On retire la node mis en paramètre
				if (x == 0 && y == 0)
					continue;

				int calcX = (int)(node.gridXY.x + x);
				int calcY = (int)(node.gridXY.y + y);

				// Vérification si la node ne sort pas de la grid
				if (calcX < 0 || calcX >= gridSize.x)
					continue;
				if (calcY < 0 || calcY >= gridSize.y)
					continue;

				// Ajout de la node à la liste des voisins
				neighbour.Add(grid[calcX, calcY]);
			}
		}

		if (neighbour.Count == 0)
			return null;

		return neighbour;
	}

	// Fonction qui convertit une position en une case du pathfinding
	public Node WorldPositionToNode(Vector2 pos)
	{
		// Calcul de la position dans la grid
		int x = (int)((pos.x - bottomLeft.x) / nodeDiameter);
		int y = (int)((pos.y - bottomLeft.y) / nodeDiameter);

		if (x < 0)
			x = 0;
		if (x >= gridSize.x)
			x = (int)gridSize.x-1;
		if (y < 0)
			y = 0;
		if (y >= gridSize.y)
			y = (int)gridSize.y-1;

		return grid[x, y];
	}

	//void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.white;
	//	Gizmos.DrawWireCube(transform.position, gridWorldSize);

	//	// Dessinage des nodes
	//	if (grid != null)
	//	{
	//		//List<Node> ns = GetNeighbour(WorldPositionToNode(GameObject.FindGameObjectWithTag("Player").transform.position));
	//		foreach (var item in grid)
	//		{
	//			Gizmos.color = Color.blue;

	//			if (item.enemyZonePenality == 10)
	//				Gizmos.color = Color.cyan;

	//			if (item.enemyZonePenality > 10)
	//				Gizmos.color = Color.black;

	//			if (!item.walkable)
	//				Gizmos.color = Color.red;

	//			//if (ns != null)
	//			//	if (ns.Contains(item))
	//			//		Gizmos.color = Color.black;

	//			Gizmos.DrawWireCube(item.pos, Vector2.one * ((90 * nodeDiameter) / 100));
	//		}
	//	}
	//}
}
