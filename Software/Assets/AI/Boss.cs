using UnityEngine;
using System.Collections;

public abstract class Boss : MonoBehaviour {

	#region Attributes and Properties
	protected StateMachine stateMachine;
	
	[SerializeField] protected int maxHP = 100;
	[SerializeField] public int MaxHP { get{ return maxHP; }}
	public int HealthPoints {get;protected set;}
	[SerializeField] protected int stunResistance = 10;
	public int StunHP {get;protected set;}
	public bool Hurt {get;protected set;}
	public bool Recovering {get; protected set;}
	
	#region Timers
	public float IdleTime {get;protected set;}
	
	[SerializeField] protected float maxStunTime = 10f;
	[SerializeField] protected float minStunTime = 5f;
	public float StunTime {get;protected set;}
	
	[SerializeField] protected float maxHurtTime = 10f;
	[SerializeField] protected float minHurtTime = 5f;
	public float HurtTime {get;protected set;}
	#endregion
	
	protected BoatController Boat = null;
	public bool Engaged {get;set;}
	
	public bool IsDead {get {return HealthPoints <= 0;}}
	public bool IsEnraged {get {return EnragedCondition();}}
	#endregion
	
	#region Monobehaviour
	public virtual void Awake()
	{
		Boat = GlobalScript.Instance.Boat;
		stateMachine = new StateMachine(new WaitingState(this));
		stateMachine.AddState(new DeadState(this));
		AddEnragedState();
		AddRegularState();
		HealthPoints = maxHP;
		StunHP = stunResistance;
		
		Hurt = false;
		Recovering = false;
		//Engaged = false;
	}
	
	public virtual void Update()
	{
		if(Network.isClient)
			return;
		stateMachine.ProcessStateMachine(Time.deltaTime);
	}
	#endregion
	
	#region Abstract methods
	public abstract bool EnragedCondition();
	protected abstract void AddRegularState();
	protected abstract void AddEnragedState();
	public abstract void DeathSetup();
	public abstract void RegularSetup();
	public abstract void EnragedSetup();
	public abstract void PlayHurtSound();
	public abstract void StopHurtSound();
	public abstract void PlayAngrySound();
	public abstract void StopAngrySound();
	public abstract void PlayVictorySound();
	#endregion
	
	#region public methods
	public void SignalHurt(float time){Hurt = true; HurtTime = time;}
	public virtual void HandleHurt(){Hurt = false;}
	
	public void SignalRecovering(float time){Recovering = true; IdleTime = time;}
	public virtual void HandleRecovery(){Recovering = false;}
	
	
	
	public void ResetStun()
	{
		StunTime = Mathf.Clamp(0.75f*StunTime,minStunTime,maxStunTime);
	}
	
	public void ResetHurt()
	{
		HurtTime = Mathf.Clamp(0.75f*HurtTime,minHurtTime,maxHurtTime);
	}
	#endregion
	
	#region Engage Detection
	
	/*public virtual void OnTriggerEnter(Collider other)
	{
		if(!Engaged && other.tag == "Player")
		{
			Engaged = true;
			SignalRecovering(5f);
		}
	}
	
	public virtual void OnTriggerExit(Collider other)
	{
		if(Engaged && other.tag == "Player")
			Engaged = false;
	}*/
	
	static public bool FoundBoatInObject(GameObject other)
	{
		BoatController boat;
		boat = other.gameObject.GetComponent<BoatController>() as BoatController;
		
		if(boat == null)
			boat = other.gameObject.GetComponentInParent<BoatController>() as BoatController;
		if(boat == null)
			boat = other.gameObject.GetComponentInChildren<BoatController>() as BoatController;
		
		return boat != null;
	}
	#endregion
	
	#region Attacks
	public virtual float Attack1(){return 0f;}
	public virtual float Attack2(){return 0f;}
	public virtual float Attack3(){return 0f;}
	public virtual float Attack4(){return 0f;}
	public virtual float Attack5(){return 0f;}
	public virtual float Attack6(){return 0f;}
	#endregion

	#region Damage
	public void ApplyDamage(int damage)
	{
		HealthPoints -= damage;
		if(HealthPoints < 0)
			HealthPoints = 0;
	}
	#endregion
}