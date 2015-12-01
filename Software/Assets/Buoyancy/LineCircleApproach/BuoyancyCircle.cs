using UnityEngine;
using System.Collections;

/// <summary>
/// Buoyancy circle is a circular wave.
/// </summary>
public class BuoyancyCircle : BuoyancyWaves {

	public float scaleSpeed { get; set; }

	void Update () 
	{
		Vector3 scale = gameObject.transform.localScale;
		scale.x += scaleSpeed * Time.deltaTime;
		scale.z += scaleSpeed * Time.deltaTime;
		gameObject.transform.localScale = scale;
	}

	public override float GetYAtPosition(Vector2 position)
	{
		Vector3 pos3 = new Vector3(position.x, gameObject.transform.position.y + 50, position.y);
		Vector2 current2DPosition = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.z);

		// upperBound and lowerBound should be changed accordingly to the 3D model if necessary.
		var upperBound = (this.GetComponent<MeshFilter> ().mesh.bounds.size.x)*(transform.localScale.x)/2;
		var lowerBound = upperBound * 0.6;
		var relativePosition = (position - current2DPosition).magnitude;
		if (relativePosition < upperBound && relativePosition > lowerBound)
		{
			RaycastHit hit;
			if (Physics.Raycast(pos3, Vector3.down, out hit))
			{
				if (hit.transform.tag == "Water")
				{
					return hit.point.y;
				}
			}
		}

		return 0;
	}
}
