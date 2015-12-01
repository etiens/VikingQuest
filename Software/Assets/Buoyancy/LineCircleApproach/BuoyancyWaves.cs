using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract class for BuoyancyPlane to use as a list of different types of waves.
/// </summary>
public abstract class BuoyancyWaves : MonoBehaviour {
	
	public abstract float GetYAtPosition(Vector2 position);
}
