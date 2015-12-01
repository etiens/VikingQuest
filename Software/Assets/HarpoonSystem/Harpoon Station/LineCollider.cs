using UnityEngine;
using System.Collections;

public class LineCollider : MonoBehaviour {
	
	int numCollision = 0;
	bool verificationMade = false;
	
	void LateUpdate(){
		if(verificationMade){
			if (numCollision > 0){
				GlobalScript.Instance.Harpooner.GetCurrentStation().setLineHit(true);
			}else{
				GlobalScript.Instance.Harpooner.GetCurrentStation().setLineHit(false);
			}
			numCollision = 0;
			verificationMade = false;
		}
	}
	
	void OnTriggerStay(Collider col){
		if (col.gameObject.tag == "Iceberg" 
		|| col.gameObject.tag == "KrakenEye" 
		|| col.gameObject.tag == "ThrowingIceberg" 
		|| col.gameObject.tag == "Tentacle"){
			numCollision++;
		}
		verificationMade = true;
	}
}
	