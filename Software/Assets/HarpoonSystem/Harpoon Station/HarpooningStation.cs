using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HarpooningStation : MonoBehaviour {

	public bool CoolingDown{ get; private set; }
	public float CooldownValue{ get; private set; }
	public float CooldownCountdown{ get; private set; }
	[SerializeField] List<Harpoon> harpoonTypes = null;
	Harpoon currentHarpoonType = null;
	Transform harpoonModel = null;
	LineRenderer line;
	[SerializeField] Material lineMaterial = null;
	[SerializeField] Color noHitColor = Color.yellow;
	[SerializeField] Color onHitColor = Color.green;
	[SerializeField] Color coolingDownColor = Color.red;
	private int colorChangeCooldown = 0;
	private const int colorChangeFrameCooldown = 1;
	[SerializeField] LineCollider lineCollider = null;
	
	List<SphereCollider> sphereCollList = new List<SphereCollider>();
	bool spheresInit = false;
	[SerializeField] int numOfTrajectoryPoints = 120;
	float trajectoryTimeStep = 0.03f;

	[SerializeField] private GameObject lineObject = null;
	public GameObject Gun;
	public GameObject Hinge;
	bool hitDetected = false;
	
	private AudioSource harpoonShotSound = null;

	// Use this for initialization
	void Awake () {
		CoolingDown = false;
		CooldownValue = 1.0f;
		CooldownCountdown = CooldownValue;
		currentHarpoonType = harpoonTypes[0];
		harpoonModel = Gun.transform.GetChild (0);	
		SetupLine();
		harpoonShotSound = GetComponent<AudioSource>();
		setTrajectoryPoints(new Vector3(0f,0f,0f));
		disableColliders();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCooldown ();
		UpdateLineAlpha();
	}
	
	void SetupLine()
	{
		line = lineObject.GetComponent<LineRenderer>();
		line.SetVertexCount(numOfTrajectoryPoints);
		line.renderer.material = lineMaterial;
		
	}

	void UpdateCooldown()
	{
		if (CooldownCountdown > 0.0f) {
			CooldownCountdown -= Time.deltaTime;	
		} else if (CoolingDown == true){
			CoolingDown = false;
		}
	}
	
	void UpdateLineAlpha()
	{
		if(line.renderer.enabled == false)
			return;
		if(CoolingDown)
			line.renderer.material.color = coolingDownColor;
		else
		{
			//do this to prevent live from flickering when holding shoot
			if(colorChangeCooldown > 0)
				colorChangeCooldown--;
			else
			{
				if(hitDetected)
					line.renderer.material.color = onHitColor;
				else
					line.renderer.material.color = noHitColor;
			}
		}
		
	}

	public float getCooldownProgressPercent()
	{
		return ((CooldownValue - CooldownCountdown) / CooldownValue) * 100;
	}

	[RPC]
	public void ShootHarpoon(Vector3 direction){
		if (!CoolingDown) {
			CooldownCountdown = CooldownValue;
			CoolingDown = true;
			colorChangeCooldown = colorChangeFrameCooldown;
			Harpoon newHarpoon = null;
			newHarpoon = Utils.Instantiate(currentHarpoonType, harpoonModel.position, harpoonModel.rotation) as Harpoon;
			harpoonShotSound.Play();
			if(newHarpoon != null)
			{
				newHarpoon.SetDirection(direction);
				newHarpoon.renderer.enabled = true;
			}
		}

	}
	public void setLineHit(bool hit){
		hitDetected = hit;
	}
		
	public void ChangeHarpoonType(){
		if(harpoonTypes.Count > 1){
			if(currentHarpoonType == harpoonTypes[0]){
				currentHarpoonType = harpoonTypes[1];
			} else {
				currentHarpoonType = harpoonTypes[0];
			}
		}
	}

	public void hideTrajectoryLine(){
		if(line != null)
			line.renderer.enabled = false;
			harpoonModel.renderer.enabled = false;
	}

	public void showTrajectoryLine(){
		if(line != null && GlobalScript.Instance.Harpooner.HasControl)
			line.renderer.enabled = true;
			harpoonModel.renderer.enabled = true;
	}
	
	public void disableColliders(){
		lineCollider.enabled = false;
	}

	public void enableColliders(){
		lineCollider.enabled = true;
	}

	public void setTrajectoryPoints(Vector3 direction)
	{
		float currentTime = 0.0f;
		float force = currentHarpoonType.speed;
		Vector3 startingPosition = Gun.transform.position;
		lineCollider.transform.position = Gun.transform.position;
		SphereCollider newSColl;
		currentTime += trajectoryTimeStep;
		for (int i = 0 ; i < numOfTrajectoryPoints ; i++)
		{
			float dx = force * direction.x * currentTime;
			float dy = force * direction.y * currentTime - (Physics2D.gravity.magnitude * currentTime * currentTime / 2.0f);
			float dz = force * direction.z * currentTime;
			Vector3 pos = new Vector3(startingPosition.x + dx, startingPosition.y + dy, startingPosition.z + dz);
			Vector3 sd = new Vector3(dx,dy,dz);
			currentTime += trajectoryTimeStep;
			line.SetPosition(i, pos);
			if(!spheresInit){
				newSColl = lineCollider.gameObject.AddComponent("SphereCollider") as SphereCollider;
				newSColl.center = sd;
				newSColl.isTrigger = true;
				sphereCollList.Add(newSColl);
			}
			
			sphereCollList[i].center = sd;
			
			
		}
		spheresInit = true;
	}
}





















