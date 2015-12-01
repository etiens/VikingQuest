using UnityEngine;
using System.Collections;

public class Iceberg : Destructible {
	
	public AudioClip harpoonImpactClip;
	public AudioClip breakingSoundClip;
	protected AudioSource breakingSoundSource;
	protected AudioSource harpoonImpactSource;
	protected bool isDying = false;
	
	// Use this for initialization
	override public void Start () {
		base.Start();
		breakingSoundSource = gameObject.AddComponent<AudioSource>();
		harpoonImpactSource = gameObject.AddComponent<AudioSource>();
		harpoonImpactSource.clip = harpoonImpactClip;
		harpoonImpactSource.loop = false;
		harpoonImpactSource.playOnAwake = false;
		breakingSoundSource.clip = breakingSoundClip;
		breakingSoundSource.loop = false;
		breakingSoundSource.playOnAwake = false;
	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update();
		ManageDeath();
		
	}
	
	protected void ManageDeath(){
		if(isDying){
			if (!breakingSoundSource.isPlaying){
				Utils.Destroy(gameObject);
			}
		}
	}
	
	[RPC]
	protected void CreateParticles(){
		if (particlesObject != null)
		{
			Utils.Instantiate(particlesObject, transform.position, transform.rotation);
		}
	
	}
	
	[RPC]
	protected virtual void Die(){
		breakingSoundSource.Play();
		isDying = true;
		collider.enabled = false;
		renderer.enabled = false;
		Utils.NetworkCommand(this, "CreateParticles");
	}
	
	override public void ApplyDamage(int damage){
		currentHealth -= damage;
		harpoonImpactSource.Play();
		Utils.NetworkCommand(this,"FacheToiRouge");
		if (currentHealth <= 0){
			Utils.NetworkCommand(this, "Die");
		}
	}
}
