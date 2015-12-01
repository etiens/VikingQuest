using UnityEngine;
using System.Collections;


// reference : http://www.paladinstudios.com/2013/07/10/how-to-create-an-online-multiplayer-game-with-unity/
public class NetworkManager : Singleton<NetworkManager> {

/*	private const string typeName = "Jambon";
	private const string gameName = "HarpoonSimulator";
	private HostData[] hostList;
	public GameObject playerPrefab; */

	public string connectionIP = "127.0.0.1";
	public int connectionPort = 25001; //25001 is the port that Unity uses.


/*	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		MasterServer.ipAddress = "127.0.0.1";
	}*/

	/*void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}*/

	/*void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUI.Label(new Rect(10, 10, 200, 20), "Status: Disconnected");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Client Connect"))
			{
				ConnectClient();
			}
			if (GUI.Button(new Rect(10, 50, 120, 20), "Initialize Server"))
			{
				InitializeServer();
			}
		}
		else if (Network.peerType == NetworkPeerType.Client)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Client");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
			{
				Disconnect();
			}
		}
		else if (Network.peerType == NetworkPeerType.Server)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Server");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
			{
				Disconnect();
			}
		}
	}*/

	public void InitializeServer()
	{
		Network.InitializeServer(32, connectionPort, false);
	}

	public void ConnectClient()
	{
		Network.Connect(connectionIP, connectionPort);
	}

	public void Disconnect()
	{
		Network.Disconnect(200);
	}

	/*
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	} */

	void OnServerInitialized()
	{
		//SpawnPlayer();
		GlobalScript.Instance.InitializeComponents ();
		GlobalScript.Instance.SetupDriver ();
	}
	
	void OnConnectedToServer()
	{
		GlobalScript.Instance.AttachComponents ();
		GlobalScript.Instance.SetupHarpooner ();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Disconnected from server: " + info);
	}
	
	private void SpawnPlayer()
	{
		//Network.Instantiate(playerPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity, 0);
	}
}
