using UnityEngine;
using System.Collections;

public class CrushWallCloser : MonoBehaviour
{
	[SerializeField] float closeSequenceTime = 2f;
	float closeSequenceTimer = 0f;
	bool sequenceStarted = false;
	[SerializeField] float submergeHeight = 80f;
	[SerializeField] BoxCollider detectionArea = null;
	Vector3 submergedPosition = Vector3.zero;
	Vector3 initialPosition = Vector3.zero;
	[SerializeField] GameObject ClosingWall = null;
	
	void Awake()
	{
		if(detectionArea == null)
			detectionArea = GetComponent<BoxCollider>() as BoxCollider;
		initialPosition = ClosingWall.transform.position;
		submergedPosition = initialPosition - Vector3.up*submergeHeight;
		ClosingWall.transform.position = submergedPosition;
		
		if(Network.isClient)
			gameObject.SetActive(false);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(sequenceStarted)
		{
			closeSequenceTimer += Time.deltaTime;
			ClosingWall.transform.position = Vector3.Lerp (submergedPosition,initialPosition,closeSequenceTimer/closeSequenceTime);
			if(closeSequenceTimer >= closeSequenceTime)
				sequenceStarted = false;
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "boat")
		{
			sequenceStarted = true;
			detectionArea.enabled = false;
			closeSequenceTimer = 0f;
		}
	}
}

