using UnityEngine;
using System.Collections;

public class ShardFallingSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject iceFallingPrefab = null;

	[SerializeField]
	private float spawnTimer;

	[SerializeField]
	private Transform spawnLocation = null;
	[SerializeField]
	private ParticleSystem particles = null;

	private bool activedParticles = false;
	private bool stopped = false;

	private float stopParticlesInSeconds = 4f;

	void Start () {
		/*particles = spawnLocation.GetComponent<ParticleSystem> ();
		string asdf = "asdf";*/
	}
	
	// Update is called once per frame
	void Update () {

		if (activedParticles && !stopped)
		{
			spawnTimer -= Time.deltaTime;

			if (spawnTimer <= 0)
			{
				var fallingIceberg = (GameObject)Utils.Instantiate (iceFallingPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);
				stopped = true;
			}
		}

		if (stopped && stopParticlesInSeconds > 0)
		{
			stopParticlesInSeconds -= Time.deltaTime;
			if (stopParticlesInSeconds <= 0)
			{
				particles.Stop();
			}
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "boat")
		{
			if (!activedParticles)
			{
				activedParticles = true;
				particles.Simulate(0f, true, true);
				particles.Play(true);
			}
		}
	}
}
