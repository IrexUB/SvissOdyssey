using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "LevelStruct", menuName = "ScriptableObjects/Level", order = 1)]
public class Level : ScriptableObject
{
	[Scene] public string traderPlace;
	[Scene] public string tower;
	[Scene] public string boss;
}
