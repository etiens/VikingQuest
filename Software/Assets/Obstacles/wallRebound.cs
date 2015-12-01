using UnityEngine;
using System.Collections;

public class wallRebound : MonoBehaviour {

	public float reboundFactor = 500f;
	private GameObject boat;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "boat")
		{
			boat = GameObject.FindGameObjectWithTag("Player");

			boat.rigidbody.AddForceAtPosition(collision.contacts[0].normal * reboundFactor * 0.4f, collision.contacts[0].point );
			boat.rigidbody.AddForce(collision.contacts[0].normal * reboundFactor * 0.8f);

		}
	}
}
