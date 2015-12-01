using UnityEngine;
using System.Collections;

public class Xbox360Input : VInput {

	public int JoypadId{ get; private set;}
	public float Threshold { get; set; }
	
	public AxisInfo LeftStickYInfo;
	private AxisInfo LeftStickXInfo;
	private AxisInfo DPadYInfo;
	private AxisInfo DPadXInfo;
	private AxisInfo RightTriggerInfo;

	public Xbox360Input(int joypadId)
	{
		JoypadId = joypadId;
		Threshold = 0.2f;
		
		LeftStickYInfo = new AxisInfo(string.Format ("360LeftStickY_{0}", JoypadId), true);
		LeftStickXInfo = new AxisInfo(string.Format ("360LeftStickX_{0}", JoypadId));
		DPadYInfo = new AxisInfo(string.Format ("360DPadY_{0}", JoypadId));
		DPadXInfo = new AxisInfo(string.Format ("360DPadX_{0}", JoypadId));
		RightTriggerInfo = new AxisInfo(string.Format ("360RightTrigger_{0}", JoypadId));
	}

	#region Methods
	private float clamp (float value)
	{
			if (Mathf.Abs (value) < Threshold)
					return 0.0f;
			return value;
	}
	#endregion

	#region Axis
	public override float LeftStickX (){return clamp(Input.GetAxisRaw (string.Format ("360LeftStickX_{0}", JoypadId)));}
	public override float LeftStickY (){return -leftStickInvert*clamp(Input.GetAxisRaw (string.Format ("360LeftStickY_{0}", JoypadId)));}
	
	public override float RightStickX (){return clamp(Input.GetAxis (string.Format ("360RightStickX_{0}", JoypadId)));}
	public override float RightStickY (){return -rightStickInvert*clamp(Input.GetAxis (string.Format ("360RightStickY_{0}", JoypadId)));}

	public override float LeftTrigger (){return clamp(Input.GetAxis (string.Format ("360LeftTrigger_{0}", JoypadId)));}
	public override float RightTrigger (){return clamp(Input.GetAxis (string.Format ("360RightTrigger_{0}", JoypadId)));}
	public override bool RightTriggerDown(){return GetAxisUp(RightTriggerInfo);}
	public override AxisInfo RTInfo(){return RightTriggerInfo;}

	//For some reason this doesn't return the value indicated by the documentation. -1*value needed
	public override float Triggers() {return -clamp(Input.GetAxis (string.Format ("360Triggers_{0}", JoypadId)));}
	
	public override float DPadX (){return clamp(Input.GetAxis (string.Format ("360DPadX_{0}", JoypadId)));}
	public override float DPadY (){return clamp(Input.GetAxis (string.Format ("360DPadY_{0}", JoypadId)));}
	public override bool DPadXRight(){return GetAxisUp(DPadXInfo);}
	public override bool DPadXLeft(){return GetAxisDown(DPadXInfo);}
	#endregion
	
	#region Axis Buttons
	//Right Stick Button
	public override bool RightStickButton (){return Input.GetButton (string.Format ("360RightStickButton_{0}", JoypadId));}
	public override bool RightStickButtonUp (){return Input.GetButtonUp (string.Format ("360RightStickButton_{0}", JoypadId));}
	public override bool RightStickButtonDown (){return Input.GetButtonDown (string.Format ("360RightStickButton_{0}", JoypadId));}
	
	//Left Stick Button
	public override bool LeftStickButton (){return Input.GetButton (string.Format ("360LeftStickButton_{0}", JoypadId));}
	public override bool LeftStickButtonUp (){return Input.GetButtonUp (string.Format ("360LeftStickButton_{0}", JoypadId));}
	public override bool LeftStickButtonDown (){return Input.GetButtonDown (string.Format ("360LeftStickButton_{0}", JoypadId));}
	#endregion
	
	
	#region Regular Buttons
	//Right Bump Button
	public override bool RightBump(){return Input.GetButton (string.Format ("360RightBump_{0}", JoypadId));}
	public override bool RightBumpUp(){return Input.GetButtonUp (string.Format ("360RightBump_{0}", JoypadId));}
	public override bool RightBumpDown(){return Input.GetButtonDown (string.Format ("360RightBump_{0}", JoypadId));}
	
	//Left Bump Button
	public override bool LeftBump(){return Input.GetButton (string.Format ("360LeftBump_{0}", JoypadId));}
	public override bool LeftBumpUp(){return Input.GetButtonUp (string.Format ("360LeftBump_{0}", JoypadId));}
	public override bool LeftBumpDown(){return Input.GetButtonDown (string.Format ("360LeftBump_{0}", JoypadId));}
	
	//A button
	public override bool A(){return Input.GetButton (string.Format ("360A_{0}", JoypadId));}
	public override bool AUp(){return Input.GetButtonUp (string.Format ("360A_{0}", JoypadId));}
	public override bool ADown(){return Input.GetButtonDown (string.Format ("360A_{0}", JoypadId));}
	
	//B button
	public override bool B(){return Input.GetButton (string.Format ("360B_{0}", JoypadId));}
	public override bool BUp(){return Input.GetButtonUp (string.Format ("360B_{0}", JoypadId));}
	public override bool BDown(){return Input.GetButtonDown (string.Format ("360B_{0}", JoypadId));}
	
	//X button
	public override bool X(){return Input.GetButton (string.Format ("360X_{0}", JoypadId));}
	public override bool XUp(){return Input.GetButtonUp (string.Format ("360X_{0}", JoypadId));}
	public override bool XDown(){return Input.GetButtonDown (string.Format ("360X_{0}", JoypadId));}
	
	//Y button
	public override bool Y(){return Input.GetButton (string.Format ("360Y_{0}", JoypadId));}
	public override bool YUp(){return Input.GetButtonUp (string.Format ("360Y_{0}", JoypadId));}
	public override bool YDown(){return Input.GetButtonDown (string.Format ("360Y_{0}", JoypadId));}
	
	//Start button
	public override bool Start(){return Input.GetButton (string.Format ("360Start_{0}", JoypadId));}
	public override bool StartUp(){return Input.GetButtonUp (string.Format ("360Start_{0}", JoypadId));}
	public override bool StartDown(){return Input.GetButtonDown (string.Format ("360Start_{0}", JoypadId));}
	
	//Select button
	public override bool Select(){return Input.GetButton (string.Format ("360Select_{0}", JoypadId));}
	public override bool SelectUp(){return Input.GetButtonUp (string.Format ("360Select_{0}", JoypadId));}
	public override bool SelectDown(){return Input.GetButtonDown (string.Format ("360Select_{0}", JoypadId));}
	#endregion

	public override void ResetInputs ()
	{
		Input.ResetInputAxes ();
	}
	
	public override void UpdateAllAxisStates ()
	{
		LeftStickYInfo.UpdateState();
		Utils.Instance.Info = LeftStickYInfo.State;
		LeftStickXInfo.UpdateState();
		DPadYInfo.UpdateState();
		DPadXInfo.UpdateState();
		RightTriggerInfo.UpdateState();
	}
	
	public override bool MenuUp(){return GetAxisUp(LeftStickYInfo) || GetAxisUp(DPadYInfo);}
	public override bool MenuDown(){return GetAxisDown(LeftStickYInfo) || GetAxisDown(DPadYInfo);}
	public override bool MenuRight(){return GetAxisUp(LeftStickXInfo) || GetAxisUp(DPadXInfo);}
	public override bool MenuLeft(){return GetAxisDown(LeftStickXInfo) || GetAxisDown(DPadXInfo);}
	public override bool MenuSelect(){return ADown();}
	public override bool MenuBack(){return BDown();}
	public override bool MenuStart(){return StartDown();}
}
