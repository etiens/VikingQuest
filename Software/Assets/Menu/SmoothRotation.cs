using UnityEngine;
using System.Collections;

public class SmoothRotation : MonoBehaviour {

	private float accel;
	private float speed = 0;

	// Update is called once per frame
	void Update () {

		accel = Utils.Instance.Player1.LeftStickY();
		speed += 0.2f*(accel - 0.4f*(speed-0.5f));

		transform.Rotate (0f, 0f, speed);

	}

	void OnEnabled ()
	{
		speed = 0;
	}
}
