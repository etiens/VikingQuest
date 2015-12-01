using UnityEngine;
using System.Collections;

public class VikingCamera : MonoBehaviour
{
	
	#region Varriables
		private IControllable target;
		[SerializeField]
		public GameObject
				Camera = null;
		[SerializeField]
		public float
				far = 10f;
		[SerializeField]
		private float
				minY = 0f;
		[SerializeField]
		private float
				maxY = 1000f;

		
		[SerializeField]
		private float
				gunDirectionWeight = 0.25f;

		//Camera smoothening values
		[SerializeField]
		private float
				cameraPositionSmoothFactor = 0.8f;
		[SerializeField]
		private float
				cameraRotationSmoothFactor = 0.8f;
		[SerializeField]
		private float
				rotationSpeed = 2f;
		[SerializeField]
		private float
				zoomSpeed = 0.3f;
				
		private float distanceToPlayer = 5.0f; 
		private Vector3 cameraToPlayer = Vector3.zero;
	#endregion

	#region Properties
		

		public float CameraPositionSmoothFactor {
				get{ return cameraPositionSmoothFactor;}
				set{ cameraPositionSmoothFactor = Mathf.Clamp01 (value);}
		}

		public float CameraRotationSmoothFactor {
				get{ return cameraRotationSmoothFactor;}
				set{ cameraRotationSmoothFactor = Mathf.Clamp01 (value);}
		}
	#endregion

	#region MonoBehaviour
	
		void Start()
		{
			if(GlobalScript.Instance.Driver.HasControl)
				TargetBoat();
			else
				TargetHarpooner();
		}
		
		void Update ()
		{
			if(GlobalScript.Instance.MenuInputHandler.MenuEnabled)
				return;
			target.Theta += Utils.Instance.Player1.RightStickX () * rotationSpeed * Time.deltaTime * 60f;
			target.Phi += Utils.Instance.Player1.RightStickY () * rotationSpeed * Time.deltaTime * 60f;
			target.Radius -= Utils.Instance.Player1.DPadY () * zoomSpeed * Time.deltaTime * 30;
			
			
			if(GlobalScript.Instance.Driver.HasControl)
			{
				cameraToPlayer = Camera.transform.position - GlobalScript.Instance.Boat.transform.position;
				distanceToPlayer = cameraToPlayer.magnitude;
				cameraToPlayer.Normalize();
				
				RaycastHit[] hits; // you can also use CapsuleCastAll() // TODO: setup your layermask it improve performance and filter your hits. 
				hits = Physics.RaycastAll(Camera.transform.position + cameraToPlayer * 200, -cameraToPlayer, distanceToPlayer + 200);
				
				foreach(RaycastHit hit in hits)
				{
					if(hit.collider.tag == "boat" || hit.collider.tag == "TentacleRange")
						continue;
					Renderer R;
					if(hit.collider.tag == "Tentacle")
					{
						R = hit.collider.GetComponentInParent<Tentacle>().GetComponentInChildren<SkinnedMeshRenderer>().renderer;
					}
					else
					{
						R = hit.collider.renderer;
					}
					if (R == null){
						//Search in children for renderers
						Renderer[] renderers = hit.collider.GetComponentsInChildren<MeshRenderer>();
						foreach(MeshRenderer r in renderers){
							AutoTransparent AT1 = r.GetComponent<AutoTransparent>();
							if (AT1 == null) // if no script is attached, attach one
							{
								AT1 = r.gameObject.AddComponent<AutoTransparent>();
							}
							AT1.BeTransparent(); // get called every frame to reset the falloff
						}
						
						renderers = hit.collider.GetComponentsInChildren<SkinnedMeshRenderer>();
						foreach(SkinnedMeshRenderer r in renderers){
							AutoTransparent AT1 = r.GetComponent<AutoTransparent>();
							if (AT1 == null) // if no script is attached, attach one
							{
								AT1 = r.gameObject.AddComponent<AutoTransparent>();
							}
							AT1.BeTransparent(); // get called every frame to reset the falloff
						}
						continue;
					}
					
					// no renderer attached? go to next hit
					// TODO: maybe implement here a check for GOs that should not be affected like the player
					
					AutoTransparent AT2 = R.GetComponent<AutoTransparent>();
					if (AT2 == null) // if no script is attached, attach one
					{
						AT2 = R.gameObject.AddComponent<AutoTransparent>();
					}
					AT2.BeTransparent(); // get called every frame to reset the falloff
				}
			}
			
		}

		// Update is called once per frame
		bool firstPostFixedUpdate = false;
		void LateUpdate ()
		{
			ProcessCamera();
			firstPostFixedUpdate = true;
		}
		
		void FixedUpdate()
		{
			ProcessCamera();
		}
	#endregion

	#region Private Methods
	
		private void ProcessCamera()
		{
			if(firstPostFixedUpdate)
			{
				firstPostFixedUpdate = false;
				return;
			}
			
			if (target == null)
				return;
			
			if (GlobalScript.Instance.Harpooner.HasControl )
				ProcessCloseUpCamera();
			else
				ProcessSphereCamera();
			
		}
		
		private void ProcessCloseUpCamera ()
		{
			ProcessGunView();
		}
		
		private void Process3rdPersonCloseUp ()
		{
			Vector3 newPosition = GlobalScript.Instance.Harpooner.GetCurrentStation ().Gun.transform.TransformPoint (Utils.SphereCoordinates (target.CloseUpTheta, target.CloseUpPhi, target.CloseUpRadius));
			Camera.transform.position = Vector3.Lerp(Camera.transform.position,newPosition,CameraPositionSmoothFactor);
			
			
			Transform gunTransform = GlobalScript.Instance.Harpooner.GetCurrentStation ().Gun.transform;
			//Rotate needed since mesh does not point the gun at front
			gunTransform.Rotate (Vector3.right, Mathf.Deg2Rad * -90, Space.Self);
			Transform stationTransform = GlobalScript.Instance.Harpooner.GetCurrentStation ().transform;
			Vector3 newForward = Vector3.Lerp (stationTransform.forward, gunTransform.forward, gunDirectionWeight);
			Quaternion newRotation = Quaternion.LookRotation(newPosition + newForward * far);
			Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation,newRotation,CameraRotationSmoothFactor);
		}
	
		private void ProcessGunView ()
		{
			Transform gunTransform = GlobalScript.Instance.Harpooner.GetCurrentStation ().Gun.transform;
			Vector3 lookAtPoint = gunTransform.TransformPoint (Utils.SphereCoordinates (0f, 90f, far));
			Quaternion newRotation = Quaternion.LookRotation(lookAtPoint - Camera.transform.position);
			Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, newRotation, CameraRotationSmoothFactor);
			
			Vector3 newPosition = gunTransform.TransformPoint (Utils.SphereCoordinates (target.CloseUpTheta, target.CloseUpPhi, target.CloseUpRadius));
			Camera.transform.position = Vector3.Lerp(Camera.transform.position,newPosition,cameraPositionSmoothFactor);
		}

		private void ProcessSphereCamera ()
		{
				//Positioning
				Vector3 newPosition = target.transform.TransformPoint (Utils.SphereCoordinates (target.Theta, target.Phi, target.Radius));
				newPosition.y = Mathf.Clamp (newPosition.y, minY, maxY);
				Camera.transform.position = Vector3.Lerp(Camera.transform.position,newPosition,CameraPositionSmoothFactor);
				
				//Lookat
				Vector3 camToBoat = (target.transform.position - Camera.transform.position);
				camToBoat.y = 0f;
				Vector3 lookAtPosition = target.transform.position + camToBoat.normalized * far;
				lookAtPosition.y = target.transform.position.y;
				Quaternion newRotation = Quaternion.LookRotation(lookAtPosition - Camera.transform.position);
				Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation,newRotation,CameraRotationSmoothFactor);
				return;
		}
	#endregion

	#region Public Methods
		public void TargetHarpooner ()
		{
				target = GlobalScript.Instance.Harpooner;
				Camera.transform.position = target.transform.TransformPoint (Utils.SphereCoordinates (target.Theta, target.Phi, target.Radius));
				ResetCameraPosition ();
		}

		public void TargetBoat ()
		{
				target = GlobalScript.Instance.Driver;
				Camera.transform.position = target.transform.TransformPoint (Utils.SphereCoordinates (target.Theta, target.Phi, target.Radius));
				ResetCameraPosition ();
		}

		public void ResetCameraPosition ()
		{
				target.Radius = target.StartingRadius;
				target.Theta = 0f;
				target.Phi = target.StartingPhi;
		}
	#endregion
}
