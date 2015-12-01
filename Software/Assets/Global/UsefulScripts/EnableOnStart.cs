using UnityEngine;
using System.Collections;

public class EnableOnStart : MonoBehaviour {

	[SerializeField]
	private GameObject objectToActivate = null;

	// Use this for initialization
	void Start () 
	{
		objectToActivate.SetActive (true);
	}
}
