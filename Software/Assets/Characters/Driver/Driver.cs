using UnityEngine;
using System.Collections;

public class Driver : IControllable
{
		#region Variables
		[SerializeField] private float speedBoost = 0.5f;
		private bool boostOn = false;
		public float SpeedBoost {get{return boostOn?speedBoost:0f;}}
		
		public float BoostAmount {get;private set;}
		[SerializeField] private float boostDepletionRate = 25f;
		[SerializeField] private float boostRegenerationRate = 20f;
		private float coolDownTimer = 0f;
		public bool CoolingDown {get{return coolDownTimer > 0f;}}
	
		public AudioClip windSoundClip;
		public AudioClip breathSoundClip;
		public AudioSource windSoundSource;
		public AudioSource breathSoundSource;
		private bool isBlowing = false;
		
		[SerializeField] private float brakeAmount = 0.7f;
		
		[SerializeField] protected Animator driverAnim = null;

		[SerializeField] private ParticleSystem windParticles = null;
		
		#endregion
		
		#region MonoBehaviour
		protected override void Awake()
		{
			base.Awake();
			IdleStartFrame = 0.90f;
			IdleStopFrame = 0.99f;
			QTEStartFrame = 0.22f;
			QTEStopFrame = 0.38f;
			IdleAnimationSpeed = 0.1f;
			QTEAnimationSpeed = 0.8f;
			
			BoostAmount = 100f;
			HasControl = false;
		}
		
		void Start()
		{
		}
	
		
		// Update is called once per frame
		void Update ()
		{
			if(CoolingDown)
			{	
				BoostAmount += boostRegenerationRate*Time.deltaTime;
				coolDownTimer -= Time.deltaTime;
			}
			else
				UpdateBoost(Utils.Instance.Player1.RightTrigger() > 0.75f);
				
			UpdateControls();
		}
		
		protected override void FixedUpdate()
		{
			base.FixedUpdate();
		}
		
		/*protected void OnGUI()
		{
			GUI.Label(new Rect(30f,30f,200,120),string.Format("Driver: currentloop: {0}, playbackTime: {1:0.00}, Right Trigger: {2}",
			CurrentPlayBackLoop,GetAnimationPlayback(),Utils.Instance.Player1.RTInfo().State));
		}*/
		#endregion
		
		#region Sounds
		public void InitSounds(){
			windSoundSource.clip = windSoundClip;
			windSoundSource.loop = true;
			windSoundSource.volume = 1.0f;
			windSoundSource.playOnAwake = false;
			breathSoundSource.clip = breathSoundClip;
			breathSoundSource.loop = false;
			breathSoundSource.volume = 0.8f;
			breathSoundSource.playOnAwake = false;
		}
		
		[RPC]
		private void PlayWindBlowingSounds()
		{
			windSoundSource.Play ();
			breathSoundSource.Play ();
			// Hijack wind sounds to also play wind particles
			windParticles.Play ();
		}
		
		[RPC]
		private void StopWindBlowingSounds()
		{
			windSoundSource.Stop ();
			breathSoundSource.Stop ();
			// Hijack wind sounds to also play wind particles
			windParticles.Stop ();
		}
		
		#endregion
		
		#region Update Helpers
		void UpdateBoost(bool boostPressed)
		{
			if (!HasControl)
					return;
			if(boostPressed)
			{
				BoostAmount -= boostDepletionRate*Time.deltaTime;
				boostOn = true;
				if(BoostAmount < 0f)
				{
					boostOn = false;
					coolDownTimer = 100f/boostRegenerationRate;
					animationControl.SetBool("Blowing",false);
					Utils.NetworkCommand(this,"StopWindBlowingSounds");
				}
				if(!isBlowing){
					isBlowing = true;
					Utils.NetworkCommand(this,"PlayWindBlowingSounds");
					animationControl.SetBool("Blowing",true);
				}
			}
			else
			{
				BoostAmount = Mathf.Clamp(BoostAmount +boostRegenerationRate*Time.deltaTime,0f,100f);
				boostOn = false;
				isBlowing = false;
				animationControl.SetBool("Blowing",false);
				Utils.NetworkCommand(this,"StopWindBlowingSounds");
			}
		}
		#endregion
		
		/*GUIStyle textStyle;
		void OnGUI()
		{
			if(textStyle == null)
				textStyle = new GUIStyle();
			
			textStyle.normal.textColor = CoolingDown?Color.red:Color.green;
			GUI.Label(new Rect(30,10,500,60), string.Format("Boost Amount: {0:0.00}", BoostAmount),textStyle);
		}*/
	
		#region IControllable implementation	
		protected override void ProcessControls()
		{
			float leftStickX = Utils.Instance.Player1.LeftStickX ();
			float brake = Utils.Instance.Player1.LeftTrigger();
			
			if(Utils.Instance.Player1.RightTriggerDown())
				CurrentPlayBackLoop++;
			
			Utils.NetworkCommand (GlobalScript.Instance.Boat, "SetInputValues", RPCMode.Server, leftStickX, SpeedBoost, brake * brakeAmount);
			//GlobalScript.Instance.Boat.SetInputValues(leftStickX, SpeedBoost, brake*brakeAmount);
		}
		
		public override void ResetControls ()
		{
			GlobalScript.Instance.Boat.SetInputValues(0f, 0f, 0f);
		}
		
		protected override void ResetCamera ()
		{
			GlobalScript.Instance.Camera.ResetCameraPosition();
		}
		#endregion
}

