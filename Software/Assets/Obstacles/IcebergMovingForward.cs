using UnityEngine;
using System.Collections;

public class IcebergMovingForward : Iceberg {

	private Vector3 initialForward;

	[SerializeField] 
	private float disappearTimer = -1;

	// Use this for initialization
	override public void Start () {
		base.Start();
		initialForward = transform.forward;
		initialForward.y = 0;

	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update();
		rigidbody.AddForce (initialForward * 50);

		if (disappearTimer > -0.5f)
		{
			disappearTimer -= Time.deltaTime;
			if (disappearTimer <= 0)
			{
				isDying = true;
			}
		}
	}
}
