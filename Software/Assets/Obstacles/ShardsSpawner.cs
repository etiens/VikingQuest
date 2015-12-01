using UnityEngine;
using System.Collections;

public class ShardsSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject iceShardPrefab = null;

	private ParticleSystem particles;

	private bool activedParticles;
	private float spawnTimer;

	void Start () {

		particles = gameObject.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (activedParticles)
		{
			spawnTimer -= Time.deltaTime;

			if (spawnTimer <= 0)
			{
				var randomShard = (GameObject)Utils.Instantiate (iceShardPrefab, transform.position, transform.rotation);

				randomShard.transform.forward = transform.forward;
				randomShard.rigidbody.velocity = new Vector3(Random.Range(-5f,5f), 0, Random.Range(-5f,5f));
				activedParticles = false;
				particles.Stop();
			}
		}
	}

	public void SpawnTimeFromNow(float seconds)
	{
		if (!activedParticles)
		{
			spawnTimer = seconds;
			activedParticles = true;
			particles.Simulate(0f, true, true);
			particles.Play(true);
		}
	}
}
