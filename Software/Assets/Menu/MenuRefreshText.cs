using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Simple class to refresh the text in menus.
/// </summary>
public class MenuRefreshText : MonoBehaviour {

	private string initialText;
	private Text textScript;

	void Start() 
	{
		textScript = this.GetComponent<Text> ();
		initialText = this.GetComponent<Text> ().text;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {

		string addText = "";

		// Hardcoded to get the "InvertYAxisRightStick" GameObject or anything similar.
		if (this.name.Contains("RightStick"))
		{
			if (!Utils.Instance.Player1.RightStickInverted)
			{
				addText = " : Normal";
			}
			else
			{
				addText = " : Inverted";
			}
		}

		// Hardcoded to get the "InvertYAxisLeftStick" GameObject or anything similar.
		else if (this.name.Contains("LeftStick"))
		{
			if (!Utils.Instance.Player1.LeftStickInverted)
			{
				addText = " : Normal";
			}
			else
			{
				addText = " : Inverted";
			}
		}

		// Hardcoded to get the "ConnectionStatus" GameObject or anything similar.
		else if (this.name.Contains("Status"))
		{
			if (Network.isClient)
			{
				addText = " : Connected (Client)";
			}
			else if (Network.isServer)
			{
				addText = " : Connected (Server)";
			}
			else
			{
				addText = " : Disconnected";
			}
		}

		textScript.text = initialText + addText;
	}
}
