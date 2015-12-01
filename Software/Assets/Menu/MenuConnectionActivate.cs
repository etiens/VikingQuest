using UnityEngine;
using System.Collections;

public class MenuConnectionActivate : MonoBehaviour {

	[SerializeField]
	private bool activatedIfConnected = true;

	void OnEnable()
	{
		RefreshActiveState ();
	}

	void OnServerInitialized()
	{
		RefreshActiveState ();
	}

	void OnConnectedToServer()
	{
		RefreshActiveState ();
	}

	private void RefreshActiveState()
	{
		if (Network.isClient || Network.isServer)
		{
			if (!activatedIfConnected)
			{
				gameObject.SetActive(false);
			}
		}
		else if (activatedIfConnected)
		{
			gameObject.SetActive(false);
		}
	}
}
