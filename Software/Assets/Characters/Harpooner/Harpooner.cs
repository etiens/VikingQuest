using UnityEngine;
using System.Collections;

public class Harpooner : IControllable {

	#region Variables
	public Vector3 Direction{get; private set;}
	public float AnglePhi{get; private set;}
	public float AngleTheta{get; private set;}
	[SerializeField] float aimSensitivity = 20f;
	[SerializeField] float minSensitivity = 10f;
	[SerializeField] float maxSensitivity = 50f;
	public float AimSensitivity{get{return aimSensitivity;}set{aimSensitivity = Mathf.Clamp(value,maxSensitivity,minSensitivity);}}
	[SerializeField] float precisionFactor = 2f;

	float startPhi = -Mathf.PI/2;
	float startTheta = Mathf.PI/2;

	public float rotationWidth = Mathf.PI/4;
	
	

	private Utils.Stations currentStation = Utils.Stations.Front;
	public Utils.Stations CurrentStation{ 
		get
		{
			return currentStation;
		} 
		set
		{
			if (GetCurrentStation() != null) {
				GetCurrentStation().hideTrajectoryLine();
				GetCurrentStation().disableColliders();
			}
			currentStation = value;
			AssignStation();
		}
	}
	#endregion
	
	#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();
		IdleStartFrame = 0f;
		IdleStopFrame = 0.05f;
		QTEStartFrame = 0.18f;
		QTEStopFrame = 0.29f;
		IdleAnimationSpeed = 0.05f;
		QTEAnimationSpeed = 0.7f;
		
		HasControl = false;
	}
	// Use this for initializati`on
	void Start () {
		AngleTheta = startTheta;
		AnglePhi = startPhi;
		AssignStation ();
		//GetCurrentStation().enableColliders();
		//setDirection(AngleTheta, AnglePhi);
	}

	// Update is called once per frame
	void Update () {
		UpdateControls ();
		MoveStation();
	}
	
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}
	
	/*protected void OnGUI()
	{
		GUI.Label(new Rect(30f,30f,200,120),string.Format("Driver: currentloop: {0}, playbackTime: {1:0.00}",
		                                                  CurrentPlayBackLoop,GetAnimationPlayback()));
	}*/
	#endregion

	

	#region IControllable implementation
	protected override void ProcessControls()
	{
		//Update shoot direction
		float yValue = Utils.Instance.Player1.LeftStickY() * Time.deltaTime * aimSensitivity;
		float xValue = -Utils.Instance.Player1.LeftStickX() * Time.deltaTime * aimSensitivity;
		if(Utils.Instance.Player1.LeftTrigger() > 0.3f)
		{
			yValue = yValue/precisionFactor;
			xValue = xValue/precisionFactor;
		}
		UpdateDirection (xValue, yValue);

		//Shooting
		if (Utils.Instance.Player1.RightTrigger () > 0.2) 
			Utils.NetworkCommand(this ,"ShootHarpoon", new object[]{(object)Direction});

		//Switching stations
		if (Utils.Instance.Player1.RightBumpDown()) {
			Utils.NetworkCommand(this ,"ChangeStationRight");
		}
		else if (Utils.Instance.Player1.LeftBumpDown ()) {
			Utils.NetworkCommand(this ,"ChangeStationLeft");
		}
		//Switching weapon
		if (Utils.Instance.Player1.DPadXRight() || Utils.Instance.Player1.DPadXLeft()){
			GetCurrentStation().ChangeHarpoonType();
		}
	}
	public override void ResetControls ()
	{
		//nothing to do here
	}
	
	protected override void ResetCamera ()
	{
		AngleTheta = startTheta;
		AnglePhi = startPhi;
		GlobalScript.Instance.Camera.ResetCameraPosition();
	}
	#endregion
	
	

	#region Changing station
	public void SetFrontHarpooningStation()
	{
		CurrentStation = Utils.Stations.Front;
	}

	private void AssignStation(){		
		GetCurrentStation().enableColliders();
		GlobalScript.Instance.MoveHarpoonerToStation(CurrentStation);
		GetCurrentStation().showTrajectoryLine();
		transform.rotation = GetCurrentStation().transform.rotation;
	}

	[RPC]
	private void ChangeStationRight(){
		int newStation = (int)CurrentStation;
		newStation += 1;
		if (newStation == 3){
			newStation = 0;
		}
		CurrentStation = (Utils.Stations)newStation;
	}

	[RPC]
	private void ChangeStationLeft(){
		int newStation = (int)CurrentStation;
		newStation -= 1;
		if (newStation == -1){
			newStation = 2;
		}
		CurrentStation = (Utils.Stations)newStation;
	}
	#endregion

	#region Harpoon Station
	public HarpooningStation GetCurrentStation(){
		return GlobalScript.Instance.Boat.GetStationFromEnum(currentStation);
	}
	[RPC]
	private void ShootHarpoon(Vector3 direction)
	{
		GetCurrentStation ().ShootHarpoon (direction);
	}
	private void UpdateDirection(float xValue, float yValue)
	{
		AnglePhi += yValue;
		AngleTheta += xValue;
		if (AnglePhi < startPhi - rotationWidth  ){
			AnglePhi = startPhi - rotationWidth;
		}
		if (AnglePhi > startPhi + rotationWidth  ){
			AnglePhi = startPhi + rotationWidth;
		}
		if (AngleTheta < startTheta - rotationWidth){
			AngleTheta = startTheta - rotationWidth;
		}
		if(AngleTheta > startTheta + rotationWidth){
			AngleTheta = startTheta + rotationWidth;
		}

		Utils.NetworkCommand(this, "ShareAngles", RPCMode.OthersBuffered, AnglePhi, AngleTheta);
	}

	[RPC]
	private void ShareAngles(float phi, float theta)
	{
		AnglePhi = phi;
		AngleTheta = theta;
	}

	private void MoveStation()
	{
		if(GetCurrentStation() != null){
			setDirection(AngleTheta, AnglePhi);
			GetCurrentStation().setTrajectoryPoints(Direction);
			
			Quaternion hingeRotation = Quaternion.Euler(0f, -AngleTheta * Mathf.Rad2Deg, 0f);
			Quaternion gunRotation = Quaternion.Euler((-AnglePhi * Mathf.Rad2Deg) - 23f, 90f, 0f);
			GetCurrentStation().Hinge.transform.localRotation = hingeRotation;
			GetCurrentStation().Gun.transform.localRotation = gunRotation;
		}
	}

	private void setDirection(float theta, float phi){
		Vector3 newDirection = new Vector3(Mathf.Cos(theta), Mathf.Cos (phi) + 90f, Mathf.Sin(theta));
		newDirection = GetCurrentStation().Gun.transform.TransformDirection(newDirection);
		newDirection.Normalize();
		Direction = newDirection;
	}
	#endregion
}
