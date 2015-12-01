using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {

	[SerializeField] protected int healthPoints = 3;
	protected int currentHealth;
	public int HealthPoints {get{return currentHealth;}}

	protected float damageReceivedPercent = 0f;
	protected Color redColor = new Color(1,0,0);
	protected Color initialColor;
	protected Material material;

	[SerializeField]
	protected GameObject particlesObject;

	// Use this for initialization
	virtual public void Start () {
		currentHealth = healthPoints;
		material = gameObject.GetComponentInChildren<MeshRenderer>().material;
		initialColor = material.color;
	}

	// Update is called once per frame
	virtual public void Update () {
		UpdateColor();
	}
	
	protected void UpdateColor()
	{
		if (damageReceivedPercent > 0f)
		{
			damageReceivedPercent -= Time.deltaTime*3;
			if (damageReceivedPercent < 0f)
			{
				damageReceivedPercent = 0f;
			}
		}
		ChangeColor();
	}
	
	protected virtual void ChangeColor()
	{
		renderer.material.SetColor("_Color",(damageReceivedPercent)*redColor + (1-damageReceivedPercent)*initialColor);
	}
	
	[RPC]
	private void FacheToiRouge(){
		damageReceivedPercent = 1f;
	}
	
	virtual public void ApplyDamage(int damage){
		currentHealth -= damage;
		if (currentHealth <= 0){
			Utils.Destroy(gameObject);
		}
		Utils.NetworkCommand (this, "FacheToiRouge");
	}

	virtual public void ApplyDamage(int damage, Vector3 position){
		ApplyDamage (damage);
	}
}
