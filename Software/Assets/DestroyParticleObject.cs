using UnityEngine;
using System.Collections;

public class DestroyParticleObject : MonoBehaviour {

	private ParticleSystem particleObject;

	private float recktTimer;

	void Start () 
	{
		particleObject = gameObject.GetComponent<ParticleSystem> ();
		recktTimer = particleObject.duration;
	}

	void Update () 
	{
		recktTimer -= Time.deltaTime;
		if (recktTimer <= 0)
		{
			Utils.Destroy(gameObject);
		}
	}

}
