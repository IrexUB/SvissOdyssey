using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AGrid))]
public class PathFinding : MonoBehaviour
{
	public AGrid grid;

	void Start()
	{
		// Récupération de la grille
		if (grid == null)
			grid = GetComponent<AGrid>();
	}

	public List<Node> GetPath(Vector2 startPos, Vector2 targetPos)
	{
		Node startNode = grid.WorldPositionToNode(startPos);
		Node targetNode = grid.WorldPositionToNode(targetPos);

		// Si la node recherché est pas marchable alors on retourne le chemin vers la node de début
		if(!targetNode.walkable)
        {
			List<Node> returnNode = new List<Node>();
			returnNode.Add(targetNode);
			return returnNode;
        }

		// Si la node de départ ou d'arrivé est vide alors on return
		if (startNode == null || targetNode == null)
			return null;

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while(openSet.Count > 0)
		{
			// Récupération de la première node de la liste
			Node currentNode = openSet[0];
			// Récupération de la node la plus proche / susceptible d'être la plus proche de la target
			for (int i = 1; i < openSet.Count; i++)
			{
				if(openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
				{
					currentNode = openSet[i];
				}
			}

			// On passe la node actuelle en vu
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			// Si la node actuelle est la node que l'on souhaite
			if (currentNode == targetNode)
			{
				// On retrace la chemin de la target jusqu'au point de départ
				return RetracePath(startNode, targetNode);
			}
			
			// Pour chaque voison de la node actuelle
			foreach(Node neighbour in grid.GetNeighbour(currentNode))
			{
				// Si la node voisine n'est pas marchable ou qu'elle à déjà était vu alors on la passe
				if(!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				// Calcul du cout pour aller au voisin
				int movementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

				// Si le cout du mouvement est inférieur au cout à la distance depuis l'arrivé
				// OU que le voisin n'est pas dans la liste des nodes à visiter
				if(movementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					// On actualise les valeurs gCost et hCost pour avoir le meilleur chemin
					neighbour.gCost = movementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);

					// Stockage du parent pour récupérer le chemin final
					neighbour.parent = currentNode;

					// Si le voisin n'est pas dans la liste des node à visiter alors on le rajoute
					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

		return null;
	}

	// Cette fonction permet de récupérer le chemin obtenue grâce à la fonction GetPath()

	List<Node> RetracePath(Node startNode, Node endNode)
	{
		// Liste qui va prendre toute les nodes pour le chemin final
		List<Node> path = new List<Node>();
		// Récupération de la dernière node
		Node currentNode = endNode;

		// Tant que la node actuelle n'est pas la node de départ
		while(currentNode != startNode)
		{
			// On ajout la node actuelle au chemin à parcourir
			path.Add(currentNode);
			// On récupère le parent de la node actuelle
			currentNode = currentNode.parent;
		}

		// On retourne la liste pour partir de la node de départ à la node finale
		path.Reverse();

		return path;
	}

	// Retourne la distance entre une node de début "startNode" et la target (targetNode)
	// par rapport au node de la grid sans compter les obstacles / penalité
	int GetDistance(Node startNode, Node targetNode)
	{
		int distanceX = Mathf.RoundToInt(Mathf.Abs(targetNode.gridXY.x - startNode.gridXY.x));
		int distanceY = Mathf.RoundToInt(Mathf.Abs(targetNode.gridXY.y - startNode.gridXY.y));

		return distanceX + distanceY;
	}
}
