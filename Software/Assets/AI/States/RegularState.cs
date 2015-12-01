using System;
using UnityEngine;

public class RegularState:IState
{
	
	StateMachine stateMachine;
	public RegularState (Boss boss, IState regularState) : base(StateIds.Regular, boss)
	{
		NextStateIds.Add(StateIds.Dead);
		NextStateIds.Add(StateIds.Waiting);
		NextStateIds.Add (StateIds.Enraged);
		
		stateMachine = new StateMachine(new IdleState(boss));
		stateMachine.AddState(new StunnedState(boss));
		stateMachine.AddState(new HurtState(boss));
		stateMachine.AddState(regularState);
	}
	
	public override int ChangeState (float deltaTime)
	{
		if(boss.IsDead)
			return StateIds.Dead;
			
		if(!boss.Engaged)
			return StateIds.Waiting;
		
		if(boss.IsEnraged)
			return StateIds.Enraged;
		
		return StateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		stateMachine.ProcessStateMachine(deltaTime);
	}
	
	public override void Setup ()
	{
		//Debug.Log("Passing in Regular Setup");
		boss.SignalRecovering(12f);
		boss.RegularSetup();
		stateMachine.CurrentState.Setup();
	}
	
	public override string ToString ()
	{
		return string.Format("Regular {0}", stateMachine.CurrentState);
	}
	
	public override void SerializeNetwork (UnityEngine.BitStream stream)
	{
		stateMachine.SerializeNetwork(stream);
	}
}

