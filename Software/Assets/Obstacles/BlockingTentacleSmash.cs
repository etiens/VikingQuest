using UnityEngine;
using System.Collections;

public class BlockingTentacleSmash : MonoBehaviour
{
		[SerializeField] BoxCollider detectionCollider = null;
		[SerializeField] Tentacle blockingTentacle = null;
		
		void Awake()
		{
			if(detectionCollider == null)
				detectionCollider = GetComponentInChildren<BoxCollider>() as BoxCollider;
				
			if(blockingTentacle == null)
				blockingTentacle = GetComponentInChildren<Tentacle>() as Tentacle;
		}
		
		void OnTriggerEnter(Collider other)
		{
			if(other.gameObject.tag == "boat")
			{
				blockingTentacle.SmashAndHold();
				detectionCollider.enabled = false;
			}
		}
}

