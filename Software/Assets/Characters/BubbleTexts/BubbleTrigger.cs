using UnityEngine;
using System.Collections;
using System.Reflection;

public class BubbleTrigger : MonoBehaviour
{
	public string dialogueTitle;
	public BubbleTextUtility.talkIcon icon;
	public AudioClip clip;
	private bool bubblePlayed = false;
	
	void OnTriggerEnter(Collider col){
	
		if(col.gameObject.tag == "boat" && !bubblePlayed){
        	BubbleTextUtility.Instance.CreateBubbleText(DialogResources.dialogues[dialogueTitle],icon,clip);
			bubblePlayed = true;
		}
	}
}

