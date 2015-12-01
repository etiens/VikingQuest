using UnityEngine;
using System.Collections;

public class VInputManager{

	public enum VInputType { XBOX360, KEYBOARD, DISCONNECTED};
	private VInputType[] InputTypes;
	private VInput[] Players;

	public VInput Player1 {get{return Players[0];}}
	public VInput Player2 {get{return Players[1];}}
	public VInput Player3 {get{return Players[2];}}
	public VInput Player4 {get{return Players[3];}}

	public VInputManager()
	{
		InputTypes = new VInputType[4];
		Players = new VInput[4];

		Players[0] = new Xbox360Input (0);
		InputTypes [0] = VInputType.XBOX360;

		InputTypes [1] = VInputType.DISCONNECTED;
		InputTypes [2] = VInputType.DISCONNECTED;
		InputTypes [3] = VInputType.DISCONNECTED;
	}

	public void SetKeyboardInput(int playerId)
	{
		playerId--;
		VerifiyPlayerId (playerId);
		Players[playerId] = new KeyboardInput();
		InputTypes [playerId] = VInputType.KEYBOARD;
	}

	public void SetXbox360Input(int playerId, int joypadId)
	{
		playerId--;
		VerifiyPlayerId (playerId);
		Players[playerId] = new Xbox360Input(joypadId);
		InputTypes [playerId] = VInputType.XBOX360;
	}

	public void Disconnect(int playerId)
	{
		playerId--;
		VerifiyPlayerId (playerId);
		Players[playerId] = null;
		InputTypes [playerId] = VInputType.DISCONNECTED;
	}

	public VInputType GetInputType(int playerId)
	{
		playerId--;
		VerifiyPlayerId (playerId);
		return InputTypes [playerId];
	}

	private void VerifiyPlayerId(int playerId)
	{
		if(playerId < 0 || playerId > 3)
			throw new System.Exception(string.Format("Player Id:{0} unexpected",playerId));
	}
}
