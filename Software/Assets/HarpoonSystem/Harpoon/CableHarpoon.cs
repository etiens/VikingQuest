using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CableHarpoon : Harpoon
{
	
	// Use this for initialization
	void Awake () {
		/*float speed = 20f;
		float lifetime = 6.0f;
		int iceDamage = 1;
		int krakenDamage = 1;*/
	}
	
	override protected void OnTriggerEnter(Collider collider) {
		if (!(collider.gameObject.tag == "boat")) {
			Utils.Destroy(gameObject);
			if(collider.gameObject.tag == "Kraken"){
				// Apply damage and effects here // 
				collider.transform.GetComponent<Destructible>().ApplyDamage(krakenDamage, transform.position);
			} else if(collider.gameObject.tag == "Iceberg"){
				// Apply damage and effects here // 
				collider.transform.GetComponent<Destructible>().ApplyDamage(iceDamage, transform.position);
			}
		}
	}
}


