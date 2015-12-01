using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Kraken : Boss{
	
	#region Variables
	[SerializeField] List<Tentacle> tentacles = null;
	public List<Tentacle> LiveTentacles {get{return tentacles.Where(x => !x.Dead).ToList();}}
	public List<Tentacle> LiveInRangeTentacles {get{return tentacles.Where(x => !x.Dead && x.InRange).ToList();}}
	[SerializeField] float regularTentacleTime = 30f;
	[SerializeField] float enragedTentacleTime = 15f;
	public float RegularTentacleTime {get{return regularTentacleTime;}}
	public float EnragedTentacleTime {get{return enragedTentacleTime;}}
	private const float smashAttackTime = 2.32f;
	[SerializeField] float smashHitLength = 10f;
	public float SmashHitLength {get {return smashHitLength;}}
	private const float throwAttackTime = 4.10f;
	private const float tentacleMvmntDelay = 3f;
	[SerializeField] float icebergSpeed = 120f;
	public float IcebergSpeed {get{return icebergSpeed;}}
	public GameObject[] ThrownIcebergPrefabs = null;
	[SerializeField] int tentacleDeathDamage = 10;
	public int TentacleDeathDamage {get{return tentacleDeathDamage;}}
	[SerializeField] float closeTentacleRadius = 15f;
	public float CloseTentacleRadius {get{return closeTentacleRadius;}}
	[SerializeField] float farTentacleRadius = 50f;
	public float FarTentacleRadius {get{return farTentacleRadius;}}
	[SerializeField] float tentacleSpacingAngle = 30f;
	public float CurrentTentacleRadius {get{return EnragedCondition()?CloseTentacleRadius:FarTentacleRadius;}}
	[SerializeField] int eyeDeathDamage = 20;
	public int EyeDeathDamage {get{return eyeDeathDamage;}}
	[SerializeField] Color enragedColor = Color.magenta; //we want 242,171,171
	//
	
	[SerializeField] KrakenEye rightEye = null;
	[SerializeField] KrakenEye leftEye = null;
	[SerializeField] Transform body = null;
	public Transform Body {get{return body;}}
	
	private bool inWaveAttack = false;
	public bool InWaveAttack {get{return inWaveAttack;}}
	private int waveAttackCount = 0;
	private float waveAttackStartTime = 0f;
	private List<Tentacle> waveAttackTentacles = null;
	private bool waveAttackType = false; //true = smash, false = throw
	private const bool smashType = true;
	private const bool throwType = false;
	
	private bool inTentacleMvmnt = false;
	private float tentacleMvmntTimer = 0f;
	private Tentacle tentacleInMovement = null;
	
	public float damageReceivedPercent = 0;
	protected Color redColor = new Color(1,0,0);
	protected Color initialColor;
	protected Material material;
	
	[SerializeField] float rotationSmoothFactor = 0.1f;
	Vector3 initialPosition = Vector3.zero;
	[SerializeField] float buoyancySimHeight = 3f;
	[SerializeField] float buoyancySimSpeed = 1f;
	
	public AudioSource KrakenSound1;
	public AudioSource KrakenSound2;
	public AudioClip victorySong;
	
	private bool died = false;
	#endregion
	
	#region Monobehaviour
	public override void Awake()
	{
		base.Awake();
		material = body.GetComponentInChildren<MeshRenderer>().material;
		initialColor = material.color;
		initialPosition = body.transform.position;
	}
	
	public void Start()
	{
		PositionTentacles(tentacleSpacingAngle, CurrentTentacleRadius);
		KrakenSound1 = gameObject.GetComponents<AudioSource>()[0];
		KrakenSound2 = gameObject.GetComponents<AudioSource>()[1];
		
	}
	
	public override void Update () {
		
		base.Update();
		UpdateDamageColor();
		
		if(inWaveAttack)
			ProcessWaveAttack();
		if(inTentacleMvmnt)
			ProcessTentacleMvmnt();
		
		if(!Network.isClient)
			Utils.NetworkCommand(this,"SendInfo",new object[]{HealthPoints,Engaged});
		
	}
	
	void FixedUpdate()
	{
		if(IsDead) return;
		UpdatePosition();
		UpdateRotation();
	}
	
	/*void OnGUI()
	{
		string tentacleStatus = string.Empty;
		foreach(Tentacle t in tentacles)
			tentacleStatus += "\t" + t.ToString() + "\n";
		string eyeStatus =  "\t" + rightEye.ToString() + "\n\t" + leftEye.ToString();
		GUI.Label(new Rect(30,30,600,600), string.Format("HealthPoints: {3}\nBoss State: {0}\nTentacles:\n{1}\nEyes:\n{2}", stateMachine.CurrentState,tentacleStatus,eyeStatus,HealthPoints));
	}*/
	
	
	#endregion
	
	#region Update Helpers
	[RPC]
	void SendInfo(int hp, bool engaged)
	{
		if(Network.isClient)
		{
			HealthPoints = hp;
			Engaged = engaged;
		}
	}
	
	void UpdateRotation()
	{
		Quaternion newRotation = Quaternion.LookRotation(Boat.transform.position - body.transform.position);
		newRotation.x = newRotation.z = 0f;
		body.transform.rotation = Quaternion.Lerp(transform.rotation,newRotation,rotationSmoothFactor);
	}
	
	void UpdatePosition()
	{
		float heightAdjust = Mathf.Sin (Time.time*Mathf.PI*2f*buoyancySimSpeed)*buoyancySimHeight;
		Vector3 newPosition = initialPosition + Vector3.up*heightAdjust;
		body.transform.position = newPosition;
	}
	
	void UpdateDamageColor()
	{
		if (damageReceivedPercent > 0)
		{
			damageReceivedPercent -= Time.deltaTime*3;
			if(IsEnraged)
				material.SetColor("_Color",enragedColor);
			else
				material.SetColor("_Color",(damageReceivedPercent)*redColor + (1-damageReceivedPercent)*initialColor);
		}
	}
	
	[RPC]
	private void FacheToiRouge(){
		damageReceivedPercent = 1f;
	}
	#endregion
	
	#region Sound Management
	public override void PlayHurtSound(){
		Utils.NetworkCommand(this,"RPCPlayHurtSound");
	}
	
	[RPC]
	private void RPCPlayHurtSound()
	{
		KrakenSound2.Play();
	}
	
	public override void StopHurtSound(){
		Utils.NetworkCommand(this,"RPCStopHurtSound");
	}
	
	[RPC]
	private void RPCStopHurtSound()
	{
		KrakenSound2.Stop();
	}
	
	public override void PlayAngrySound(){
		Utils.NetworkCommand(this,"RPCPlayAngrySound");
	}
	
	[RPC]
	private void RPCPlayAngrySound()
	{
		KrakenSound1.Play();
	}
	
	public override void PlayVictorySound(){
		Utils.NetworkCommand(this,"RPCPlayVictorySound");
	}
	
	[RPC]
	private void RPCPlayVictorySound()
	{
		GlobalScript.Instance.musicBossSource.Stop();
		GlobalScript.Instance.musicBossSource.volume = 0.25f;
		GlobalScript.Instance.musicBossSource.PlayOneShot(victorySong);
	}
	
	public override void StopAngrySound(){
		Utils.NetworkCommand(this,"RPCStopAngrySound");
	}
	
	[RPC]
	private void RPCStopAngrySound()
	{
		KrakenSound1.Stop();
	}
	#endregion
	
	#region Tentacle Manipulation
	private int RightToLeft(Tentacle a, Tentacle b)
	{
		//position far right of Kraken
		Vector3 comparePos = body.transform.right*100000 + body.transform.position;
		float aDist = Vector3.Distance(a.transform.position,comparePos);
		float bDist = Vector3.Distance(b.transform.position,comparePos);
		int returnVal = 0;
		if(aDist < bDist)
			returnVal = -1;
		else if(bDist < aDist)
			returnVal = 1;
		return returnVal;
	}
	
	private int DistanceToBoat(Tentacle a, Tentacle b)
	{
		int returnVal = 0;
		float aDist = a.Distance;
		float bDist = b.Distance;
		if(aDist < bDist)
			returnVal = -1;
		else if(bDist < aDist)
			returnVal = 1;
		return returnVal;
	}
	
	//space the tentacles in a cone in front of the kraken with a specified spacing in angles
	private void PositionTentacles(float spacing, float radius)
	{
		int tentCount = tentacles.Count;
		float initialAngle = 0f;
		if(tentCount%2 == 0)
			initialAngle = ((int)(tentCount/2))*-spacing  + spacing/2f;
		else
			initialAngle = ((int)(tentCount/2))*-spacing;
		
		//To have infornt
		initialAngle += 180f;
			
		for(int i = 0; i < tentCount; i++)
		{
			float angle = initialAngle + i*spacing;
			tentacles[i].AngleRelativeToKraken = angle;
			Vector3 newPosition = TentacleAnglePosition(angle,radius);
			//Start under water
			newPosition = newPosition - Vector3.up*80;
			//tentacles[i].transform.position = newPosition;
			tentacles[i].transform.position = newPosition;
			//tentacles[i].transform.rotation = tentacles[i].lookAwayFromKraken;
			tentacles[i].transform.rotation = tentacles[i].lookAwayFromKraken;
		}
	}
	
	public Vector3 TentacleAnglePosition(float angle, float radius)
	{
		return body.transform.TransformPoint(Utils.SphereCoordinates(angle,0f,radius));
	}
	
	public void SendTentacleToPosition(Tentacle tentacle, Vector3 position, Quaternion lookAtRotation)
	{
		tentacle.MovePosition(position,tentacleMvmntDelay);
		tentacle.MoveRotation(lookAtRotation,tentacleMvmntDelay);
	}
	
	public void SendAllTentaclesAroundKraken()
	{
		foreach(Tentacle t in tentacles)
		{
			Vector3 newPosition = TentacleAnglePosition(t.AngleRelativeToKraken,CurrentTentacleRadius);
			newPosition.y = t.transform.position.y;
			
			t.MovePosition(newPosition, tentacleMvmntDelay);
			t.MoveRotation(t.lookAwayFromKraken,tentacleMvmntDelay);
			
		}
	}
	
	public void StopAnimations()
	{
		foreach(Tentacle t in tentacles)
		{
			t.StopHurtAnimation();
			t.StopSmashAnimation();
			t.StopThrowAnimation();
		}
	}
	#endregion
	
	#region State related overrides
	public override bool EnragedCondition ()
	{
		return rightEye.Closed && leftEye.Closed;
	}
	
	protected override void AddRegularState ()
	{
		stateMachine.AddState(new RegularState(this,new KrakenRegularState(this)));
	}
	
	
	public override void DeathSetup()
	{
		foreach(Tentacle t in tentacles)
		{
			t.StartHurtAnimation();
			t.MovePosition(TentacleAnglePosition(t.AngleRelativeToKraken,CurrentTentacleRadius) - 80f*Vector3.up,10f);
		}
		PlayHurtSound();
	}
	
	protected override void AddEnragedState ()
	{
		stateMachine.AddState(new EnragedState(this,new KrakenEnragedState(this)));
	}
	
	public override void RegularSetup ()
	{
		Utils.NetworkCommand(this,"NetworkRegularSetup");
		foreach(Tentacle t in LiveTentacles)
		{
			t.MovePosition(TentacleAnglePosition(t.AngleRelativeToKraken,CurrentTentacleRadius),3f);
			//t.SetAnimationRandomFrame();
		}
	}
	
	[RPC]
	public void NetworkRegularSetup()
	{
		KrakenSound2.Play ();
	}
	
	public override void EnragedSetup()
	{
		Utils.NetworkCommand(this,"NetworkEnragedSetup");
		foreach(Tentacle t in LiveTentacles)
		{
			t.MovePosition(TentacleAnglePosition(t.AngleRelativeToKraken,CurrentTentacleRadius),3f);
		}
	}
	
	[RPC]
	public void NetworkEnragedSetup()
	{
		material.SetColor("_Color",enragedColor);
		KrakenSound2.Play ();
	}
	#endregion
	
	#region Hurt, Stun and Recovery
	public override void HandleHurt ()
	{
		base.HandleHurt();
		inWaveAttack = false;
	}
	
	public override void HandleRecovery ()
	{
		base.HandleRecovery();
		inWaveAttack = false;
	}
	#endregion

	#region Attacks
	//Smash attack if possilbe, throw otherwise
	public override float Attack1 ()
	{
		float attackTime = 0f;
		Tentacle tentacle = ClosestTentacle();
		if(tentacle == null)
			return 0f;
		if(tentacle.InRange)
		{
			tentacle.EngageSmashAttack(true);
			attackTime = smashAttackTime + Tentacle.smashWaitTime;
		}
		else
		{
			if(Utils.Instance.Rng.Next()%4 == 0)
			{
				attackTime = Attack6();
			}
			else
			{
				tentacle.EngageThrowAttack(Boat.transform.position);
				attackTime = throwAttackTime + Tentacle.throwWaitTime;
			}
		}
		return attackTime;;
	}

	//Throw iceberg with closest tentacle
	public override float Attack2 ()
	{
		Tentacle tentacle = ClosestTentacle();
		if(tentacle == null)
			return 0f;
		tentacle.EngageThrowAttack(Boat.transform.position);
		return throwAttackTime + Tentacle.throwWaitTime;
	}
	
	//All tentacles smash in front of them one after the other
	public override float Attack3 ()
	{
		//Move all tentacle to initial position
		SendAllTentaclesAroundKraken();
		//Initiate attack
		waveAttackTentacles = LiveTentacles;
		waveAttackTentacles.Sort(RightToLeft);
		int count = waveAttackTentacles.Count();
		if(count < 1) return 0f;
		waveAttackStartTime = Time.time;
		waveAttackCount = 0;
		inWaveAttack = true;
		waveAttackType = smashType;
		return count*smashAttackTime + tentacleMvmntDelay + Tentacle.smashWaitTime;
	}
	
	//All tentacles throw an iceberg at boat location in wave
	public override float Attack4()
	{
		//Move all tentacle to initial position
		SendAllTentaclesAroundKraken();
		
		//Initiate attack
		waveAttackTentacles  = LiveTentacles;
		waveAttackTentacles.Sort(RightToLeft);
		int count = waveAttackTentacles.Count();
		if(count < 1) return 0f;
		waveAttackStartTime = Time.time;
		waveAttackCount = 0;
		inWaveAttack = true;
		waveAttackType = throwType;
		return count*throwAttackTime + tentacleMvmntDelay + Tentacle.throwWaitTime;
	}
	
	//throw an iceberg at boat position and infront of boat
	public override float Attack5 ()
	{
		List<Tentacle> orderedTentacles = LiveTentacles;
		orderedTentacles.Sort(DistanceToBoat);
		if(orderedTentacles.Count () < 2)
			return 0f;
		orderedTentacles[0].EngageThrowAttack(Boat.transform.position);
		orderedTentacles[1].EngageThrowAttack(Boat.transform.position + Boat.transform.forward*WaterLevel.Instance.smashHitMarkerWidth);
		return throwAttackTime + Tentacle.throwWaitTime;
	}
	
	//Send tentacle to boat position and slam
	public override float Attack6()
	{
		tentacleInMovement = ClosestTentacle();
		if(tentacleInMovement == null)
			return 0f;
		//vector between tentacle and boat
		Vector3 newPosition = (Boat.transform.position - tentacleInMovement.transform.position);
		//level the vector on XZ plane
		newPosition.y = 0;
		
		//find travel distance for a good hit
		float distance = newPosition.magnitude;
		distance = distance - tentacleInMovement.Range/2;
		
		//apply distance to position of boat
		newPosition = tentacleInMovement.transform.position  + newPosition.normalized*distance;
		Quaternion lookAt = Quaternion.LookRotation(Boat.transform.position - newPosition);
		
		SendTentacleToPosition(tentacleInMovement,newPosition,lookAt);
		tentacleMvmntTimer = 0f;
		inTentacleMvmnt = true;
		
		return tentacleMvmntDelay + smashAttackTime + Tentacle.smashWaitTime;
	}
	
	private void ProcessWaveAttack()
	{
		float attackTime = waveAttackType?smashAttackTime:throwAttackTime;
		if(Time.time > waveAttackStartTime + waveAttackCount*attackTime + tentacleMvmntDelay)
		{
			if(!waveAttackTentacles[waveAttackCount].Dead)
			{
				if(waveAttackType)
					waveAttackTentacles[waveAttackCount].EngageSmashAttack(false);
				else
					waveAttackTentacles[waveAttackCount].EngageThrowAttack(Boat.transform.position);
			}
			waveAttackCount++;
			if(waveAttackCount >= waveAttackTentacles.Count())
				inWaveAttack = false;
		}
	}
	
	private void ProcessTentacleMvmnt()
	{
		tentacleMvmntTimer += Time.deltaTime;
		if(tentacleMvmntTimer > tentacleMvmntDelay)
		{
			inTentacleMvmnt = false;
			tentacleInMovement.EngageSmashAttack(true);
		}
	}
	
	private Tentacle ClosestTentacle()
	{
		Tentacle closestTentacle = null;
		Vector3 boatPos = GlobalScript.Instance.Boat.transform.position;
		float distance = 0f;
		foreach (Tentacle t in LiveTentacles)
		{
			if(closestTentacle == null)
			{
				closestTentacle = t;
				distance = t.Distance;
			}
			else 
			{
				float tDistance = t.Distance;
				if(tDistance < distance)
				{
					distance = tDistance;
					closestTentacle = t;
				}
				
			}
		}
		return closestTentacle;
	}
	#endregion
}
