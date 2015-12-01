using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterLevel : Singleton<WaterLevel> {
	public float phaseOffset; // 0 to 1
	public float speed; // 0 to 10
	
	public float xDrift; // 0 to 2
	public float zDrift; // 0 to 2
	
	public float scale; // 0.1 to 10
	public float depth; // 0 to 1

	public bool wave1Activated;
	public Vector3 wave1Position;
	public Vector3 wave1Orientation;
	public Vector3 wave1Direction;
	public float wave1Speed;
	public float wave1Width;
	public float wave1Height;
	public float wave1Length;
	public float wave1MaxDistance;
	private float wave1DistanceTravelled;
	
	public bool wave2Activated;
	public Vector3 wave2Position;
	public Vector3 wave2Orientation;
	public Vector3 wave2Direction;
	public float wave2Speed;
	public float wave2Width;
	public float wave2Height;
	public float wave2Length;
	public float wave2MaxDistance;
	private float wave2DistanceTravelled;
	
	public bool throwHitMarkerActivated;
	public Vector3 throwHitMarkerPosition;
	public float throwHitMarkerRadius;
	
	public bool smashHitMarkerActivated;
	public Vector3 smashHitMarkerPosition1;
	public Vector3 smashHitMarkerPosition2;
	public float smashHitMarkerWidth;

	private float currentTime;
	[SerializeField] private float timeResycThreshold = 0.1f;


	private List<Material> waterMaterials = new List<Material>();
	public Material waterMat;

	// Use this for initialization
	void Start () {
		if(waterMat == null){
			MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer> ();
			waterMaterials.Clear ();
			foreach(MeshRenderer renderer in renderers){
				waterMaterials.Add(renderer.material);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Network.isServer || Utils.IsLocal){
			currentTime = Time.time;
		}
		else
		{
			currentTime += Time.deltaTime;
		}

		if(wave1Activated){
			float deltaDistance = wave1Speed * Time.deltaTime;
			wave1DistanceTravelled += deltaDistance;
			wave1Position += wave1Direction * deltaDistance;

			if(wave1DistanceTravelled > wave1MaxDistance){
				wave1DistanceTravelled = 0f;
				wave1Activated = false;
			}
		}
		if(wave2Activated){
			float deltaDistance = wave2Speed * Time.deltaTime;
			wave2DistanceTravelled += deltaDistance;
			wave2Position += wave2Direction * deltaDistance;

			if(wave2DistanceTravelled > wave2MaxDistance){
				wave2DistanceTravelled = 0f;
				wave2Activated = false;
			}
		}
		
		if(waterMat == null){
			foreach(Material waterMaterial in waterMaterials){
				waterMaterial.SetFloat ("_PhaseOffset", phaseOffset);
				waterMaterial.SetFloat ("_Speed", speed);
				waterMaterial.SetFloat ("_XDrift", xDrift);
				waterMaterial.SetFloat ("_ZDrift", zDrift);
				waterMaterial.SetFloat ("_Scale", scale);
				waterMaterial.SetFloat ("_Depth", depth);
				waterMaterial.SetFloat ("_InTime", currentTime);
	
				waterMaterial.SetFloat ("_Wave1Activated", wave1Activated?1.0f:0.0f);
				waterMaterial.SetVector ("_Wave1Origin", wave1Position);
				waterMaterial.SetVector ("_Wave1Orientation", wave1Orientation);
				
				waterMaterial.SetFloat ("_Wave1Width", wave1Width);
				waterMaterial.SetFloat ("_Wave1Length", wave1Length);
				waterMaterial.SetFloat ("_Wave1Height", wave1Height);
				
				waterMaterial.SetFloat ("_Wave2Activated", wave2Activated?1.0f:0.0f);
				waterMaterial.SetVector ("_Wave2Origin", wave2Position);
				waterMaterial.SetVector ("_Wave2Orientation", wave2Orientation);
				
				waterMaterial.SetFloat ("_Wave2Width", wave2Width);
				waterMaterial.SetFloat ("_Wave2Length", wave2Length);
				waterMaterial.SetFloat ("_Wave2Height", wave2Height);
				
				waterMaterial.SetFloat ("_ThrowHitMarkerActivated", throwHitMarkerActivated?1.0f:0.0f);
				waterMaterial.SetVector ("_ThrowHitPosition", throwHitMarkerPosition);
				waterMaterial.SetFloat ("_ThrowHitRadius", throwHitMarkerRadius);
				
				waterMaterial.SetFloat ("_SmashHitMarkerActivated", smashHitMarkerActivated?1.0f:0.0f);
				waterMaterial.SetVector ("_SmashHitPosition1", smashHitMarkerPosition1);
				waterMaterial.SetVector ("_SmashHitPosition2", smashHitMarkerPosition2);
				waterMaterial.SetFloat ("_SmashHitWidth", smashHitMarkerWidth);

				waterMaterial.SetVector ("_BoatPosition1", GlobalScript.Instance.Boat.FloorPosition1.position);
				waterMaterial.SetVector ("_BoatPosition2", GlobalScript.Instance.Boat.FloorPosition2.position);
				waterMaterial.SetVector ("_BoatPosition3", GlobalScript.Instance.Boat.FloorPosition3.position);
				waterMaterial.SetVector ("_BoatPosition4", GlobalScript.Instance.Boat.FloorPosition4.position);
				waterMaterial.SetVector ("_BoatPosition5", GlobalScript.Instance.Boat.FloorPosition5.position);
				waterMaterial.SetVector ("_BoatPosition6", GlobalScript.Instance.Boat.FloorPosition6.position);
			}
		}
		else{
			waterMat.SetFloat ("_PhaseOffset", phaseOffset);
			waterMat.SetFloat ("_Speed", speed);
			waterMat.SetFloat ("_XDrift", xDrift);
			waterMat.SetFloat ("_ZDrift", zDrift);
			waterMat.SetFloat ("_Scale", scale);
			waterMat.SetFloat ("_Depth", depth);
			waterMat.SetFloat ("_InTime", currentTime);
			
			waterMat.SetFloat ("_Wave1Activated", wave1Activated?1.0f:0.0f);
			waterMat.SetVector ("_Wave1Origin", wave1Position);
			waterMat.SetVector ("_Wave1Orientation", wave1Orientation);
			
			waterMat.SetFloat ("_Wave1Width", wave1Width);
			waterMat.SetFloat ("_Wave1Length", wave1Length);
			waterMat.SetFloat ("_Wave1Height", wave1Height);
			
			waterMat.SetFloat ("_Wave2Activated", wave2Activated?1.0f:0.0f);
			waterMat.SetVector ("_Wave2Origin", wave2Position);
			waterMat.SetVector ("_Wave2Orientation", wave2Orientation);
			
			waterMat.SetFloat ("_Wave2Width", wave2Width);
			waterMat.SetFloat ("_Wave2Length", wave2Length);
			waterMat.SetFloat ("_Wave2Height", wave2Height);
			
			waterMat.SetFloat ("_ThrowHitMarkerActivated", throwHitMarkerActivated?1.0f:0.0f);
			waterMat.SetVector ("_ThrowHitPosition", throwHitMarkerPosition);
			waterMat.SetFloat ("_ThrowHitRadius", throwHitMarkerRadius);
			
			waterMat.SetFloat ("_SmashHitMarkerActivated", smashHitMarkerActivated?1.0f:0.0f);
			waterMat.SetVector ("_SmashHitPosition1", smashHitMarkerPosition1);
			waterMat.SetVector ("_SmashHitPosition2", smashHitMarkerPosition2);
			waterMat.SetFloat ("_SmashHitWidth", smashHitMarkerWidth);
			
			waterMat.SetVector ("_BoatPosition1", GlobalScript.Instance.Boat.FloorPosition1.position);
			waterMat.SetVector ("_BoatPosition2", GlobalScript.Instance.Boat.FloorPosition2.position);
			waterMat.SetVector ("_BoatPosition3", GlobalScript.Instance.Boat.FloorPosition3.position);
			waterMat.SetVector ("_BoatPosition4", GlobalScript.Instance.Boat.FloorPosition4.position);
			waterMat.SetVector ("_BoatPosition5", GlobalScript.Instance.Boat.FloorPosition5.position);
			waterMat.SetVector ("_BoatPosition6", GlobalScript.Instance.Boat.FloorPosition6.position);
		}
		
		//		if(Input.GetKeyDown(KeyCode.Space)){
//			InitiateWave(new Vector3(0,0,-5), new Vector3(0,0,1), new Vector3(-1,0,0),
//			             0.025f, 30, 3, 50, 300);
//		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		float networkedCurrentTime = currentTime;
		if (stream.isWriting)
		{
			stream.Serialize(ref networkedCurrentTime);
		}
		else
		{
			stream.Serialize(ref networkedCurrentTime);
			if(Mathf.Abs(networkedCurrentTime - currentTime) > timeResycThreshold)
				currentTime = networkedCurrentTime;
		}
	}

	[RPC]
	private void SetTime(float time)
	{
		currentTime = time;
	}

	[RPC]
	public void InitiateWave(Vector3 position, Vector3 orientation, Vector3 direction,
	                         float speed, float width, float height, float length, float maxDistance){
		
		if(!wave1Activated){
			wave1Position = position;
			wave1Orientation = orientation;
			wave1Direction = direction;
			wave1Speed = speed;
			wave1Width = width;
			wave1Height = height;
			wave1Length = length;
			wave1MaxDistance = maxDistance;
			wave1Activated = true;
			wave1DistanceTravelled = 0f;
		}
		else if(!wave2Activated){
			wave2Position = position;
			wave2Orientation = orientation;
			wave2Direction = direction;
			wave2Speed = speed;
			wave2Width = width;
			wave2Height = height;
			wave2Length = length;
			wave2MaxDistance = maxDistance;
			wave2Activated = true;
			wave2DistanceTravelled = 0f;
		}
		else{
			// problem ... trying to initiate a third wave but only 2 are supported
		}
	}
	
	[RPC]
	public void InitiateThrowHitMarker(Vector3 position, float radius){
		throwHitMarkerActivated = true;
		throwHitMarkerPosition = position;
		throwHitMarkerRadius = radius;
	}
	
	[RPC]
	public void InitiateSmashHitMarker(Vector3 position1, Vector3 position2, float width){
		smashHitMarkerActivated = true;
		smashHitMarkerPosition1 = position1;
		smashHitMarkerPosition2 = position2;
		smashHitMarkerWidth = width;
	}
	
	[RPC]
	public void DisableThrowHitMarker(){
		throwHitMarkerActivated = false;
	}
	
	[RPC]
	public void DisableSmashHitMarker(){
		smashHitMarkerActivated = false;
	}

	public float GetYWaterLevel(Vector3 position){
		Vector2 pixelUV = new Vector2(position.x, position.z);
		
		float yValue = transform.position.y;

		// Some animation values
		float phase = phaseOffset * (3.14f * 2f);
		float phase2 = phaseOffset * (3.14f * 1.123f);
		float _speed = currentTime * speed;
		float _speed2 = currentTime * (speed * 0.33f );
		float _Depth2 = depth * 1.0f;
		float v0alt = pixelUV.x * xDrift + pixelUV.y * zDrift;
		
		// Modify the real vertex and two theoretical samples by the distortion algorithm (here a simple sine wave on Y, driven by local X pos)
		yValue += Mathf.Sin( phase  + _speed  + ( pixelUV.x  * scale ) ) * depth;
		yValue += Mathf.Sin( phase2 + _speed2 + ( v0alt * scale ) ) * _Depth2; // This is just another wave being applied for a bit more complexity.
		
		if(wave1Activated){
			float distanceFromWave =  (Vector3.Cross(wave1Orientation.normalized, position - wave1Position)).magnitude;
			
			if(distanceFromWave < wave1Width){
				float addedY = wave1Height * Mathf.Cos(3.14f*distanceFromWave/wave1Width)* Mathf.Exp(-2f*3.14f*distanceFromWave*distanceFromWave/wave1Width);
				yValue += addedY;
			}
		}

		if(wave2Activated){
			float distanceFromWave =  (Vector3.Cross(wave2Orientation.normalized, position - wave2Position)).magnitude;
			
			if(distanceFromWave < wave2Width){
				float addedY = wave2Height * Mathf.Cos(3.14f*distanceFromWave/wave2Width)* Mathf.Exp(-2f*3.14f*distanceFromWave*distanceFromWave/wave2Width);
				yValue += addedY;
			}
		}

		return yValue;
	}
}
