using System;
using UnityEngine;

public class KrakenEye: MonoBehaviour
{
	[SerializeField] int healthPoints = 100;
	public int HealthPoints {get{return healthPoints;}}
	public bool Closed {get{return healthPoints <= 0;}}
	private Kraken kraken;
	[SerializeField] MeshRenderer closedEyeRenderer = null;
	
	public void Start()
	{
		kraken = gameObject.GetComponentInParent<Kraken>() as Kraken;
		if(closedEyeRenderer == null) closedEyeRenderer = GetComponent<MeshRenderer>() as MeshRenderer;
		closedEyeRenderer.enabled = false;
	}
	
	public void ApplyDamage(int damage)
	{
		if(Closed)
			return;
		healthPoints -= damage;
		Utils.NetworkCommand(kraken,"FacheToiRouge");
		if(healthPoints <= 0)
		{
			kraken.ApplyDamage(kraken.EyeDeathDamage);
			kraken.SignalHurt(2f);
			Utils.NetworkCommand(this,"CloseEye");
		}
	}
	
	[RPC]
	void CloseEye()
	{
		closedEyeRenderer.enabled = true;
		gameObject.tag = "Untagged";
		healthPoints = -1;
	}
	
	public override string ToString ()
	{
		return string.Format("{0} eye: {1}",gameObject.name,Closed?"Closed":string.Format("{0} HP",HealthPoints));
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int _healthPoints = healthPoints;
		if(stream.isWriting)
		{
			stream.Serialize(ref _healthPoints);
		}else{
			stream.Serialize(ref _healthPoints);
			healthPoints = _healthPoints;
		}
	}
}
