using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
	public Dictionary<int,IState> States {get; private set;}
	
	public IState CurrentState {get; private set;}
	
	public StateMachine(IState initialState)
	{
		States = new Dictionary<int, IState>();
		CurrentState = initialState;
		AddState(CurrentState);
		
	}
	
	public void AddState(IState state)
	{
		States.Add(state.StateId, state);
	}
	
	public bool ChooseState(int stateId)
	{
		if(stateId == CurrentState.StateId)
			return false;
		
		bool stateChanged = false;
		
		if(States.ContainsKey(stateId))
			if(CurrentState.CanChooseState(stateId))
			{
				CurrentState = States[stateId];
				stateChanged = true;
			}
		
		return stateChanged;
	}
	
	//Let the state dictate what the next state should be
	public void ProcessStateMachine(float deltaTime)
	{
		int oldStateId = CurrentState.StateId;
		//Check what state we should be in
		int stateId = CurrentState.ChangeState(deltaTime);
		CurrentState = States[stateId];
		if(CurrentState.StateId != oldStateId)
		{
			Debug.Log(string.Format("Changing State to: {0}",CurrentState));
			CurrentState.Setup();
		}
		//Process state with time between frames
		CurrentState.ProcessState(deltaTime);
	}
	
	public void SerializeNetwork(BitStream stream)
	{
		int currentStateId = CurrentState.StateId;
		if(stream.isWriting)
		{
			stream.Serialize(ref currentStateId);
		}
		else {
			stream.Serialize(ref currentStateId);
			ChooseState(currentStateId);
		}
		CurrentState.SerializeNetwork(stream);
	}
}
