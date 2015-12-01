using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class NetworkManager2 : Singleton<NetworkManager2> {
	
	//public string connectionIP = "127.0.0.1";
	public int connectionPort = 25001; //25001 is the port that Unity uses.
	public int communicationPort = 25002;
	public bool waitingForClient = false;
	bool isServer = false;

	bool serverFound = false;
	IPAddress serverAddress = null;
	bool connectionEstablished = false;

	UdpClient sender;
	UdpClient receiver;
	
	public MenuInputHandler inputHandler;

	void Update(){
		if (serverFound && !connectionEstablished) {
			Network.Connect(serverAddress.ToString(), connectionPort);
			connectionEstablished = true;
		}
	}
	
	public void InitializeServer()
	{
		Debug.Log("In InitializeServer");
		Network.InitializeServer(32, connectionPort, false);
		isServer = true;
		
		try {
			if (receiver == null) {
				receiver = new UdpClient (communicationPort);
				receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			}
			if(sender == null) {
				sender = new UdpClient (communicationPort+1, AddressFamily.InterNetwork);
				sender.EnableBroadcast = true;
				Debug.Log("InitializeServer Success");
			}
		} catch (SocketException e) {
			Debug.LogError(string.Format("InitializeServer Fail: {0}",e.Message));
		}
		Debug.Log("Out InitializeServer");
	}
	
	public void ConnectClient()
	{
		//Network.Connect(connectionIP, connectionPort);
		isServer = false;
		try {
			if (receiver == null) {
				receiver = new UdpClient (communicationPort);
				receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			}
			if(sender == null) {
				sender = new UdpClient (communicationPort+1, AddressFamily.InterNetwork);
				sender.EnableBroadcast = true;
      		}
			RequestHost();
		} catch (SocketException e) {
			Debug.Log (e.Message);
		}
	}
	
	public void Disconnect()
	{
		Network.Disconnect(200);
	}

	private void ReceiveData (IAsyncResult result)
	{
		Debug.Log("In ReceiveData");
		IPEndPoint receiveIPGroup = new IPEndPoint (IPAddress.Any, communicationPort);
		byte[] received;
		if (receiver != null) 
		{
			received = receiver.EndReceive (result, ref receiveIPGroup);
			string receivedString = System.Text.Encoding.UTF8.GetString(received);

			string receivedIp = receiveIPGroup.Address.ToString();
			string localIp = getLocalIPAddress();
			Debug.Log(string.Format("Receiving data from {0}",receivedIp));
			if(isServer){
				Debug.Log("In ReceiveData as Server");
				IPEndPoint groupEP = new IPEndPoint (receiveIPGroup.Address, communicationPort);
				sender.EnableBroadcast = false;
				sender.Connect (groupEP);
				receiveIPGroup.Port = communicationPort;
				sender.Connect (receiveIPGroup);
				string customMessage = "IAmLegend";
				Debug.Log(string.Format("Sent out message to client {0}",receivedIp));
				sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
			}
			else if (receivedString == "IAmLegend"){
				// From here, we have the server info and we simply connect to it
				
				/***** CONNECTION TO SERVER ********/
				//Network.Connect(receiveIPGroup.Address.ToString(), connectionPort);
				serverAddress = receiveIPGroup.Address;
				serverFound = true;
				Debug.Log("Server Found");
			}
			else{
				receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			}
		} 
		else 
		{
			Debug.Log("Receiver is Null");
			return;
		}
	}

	string getLocalIPAddress()
	{
		IPHostEntry host;
		string localIP = "";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				localIP = ip.ToString();
				break;
			}
		}
		return localIP;
	}

	bool isAddressLocal(string address){
		IPHostEntry host;
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				if(address == ip.ToString())
					return true;
			}
		}
		return false;
	}

	void RequestHost ()
	{
		Debug.Log("In RequestHost");
		string customMessage = "bob"+" * "+"169.254.235.203"+" * "+"jambon";
		IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, communicationPort);
		sender.EnableBroadcast = true;
		sender.Connect (groupEP);
    	sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
		Debug.Log("Out RequestHost");
	}
	
	void OnServerInitialized()
	{
		//SpawnPlayer();
		GlobalScript.Instance.InitializeComponents ();
		GlobalScript.Instance.SetupDriver ();
	}
	
	void OnConnectedToServer()
	{
		inputHandler.AcceptConnectClient();
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
