using UnityEngine;
using System.Collections;

public class PositionSwitchingManager : Singleton<PositionSwitchingManager>
{
		[SerializeField]
		private float
				promptTime = 3f;
		[SerializeField]
		private float
				cooldown = 3f;
				
		private float promptTimer = 0f;
		private float cooldownTimer = 0f;
		
		public bool Prompting { get { return promptTimer - Time.time > 0f; } }

		public bool CoolingDown { get { return cooldownTimer - Time.time > 0f; } }
		
		private bool clientAsked = false;
		private bool serverAsked = false;
		
		public enum SwitchState
		{
				idle,
				prompting,
				cooldown
		}
		private SwitchState state = SwitchState.idle;
		public SwitchState State { get { return state; } }
		
		void Awake ()
		{
				state = SwitchState.idle;
		}
		
		// Update is called once per frame
		void Update ()
		{
				if (state == SwitchState.prompting && !Prompting) {
						cooldownTimer = Time.time + cooldown;
						state = SwitchState.cooldown;
				} else if (state == SwitchState.cooldown && !CoolingDown) {
						state = SwitchState.idle;
				}
		}
		
		public void OnGUI()
		{
			// This has been replaced by the HUD/UIScript and by objects in UI Container in the editor.
			/*
			if(!Prompting)
				return;
			Rect pos = new Rect(Screen.width - HarpoonerToDriverTexture.width*xScale - distanceFromRight, distanceFromTop, HarpoonerToDriverTexture.width*xScale, HarpoonerToDriverTexture.height*yScale);
			if(GlobalScript.Instance.Driver.HasControl)
				GUI.DrawTexture(pos,DriverToHarpoonerTexture);
			else
				GUI.DrawTexture(pos,HarpoonerToDriverTexture);
			*/
		}
		
		[RPC]
		public void AskSwitch (bool askerIsServer)
		{
				switch (state) {
				case SwitchState.idle:
						if (askerIsServer)
								serverAsked = true;
						else
								clientAsked = true;
			
						promptTimer = Time.time + promptTime;
						state = SwitchState.prompting;
						break;
						
				case SwitchState.prompting:
						if ((askerIsServer && clientAsked) || (!askerIsServer && serverAsked)) {
								SwitchPlaces ();
								cooldownTimer = Time.time + cooldown;
								state = SwitchState.cooldown;
								promptTimer = -1f;
						}
						break;
						
				case SwitchState.cooldown:
						break;
				}
				
		}
		
		void SwitchPlaces ()
		{
				GlobalScript.Instance.SwitchPosition ();
			
				clientAsked = false;
				serverAsked = false;
		}
}

