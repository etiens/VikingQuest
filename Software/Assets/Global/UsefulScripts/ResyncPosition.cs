using UnityEngine;
using System.Collections;

public class ResyncPosition : MonoBehaviour
{
		private float angleDistance = 0f;
		[SerializeField] float resyncAngle = 5f;
		private float oldAngle = 0f;
		[SerializeField] float resyncDistance = 0.5f;
		[SerializeField] float positionSmoothFactor = 0.1f;
		[SerializeField] float rotationSmoothFactor = 0.1f;
		[SerializeField] float teleportResyncFactor = 10f;
		
		private Vector3 targetPosition = Vector3.zero;
		private Vector3 targetForward = Vector3.zero;
		private Vector3 speed = Vector3.zero;
		private Vector3 angularSpeed = Vector3.zero;
		
		// Use this for initialization
		void Awake ()
		{
			oldAngle = resyncAngle;
			angleDistance = AngleDistance(resyncAngle);
		}
		
		void Update()
		{
			if(oldAngle != resyncAngle)
			{
				oldAngle = resyncAngle;
				angleDistance = AngleDistance(resyncAngle);
			}
		}
		
		private float AngleDistance(float angle)
		{
			Vector3 forward = Vector3.forward;
			Vector3 deltaForward = (forward + Vector3.up*Mathf.Tan(resyncAngle*Mathf.Deg2Rad)).normalized;
			return Vector3.Distance(forward,deltaForward);
		}
		
		void FixedUpdate()
		{
			if(!Network.isClient) return;
			float distance = (rigidbody.position - targetPosition).magnitude;
			if(distance > resyncDistance*teleportResyncFactor){
				if(!rigidbody.isKinematic)
					rigidbody.position = targetPosition;
				else
					transform.position = targetPosition;
			}else if(distance > resyncDistance){
				if(!rigidbody.isKinematic)
					rigidbody.position = Vector3.Lerp(rigidbody.position,targetPosition,positionSmoothFactor);
				else
					transform.position = Vector3.Lerp(rigidbody.position,targetPosition,positionSmoothFactor);
			}
			
			distance = (targetForward - transform.forward).magnitude;
			if(distance > angleDistance*teleportResyncFactor)
				transform.rotation = Quaternion.LookRotation(targetForward);
			else if(distance > angleDistance)
				transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(targetForward),rotationSmoothFactor);
				
			if(!rigidbody.isKinematic){
				rigidbody.velocity = speed;
				rigidbody.angularVelocity = angularSpeed;
			}
		}
	
		// Update is called once per frame
		void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		{
			Vector3 position = rigidbody.position;
			Vector3 forward = transform.forward;
			Vector3 velocity = rigidbody.velocity;
			Vector3 angularVelocity = rigidbody.angularVelocity;
			
			if(stream.isWriting)
			{
				stream.Serialize(ref position);
				stream.Serialize(ref forward);
				stream.Serialize(ref velocity);
				stream.Serialize(ref angularVelocity);
			}
			else
			{
				stream.Serialize(ref position);
				targetPosition = position;
				
				stream.Serialize(ref forward);
				targetForward = forward;
				
				stream.Serialize(ref velocity);
				speed = velocity;
				
				stream.Serialize(ref angularVelocity);
				angularSpeed = angularVelocity;
			}
		}
	}
		
}

