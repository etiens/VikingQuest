using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class GlobalScript : Singleton<GlobalScript> {

	[SerializeField] private Harpooner harpooner;
	public Harpooner Harpooner{get {return harpooner;} private set{harpooner = value;}}
	private Vector3 harpoonerPosition;
	[SerializeField] private BoatController boat;
	public BoatController Boat{ get {return boat;} private set{boat = value;} }
	[SerializeField] private Driver driver;
	public Driver Driver { get {return driver;} private set{driver = value;} }
	[SerializeField] private MenuInputHandler menuInputHandler = null;
	public MenuInputHandler MenuInputHandler {get {return menuInputHandler;}}

	public QTEManager QTEManager;

	public VikingCamera Camera { get; private set; }
	[SerializeField] private bool isGamepad = false;
	[SerializeField] private bool rightStickInverted = false;
	[SerializeField] private bool leftStickInverted = false;

	public AudioClip musicDefaultClip;
	public AudioClip musicBossClip;
	public AudioSource musicDefaultSource;
	public AudioSource musicBossSource;
	private static float musicDefaultTime = -1;
	//private static int musicDefaultTime;

	public static int CheckpointToLoad = 2;
	public int CurrentCheckPointToLoad = 2;
	public static bool setupImmediately = false;
	public GameObject[] BoatSpawnLocations;
	public bool isLocal = true;
	
	public GUIText textUI;
	public List<TimedObject<GameObject>> DestroyList;

	public List<AudioSource> audioSources;
	public static List<float> audioSourcesTime;
	private Thread assignAudioThread;
	private AutoResetEvent assignAudioEvent;

	// -1 failed, 1 victory.
	private int victoryStatus = 0;
	public int VictoryStatus { get { return victoryStatus; } set { victoryStatus = value; } }
	
	private AsyncOperation async;
	
	public Kraken kraken;
	
	void Awake(){
		InitializeComponents ();
	}

	void Start() 
	{
		SetupControlPreferences ();

		if(Network.isClient)
			SetupHarpooner ();
		else
			SetupDriver();

		// Chaque fois qu'on pars le script, on verifie si on a pas changer de checkpoint
		if(CurrentCheckPointToLoad != CheckpointToLoad)
			CheckpointToLoad = CurrentCheckPointToLoad;
		
		harpoonerPosition = new Vector3(0f,0f,-3f);
		transform.position = new Vector3(0f,0f,0f);
		transform.localScale = new Vector3(1f,1f,1f);
		
		DestroyList = new List<TimedObject<GameObject>>();

		InitSounds ();
	}
	// Update is called once per frame
	void Update ()
	{
		ProcessControls ();
		Utils.Instance.Player1.UpdateAllAxisStates();
		UpdateDestroyList();
		SendCheckpointValue();
	}

	public void InitializeComponents ()
	{
		if (Network.isServer || isLocal)
		{
			if (BoatSpawnLocations.Length > CheckpointToLoad - 1)
			{
				Boat.transform.position = BoatSpawnLocations[CheckpointToLoad - 1].transform.position;
				Boat.transform.rotation = BoatSpawnLocations[CheckpointToLoad - 1].transform.rotation;
			}
		}

		AttachComponents();
	}
	
	void SendCheckpointValue()
	{
		if(Network.isClient) return;
		Utils.NetworkCommand(this,"RPCSendCheckpointValue",new object[]{CheckpointToLoad});
	}
	
	[RPC]
	void RPCSendCheckpointValue(int value)
	{
		CheckpointToLoad = value;
	}

	private void InitSounds(){
		musicDefaultSource = Camera.Camera.gameObject.AddComponent<AudioSource>();
		musicBossSource = Camera.Camera.gameObject.AddComponent<AudioSource>();
		musicDefaultSource.clip = musicDefaultClip;
		musicDefaultSource.loop = true;
		musicDefaultSource.volume = 0.15f;
		if(musicDefaultTime > 0.0)
			musicDefaultSource.time = musicDefaultTime;
		musicDefaultSource.playOnAwake = true;
		musicDefaultSource.Play();
		musicBossSource.clip = musicBossClip;
		musicBossSource.loop = true;
		musicBossSource.volume = 0.12f;
		musicBossSource.playOnAwake = false;
		driver.windSoundSource = GlobalScript.Instance.Camera.Camera.gameObject.AddComponent<AudioSource>();
		driver.breathSoundSource = GlobalScript.Instance.Camera.Camera.gameObject.AddComponent<AudioSource>();
		driver.InitSounds();
	}
	
	public void changeMusic(){
		musicDefaultSource.Stop ();
		musicBossSource.Play ();
	}

	public void changeMusicDelayed(float delay){
		musicDefaultSource.Stop ();
		musicBossSource.time = delay;
		musicBossSource.Play ();
	}
	
	void UpdateDestroyList()
	{
		List<TimedObject<GameObject>> objectsToRemove = new List<TimedObject<GameObject>>();
		foreach(TimedObject<GameObject> to in DestroyList)
			if(to.Update(Time.deltaTime) <= 0f)
				objectsToRemove.Add(to);
				
		foreach(TimedObject<GameObject> to in objectsToRemove)
			if(DestroyList.Remove(to))
				Utils.Destroy(to.Value);
	}
	
	[RPC]
	public void LoadKrakenLevel(){
		Application.LoadLevel("prototype_kraken_fight");
	}

	public void LoadAsync(int level)
	{
		//assignAudioThread = new Thread(new ThreadStart(AssignAudio));
		//assignAudioThread.Start();
		async = Application.LoadLevelAsync(0);
		StartCoroutine(LoadScene());
		async.allowSceneActivation=false;
	}
	
	IEnumerator LoadScene()
	{
		async.allowSceneActivation = false;
		Debug.Log("Loading progress : " + async.progress);
		
		// Currently loading another scene
		// == 0.9f is the magic Unity async progress number apparently
		while(async.progress < 0.9f)
		{
		yield return null;
		}
		Debug.Log("Activating scene activation");
		musicDefaultTime = musicDefaultSource.time;
		async.allowSceneActivation = true;
		Utils.ReloadSceneCleanup();

		yield return async;
	}

	public void AssignAudio(){
		AsyncOperation async = (AsyncOperation)menuInputHandler.LoadAsync(0);
		async.allowSceneActivation = false;
		while(!async.isDone){
			Thread.Sleep(500);
		}
		musicDefaultTime = musicDefaultSource.time;
		async.allowSceneActivation = true;
	}

	public void EngageQTE(VInput.Button button, float maxTime, int requiredHit){
		if(!QTEManager.InQTEMode && !kraken.IsDead){
			Boat.qteHugRight = !Boat.qteHugRight;
			QTEManager.EngageQTE (button, maxTime, requiredHit);
			QTEManager.QTECompletedEvent += OnQTECompleted;
		}
	}
	
	
	public void OnQTECompleted(bool success){

		QTEManager.QTECompletedEvent -= OnQTECompleted;
		
		if(success){
			UIScript.Instance.EnableAllUI();
			Boat.Health = 30f;
			if(harpooner.HasControl)
				Harpooner.GetCurrentStation().showTrajectoryLine();
		}
		else{
			DeathUponYou();
		}
		driver.LeaveQTEAnimation();
		harpooner.LeaveQTEAnimation();
		// Do something with QTE completion
	}
	
	[RPC]
	public void DeathUponYou()
	{
		if(kraken != null && kraken.Engaged)
			kraken.Engaged = false;
		VictoryStatus = -1;
		menuInputHandler.OpenEndGameMenu();
	}
	
	[RPC]
	public void BigBigWinner()
	{
		if(kraken != null && kraken.Engaged)
			kraken.Engaged = false;
		VictoryStatus = 1;
		menuInputHandler.OpenEndGameMenu();
	}

	public void AssignCheckpoint(int checkpointToLoad){
		CurrentCheckPointToLoad = checkpointToLoad;
	}

	public void AttachComponents()
	{
		Harpooner.SetFrontHarpooningStation ();
		AttachCamera();
	}
	
	public void MoveHarpoonerToStation(Utils.Stations stationEnum)
	{
		HarpooningStation station = Boat.GetStationFromEnum (stationEnum);
		Harpooner.transform.position = station.transform.TransformPoint (harpoonerPosition);
		Harpooner.transform.forward = station.transform.forward;
		Harpooner.transform.SetParent (station.transform,true);
	}


	private void AttachCamera()
	{
		Camera = gameObject.GetComponent<VikingCamera> ();
	}

	private void SetupControlPreferences()
	{
		if(!isGamepad)
			Utils.Instance.InputManager.SetKeyboardInput (Utils.Player1Id);
		Utils.Instance.Player1.LeftStickInverted = leftStickInverted;
		Utils.Instance.Player1.RightStickInverted = rightStickInverted;
	}

	public void SetupHarpooner()
	{
		//Setup Controls
		GiveHarpoonerControls ();
		//Setup Alpha
		boat.SetupHarpoonerAlphas();
		//Setup camera
		Camera.TargetHarpooner ();
	}

	public void SetupDriver()
	{
		//Setup Controls
		GiveDriverControls ();
		//Setup Alpha
		boat.SetupDriverAlphas();
		//Setup Camera
		Camera.TargetBoat ();
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

	private void GiveDriverControls()
	{
		Driver.HasControl = true;
		Harpooner.HasControl = false;

		Boat.GetStationFromEnum(Harpooner.CurrentStation).hideTrajectoryLine();
	}

	private void GiveHarpoonerControls()
	{
		Harpooner.HasControl = true;
		Driver.HasControl = false;

		Boat.GetStationFromEnum(Harpooner.CurrentStation).showTrajectoryLine();
	}

	private void ProcessControls()
	{
		if(Utils.Instance.Player1.XDown())
			if(Utils.IsNetwork)
				Utils.NetworkCommand(PositionSwitchingManager.Instance,"AskSwitch",new object[]{Network.isServer});
			else
				SwitchPosition();
				
		if (Input.GetKeyDown (KeyCode.L))
			GlobalScript.Instance.SwitchPosition();
	}

	public void SwitchPosition()
	{
		//Boat -> switch to harpooner
		if(Driver.HasControl)
		{
			Driver.ResetControls();
			SetupHarpooner();
		}
		//Harpooner -> switch to driver
		else
		{
			Harpooner.ResetControls();
			SetupDriver();
		}
		
	}

	void OnLevelWasLoaded(int level) {
		AssignCheckpoint(CheckpointToLoad);
	}

}

