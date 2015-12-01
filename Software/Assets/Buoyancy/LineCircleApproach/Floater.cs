using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A floater script handles the buoyancy of its parent's  RigidBody.
/// The floaters should be placed at corners as an empty 3D child of the RigidBody.
/// </summary>
public class Floater : MonoBehaviour {

	[SerializeField]
	private float bounceDamp = 0.5f;

	private Rigidbody body;
	private BuoyancyPlane water;

	[SerializeField]
	private float floaterCount = 0;

	[SerializeField]
	private float floatDistance = 2;

	void Start()
	{
		body = gameObject.GetComponentInParent<Rigidbody> ();
		water = GameObject.Find ("MainWaterPlane").GetComponentInChildren<BuoyancyPlane> ();

		if (floaterCount == 0)
		{
			floaterCount = gameObject.GetComponentsInParent<Floater> ().Count ();
		}
	}

	void FixedUpdate ()
	{
		Vector3 currentPosition = transform.position;
		Vector2 current2DPosition = new Vector2(currentPosition.x, currentPosition.z);

		float forceFactor = 1f - (((currentPosition.y - floatDistance) - water.GetYAtPosition(current2DPosition)));
		
		if (forceFactor > 0f)
		{
			Vector3 uplift = -Physics.gravity * (forceFactor - body.velocity.y * bounceDamp) / floaterCount;
			body.AddForceAtPosition(uplift, currentPosition);
		}

	}
}
