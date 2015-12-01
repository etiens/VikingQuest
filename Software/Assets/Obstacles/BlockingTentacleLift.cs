using UnityEngine;
using System.Collections;

public class BlockingTentacleLift : MonoBehaviour
{

	[SerializeField] BoxCollider detectionCollider = null;
	[SerializeField] Tentacle blockingTentacle = null;
	
	void Awake()
	{
		if(detectionCollider == null)
			detectionCollider = GetComponentInChildren<BoxCollider>() as BoxCollider;
		
		if(blockingTentacle == null)
			blockingTentacle = GetComponentInChildren<Tentacle>() as Tentacle;
			
		blockingTentacle.transform.position = blockingTentacle.transform.position - Vector3.up*80f;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "boat")
		{
			blockingTentacle.MovePosition(blockingTentacle.transform.position + Vector3.up*80f,3f);
			detectionCollider.enabled = false;
		}
	}
}

