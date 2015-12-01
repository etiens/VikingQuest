using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class UDPBroadcast : MonoBehaviour {

	UdpClient sender;
	float waitingTime = 0;
	
	int remotePort = 19784;

	int connectionPort = 25001;

	bool connectionEstablished = false;

	bool isServer = false;
	
	void Start()
	{
		sender = new UdpClient (connectionPort, AddressFamily.InterNetwork);
		sender.EnableBroadcast = true;
		IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, remotePort);
		sender.Connect (groupEP);
		SendData ();

		waitingTime = 30;
		StartReceivingIP ();
	}

	void Update(){
		if (waitingTime > 0) {
			waitingTime -= Time.deltaTime;
		} else if(!connectionEstablished){
			// Done waiting
			IPEndPoint oldConnection = new IPEndPoint (IPAddress.Any, remotePort);
			receiver.EndReceive(null, ref oldConnection);

			// Start Server !
			isServer = true;
			Network.InitializeServer(32, connectionPort, false);
			GlobalScript.Instance.InitializeComponents ();
			GlobalScript.Instance.SetupDriver ();
			StartReceivingIP();

		}
	}

	void SendData ()
	{
		string customMessage = "bob"+" * "+"169.254.235.203"+" * "+"jambon";

		sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
	}

	UdpClient receiver;
	public void StartReceivingIP ()
	{

		try {
				if (receiver == null) {
						receiver = new UdpClient (remotePort);
				receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
				}
		} catch (SocketException e) {
				Debug.Log (e.Message);
		}
				
	}
	private void ReceiveData (IAsyncResult result)
	{
		IPEndPoint receiveIPGroup = new IPEndPoint (IPAddress.Any, remotePort);
		byte[] received;
		if (receiver != null) 
		{
			received = receiver.EndReceive (result, ref receiveIPGroup);

			if(isServer){
				// 1.byte to string ?
				// 2.est-ce que cest une demande de connection
				// Si non, disregard
				// si oui : renvoyer confirmations presence de serveur
				sender = new UdpClient (6767, AddressFamily.InterNetwork);
				sender.EnableBroadcast = true;
				IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, remotePort);
				sender.Connect (receiveIPGroup);
				string customMessage = "ouais ouais";
				sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
			}
			else{
				// From here, we have the server info and we simply connect to it
				
				/***** CONNECTION TO SERVER ********/
				Network.Connect(receiveIPGroup.Address.ToString(), connectionPort);
			}
		} 
		else 
		{
			return;
		}


	}


}
