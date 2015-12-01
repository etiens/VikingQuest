using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public abstract class VInput
{

		private static float axisActivationThreshold = 0.75f;
		private static float axisDeactivationThreshold = 0.30f;
		protected int leftStickInvert = 1;
		protected int rightStickInvert = -1;

		public bool LeftStickInverted {
				get {
						return leftStickInvert < 0;
				}
				set {
						leftStickInvert = value ? -1 : 1;
				}
		}
	
		public bool RightStickInverted {
				get {
						return rightStickInvert < 0;
				}
				set {
						rightStickInvert = value ? -1 : 1;
				}
		}

		public abstract void ResetInputs ();

	#region Axis Up and Down
		protected bool GetAxisUp (AxisInfo axis)
		{
				if (axis.State == AxisState.Up) {
						axis.Take ();
						return true;
				}
		
				return false;
		}

		protected bool GetAxisDown (AxisInfo axis)
		{
				if (axis.State == AxisState.Down) {
						axis.Take ();
						return true;
				}
		
				return false;
		}

		public abstract void UpdateAllAxisStates ();
		
		public class AxisInfo
		{
				public string Name { get; private set; }

				public AxisState State { get; private set; }

				public bool NativelyInverted { get; private set; }
				
				public AxisInfo (string name)
				{
						Name = name;
						State = AxisState.Idle;
						NativelyInverted = false;
				}

				public AxisInfo (string name, bool nativelyInverted)
				{
						Name = name;
						State = AxisState.Idle;
						NativelyInverted = nativelyInverted;
				}
			
				public void Take ()
				{
						switch (State) {
						case AxisState.Up:
								State = AxisState.UpTaken;
								break;
						case AxisState.Down:
								State = AxisState.DownTaken;
								break;
						}
				}
			
				public void UpdateState ()
				{
						float input = Input.GetAxisRaw (Name);
						if (NativelyInverted)
								input *= -1;
				
						switch (State) {
						case AxisState.Idle:
								if (input > axisActivationThreshold)
										State = AxisState.Up;
								else if (input < -axisActivationThreshold)
										State = AxisState.Down;
								break;
						
						case AxisState.Up:
						case AxisState.UpTaken:
								if (input < -axisActivationThreshold)
										State = AxisState.Down;
								else if (input < axisDeactivationThreshold)
										State = AxisState.Idle;
								break;
						
						case AxisState.Down:
						case AxisState.DownTaken:
								if (input > axisActivationThreshold)
										State = AxisState.Up;
								else if (input > -axisDeactivationThreshold)
										State = AxisState.Idle;
								break;
						}
				}
			
		}

		public enum AxisState
		{
				Idle,
				Up,
				Down,
				UpTaken,
				DownTaken}
		;
	#endregion

	#region Axis
		public abstract float LeftStickX ();

		public abstract float LeftStickY ();

		public abstract float RightStickX ();

		public abstract float RightStickY ();

		public abstract float LeftTrigger ();

		public abstract float RightTrigger ();
		
		public abstract bool RightTriggerDown();
		public abstract AxisInfo RTInfo();

		public abstract float Triggers ();

		public abstract float DPadX ();

		public abstract bool DPadXRight ();

		public abstract bool DPadXLeft ();

		public abstract float DPadY ();
	#endregion

	#region Axis Buttons

		//Right Stick Button
		public abstract bool RightStickButton ();

		public abstract bool RightStickButtonUp ();

		public abstract bool RightStickButtonDown ();

		//Left Stick Button
		public abstract bool LeftStickButton ();

		public abstract bool LeftStickButtonUp ();

		public abstract bool LeftStickButtonDown ();
	#endregion
	
	#region Regular Buttons
		//Right Bump Button
		public abstract bool RightBump ();

		public abstract bool RightBumpUp ();

		public abstract bool RightBumpDown ();
	
		//Left Bump Button
		public abstract bool LeftBump ();

		public abstract bool LeftBumpUp ();

		public abstract bool LeftBumpDown ();

		//A button
		public abstract bool A ();

		public abstract bool AUp ();

		public abstract bool ADown ();

		//B button
		public abstract bool B ();

		public abstract bool BUp ();

		public abstract bool BDown ();

		//X button
		public abstract bool X ();

		public abstract bool XUp ();

		public abstract bool XDown ();

		//Y button
		public abstract bool Y ();

		public abstract bool YUp ();

		public abstract bool YDown ();

		//Start button
		public abstract bool Start ();

		public abstract bool StartUp ();

		public abstract bool StartDown ();

		//Select button
		public abstract bool Select ();

		public abstract bool SelectUp ();

		public abstract bool SelectDown ();
    #endregion

	#region QTE
		public bool GetButtonState (Button button)
		{
				switch (button) {
				case Button.A:
						return A ();
				case Button.B:
						return B ();
				case Button.X:
						return X ();
				case Button.Y:
						return Y ();
				case Button.Start:
						return Start ();
				case Button.Select:
						return Select ();
				}

				return false;
		}

		public bool GetButtonUp (Button button)
		{
				switch (button) {
				case Button.A:
						return AUp ();
				case Button.B:
						return BUp ();
				case Button.X:
						return XUp ();
				case Button.Y:
						return YUp ();
				case Button.Start:
						return StartUp ();
				case Button.Select:
						return SelectUp ();
				}

				return false;
		}

		public bool GetButtonDown (Button button)
		{
				switch (button) {
				case Button.A:
						return ADown ();
				case Button.B:
						return BDown ();
				case Button.X:
						return XDown ();
				case Button.Y:
						return YDown ();
				case Button.Start:
						return StartDown ();
				case Button.Select:
						return SelectDown ();
				}

				return false;
		}

		public enum Button
		{
				A,
				B,
				X,
				Y,
				Start,
				Select,
		}
	#endregion

	#region MenuInputs

		public abstract bool MenuUp ();

		public abstract bool MenuDown ();

		public abstract bool MenuRight ();

		public abstract bool MenuLeft ();

		public abstract bool MenuSelect ();

		public abstract bool MenuBack ();
		
		public abstract bool MenuStart ();

	#endregion

}
