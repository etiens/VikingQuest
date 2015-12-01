using UnityEngine;
using System.Collections;

public class ResetThisTransform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (0, 0, 0);
		transform.localScale = new Vector3 (1, 1, 1);
		transform.rotation = Quaternion.identity;
	}
}
