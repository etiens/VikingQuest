using UnityEngine;
using System.Collections;

public class KeyboardInput : VInput {

	#region Axis
	public override float LeftStickX ()
	{
		float val = 0;
		val -= Input.GetKey (KeyCode.A) ? 1 : 0;
		val += Input.GetKey (KeyCode.D) ? 1 : 0;
		return val;
	}
	public override float LeftStickY ()
	{
		float val = 0;
		val -= Input.GetKey (KeyCode.S) ? 1 : 0;
		val += Input.GetKey (KeyCode.W) ? 1 : 0;
		return leftStickInvert*val;
	}
	
	public override float RightStickX (){return Input.GetAxis ("Mouse X");}
	public override float RightStickY (){return rightStickInvert*Input.GetAxis ("Mouse Y");}
	
	public override float LeftTrigger (){return Input.GetKey (KeyCode.Q) ? 1 : 0;}
	public override float RightTrigger (){return Input.GetKey (KeyCode.E) ? 1 : 0;}
	public override bool RightTriggerDown(){return Input.GetKeyDown(KeyCode.E);}
	public override AxisInfo RTInfo(){return null;}

	public override float Triggers(){return RightTrigger() - LeftTrigger();}
	
	public override float DPadX ()
	{
		float val = 0;
		val -= Input.GetKey (KeyCode.LeftArrow) ? 1 : 0;
		val += Input.GetKey (KeyCode.RightArrow) ? 1 : 0;
		return val;
	}
	public override bool DPadXRight(){return Input.GetKeyDown (KeyCode.RightArrow);}
	public override bool DPadXLeft(){return Input.GetKeyDown (KeyCode.LeftArrow);}

	public override float DPadY ()
	{
		float val = 0;
		val -= Input.GetKey (KeyCode.DownArrow) ? 1 : 0;
		val += Input.GetKey (KeyCode.UpArrow) ? 1 : 0;
		return val;
	}
	#endregion
	
	#region Axis Buttons
	//Right Stick Button
	public override bool RightStickButton (){return Input.GetKey(KeyCode.Alpha2);}
	public override bool RightStickButtonUp (){return Input.GetKeyUp(KeyCode.Alpha2);}
	public override bool RightStickButtonDown (){return Input.GetKeyDown(KeyCode.Alpha2);}
	
	//Left Stick Button
	public override bool LeftStickButton (){return Input.GetKey(KeyCode.Alpha1);}
	public override bool LeftStickButtonUp (){return Input.GetKeyUp(KeyCode.Alpha1);}
	public override bool LeftStickButtonDown (){return Input.GetKeyDown(KeyCode.Alpha1);}
	#endregion
	
	
	#region Regular Buttons
	//Right Bump Button
	public override bool RightBump(){return Input.GetKey(KeyCode.Alpha4);}
	public override bool RightBumpUp(){return Input.GetKeyUp(KeyCode.Alpha4);}
	public override bool RightBumpDown(){return Input.GetKeyDown(KeyCode.Alpha4);}
	
	//Left Bump Button
	public override bool LeftBump(){return Input.GetKey(KeyCode.Alpha3);}
	public override bool LeftBumpUp(){return Input.GetKeyUp(KeyCode.Alpha3);}
	public override bool LeftBumpDown(){return Input.GetKeyDown(KeyCode.Alpha3);}
	
	//A button
	public override bool A(){return Input.GetMouseButton (0);}
	public override bool AUp(){return Input.GetMouseButtonUp (0);}
	public override bool ADown(){return Input.GetMouseButtonDown (0);}
	
	//B button
	public override bool B(){return Input.GetMouseButton (1);}
	public override bool BUp(){return Input.GetMouseButtonUp (1);}
	public override bool BDown(){return Input.GetMouseButtonDown (1);}
	
	//X button
	public override bool X(){return Input.GetKey(KeyCode.Space);}
	public override bool XUp(){return Input.GetKeyUp(KeyCode.Space);}
	public override bool XDown(){return Input.GetKeyDown(KeyCode.Space);}
	
	//Y button
	public override bool Y(){return Input.GetKey(KeyCode.C);}
	public override bool YUp(){return Input.GetKeyUp(KeyCode.C);}
	public override bool YDown(){return Input.GetKeyDown(KeyCode.C);}
	
	//Start button
	public override bool Start(){return Input.GetKey(KeyCode.Escape);}
	public override bool StartUp(){return Input.GetKeyUp(KeyCode.Escape);}
	public override bool StartDown(){return Input.GetKeyDown(KeyCode.Escape);}
	
	//Select button
	public override bool Select(){return Input.GetKey(KeyCode.Tab);}
	public override bool SelectUp(){return Input.GetKeyUp(KeyCode.Tab);}
	public override bool SelectDown(){return Input.GetKeyDown(KeyCode.Tab);}
	#endregion

	public override void ResetInputs ()
	{
		Input.ResetInputAxes ();
	}

	#region Menu Inputs
		public override void UpdateAllAxisStates ()
		{
		}
		public override bool MenuUp ()
		{
			return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
		}
		public override bool MenuDown ()
		{
			return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
		}
		public override bool MenuRight ()
		{
			return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
		}
		public override bool MenuLeft ()
		{
			return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
		}
		public override bool MenuSelect ()
		{
			return Input.GetKeyDown(KeyCode.Return);
		}
		public override bool MenuBack ()
		{
			return Input.GetKeyDown(KeyCode.Backspace);
		}
		public override bool MenuStart ()
		{
			return Input.GetKeyDown(KeyCode.Escape);
		}
	#endregion
}
