using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
	protected Boss boss = null;
	public int StateId {get; private set;}
	public List<int> NextStateIds {get; private set;}
	
	protected IState(int id, Boss b)
	{
		boss = b;
		StateId = id;
		//Rigester current state as a possibility to stay within that state
		NextStateIds = new List<int>{id};
	}
	
	public void AddState(int stateId)
	{
		NextStateIds.Add(stateId);
	}
	
	public bool CanChooseState(int nextState)
	{
		return NextStateIds.Contains(nextState);
	}
	
	public abstract int ChangeState(float deltaTime);
	public abstract void ProcessState(float deltaTime);
	public abstract void Setup();
	public abstract void SerializeNetwork(BitStream stream);
	
	public override string ToString ()
	{
		return StateIds.Name(StateId);
	}
}
