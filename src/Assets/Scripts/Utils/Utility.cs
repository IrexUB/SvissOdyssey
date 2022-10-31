using System.Collections;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static float GetAngleTowardsMouse(Transform startPos)
    {
		Vector2 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPos.position;
		diff.Normalize();

		float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		return angle;
	}

	public static Vector2 GetMouseDirection(Transform startPos)
	{
		Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPos.position;
		direction.Normalize();

		return direction;
	}
}