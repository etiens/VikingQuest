using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogResources : MonoBehaviour{
	
	public static Dictionary<string, string> dialogues = new Dictionary<string, string>();
	public static List<string> tauntsFromHarpooner = new List<string>();
	public static List<string> tauntsFromDriver = new List<string>();
	public static List<AudioClip> staticClipsFromHarpooner = new List<AudioClip>();
	public static List<AudioClip> staticClipsFromDriver = new List<AudioClip>();
	public List<AudioClip> clipsFromHarpooner = new List<AudioClip>();
	public List<AudioClip> clipsFromDriver = new List<AudioClip>();
	
	private float currentBubbleCooldownTime = 0f;
	private float bubbleTextCooldown = 8f;
	
	void Start()
	{
		if(dialogues.Count < 1)
		{
			dialogues.Add("introText", "The way inside\n is straight ahead!");
			dialogues.Add("barrierText", "We have to \n destroy that barrier \n to go forward!");
			dialogues.Add("icebergText", "Use the front \n station to destroy \n icebergs!");
			dialogues.Add("fallingText", "Watch out!! \n Icebergs falling \n from the sky!");
			dialogues.Add("wallsText", "Quickly!! \n The walls are closing in!");
			dialogues.Add("tentacleText", "A monstrous tentacle!! \n Use the side stations \n to kill it!");
			dialogues.Add("krakenText", "The legendary Kraken!! \n Aim for the eyes!");
		}
		if(tauntsFromHarpooner.Count < 1)
		{
			tauntsFromHarpooner.Add("Try to go AROUND the icebergs this time !");
			tauntsFromHarpooner.Add("Go faster, this is too slow !");
		}
		if(tauntsFromDriver.Count < 1)
		{
			tauntsFromDriver.Add("Nice shot ! If you were aiming at nothing that is...");
			tauntsFromDriver.Add("What was that supposed to hit again ?");
		}
		foreach (AudioClip clip in clipsFromHarpooner){
			staticClipsFromHarpooner.Add(clip);
		}
		foreach (AudioClip clip in clipsFromDriver){
			staticClipsFromDriver.Add(clip);
		}
	}
	
	void Update()
	{
		currentBubbleCooldownTime += Time.deltaTime;
		if (Utils.Instance.Player1.YDown() && currentBubbleCooldownTime > bubbleTextCooldown)
		{
			currentBubbleCooldownTime = 0.0f;
			if (GlobalScript.Instance.Harpooner.HasControl)
			{
				var tauntNumber = Random.Range(0, tauntsFromHarpooner.Count);
				BubbleTextUtility.Instance.CreateBubbleTextNetwork(tauntsFromHarpooner[tauntNumber], BubbleTextUtility.talkIcon.Harpooner, tauntNumber);
			}
			else 
			{
				var tauntNumber = Random.Range(0, tauntsFromDriver.Count);
				BubbleTextUtility.Instance.CreateBubbleTextNetwork(tauntsFromDriver[tauntNumber], BubbleTextUtility.talkIcon.Driver, tauntNumber);
			}
		}
	}
}

