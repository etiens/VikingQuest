using UnityEngine;
using System.Collections;

public class ScaleBoopSharp : MonoBehaviour
{
	private Vector3 startScale;
	private float boopAmount = 0.3f;
	private float boopTime = 0.3f;
	private float currentTimer = 0f;
	
	// Use this for initialization
	void Start ()
	{
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentTimer += Time.deltaTime;
		
		transform.localScale = startScale + Vector3.one * boopAmount * ((boopTime - currentTimer) / boopTime);
		if (currentTimer > boopTime)
		{
			currentTimer -= boopTime;
		}
	}
}