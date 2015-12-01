using UnityEngine;
using System.Collections;

public class icebergBarrier : Iceberg {

	[SerializeField]
	protected ParticleSystem bigEmitter;
	[SerializeField]
	protected ParticleSystem smallEmitter;

	[SerializeField]
	protected int damageThreshold = 3;
	[SerializeField]
	protected GameObject iceShardPrefab;
	
	// Use this for initialization
	override public void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update();
		
	}
	
	protected override void ChangeColor()
	{
		foreach(Transform t in transform)
		{
			if(t.renderer != null && t.renderer is MeshRenderer)
			{
				t.renderer.material.SetColor("_Color",(damageReceivedPercent)*redColor + (1-damageReceivedPercent)*initialColor);
			}
		}
	}

	[RPC]
	override protected void Die(){
		breakingSoundSource.Play();
		isDying = true;
		collider.enabled = false;
		foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>())
		{
			rend.enabled = false;
		}
		Utils.NetworkCommand(this, "CreateParticles");
	}

	override public void ApplyDamage(int damage, Vector3 position){
		base.ApplyDamage (damage);
		harpoonImpactSource.Play();

		if (position != null)
		{
			if ((healthPoints - currentHealth) % damageThreshold != 0)
			{
				smallEmitter.gameObject.transform.position = position;
				smallEmitter.Simulate(0f, true, true);
				smallEmitter.Play(true);
			}
			else
			{
				bigEmitter.gameObject.transform.position = position;
				bigEmitter.Simulate(0f, true, true);
				bigEmitter.Play(true);

				var randomShard1 = (GameObject)Utils.Instantiate (iceShardPrefab, position, Quaternion.identity);
				if(randomShard1!=null)
				{
					randomShard1.transform.forward = new Vector3(Random.Range(0f,1f), 0, Random.Range(0f,1f));
					randomShard1.rigidbody.velocity = new Vector3(Random.Range(-20f,20f), 0, Random.Range(-20f,20f));
				}
				var randomShard2 = (GameObject)Utils.Instantiate (iceShardPrefab, position, Quaternion.identity);
				if(randomShard2 != null)
				{
					randomShard2.transform.forward = new Vector3(Random.Range(0f,1f), 0, Random.Range(0f,1f));
					randomShard2.rigidbody.velocity = new Vector3(Random.Range(-20f,20f), 0, Random.Range(-20f,20f));
				}
			}
		}
	}
}
