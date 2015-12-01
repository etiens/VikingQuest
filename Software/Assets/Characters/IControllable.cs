using UnityEngine;
using System.Collections;

public abstract class IControllable : MonoBehaviour
{
	public bool HasControl { get; set; }

	protected abstract void ProcessControls ();
	public abstract void ResetControls ();
	protected abstract void ResetCamera();
	
	protected Animator animationControl = null;
	protected float IdleStartFrame = 0.85f;
	protected float IdleStopFrame = 0.99f;
	protected float QTEStartFrame = 0.22f;
	protected float QTEStopFrame = 0.38f;
	protected float IdleAnimationSpeed = 0.2f;
	protected float QTEAnimationSpeed = 0.8f;
	protected int currentPlaybackLoop = 0;
	public int CurrentPlayBackLoop {get {return currentPlaybackLoop;} set{currentPlaybackLoop = Mathf.Clamp(value,0,1000);}}
	
	protected bool stopControls = false;
	public bool StopControls {get{return stopControls;} set{stopControls = value;}}

	protected virtual void Awake()
	{
		animationControl = GetComponentInChildren<Animator>() as Animator;
		animationControl.speed = 1f;
		stopControls = false;
	}
	
	protected virtual void FixedUpdate()
	{
		/*if(GlobalScript.Instance.QTEManager.InQTEMode)
			UpdateQTEAnimation();
		else
			UpdateIdleAnimation();*/
	}
	
	protected void UpdateControls ()
	{
		
		ProcessDevKeys ();
		if(HasControl)
		{

			
			if(GlobalScript.Instance.QTEManager.InQTEMode || stopControls){
				ResetControls();
				if(Utils.Instance.Player1.GetButtonDown(GlobalScript.Instance.QTEManager.QTEButton)){
					// QTE event occured. broadcast it to server
					GlobalScript.Instance.QTEManager.TriggerQTE();
				}
			}
			else if(!GlobalScript.Instance.MenuInputHandler.MenuEnabled){
				
				if(Utils.Instance.Player1.RightStickButtonDown())
					ResetCamera();
				/*if(Utils.Instance.Player1.YDown())
					GlobalScript.Instance.EngageQTE(VInput.Button.A,100f,5);*/
				ProcessControls ();
			}
			//ProcessControls();
		}
		else
			ResetControls();
	}
	
	protected void UpdateQTEAnimation()
	{
		UpdateAnimationSpeed(QTEStartFrame,QTEStopFrame, QTEAnimationSpeed);
	}
	
	protected void UpdateIdleAnimation()
	{
		UpdateAnimationSpeed(IdleStartFrame,IdleStopFrame, IdleAnimationSpeed);
	}
	
	protected void UpdateAnimationSpeed(float startFrame, float stopFrame, float animationSpeed)
	{
		float playbackFrame = GetAnimationPlayback() - currentPlaybackLoop;
		
		if(playbackFrame > stopFrame)
			animationControl.speed = -animationSpeed;
		else if(playbackFrame < startFrame)
			animationControl.speed = animationSpeed;
	}
	
	protected float GetAnimationPlayback()
	{
		AnimatorStateInfo currentState = animationControl.GetCurrentAnimatorStateInfo(0);
		return currentState.normalizedTime;
	}
	
	public virtual void EngageQTEAnimation()
	{
		animationControl.SetBool("QTE",true);
	}
	
	public virtual void LeaveQTEAnimation()
	{
		animationControl.SetBool("QTE",false);
	}

	public void ProcessDevKeys()
	{
		if (Input.GetKeyDown (KeyCode.J))
			Utils.Instance.InputManager.SetXbox360Input (Utils.Player1Id, 0);
		else if (Input.GetKeyDown (KeyCode.K))
			Utils.Instance.InputManager.SetKeyboardInput (Utils.Player1Id);
	}
	
	#region Camera Related stuff
	//Values to set driver Camera
	//coordonnees speherique rayon-colatitude-longitude
	//Phi and Theta in Degrees
	private float radius = 5f;
	[SerializeField]
	private float
		minRadius = 1f;
	[SerializeField]
	private float
		maxRadius = 20f;
	
	private float theta = 0f;
	[SerializeField]
	private float
		minTheta = -90f;
	[SerializeField]
	private float
		maxTheta = 90f;
	private float phi = 20f;
	[SerializeField]
	private float
		minPhi = 0f;
	[SerializeField]
	private float
		maxPhi = 90f;
	
	//Values to set harpooner camera
	[SerializeField]
	private float
		closeUpPhi = 10f;
	[SerializeField]
	private float
		closeUpTheta = -25f;
	[SerializeField]
	private float
		closeUpRadius = 7f;
	[SerializeField]
	private float
		minCloseUpRadius = 0f;
	[SerializeField]
	private float
		maxCloseUpRadius = 2f;
		
	public float Radius {
		get{ return radius;}
		set{ radius = Mathf.Clamp (value, minRadius, maxRadius);}
	}
	
	public float Theta {
		get{ return theta;}
		set{ theta = Mathf.Clamp (value, minTheta, maxTheta);}
	}
	
	public float Phi {
		get{ return phi;}
		set { 
			phi = Mathf.Clamp (value, minPhi, maxPhi);
		}
	}
	
	public float CloseUpRadius {
		get{ return closeUpRadius;}
		set{ closeUpRadius = Mathf.Clamp (value, minCloseUpRadius, maxCloseUpRadius);}
	}
	
	public float CloseUpTheta {
		get{ return closeUpTheta;}
		set{ closeUpTheta = value;}
	}
	
	public float CloseUpPhi {
		get{ return closeUpPhi;}
		set { 
			closeUpPhi = Mathf.Clamp (value, minPhi, maxPhi);
		}
	}
	[SerializeField]
	private float startingRadius = 15f;
	public float StartingRadius {get{return startingRadius;}}
	
	[SerializeField]
	private float startingPhi = 40f;
	public float StartingPhi {get{return startingPhi;}}
	#endregion
}

