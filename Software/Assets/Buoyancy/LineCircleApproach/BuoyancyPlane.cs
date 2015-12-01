using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents any body of water.
/// </summary>
public class BuoyancyPlane : MonoBehaviour {

	private List<GameObject> wavesList = new List<GameObject>();
	public float yPos;

	public void Start()
	{
		yPos = gameObject.transform.position.y;
	}

	public float GetYAtPosition(Vector2 position)
	{
		float highestPosition = yPos;
		foreach(GameObject wave in wavesList)
		{
			highestPosition = Mathf.Max(highestPosition, wave.GetComponentInChildren<BuoyancyWaves>().GetYAtPosition(position));
		}
		return highestPosition;
	}

	public void CreateWaveLinear(Vector3 position, Vector3 orientation, Vector3 direction, float speed)
	{
		GameObject newWave = Instantiate (Resources.Load ("WaveLinePrefab")) as GameObject;
		newWave.transform.position = position;
		Vector3 rotation = newWave.transform.rotation.eulerAngles;
		rotation.y = (float) (System.Math.Atan2 (orientation.z, orientation.x) * 180 / System.Math.PI);
		newWave.transform.rotation = Quaternion.Euler(rotation);
		newWave.GetComponentInChildren<BuoyancyLine> ().speed = speed;
		newWave.GetComponentInChildren<BuoyancyLine> ().direction= direction;
		newWave.GetComponentInChildren<BuoyancyLine> ().orientation= orientation;
		wavesList.Add (newWave);
	}

	public void CreateWaveCircle(Vector3 position, float speed)
	{
		GameObject newWave = Instantiate (Resources.Load ("WaveCirclePrefab")) as GameObject;
		newWave.transform.position = position;
		newWave.GetComponentInChildren<BuoyancyCircle> ().scaleSpeed = speed;
		wavesList.Add (newWave);
	}

}
