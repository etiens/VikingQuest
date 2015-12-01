using System;

public class EnragedState:IState
{

	StateMachine stateMachine;
	public EnragedState (Boss boss, IState enragedState) : base(StateIds.Enraged, boss)
	{
		NextStateIds.Add(StateIds.Dead);
		NextStateIds.Add(StateIds.Waiting);
		NextStateIds.Add (StateIds.Regular);
		
		stateMachine = new StateMachine(enragedState);
		stateMachine.AddState(new StunnedState(boss));
		stateMachine.AddState(new HurtState(boss));
		stateMachine.AddState(new IdleState(boss));
	}
	
	public override int ChangeState (float deltaTime)
	{
		if(boss.IsDead)
			return StateIds.Dead;
		
		if(!boss.Engaged)
			return StateIds.Waiting;
		
		if(!boss.IsEnraged)
			return StateIds.Regular;
		
		return StateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		stateMachine.ProcessStateMachine(deltaTime);
	}
	
	public override void Setup ()
	{
		stateMachine.CurrentState.Setup();
		boss.EnragedSetup();
	}
	
	public override string ToString ()
	{
		return string.Format("Enraged {0}", stateMachine.CurrentState);
	}
	
	public override void SerializeNetwork (UnityEngine.BitStream stream)
	{
		stateMachine.SerializeNetwork(stream);
	}
}

