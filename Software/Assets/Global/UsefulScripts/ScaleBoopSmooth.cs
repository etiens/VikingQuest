using UnityEngine;
using System.Collections;

public class ScaleBoopSmooth : MonoBehaviour
{
	private Vector3 startScale;
	private float boopAmount = 0.4f;
	private float boopHalfTime = 0.5f;
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

		if (currentTimer < boopHalfTime)
		{
			transform.localScale = startScale + Vector3.one * boopAmount * (currentTimer / boopHalfTime);
		}
		else
		{
			transform.localScale = startScale + Vector3.one * boopAmount - Vector3.one * boopAmount * ((currentTimer - boopHalfTime) / boopHalfTime);
		}
		
		if (currentTimer > boopHalfTime*2)
		{
			currentTimer -= boopHalfTime*2;
		}
	}
}