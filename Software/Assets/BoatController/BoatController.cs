using UnityEngine;
using System.Collections;
using System;

public class BoatController : MonoBehaviour {

	#region Variables
	[SerializeField] private float ConstantSpeed = 0.5f;
	public float acceleration = 1.0f;
	public float rudderDelta = 2.0f;
	public float tiltingDelta = 4.0f;
	public float tiltingMaxAngle = 285;
	public float tiltingMinAngle = 255;

	//we have values of inputs here since we use them in FixedUpdate
	//inputs should be taken in Update since Unity checks for inputs each frame
	private float leftStickX = 0f;
	private float speedBoost = 0f;
	private float brake = 0f;
	
	public bool qteHugRight = false;
	[SerializeField] private float qteMaxDegreeSway = 80f;
	[SerializeField] private float qteSwayImpulse = 1f;

	public HarpooningStation FrontHarpooningStation = null;
	public HarpooningStation RightHarpooningStation = null;
	public HarpooningStation LeftHarpooningStation = null;
	
	private ParticleSystem swirlParticles1;
	private ParticleSystem swirlParticles2;
	
	private float maxHealth = 100f;
	public float MaxHealth{get {return maxHealth;} private set{maxHealth = value;}}
	public float Health;
	private float healthRegenPerSecond = 1.2f;
	public float DamageRate = 4f;

	public float invulnerabilityTime = 1.0f;
	private float damageTakenCountdown;
	
	private Buoyancy buoyancy = null;
	
	[SerializeField] Renderer boatRenderer = null;
	[SerializeField] Renderer mastRenderer = null;
	[SerializeField] Renderer shieldsRenderer = null;
	[SerializeField] float harpoonerAlpha = 0.2f;
	private Color solidColor = Color.white;
	private Color transparentColor = Color.white;
	[SerializeField] Material boatSolidMaterial = null;
	[SerializeField] Material mastSolidMaterial = null;
	[SerializeField] Material shieldsSolidMaterial = null;
	[SerializeField] Material boatTransparentMaterial = null;
	[SerializeField] Material mastTransparentMaterial = null;
	[SerializeField] Material shieldsTransparentMaterial = null;
	
	public AudioClip icebergHitClip = null;
	private AudioSource icebergHitSource = null;
	public AudioClip krakenHitClip = null;
	private AudioSource krakenHitSource = null;

	private int leftWallCounter = 0;
	private int rightWallCounter = 0;
	[SerializeField] int wallSqueechFrameCount = 3;
	private bool isBeingCrushed = false;
	private float beingCrusedTimer = 0f;

	public Transform FloorPosition1 = null;
	public Transform FloorPosition2 = null;
	public Transform FloorPosition3 = null;
	public Transform FloorPosition4 = null;
	public Transform FloorPosition5 = null;
	public Transform FloorPosition6 = null;
	
	public bool StopBoat{
		get
		{
			return GlobalScript.Instance.Driver.HasControl?
			GlobalScript.Instance.Driver.StopControls:
			GlobalScript.Instance.Harpooner.StopControls;
		}}
	#endregion 


	#region Monobehaviour
	// TODO: http://formulas.tutorvista.com/physics/buoyancy-formula.html

	// Use this for initialization
	void Awake () {
		ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>(); 
		swirlParticles1 = particles[0];
		swirlParticles2 = particles[1];
		Health = maxHealth;
		damageTakenCountdown = invulnerabilityTime;
		solidColor = Color.white;
		transparentColor = Color.white;
		transparentColor.a = harpoonerAlpha;
		boatSolidMaterial.color = solidColor;
		mastSolidMaterial.color = solidColor;
		shieldsSolidMaterial.color = solidColor;
		boatTransparentMaterial.color = transparentColor;
		mastTransparentMaterial.color = transparentColor;
		shieldsTransparentMaterial.color = transparentColor;
	}
	
	void Start()
	{
		buoyancy = GetComponentInChildren<Buoyancy>() as Buoyancy;
		icebergHitSource = gameObject.AddComponent<AudioSource>();
		icebergHitSource.clip = icebergHitClip;
		krakenHitSource = gameObject.AddComponent<AudioSource>();
		krakenHitSource.clip = krakenHitClip;
	}

	void Update()
	{
		UpdateWallSqueech ();
		if (Health<maxHealth && Health>0f){
			Health += Time.deltaTime*healthRegenPerSecond;
		}
		if (damageTakenCountdown>0f){
			damageTakenCountdown -= Time.deltaTime;
		}
		swirlParticles1.emissionRate = rigidbody.velocity.magnitude * 3;
		swirlParticles2.emissionRate = rigidbody.velocity.magnitude * 5;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(!Network.isClient)
		{
			BoatMechanism(leftStickX, speedBoost, brake);
			if(GlobalScript.Instance.QTEManager.InQTEMode)
				QTESway();
		}
		/*if (currentAccelForce != Vector3.zero) {
			rigidbody.AddForce(currentAccelForce, ForceMode.Force);
		}*/
	}
	
	public void PlayIcebergImpactSound(){
		Utils.NetworkCommand(this,"RPCPlayIcebergImpactSound");
	}
	
	[RPC]
	private void RPCPlayIcebergImpactSound()
	{
		icebergHitSource.Play();
	}
	
	public void PlayKrakenImpactSound(){
		Utils.NetworkCommand(this,"RPCPlayKrakenImpactSound");
	}
	
	[RPC]
	private void RPCPlayKrakenImpactSound()
	{
		krakenHitSource.Play();
	}
	
	public void OnCollisionEnter(Collision collision) 
	{
		if(!Network.isClient)
		{
			var actualCollider = collision.contacts[0];
			if(collision.rigidbody != null)
			{
				ApplyDamage((rigidbody.velocity - collision.rigidbody.velocity).magnitude*DamageRate);
				if (collision.gameObject.tag == "Tentacle"){
					PlayKrakenImpactSound();
				}else{
					PlayIcebergImpactSound();
				}
			}
			
			if(actualCollider.otherCollider.tag == "Instakill")
			{
				Health = 0f;
				Utils.NetworkCommand(this,"EngageQTE");
				collision.gameObject.tag = "Iceberg";
			}
			if(actualCollider.otherCollider.tag == "leftWall")
			{
				leftWallCounter = wallSqueechFrameCount;
			}
			if(actualCollider.otherCollider.tag == "rightWall")
			{
				rightWallCounter = wallSqueechFrameCount;
			}
		}
	}

	
	public void OnTriggerEnter(Collider col) {
		if(!Network.isClient){
			if(col.tag == "ThrowingIceberg" && col.rigidbody != null)
			{
				icebergHitSource.Play();
				this.rigidbody.AddForce(col.rigidbody.velocity / 4.0f);
				
				Iceberg iceberg = col.GetComponent<Iceberg>();
				if(iceberg != null){
					iceberg.ApplyDamage(1);
					col.tag = "Untagged";
				}
				ApplyDamage(this.Health);
			}
			else if (col.tag == "EndOfLevel"){
				ScreenFader.Instance.SceneEnding = true;
				Network.RemoveRPCsInGroup(0);
				Network.RemoveRPCsInGroup(1);
				Utils.NetworkCommand(GlobalScript.Instance, "LoadKrakenLevel");
			}
			else if(col.tag == "Checkpoint1"){
				GlobalScript.CheckpointToLoad = 2;
			}
			else if(col.tag == "Checkpoint2"){
				GlobalScript.CheckpointToLoad = 3;
			}
			else if(col.tag == "Checkpoint3"){
				GlobalScript.CheckpointToLoad = 4;
			}
		}
	}
	#endregion

	private void UpdateWallSqueech()
	{
		if(leftWallCounter > 0 && rightWallCounter > 0)
		{
			if(!Network.isClient)
				Utils.NetworkCommand(GlobalScript.Instance,"DeathUponYou");
		}
		if (leftWallCounter > 0)
			leftWallCounter --;
		if (rightWallCounter > 0)
			rightWallCounter --;

	}
	#region Boat Movement
	[RPC]
	public void SetInputValues(float lsx, float sb, float br)
	{
		leftStickX = lsx;
		speedBoost = sb;
		brake = br;
	}

	private void BoatMechanism(float horizontal, float boost, float brakeAmount)
	{
		rigidbody.AddTorque (horizontal * transform.up * rudderDelta);
		rigidbody.AddTorque (-horizontal * transform.forward * tiltingDelta);
		Vector3 rotation = transform.localRotation.eulerAngles;
		if (rotation.z < 180 && rotation.z > tiltingMaxAngle) {
			rotation.Set (rotation.x, rotation.y, tiltingMaxAngle);
		}
		else if (rotation.z > 180 && rotation.z < 360-tiltingMaxAngle) {
			rotation.Set (rotation.x, rotation.y, 360-tiltingMaxAngle);
		}
		Quaternion currentRot = transform.localRotation;
		currentRot.eulerAngles = rotation;
		transform.localRotation = currentRot;
		
		
		
		float forwardSpeed = boost + ((buoyancy.TotalForce > 0f)?ConstantSpeed - brakeAmount:0f);
		if(GlobalScript.Instance.QTEManager.InQTEMode || StopBoat)
			forwardSpeed = 0f;
		Vector3 accelForce = forwardSpeed * transform.forward * acceleration * Time.deltaTime;
		//tempAccel = boost + ConstantSpeed - brakeAmount;
		//currentAccelForce = accelForce;
		if (accelForce != Vector3.zero) {
			rigidbody.AddForce(accelForce, ForceMode.Force);
		}
	}
	#endregion
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
		
			float health = Health;
			stream.Serialize(ref health);
			
		} else {
			float health = 0f;
			stream.Serialize(ref health);
			Health = health;
		}
	}
	
	#region QTE
	[RPC]
	public void EngageQTE()
	{
		GlobalScript.Instance.EngageQTE(VInput.Button.A, 2f, 10);
	}
	
	
	
	public void QTESway()
	{
		if(qteHugRight)
		{
			if(transform.localRotation.z > -qteMaxDegreeSway)
				rigidbody.AddTorque(transform.forward * -qteSwayImpulse);
		}
		else
		{
			if(transform.localRotation.z < qteMaxDegreeSway)
				rigidbody.AddTorque(transform.forward * qteSwayImpulse);
		}
	}
	#endregion


	#region Public Methods
	public HarpooningStation GetStationFromEnum(Utils.Stations enumVal)
	{
		HarpooningStation station = null;
		switch(enumVal)
		{
			default:
				break;

			case Utils.Stations.Front:
				station = FrontHarpooningStation;
				break;

			case Utils.Stations.Right:
				station = RightHarpooningStation;
				break;

			case Utils.Stations.Left:
				station = LeftHarpooningStation;
				break;
		}

		return station;
	}
	
	public void ApplyDamage(float damage){
		if (damageTakenCountdown <= 0f){
			damageTakenCountdown = invulnerabilityTime;
			Health -= damage;
			if(Health <= 0f){
				// Start QTE 
				Health = 0f;
				Utils.NetworkCommand(this,"EngageQTE");
			}
		}
	}
	
	public void SetupHarpoonerAlphas()
	{
		boatRenderer.material = boatTransparentMaterial;
		mastRenderer.material = mastTransparentMaterial;
		shieldsRenderer.material = shieldsTransparentMaterial;
	}
	
	public void SetupDriverAlphas()
	{
		boatRenderer.material = boatSolidMaterial;
		mastRenderer.material = mastSolidMaterial;
		shieldsRenderer.material = shieldsSolidMaterial;
	}
	#endregion
}