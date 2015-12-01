using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IPInputFieldListener : MonoBehaviour
{
	[SerializeField]
	private MenuInputHandler menuInputHandler = null;

	private bool firstFrameProcessingInput;

	private InputField inputField;

	void OnEnable()
	{
		firstFrameProcessingInput = true;
	}

	void Start ()
	{
		inputField = gameObject.GetComponent<InputField> ();
		inputField.text = "";
	}
	
	void Update () 
	{

		// Modified code from http://answers.unity3d.com/questions/649973/inputinputstring-confused-as-to-how-code-is-proces.html

		// Ignores the first frame where the text box appeared with a '\n'
		if (firstFrameProcessingInput) {
			firstFrameProcessingInput = false;
			return;
		}
			
		foreach (char c in Input.inputString) 
		{
			if (c == '\b') 
			{
				if (inputField.text.Length != 0) 
				{
					inputField.text = inputField.textComponent.text.Substring(0, inputField.text.Length - 1);
				}
			}
			else 
			{
				if (c == '\n' || c == '\r') 
				{
					NetworkManager.Instance.connectionIP = inputField.textComponent.text;
					menuInputHandler.AcceptConnectClient();
				}
				else
				{
					inputField.text += c;
				}
			}
		}
		inputField.textComponent.text = inputField.text;
		inputField.MoveTextEnd (false);
	}
}

