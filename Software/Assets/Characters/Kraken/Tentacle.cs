using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tentacle : MonoBehaviour
{
	#region Variables
	[SerializeField]
	int maxHealthPoints = 100;
	[SerializeField]
	Vector3 icebergPosition = Vector3.zero;
	private GameObject instantiatedIceberg = null;
	private Vector3 icebergSpeedVector;
	private bool icebergSpawned = false;
	private Vector3 hitPoint = Vector3.zero;
	//[SerializeField] float IcebergDuration = 0.0f;
	Animator tentaculeAnim = null;
	private float animationSpeedMod = 1f;
	WaterLevel waterLevel = null;
	private int inRangeCount = 0;
	public bool InRange {get{return inRangeCount > 0;}}
	
	private bool throwEngaged = false;
	private float throwWaitTimer = 0f;
	public static float throwWaitTime = 1f;
	private bool smashEngaged = false;
	private float smashWaitTimer = 0f;
	public static float smashWaitTime = 3f;
	private bool smashAndHoldEngaged = false;
	private bool holding = false;
	
	bool deathAnimPlaying = false;
	bool revivingAnimPlaying = false;
	
	public bool Dead {get; private set;}
	private int deathCounter = 0;
	[SerializeField] int numberRevives = 3;
	private float deathTimer = 0f;
	private float animationTimer = 0f;
	[SerializeField] float deathMoveTime = 5f;
	[SerializeField] float reviveMoveTime = 5f;
	private AudioSource harpoonImpact = null;
	
	Vector3 initialPosition = Vector3.zero;
	Vector3 destinationPosition = Vector3.zero;
	float movePositionStartTime = 0f;
	float movePositionEndTime = 0f;
	float movePositionTime = 0f;
	bool moving = false;
	
	Quaternion initialRotation = Quaternion.identity;
	Quaternion destinationRotation = Quaternion.identity;
	float moveRotationStartTime = 0f;
	float moveRotationEndTime = 0f;
	float moveRotationTime = 0f;
	bool rotating = false;
	public Quaternion lookAwayFromKraken 
	{
		get
		{
			Vector3 krakenPosition = kraken.Body.transform.position;
			krakenPosition.y = transform.position.y;
			Quaternion newRot = Quaternion.LookRotation(transform.position - krakenPosition);
			return newRot;
		}
	}
	public float AngleRelativeToKraken = 0f;
	
	public int HealthPoints {get; private set;}
	private Kraken kraken;
	
	public float Distance {get{return Vector3.Distance(transform.position,GlobalScript.Instance.Boat.transform.position);}}
	
	protected float damageReceivedPercent = 0;
	protected Color redColor = new Color(1,0,0);
	protected Color initialColor;
	protected Material material;
	protected SphereCollider rangeDetector = null;
	public float Range {get{return rangeDetector.radius*transform.lossyScale.magnitude;}}
	#endregion
	
	#region MonoBehaviour
	void Awake()
	{
		HealthPoints = maxHealthPoints;
		tentaculeAnim = gameObject.GetComponent<Animator>() as Animator;
		kraken = gameObject.GetComponentInParent<Kraken>() as Kraken;
		harpoonImpact = GetComponent<AudioSource>();
		Dead = false;
		waterLevel = WaterLevel.Instance;
		material = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
		initialColor = material.color;
		rangeDetector = GetComponent<SphereCollider>() as SphereCollider;
		animationSpeedMod = ((float)Utils.Instance.Rng.NextDouble() - 0.5f)*0.75f + 1f;
		tentaculeAnim.speed = animationSpeedMod;
	}
	
	void Update()
	{
		if(revivingAnimPlaying)
		{
			UpdateReviveAnimation();
		}
		else if(Dead)
		{
			UpdateDeathTimer();
		}
		if(deathAnimPlaying)
		{
			UpdateDeathAnimation();
		}
		if(throwEngaged){
			UpdateThrowAttack();
		}
		if(smashEngaged){
			UpdateSmashAttack();
		}
		
		
		UpdateForward();
		UpdateDamageColor();
	}
	
	void FixedUpdate()
	{
		if(moving && !smashEngaged && !throwEngaged)
			UpdatePosition();
		if(rotating && !smashEngaged && !throwEngaged)
			UpdateRotation();
		if(smashAndHoldEngaged)
			UpdateSmashAndHold();
		if(holding)
			UpdateHolding();
	}
	
	/*void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int _inRangeCount = inRangeCount;
		bool _throwEngaged = throwEngaged;
		bool _smashEngaged = smashEngaged;
		bool _deathAnimPlaying = deathAnimPlaying;
		bool _revivingAnimPlaying = revivingAnimPlaying;
		bool _dead = Dead;
		float _deathTimer = deathTimer;
		float _animationTimer = animationTimer;
		float _HealthPoints = HealthPoints;
		
		if(stream.isWriting)
		{
			stream.Serialize(ref _inRangeCount);
			stream.Serialize(ref _throwEngaged);
			stream.Serialize(ref _smashEngaged);
			stream.Serialize(ref _deathAnimPlaying);
			stream.Serialize(ref _revivingAnimPlaying);
			stream.Serialize(ref _dead);
			stream.Serialize(ref _deathTimer);
			stream.Serialize(ref _animationTimer);
			stream.Serialize(ref _HealthPoints);
		}
		else{
			stream.Serialize(ref _inRangeCount);
			inRangeCount = _inRangeCount;
			stream.Serialize(ref _throwEngaged);
			throwEngaged = _throwEngaged;
			stream.Serialize(ref _smashEngaged);
			smashEngaged = _smashEngaged;
			stream.Serialize(ref _deathAnimPlaying);
			deathAnimPlaying = _deathAnimPlaying;
			stream.Serialize(ref _revivingAnimPlaying);
			revivingAnimPlaying = _revivingAnimPlaying;
			stream.Serialize(ref _dead);
			Dead = _dead;
			stream.Serialize(ref _deathTimer);
			deathTimer = _deathTimer;
			stream.Serialize(ref _animationTimer);
			animationTimer = _animationTimer;
			stream.Serialize(ref _HealthPoints);
			HealthPoints = _HealthPoints;
		}
				
	}*/
	
	void OnTriggerEnter(Collider other)
	{
		if(Boss.FoundBoatInObject(other.gameObject))
			inRangeCount++;
	}
	
	void OnTriggerExit(Collider other)
	{
		if(Boss.FoundBoatInObject(other.gameObject))
			inRangeCount--;
	}
	#endregion
	
	#region Update Helpers
	void UpdateDamageColor()
	{
		if (damageReceivedPercent > 0f)
		{
			damageReceivedPercent -= Time.deltaTime*3;
			if (damageReceivedPercent < 0f)
			{
				damageReceivedPercent = 0f;
			}
		}
		material.SetColor("_Color",(damageReceivedPercent)*redColor + (1-damageReceivedPercent)*initialColor);
	}
	
	[RPC]
	private void FacheToiRouge(){
		damageReceivedPercent = 1f;
	}
	
	bool lastFrameInRange = false;
	private void UpdateForward()
	{
		if (smashEngaged || throwEngaged)
			return;
		if(kraken != null && kraken.InWaveAttack)
			return;
		if (kraken != null && kraken.Engaged) {
			if (InRange) {
					Quaternion newRot = Quaternion.LookRotation (GlobalScript.Instance.Boat.transform.position - transform.position);
					newRot.x = newRot.z = 0f;
					MoveRotation (newRot, 0.1f);
			} else if (!lastFrameInRange)
					MoveRotation (lookAwayFromKraken, 1f);
		}
		else //Blocking Tentacle doesn't have a kraken
		{
			// don't rotate
		}
	}
	
	public void MovePosition(Vector3 newPosition, float time)
	{
		if(Dead) return;
		movePositionStartTime = Time.time;
		movePositionEndTime = movePositionStartTime + time;
		movePositionTime = time;
		initialPosition = transform.position;
		destinationPosition = newPosition;
		moving = true;
	}
	
	private void UpdatePosition()
	{
 		transform.position = Vector3.Lerp(initialPosition,destinationPosition,(Time.time - movePositionStartTime)/movePositionTime);
		if(Time.time >= movePositionEndTime)
			moving = false;
	}
	
	public void MoveRotation(Quaternion newRotation, float time)
	{
		if(Dead) return;
		moveRotationStartTime = Time.time;
		moveRotationEndTime = moveRotationStartTime + time;
		moveRotationTime = time;
		initialRotation = transform.rotation;
		destinationRotation = newRotation;
		rotating = true;
	}
	
	private void UpdateRotation()
	{
		transform.rotation = Quaternion.Lerp(initialRotation,destinationRotation,(Time.time - moveRotationStartTime)/moveRotationTime);
		if(Time.time >= moveRotationEndTime)
			rotating = false;
	}
	#endregion
	
	#region Damage, Death and Revival
	public void ApplyDamage(int damage)
	{
		if(Dead)
			return;
		HealthPoints -= damage;
		Utils.NetworkCommand(this,"FacheToiRouge");
		if(kraken != null)
			kraken.ApplyDamage(damage/10);
		PlayImpactSound();
		if(HealthPoints <= 0)
			Kill();
	}
	
	void PlayImpactSound()
	{
		Utils.NetworkCommand(this,"RPCPlayImpactSound");
	}
	
	[RPC]
	void RPCPlayImpactSound()
	{
		harpoonImpact.Play();
	}
	
	private void Kill()
	{
		if(kraken != null)
		{
			kraken.SignalHurt(2f);
			kraken.ApplyDamage(kraken.TentacleDeathDamage);
			deathTimer = kraken.RegularTentacleTime;
		}
		else
		{
			if(throwEngaged && instantiatedIceberg != null)
			{
				Iceberg iceberg = instantiatedIceberg.GetComponent<Iceberg>() as Iceberg;
				iceberg.ApplyDamage(iceberg.HealthPoints);
			}
			deathTimer = 0f;
		}
		initialPosition = transform.position;
		destinationPosition = initialPosition - Vector3.up * 80f;
		Dead = true;
		deathCounter++;
		animationTimer = 0f;
		tentaculeAnim.speed = animationSpeedMod;
		StartHurtAnimation();
		StopHolding();
		deathAnimPlaying = true;
	}
	
	private void Revive()
	{
		//permanent death check
		if(deathCounter > numberRevives || kraken == null)
			return;
		destinationPosition = kraken.TentacleAnglePosition(AngleRelativeToKraken,kraken.CurrentTentacleRadius);
		initialPosition = transform.position;
		animationTimer = 0f;
		tentaculeAnim.speed = animationSpeedMod;
		revivingAnimPlaying = true;
	}
	
	private void UpdateDeathTimer()
	{
		deathTimer -= Time.deltaTime;
		if(deathTimer <= 0)
		{
			Revive();
		}
	}
	
	private void UpdateDeathAnimation()
	{
		//play the animation
		//move the tentacle far down so we don't see it
		animationTimer += Time.deltaTime;
		transform.position = Vector3.Lerp(initialPosition,destinationPosition,animationTimer/deathMoveTime);
		if(animationTimer >= deathMoveTime)
		{
			deathAnimPlaying = false;
		}
	}
	
	private void UpdateReviveAnimation()
	{
		//play the animation
		//move the tentacle up to initial position
		animationTimer += Time.deltaTime;
		transform.position = Vector3.Lerp(initialPosition,destinationPosition,animationTimer/reviveMoveTime);
		if(animationTimer >= reviveMoveTime)
		{
			revivingAnimPlaying = false;
			HealthPoints = maxHealthPoints;
			Dead = false;
			StopHurtAnimation();
		}
	}
	
	void OnDestroy()
	{
		//Debug.Log("On destroy Called on tentacle");
	}
	#endregion
	
	#region Throw Attack
	[SerializeField] Transform InitialIcebergTransform = null;
	public void EngageThrowAttack(Vector3 position){
		if(Dead || kraken == null)
			return;
		// Initiate iceberg throw attack
		throwEngaged = true;
		
		hitPoint = position;
		Utils.NetworkCommand(waterLevel,"InitiateThrowHitMarker",new object[]{hitPoint, 10f});
		Utils.NetworkCommand(waterLevel,"DisableSmashHitMarker");
		Vector3 lookPos = GlobalScript.Instance.Boat.transform.position;
		lookPos.y = tentaculeAnim.transform.position.y;
		tentaculeAnim.transform.LookAt(lookPos);
		
		throwWaitTimer = 0f;
		icebergSpawned = false;
	}
	
	private void UpdateThrowAttack(){
		if(throwWaitTimer < throwWaitTime)
		{
			throwWaitTimer += Time.deltaTime;
			if(throwWaitTimer >= throwWaitTime && tentaculeAnim != null)
			{
				tentaculeAnim.speed = animationSpeedMod;
				StartThrowAnimation();
			}
			return;
		}
		AnimatorStateInfo currentState = tentaculeAnim.GetCurrentAnimatorStateInfo(0);
		float playbackTime = currentState.normalizedTime % 1;
		AnimationInfo[] clips = tentaculeAnim.GetCurrentAnimationClipState (0);
		if (clips.Length == 1 && clips[0].clip.name == "Throw") {
			
			if(playbackTime > 0.6){
				//StopThrowAnimation();
				if(instantiatedIceberg != null)
				{ 
					instantiatedIceberg.transform.SetParent(GlobalScript.Instance.transform,true);
					//calculateTrajectory(instantiatedIceberg.transform.position, kraken.IcebergSpeed, hitPoint, new Vector3(0,0,0));
					calculateTrajectory2(instantiatedIceberg.transform.position, kraken.IcebergSpeed, hitPoint);
					instantiatedIceberg.rigidbody.velocity = Vector3.zero;
					instantiatedIceberg.rigidbody.AddForce(icebergSpeedVector, ForceMode.VelocityChange);
					instantiatedIceberg.AddComponent<ThrownToNormal>();
				}
				
				Utils.NetworkCommand(waterLevel,"DisableThrowHitMarker");
				throwEngaged = false;
				StopThrowAnimation();
			}
			else if(playbackTime > 0.18 && !icebergSpawned){
			
				int randomPrefabIndex = Random.Range(0, kraken.ThrownIcebergPrefabs.Length - 1);
				instantiatedIceberg = (GameObject)Utils.Instantiate(kraken.ThrownIcebergPrefabs[randomPrefabIndex], InitialIcebergTransform.position, InitialIcebergTransform.rotation);
				if(instantiatedIceberg != null)
					instantiatedIceberg.transform.SetParent(InitialIcebergTransform,true);
				//Utils.Destroy (instantiatedIceberg, IcebergDuration);
				
				icebergSpawned = true;
			}
		}
	}
	
	// Strategie for AI aiming iceberg
	private void calculateTrajectory(Vector3 initPos, float desiredSpeed, Vector3 targetPos, Vector3 targetSpeed){
		//http://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php
		float distance = Vector3.Distance(targetPos, initPos);
		float d2 = distance * distance;
		Vector3 velocityVector = new Vector3(0,0,0);
		float targetVelocity = velocityVector.magnitude;
		float sb2 = desiredSpeed * desiredSpeed;
		float st2 = targetVelocity * targetVelocity;
		float cosDeAngle = Vector3.Dot( (initPos - targetPos).normalized, (targetSpeed).normalized );
		float t1 = ( -2.0f*distance*targetVelocity*cosDeAngle + Mathf.Sqrt( Mathf.Pow((2.0f*distance*targetVelocity*cosDeAngle),2.0f) + 4.0f*/*Mathf.Abs*/(sb2 - st2) * d2 ) ) / (2.0f*(sb2 - st2));
		float t2 = ( -2.0f*distance*targetVelocity*cosDeAngle - Mathf.Sqrt( Mathf.Pow((2.0f*distance*targetVelocity*cosDeAngle),2.0f) + 4.0f*/*Mathf.Abs*/(sb2 - st2) * d2 ) ) / (2.0f*(sb2 - st2));
		float t = t1;
		if(t1 < 0.0)
			t = t2;
		else if(t2 < 0.0){
			t = t1;
		}
		else{
			t = Mathf.Max(t1, t2);
		}
		
		icebergSpeedVector = targetSpeed - 0.5f * Physics.gravity * t + ((targetPos - InitialIcebergTransform.position) / t);
	}
	
	private void calculateTrajectory2(Vector3 initPos, float desiredSpeed, Vector3 targetPos){
		Vector3 baseDirection = targetPos - initPos;
		baseDirection.y = 0;
		float xyPlaneDistance = baseDirection.magnitude;
		baseDirection.Normalize();
		
		// Now we have correct direction, calculate a desired time for projectile to reach target
		//float time = xyPlaneDistance / desiredSpeed;
		//float time = initPos.y / desiredSpeed;
		//float t;
		float t = (-desiredSpeed-Mathf.Sqrt(desiredSpeed*desiredSpeed - 2.0f*Physics.gravity.y*initPos.y))/(Physics.gravity.y);
		//float t2 = (-desiredSpeed+Mathf.Sqrt(desiredSpeed*desiredSpeed - 2*Physics.gravity.y*initPos.y))/(2*Physics.gravity.y);
		//t = Mathf.Sqrt(2)*desiredSpeed/Mathf.Abs(Physics.gravity.y);
		//float YdistanceToTravel = initPos.y;
		// y = yi + vy*t + 0.5*a*t*t
		// vy = (y-yi - 0.5*a*t*t)/t
		
		/*if(t1 < 0.0)
			t = t2;
		else if(t2 < 0.0){
			t = t1;
		}
		else{
			t = Mathf.Max(t1, t2);
		}*/
		
		//Debug.Log("Time for projectile to reach : " + t);
		
		float xySpeed = xyPlaneDistance/t;//(-initPos.y - 0.5f*Physics.gravity.y*time*time)/time;
		
		baseDirection *= xySpeed;
		baseDirection.y = desiredSpeed;//xyPlaneDistance * Mathf.Tan(Mathf.Deg2Rad * desiredAngle);
		
		icebergSpeedVector = baseDirection;
		
	}
	#endregion
	
	#region Smash Attack
	public void EngageSmashAttack(bool hitBoat){
		if(Dead || kraken == null)
			return;
		smashEngaged = true;
		
		Vector3 forward = Vector3.zero;
		if(hitBoat)
			forward = (GlobalScript.Instance.Boat.transform.position - transform.position);
		else
			forward = transform.forward;
		
		forward.y = 0;
		hitPoint = transform.position + forward.normalized*kraken.SmashHitLength;
		
		Utils.NetworkCommand(waterLevel,"InitiateSmashHitMarker",new object[]{tentaculeAnim.transform.position, hitPoint, 10f});
		Utils.NetworkCommand(waterLevel,"DisableThrowHitMarker");
		
		Vector3 lookPos = hitPoint;
		lookPos.y = tentaculeAnim.transform.position.y;
		tentaculeAnim.transform.LookAt(lookPos);
		
		smashWaitTimer = 0f;
	}
	
	private void UpdateSmashAttack(){
		if(smashWaitTimer < smashWaitTime)
		{
			smashWaitTimer += Time.deltaTime;
			if(smashWaitTimer >= smashWaitTime && tentaculeAnim != null)
			{
				tentaculeAnim.speed = animationSpeedMod;
				StartSmashAnimation();
			}
			return;
		}
		AnimatorStateInfo currentState = tentaculeAnim.GetCurrentAnimatorStateInfo(0);
		float playbackTime = currentState.normalizedTime % 1;
		AnimationInfo[] clips = tentaculeAnim.GetCurrentAnimationClipState (0);
		if (clips.Length == 1 && clips[0].clip.name == "Smash" && playbackTime > 0.55f) {
			StopSmashAnimation();
			Vector3 orientation = (hitPoint - tentaculeAnim.transform.position).normalized;
			Vector3 direction = Vector3.Cross(orientation, transform.up).normalized;
			Utils.NetworkCommand(waterLevel,"InitiateWave",new object[]{hitPoint, orientation, direction, 5f, 50f, 3f, 10f, 50f});
			Utils.NetworkCommand(waterLevel,"InitiateWave",new object[]{hitPoint, orientation, -direction, 5f, 50f, 3f, 10f, 50f});
			Utils.NetworkCommand(waterLevel,"DisableSmashHitMarker");
			smashEngaged = false;
			
			// Verify if we hit the boat :
			float distance = DistancePointLine2(GlobalScript.Instance.Boat.transform.position, tentaculeAnim.transform.position, hitPoint);
			bool isBetweenLine = PointIsBetweenLine(GlobalScript.Instance.Boat.transform.position, tentaculeAnim.transform.position, hitPoint);
			if(distance < 8f && isBetweenLine){
				GlobalScript.Instance.Boat.ApplyDamage(GlobalScript.Instance.Boat.Health);
			}
		}
	}
	
	float DistancePointLine2(Vector3 point1, Vector3 lineStart, Vector3 lineEnd)
	{
		float num = Mathf.Abs((lineEnd.z - lineStart.z)*point1.x-(lineEnd.x - lineStart.x)*point1.z+lineEnd.x*lineStart.z-lineEnd.z*lineStart.x);
		float denum = Mathf.Sqrt(Mathf.Pow(lineEnd.z-lineStart.z,2) + Mathf.Pow(lineEnd.x-lineStart.x,2));
		return num/denum;
	}
	
	bool PointIsBetweenLine(Vector3 point, Vector3 a, Vector3 b)
	{
		float dotproduct = (point.x - a.x) * (b.x - a.x) + (point.z - a.z)*(b.z - a.z);
		if (dotproduct < 0)
			return false;
		
		float squaredlengthba = (b.x - a.x)*(b.x - a.x) + (b.z - a.z)*(b.z - a.z);
		if (dotproduct > squaredlengthba)
			return false;
		
		return true;
	}
    #endregion
	
	#region Animation Manipulation
	
	
	public void StartHurtAnimation()
	{
		Utils.NetworkCommand(this,"RPCStartHurtAnimation");
	}
	
	public void StopHurtAnimation()
	{
		Utils.NetworkCommand(this,"RPCStopHurtAnimation");
	}
	
	public void StartSmashAnimation()
	{
		Utils.NetworkCommand(this,"RPCStartSmashAnimation");
	}
	
	public void StopSmashAnimation()
	{
		Utils.NetworkCommand(this,"RPCStopSmashAnimation");
	}
	
	public void StartThrowAnimation()
	{
		Utils.NetworkCommand(this,"RPCStartThrowAnimation");
	}
	
	
	public void StopThrowAnimation()
	{
		Utils.NetworkCommand(this,"RPCStopThrowAnimation");
	}
	
	#region RPC animations
	[RPC]
	public void RPCStartHurtAnimation()
	{
		tentaculeAnim.SetBool("VerticalSmash", false);
		tentaculeAnim.SetBool("IcebergThrow", false);
		tentaculeAnim.SetBool("Hurt", true);
	}
	
	[RPC]
	public void RPCStopHurtAnimation()
	{
		tentaculeAnim.SetBool("Hurt", false);
	}
	
	[RPC]
	public void RPCStartSmashAnimation()
	{
		tentaculeAnim.SetBool("VerticalSmash", true);
		tentaculeAnim.SetBool("IcebergThrow", false);
		tentaculeAnim.SetBool("Hurt", false);
	}
	
	[RPC]
	public void RPCStopSmashAnimation()
	{
		tentaculeAnim.SetBool("VerticalSmash", false);
	}
	
	[RPC]
	public void RPCStartThrowAnimation()
	{
		tentaculeAnim.SetBool("VerticalSmash", false);
		tentaculeAnim.SetBool("IcebergThrow", true);
		tentaculeAnim.SetBool("Hurt", false);
	}
	
	[RPC]
	public void RPCStopThrowAnimation()
	{
		tentaculeAnim.SetBool("IcebergThrow", false);
	}
	#endregion
	
	public void SetAnimationRandomFrame()
	{
		float seed = (float)Utils.Instance.Rng.NextDouble();
		//tentaculeAnim.animation["Idle"].normalizedTime = seed; 
	}
	
	public void SmashAndHold()
	{
		if(Dead)
			return;
		StartSmashAnimation();
		smashAndHoldEngaged = true;
		
		hitPoint = transform.position + transform.transform.forward*50f;
		Utils.NetworkCommand(waterLevel,"InitiateSmashHitMarker",new object[]{tentaculeAnim.transform.position, hitPoint, 10f});
		Utils.NetworkCommand(waterLevel,"DisableThrowHitMarker");
		
		Vector3 lookPos = hitPoint;
		lookPos.y = tentaculeAnim.transform.position.y;
		tentaculeAnim.transform.LookAt(lookPos);
	}
	
	protected void UpdateSmashAndHold()
	{
		AnimatorStateInfo currentState = tentaculeAnim.GetCurrentAnimatorStateInfo(0);
		float playbackTime = currentState.normalizedTime % 1;
		AnimationInfo[] clips = tentaculeAnim.GetCurrentAnimationClipState (0);
		if (clips.Length == 1 && clips[0].clip.name == "Smash" && playbackTime > 0.52)
		{
			smashAndHoldEngaged = false;
			Vector3 orientation = (hitPoint - tentaculeAnim.transform.position).normalized;
			Vector3 direction = Vector3.Cross(orientation, transform.up).normalized;
			Utils.NetworkCommand(waterLevel,"InitiateWave",new object[]{hitPoint, orientation, direction, 5f, 50f, 3f, 10f, 50f});
			Utils.NetworkCommand(waterLevel,"InitiateWave",new object[]{hitPoint, orientation, -direction, 5f, 50f, 3f, 10f, 50f});
			Utils.NetworkCommand(waterLevel,"DisableSmashHitMarker");
			holding = true;
			
			float distance = DistancePointLine2(GlobalScript.Instance.Boat.transform.position, tentaculeAnim.transform.position, hitPoint);
			if(distance < 10f && !Network.isClient)
				GlobalScript.Instance.Boat.ApplyDamage(GlobalScript.Instance.Boat.Health);
		}
	}
	
	
	protected void UpdateHolding()
	{
		AnimatorStateInfo currentState = tentaculeAnim.GetCurrentAnimatorStateInfo(0);
		float playbackTime = currentState.normalizedTime % 1;
		if(playbackTime > 0.57)
			tentaculeAnim.speed = -0.1f;
		else if(playbackTime < 0.54f)
			tentaculeAnim.speed = 0.1f;
	}
	
	protected void StopHolding()
	{
		holding = false;
		tentaculeAnim.speed = animationSpeedMod;
	}
	
	#endregion
	
	#region ToString
	public override string ToString ()
	{
		string info = string.Empty;
		char space = ' ';
		info += gameObject.name + space;
		string deathString = revivingAnimPlaying? string.Format("reviving in {0:0.00} ",reviveMoveTime - animationTimer) : string.Format("dead for {0:0.00} ",deathTimer);
		info += Dead? deathString : "HP: " + HealthPoints + space;
		info += InRange? "in range" : "out of range ";
		info += "Lives: " + (numberRevives - deathCounter) + "/" + numberRevives + space;
		info += "Anim Speed: " + string.Format("{0:0.00}",animationSpeedMod);
		return info;
	}
	#endregion
}
