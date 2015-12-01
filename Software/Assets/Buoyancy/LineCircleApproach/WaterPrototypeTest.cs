using UnityEngine;
using System.Collections;

/// <summary>
/// Simple manager to create waves at intervals.
/// </summary>
public class WaterPrototypeTest : MonoBehaviour {

	private BuoyancyPlane water;

	private float totalTime = 0;
	
	void Start()
	{
		water = GameObject.Find ("MainWaterPlane").GetComponentInChildren<BuoyancyPlane> ();
	}

	void Update()
	{
		totalTime += Time.deltaTime;

		if (totalTime > 5)
		{
			totalTime -= 25;
			water.CreateWaveLinear(Vector3.zero, new Vector3(1,0,1), new Vector3(-1,0,1), 10);
			water.CreateWaveCircle(Vector3.zero, 20);
		}
	}

}
