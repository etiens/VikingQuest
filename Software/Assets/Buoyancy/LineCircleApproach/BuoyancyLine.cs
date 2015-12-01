//using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// Buoyancy line is an "infinite" length linear wave.
/// </summary>
public class BuoyancyLine : BuoyancyWaves {

	public float speed { get; set; }
	public Vector3 orientation { get; set; }
	public Vector3 direction { get; set; }

	void Update () 
	{
		Vector3 position = this.transform.parent.position;
		position += direction * speed * Time.deltaTime;
		this.transform.parent.position = position;
	}

	public override float GetYAtPosition(Vector2 position)
	{
		Vector2 currentPosition = new Vector2(gameObject.transform.parent.position.x, gameObject.transform.parent.position.z);
		Vector2 currentDirection = currentPosition;
		currentDirection.x += orientation.x;
		currentDirection.y += orientation.z;

		var vectorA = currentDirection - currentPosition;
		var vectorB = position - currentPosition;

		float distance = Mathf.Abs((vectorA.x * vectorB.y - vectorA.y * vectorB.x) / (currentDirection - currentPosition).magnitude) ;

		// Warning : these values are hardcoded to match the current 3D prefab.
		return 10 - distance / 2;
	}
}
