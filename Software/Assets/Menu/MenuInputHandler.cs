using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class MenuInputHandler : MonoBehaviour {

	// State boolean, if the menu is activated.
	private bool menuEnabled;
	public bool MenuEnabled {get{return menuEnabled;}}

	// The necessary event system to control the selected unity ui.
	[SerializeField]
	private EventSystem eventSystem;

	// This class is stricly coded to handle a vertical menu.
	private Selectable lowestSelectable;
	private Selectable highestSelectable;
	private Selectable currentSelected;

	// Start network panel.
	[SerializeField]
	private GameObject startNetworkPanel = null;

	// Default network panel.
	[SerializeField]
	private GameObject defaultNetworkPanel = null;

	// Default local panel.
	[SerializeField]
	private GameObject localPanel = null;

	// End game panel for winning and losing.
	[SerializeField]
	private GameObject endGamePanelWin = null;
	[SerializeField]
	private GameObject endGamePanelLose = null;

	private bool endGameMenu = false;
	
	public Camera menuCamera;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		endGameMenu = false;

		// Menus are open by default.
		Utils.NetworkCommand(this, "OpenMenu");

		//ScreenFader.Instance.FadedOutCompletedEvent += OpenEndGameMenu;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () 
	{
		// Handles controls for the menu.
		if (menuEnabled)
		{
			/*GameObject currentPanel = transform.FindChild (CurrentDefaultPanel()).gameObject;
			if(!currentPanel.activeSelf)
				ShowPanel(CurrentDefaultPanel());*/
		
			if (Utils.Instance.Player1.MenuUp())
			{
				SelectUp();
			}
			else if (Utils.Instance.Player1.MenuDown())
			{
				SelectDown();
			}
			
			// Check and handler for a selection
			if (Utils.Instance.Player1.MenuSelect())
			{
				var method = typeof(MenuInputHandler).GetMethod(currentSelected.gameObject.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (method != null)
				{
					method.Invoke(this, null);
				}
			}
			
		}

		// Simple check for toggling the menu.
		if (Utils.Instance.Player1.MenuStart ())
		{
			if (CurrentDefaultPanel() == startNetworkPanel.name || endGameMenu)
			{
				HidePanels();
				ShowPanel();
				menuEnabled = true;
			}
			else if (menuEnabled)
			{
				Utils.NetworkCommand(this, "CloseMenu");
			}
			else
			{
				Utils.NetworkCommand(this, "OpenMenu");
			}
		}
	}

	/// <summary>
	/// Finds the relevant menu panel according to the network state.
	/// </summary>
	/// <returns>The current default panel name.</returns>
	public string CurrentDefaultPanel()
	{
		if (endGameMenu)
		{
			if (GlobalScript.Instance.VictoryStatus == 1)
			{
				return endGamePanelWin.name;
			}
			else
			{
				return endGamePanelLose.name;
			}
		}
		if (GlobalScript.Instance.isLocal)
		{
			return localPanel.name;
		}
		else if (Utils.IsNetwork)
		{
			return defaultNetworkPanel.name;
		}
		else
			return startNetworkPanel.name;
	}

	/// <summary>
	/// Closes the menu.
	/// </summary>
	[RPC]
	private void CloseMenu()
	{
		Time.timeScale = 1;
		if(menuCamera != null && menuCamera.enabled){
			menuCamera.gameObject.SetActive(false);
			GlobalScript.Instance.Camera.Camera.SetActive (true);
			// Also first setup just in case
			/*if(Network.isClient)
				GlobalScript.Instance.SetupHarpooner ();
			else
				GlobalScript.Instance.SetupDriver();*/
		}
		HidePanels ();
		menuEnabled = false;

		UIScript.Instance.EnableAllUI ();
	}

	/// <summary>
	/// Opens the menu.
	/// </summary>
	[RPC]
	public void OpenMenu()
	{
		Time.timeScale = 0;
		if(menuCamera != null && CurrentDefaultPanel() != defaultNetworkPanel.name && !endGameMenu){
			menuCamera.gameObject.SetActive(true);
			GlobalScript.Instance.Camera.Camera.SetActive (false);
		}
		ShowPanel ();
		menuEnabled = true;

		UIScript.Instance.DisableAllUI ();
	}

	/// <summary>
	/// Indicates that the game should show only the end game menu.
	/// </summary>
	[RPC]
	public void OpenEndGameMenu()
	{
		//ScreenFader.Instance.FadedOutCompletedEvent -= OpenEndGameMenu;
		endGameMenu = true;
		OpenMenu ();
	}

	/// <summary>
	/// Hides all panels.
	/// </summary>
	public void HidePanels()
	{
		foreach (Transform t in gameObject.transform)
		{
			t.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Opens the default panel.
	/// </summary>
	public void ShowPanel()
	{
		ShowPanel (CurrentDefaultPanel());
	}

	/// <summary>
	/// Shows a specific panel.
	/// </summary>
	/// <param name="panelName"> The requested panel name.</param>
	public void ShowPanel(String panelName)
	{
		currentSelected = null;
		GameObject currentPanel = transform.FindChild (panelName).gameObject;
		currentPanel.SetActive (true);
		foreach (Transform t in currentPanel.transform)
		{
			// Activate the game panel.
			t.gameObject.SetActive(true);

			// Find any selectable object.
			if (currentSelected == null)
			{
				currentSelected = t.gameObject.GetComponent<Selectable>();
			}
		}

		lowestSelectable = currentSelected;
		highestSelectable = currentSelected;
		
		while (lowestSelectable.FindSelectableOnDown() != null)
		{
			lowestSelectable = lowestSelectable.FindSelectableOnDown();
		}
		while (highestSelectable.FindSelectableOnUp() != null)
		{
			highestSelectable = highestSelectable.FindSelectableOnUp();
		}

		currentSelected = highestSelectable;

		// This is necessary to refresh the selected game object if the same was already selected.
		if (eventSystem.currentSelectedGameObject == highestSelectable.gameObject)
		{
			eventSystem.SetSelectedGameObject (lowestSelectable.gameObject);
		}
		eventSystem.SetSelectedGameObject (highestSelectable.gameObject);
		
		// This is necessary to reopen any cute menu elements that are not option panels.
		foreach (Transform t in gameObject.transform)
		{
			if (!(t.gameObject.name.ToLower().Contains("panel")))
			{
				t.gameObject.SetActive(true);
			}
		}
	}

	/// <summary>
	/// Selects up.
	/// </summary>
	private void SelectUp()
	{
		Selectable nextSelected;
		nextSelected = currentSelected.FindSelectableOnUp();
		if (nextSelected == null)
		{
			nextSelected = lowestSelectable;
		}
		eventSystem.SetSelectedGameObject (nextSelected.gameObject);
		currentSelected = nextSelected;
	}

	/// <summary>
	/// Selects down.
	/// </summary>
	private void SelectDown()
	{
		Selectable nextSelected;
		nextSelected = currentSelected.FindSelectableOnDown();
		if (nextSelected == null)
		{
			nextSelected = highestSelectable;
		}
		eventSystem.SetSelectedGameObject (nextSelected.gameObject);
		currentSelected = nextSelected;
	}

	#region Custom Handlers
	// Every time a GameObject is selected, a method here is called with the same name as the object (not the text !)
	// These calls should not be case sensitive.

	/// <summary>
	/// Resume the game.
	/// </summary>
	public void Resume()
	{
		Utils.NetworkCommand(this, "CloseMenu");
	}

	/// <summary>
	/// Quit the game.
	/// </summary>
	public void Quit()
	{
		Application.Quit ();
	}

	/// <summary>
	/// Open the options menu.
	/// </summary>
	public void Options()
	{
		HidePanels ();
		ShowPanel("OptionsPanel");
	}

	/// <summary>
	/// Open the checkpoints menu.
	/// </summary>
	public void Checkpoints()
	{
		HidePanels ();
		ShowPanel("CheckpointsPanel");
	}

	/// <summary>
	/// Return to the default panel.
	/// </summary>
	public void Return()
	{
		HidePanels ();
		ShowPanel ();
	}

	public void TryAgain()
	{
		//Utils.NetworkCommand (this, "GotoCheckpoint", GlobalScript.CheckpointToLoad);
		Utils.NetworkCommand(this, "TryAgainAsync", true);
	}

	public IEnumerator LoadAsync(int level)
	{
		AsyncOperation async = Application.LoadLevelAsync(level);
		yield return async;
		Debug.Log("Loading complete");
	}

	/// <summary>
	/// Inverts the Y axis of the left stick.
	/// </summary>
	public void InvertYAxisLeftStick()
	{
		Utils.Instance.Player1.LeftStickInverted = !Utils.Instance.InputManager.Player1.LeftStickInverted;
	}

	/// <summary>
	/// Inverts the Y axis of the right stick.
	/// </summary>
	public void InvertYAxisRightStick()
	{
		Utils.Instance.Player1.RightStickInverted = !Utils.Instance.InputManager.Player1.RightStickInverted;
	}

	/// <summary>
	/// Asks the network manager to initialize a server.
	/// </summary>
	public void ConnectServer()
	{
		NetworkManager2.Instance.inputHandler = this;
		NetworkManager2.Instance.InitializeServer ();
		HidePanels ();
		ShowPanel ();
	}

	/// <summary>
	/// Enables a standard input manager for key strokes in the text field for the IP address.
	/// </summary>
	public void ConnectClient()
	{
		HidePanels ();
		//ShowPanel("EnterIPAddressPanel");
		NetworkManager2.Instance.inputHandler = this;
		NetworkManager2.Instance.ConnectClient ();
		ShowPanel ();
	}

	/// <summary>
	/// Asks the network manager to connect as client.
	/// </summary>
	public void AcceptConnectClient()
	{
		//NetworkManager2.Instance.ConnectClient ();
		HidePanels ();
		ShowPanel ();
	}

	/// <summary>
	/// Asks the network manager to disconnect.
	/// </summary>
	public void Disconnect()
	{
		Utils.NetworkCommand(this, "TryAgainAsync",false);
		//Application.LoadLevel (0);
	}

	/// <summary>
	/// Reload at the ch
	public void GotoCheckpoint1()
	{
		Utils.NetworkCommand (this, "GotoCheckpoint", 1);
	}

	/// <summary>
	/// Reload at the checkpoint2.
	/// </summary>
	public void GotoCheckpoint2()
	{
		Utils.NetworkCommand (this, "GotoCheckpoint", 2);
	}

	/// <summary>
	/// Reload at the checkpoint3.
	/// </summary>
	public void GotoCheckpoint3()
	{
		Utils.NetworkCommand (this, "GotoCheckpoint", 3);
	}

	/// <summary>
	/// Reload at the checkpoint4.
	/// </summary>
	public void GotoCheckpoint4()
	{
		Utils.NetworkCommand (this, "GotoCheckpoint", 4);
	}

	[RPC]
	public void GotoCheckpoint(int c)
	{
		GlobalScript.CheckpointToLoad = c;
		HidePanels();
		Utils.ReloadSceneAsync (true);
	}

	[RPC]
	public void TryAgainAsync(bool setupImmediately)
	{
		//GlobalScript.CheckpointToLoad = c;
		menuEnabled = false;
		HidePanels();
		Utils.ReloadSceneAsync (setupImmediately);
	}

	#endregion
	
	GameObject[] FindGameObjectsWithLayer (int layer){
		GameObject[] goArray = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		List<GameObject> goList = new List<GameObject>();
		for (int i = 0; i < goArray.Length; i++) {
			if (goArray[i].layer == layer) {
				goList.Add(goArray[i]);
			}
		}
		if (goList.Count == 0) {
			return null;
		}
		return goList.ToArray();
	}
}
