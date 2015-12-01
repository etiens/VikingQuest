using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BubbleTextUtility : Singleton<BubbleTextUtility> {
	
	private List<string> bubbleListText = new List<string> ();
	public string CurrentText { get { if (bubbleListText.Count == 0) return null; else return bubbleListText[0]; } }

	private List<float> bubbleListTime = new List<float> ();
	public float MaxTime { get { if (bubbleListTime.Count == 0) return 0f; else return bubbleListTime[0]; } }

	private float currentTime = 0f;
	public float CurrentTime { get { return currentTime; } }
	
	private List<AudioClip> bubbleListClip = new List<AudioClip> ();
	public AudioClip CurrentClip { get { if (bubbleListClip.Count == 0) return null; else return bubbleListClip[0]; } }

	public enum talkIcon { Driver = 0, Harpooner = 1};
	private List<talkIcon> bubbleListIcon = new List<talkIcon>();
	public talkIcon CurrentIcon { get { if (bubbleListIcon.Count == 0) return talkIcon.Driver; else return bubbleListIcon[0]; } }
	
	public DialogResources dialogResources;

	void Update()
	{
		if (CurrentText != null)
		{
			currentTime += Time.deltaTime;

			if (currentTime > MaxTime)
			{
				bubbleListText.RemoveAt(0);
				bubbleListTime.RemoveAt(0);
				bubbleListIcon.RemoveAt(0);
				bubbleListClip.RemoveAt(0);
				currentTime = 0f;
			}
		}
	}

	/// <summary>
	/// Adds a chat bubble text at the end of the chat list for this player only.
	/// </summary>
	/// <param name="text">The text to show.</param>
	/// <param name="icon">The icon to show.</param>
	/// <param name="showTime">Time to show the text. It's recommended to leave it at 0 for an automatic calcul.</param>
	public void CreateBubbleText(string text, talkIcon icon, AudioClip clip){CreateBubbleText(text, icon, 0f, clip);}
	public void CreateBubbleText(string text, talkIcon icon, float showTime, AudioClip clip)
	{
		LocalCreateBubbleText(text, (int)icon, showTime, clip);
	}

	/// <summary>
	/// Adds a chat bubble text at the end of the chat list for both players.
	/// </summary>
	/// <param name="text">The text to show.</param>
	/// <param name="icon">The icon to show.</param>
	/// <param name="showTime">Time to show the text. It's recommended to leave it at 0 for an automatic calcul.</param>
	public void CreateBubbleTextNetwork(string text, talkIcon icon, int clipIndex){CreateBubbleTextNetwork(text, icon, 0f, clipIndex);}
	public void CreateBubbleTextNetwork(string text, talkIcon icon, float showTime, int clipIndex)
	{
		Utils.NetworkCommand (this, "RPCCreateBubbleText", RPCMode.AllBuffered, text, (int)icon, showTime, clipIndex);
	}

	// Gimmick because enum can't be sent through rpc (unity pls)
	[RPC]
	public void RPCCreateBubbleText(string text, int icon, float showTime, int clipIndex)
	{
		talkIcon actualIcon = (talkIcon)icon;
		
		if (showTime <= 0f)
		{
			showTime = text.Length/10 + 3;
		}
		
		bubbleListText.Add (text);
		bubbleListIcon.Add (actualIcon);
		bubbleListTime.Add (showTime);
		if(actualIcon == talkIcon.Driver)
			bubbleListClip.Add (dialogResources.clipsFromDriver[clipIndex]);
		else //if(actualIcon == talkIcon.Harpooner)
			bubbleListClip.Add (dialogResources.clipsFromHarpooner[clipIndex]);
	}
	
	public void LocalCreateBubbleText(string text, int icon, float showTime, AudioClip clip)
	{
		talkIcon actualIcon = (talkIcon)icon;
		
		if (showTime <= 0f)
		{
			showTime = text.Length/10 + 3;
		}
		
		bubbleListText.Add (text);
		bubbleListIcon.Add (actualIcon);
		bubbleListTime.Add (showTime);
		bubbleListClip.Add (clip);
	}

	/// <summary>
	/// Adds a chat bubble text at the end of the chat list for this player only.
	/// </summary>
	/// <param name="text">The text to show.</param>
	/// <param name="icon">The icon to show.</param>
	/// <param name="showTime">Time to show the text. It's recommended to leave it at 0 for an automatic calcul.</param>
	public void CreateBubbleTextAsap(string text, talkIcon icon, AudioClip clip){CreateBubbleTextAsap(text, icon, 0f, clip);}
	public void CreateBubbleTextAsap(string text, talkIcon icon, float showTime, AudioClip clip)
	{
		RPCCreateBubbleTextAsap(text, (int)icon, showTime, clip);
	}

	/// <summary>
	/// Adds a chat bubble text at the front of the chat list for both players.
	/// </summary>
	/// <param name="text">The text to show.</param>
	/// <param name="icon">The icon to show.</param>
	/// <param name="showTime">Time to show the text. It's recommended to leave it at 0 for an automatic calcul.</param>
	public void CreateBubbleTextNetworkAsap(string text, talkIcon icon, AudioClip clip){CreateBubbleTextNetworkAsap(text, icon, 0f, clip);}
	public void CreateBubbleTextNetworkAsap(string text, talkIcon icon, float showTime, AudioClip clip)
	{
		Utils.NetworkCommand (this, "RPCCreateBubbleTextAsap", RPCMode.AllBuffered, text, (int)icon, showTime, clip);
	}

	// Gimmick because enum can't be sent through rpc (unity pls)
	[RPC]
	public void RPCCreateBubbleTextAsap(string text, int icon, float showTime, AudioClip clip)
	{
		talkIcon actualIcon = (talkIcon)icon;
		
		if (showTime <= 0f)
		{
			showTime = text.Length/10 + 3;
		}
		
		bubbleListText.Insert (0, text);
		bubbleListIcon.Insert (0, actualIcon);
		bubbleListTime.Insert (0, showTime);
		bubbleListClip.Insert (0, clip);
	}
	
}
