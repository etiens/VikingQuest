using UnityEngine;
using System.Collections;

public class FollowObjectForCamera : MonoBehaviour {

	public Transform followThis;
	private GameObject cameraToFollow;

	void Update () {

		if (cameraToFollow == null)
		{
			if (GlobalScript.Instance.Camera != null)
			{
				cameraToFollow = GameObject.Find("Main Camera");
			}
		}

		if (followThis != null)
		{
			var position = transform.position;
			position.x += (followThis.position.x - position.x) / 2f;
			position.y += (followThis.position.y - position.y) / 2f;
			position.z += (followThis.position.z - position.z) / 2f;
			transform.position = position;
		}

		if (cameraToFollow != null)
		{
			var rotation = transform.rotation;
			var orientation = rotation.eulerAngles;
			orientation.y = cameraToFollow.transform.rotation.eulerAngles.y;
			rotation.eulerAngles = orientation;
			transform.rotation = rotation;
		}
	}
}
