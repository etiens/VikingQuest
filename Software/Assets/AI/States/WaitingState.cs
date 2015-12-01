using UnityEngine;
using System.Collections;

public class WaitingState : IState
{
	public WaitingState(Boss boss) : base(StateIds.Waiting, boss)
	{
		NextStateIds.Add(StateIds.Regular);
		NextStateIds.Add(StateIds.Enraged);
	}
	
	public override int ChangeState (float deltaTime)
	{
		if(boss.Engaged)
			if(!boss.IsEnraged)
				return StateIds.Regular;
			else
				return StateIds.Enraged;
		else
			return StateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		//play idle animation
	}
	
	public override void Setup ()
	{
		
	}
	
	public override void SerializeNetwork (BitStream stream)
	{
		
	}
}

