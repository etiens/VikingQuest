using UnityEngine;
using System.Collections;

public class VInputTestScript : MonoBehaviour {

	[SerializeField] private bool isGamepad = false;
	[SerializeField] private float rotationSpeed = 5f;
	[SerializeField] private float xMin = -7.3f;
	[SerializeField] private float xMax = 7.3f;
	[SerializeField] private float yMin = -4f;
	[SerializeField] private float yMax = 6f;

	// Use this for initialization
	void Start () {
		if(!isGamepad)
			Utils.Instance.InputManager.SetKeyboardInput (Utils.Player1Id);
		gameObject.renderer.material.color = new Color (0f, 1f, 0f, 0.3f);
	}
	
	// Update is called once per frame
	void Update () {
		ChangeInputType ();
		ChangeInversion ();
		ChangeColor ();
		Movement ();
		Rotation ();
		StopMovement ();
	}

	void OnGUI()
	{
		//GUI.Label (new Rect(10, 10, 1000, 50), string.Format ("Angualr Velocity: {0}",angularVelocity));
	}

	private void ChangeInputType()
	{
		//Switching input method
		if (Utils.Instance.InputManager.Player1.SelectDown()) {
			if(isGamepad)
				Utils.Instance.InputManager.SetKeyboardInput(Utils.Player1Id);
			else
				Utils.Instance.InputManager.SetXbox360Input(Utils.Player1Id,0);
			
			isGamepad = !isGamepad;
		}
	}

	private void ChangeInversion()
	{
		if (Utils.Instance.Player1.LeftStickButtonDown ())
			Utils.Instance.Player1.LeftStickInverted = !Utils.Instance.InputManager.Player1.LeftStickInverted;

		if (Utils.Instance.Player1.RightStickButtonDown ())
			Utils.Instance.Player1.RightStickInverted = !Utils.Instance.InputManager.Player1.RightStickInverted;
	}

	private void ChangeColor()
	{
		if (Utils.Instance.Player1.ADown ())
			gameObject.renderer.material.color = new Color (0f, 1f, 0f, 0.3f);
		else if (Utils.Instance.Player1.BDown())
			gameObject.renderer.material.color = new Color (1f, 0f, 0f, 0.3f);
		else if (Utils.Instance.Player1.XDown())
			gameObject.renderer.material.color = new Color (0f, 0f, 1f, 0.3f);
		else if (Utils.Instance.Player1.YDown())
			gameObject.renderer.material.color = new Color (1f, 1f, 0f, 0.3f);
	}

	private void Movement()
	{
		float leftStickX = Utils.Instance.Player1.LeftStickX ();
		float leftStickY = Utils.Instance.Player1.LeftStickY ();

		Vector3 position = gameObject.transform.position + new Vector3 (leftStickX, leftStickY, 0.0f);
		gameObject.transform.position = new Vector3 (Mathf.Clamp(position.x,xMin,xMax),Mathf.Clamp(position.y,yMin,yMax));
	}

	private Vector3 xAxis = new Vector3 (1f, 0f, 0f);
	private Vector3 yAxis = new Vector3 (0f, 1f, 0f);
	private void Rotation()
	{
		float leftTrigger = Utils.Instance.Player1.LeftTrigger ();
		float rightTrigger = Utils.Instance.Player1.RightTrigger ();

		gameObject.transform.Rotate (xAxis, leftTrigger*rotationSpeed);
		gameObject.transform.Rotate (yAxis, rightTrigger*rotationSpeed);

		int leftBumpFactor = 0;
		int rightBumpFactor = 0;
		if (Utils.Instance.Player1.LeftBumpDown ())
			leftBumpFactor = -5;
		if(Utils.Instance.Player1.RightBumpDown())
			rightBumpFactor = -5;

		gameObject.rigidbody.angularVelocity += xAxis*(leftTrigger*rotationSpeed + leftBumpFactor);
		gameObject.rigidbody.angularVelocity += yAxis*(rightTrigger*rotationSpeed + rightBumpFactor);
	}

	private void StopMovement ()
	{
		if (Utils.Instance.Player1.StartDown ()) {
			Vector3 zeros = new Vector3 (0f, 0f, 0f);
			gameObject.rigidbody.angularVelocity = zeros;
			gameObject.transform.position = zeros;
			Utils.Instance.Player1.ResetInputs();
		}
	}
}
