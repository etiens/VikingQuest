using System;

public class HurtState:IState
{
	private float HurtTime = 0f;
	
	public HurtState (Boss boss) : base(StateIds.Hurt, boss)
	{
		NextStateIds.Add(StateIds.Attacking);
	}
	
	public override int ChangeState (float deltaTime)
	{
		int newStateId = StateId;
		
		if(HurtTime <= 0)
		{
			newStateId = StateIds.Attacking;
		}
		return newStateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		HurtTime -= deltaTime;
		//play hurt animation
	}
	
	public override void Setup ()
	{
		boss.PlayHurtSound();
		HurtTime = boss.HurtTime;
		//Change the next hurt time
		boss.ResetHurt();
		//Tell boss hurt was handled
		boss.HandleHurt();
	}
	
	public override void SerializeNetwork (UnityEngine.BitStream stream)
	{
		float _hurtTime = HurtTime;
		if(stream.isWriting)
		{
			stream.Serialize(ref _hurtTime);
		}
		else{
			stream.Serialize(ref _hurtTime);
			HurtTime = _hurtTime;
		}
	}
}
