using UnityEngine;
using System.Collections;

public class Harpoon : MonoBehaviour {
	
	public Vector3 Direction{get; private set;}
	public float speed = 20f;
	public float lifetime = 3.0f;
	public int iceDamage = 1;
	public int krakenDamage = 1;
	public int tentacleDamage = 1;
	public int eyeDamage = 35;
	
	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		if (this.networkView.isMine || Utils.IsLocal) {
			Quaternion newRotation = Quaternion.LookRotation(rigidbody.velocity);
			transform.rotation = newRotation;
			transform.Rotate(90f, 0f, 0f);
			lifetime -= Time.deltaTime;
			if(lifetime < 0.0f){
				Utils.Destroy(gameObject);
			}
		}
	}

	virtual protected void OnTriggerEnter(Collider collider) {
		if(Network.isClient)
			return;
			
		switch(collider.gameObject.tag)
		{
			case "Iceberg":
			collider.transform.GetComponent<Destructible>().ApplyDamage(iceDamage, transform.position);
			Utils.Destroy(gameObject);
			break;
			
			case "Kraken":
			collider.transform.GetComponentInParent<Boss>().ApplyDamage(krakenDamage);
			Utils.Destroy(gameObject);
			break;
			
			case "KrakenEye":
			collider.transform.GetComponent<KrakenEye>().ApplyDamage(eyeDamage);
			Utils.Destroy(gameObject);
			break;
			
			case "Tentacle":
			collider.transform.GetComponentInParent<Tentacle>().ApplyDamage(tentacleDamage);
			Utils.Destroy(gameObject);
			break;
			
			case "Water":
			Utils.Destroy(gameObject);
			break;
			
			case "ThrowingIceberg":
			collider.gameObject.GetComponent<Iceberg>().ApplyDamage(iceDamage);
			Utils.Destroy(gameObject);
			break;
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		bool isEnabled = false;
		if (stream.isWriting) {
			isEnabled = renderer.enabled;
			stream.Serialize(ref isEnabled);
		} else {
			stream.Serialize(ref isEnabled);
			renderer.enabled = isEnabled;
		}
	}
	
	virtual public void SetDirection(Vector3 direction){
		Direction = direction;
		Quaternion newRotation = Quaternion.LookRotation(Direction);
		transform.rotation = newRotation;
		transform.Rotate(90f, 0f, 0f);
		transform.rigidbody.AddForce(Direction * speed, ForceMode.VelocityChange);
		renderer.enabled = true;
	}
	
	public void EnableRenderer()
	{
		renderer.enabled = true;
	}
}
