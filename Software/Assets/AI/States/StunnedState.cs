using System;

public class StunnedState:IState
{
	private float StunTime = 0f;
	
	public StunnedState (Boss boss) : base(StateIds.Stunned, boss)
	{
		NextStateIds.Add(StateIds.Attacking);
	}
	
	public override int ChangeState (float deltaTime)
	{
		int newStateId = StateId;
		
		if(boss.Hurt)
			return StateIds.Hurt;
		
		if(StunTime <= 0)
		{
			newStateId = StateIds.Attacking;
		}
		return newStateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		StunTime -= deltaTime;
		//play stunned animation
	}
	
	public override void Setup ()
	{
		StunTime = boss.StunTime;
		boss.ResetStun();
	}
	
	public override void SerializeNetwork (UnityEngine.BitStream stream)
	{
		float _stunTime = StunTime;
		if(stream.isWriting)
		{
			stream.Serialize(ref _stunTime);
		}
		else{
			stream.Serialize(ref _stunTime);
			StunTime = _stunTime;
		}
	}
}

