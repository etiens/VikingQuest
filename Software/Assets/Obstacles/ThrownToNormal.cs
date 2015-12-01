using UnityEngine;
using System.Collections;

public class ThrownToNormal : MonoBehaviour {
	
	private float drag = 0f;
	// Use this for initialization
	void Start () {
		drag = rigidbody.drag;
		rigidbody.drag = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(/*rigidbody.velocity.magnitude > 1 && */transform.position.y < 0.0f){
			this.tag = "Iceberg";
			rigidbody.drag = drag;
			GetComponent<MeshCollider>().isTrigger = false;
			Destroy (this);
		}
	}
}
