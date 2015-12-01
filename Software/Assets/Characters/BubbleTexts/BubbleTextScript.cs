using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BubbleTextScript : MonoBehaviour {

	private float timeBeforeFull = 0.5f;
	private float boopTime = 0.3f;
	private float timeForFade = 0.2f;
	private bool soundPlayed = false;
	public AudioSource source;

	[SerializeField]
	private Text textSprite = null;
	[SerializeField]
	private BubbleTextUtility.talkIcon owner = BubbleTextUtility.talkIcon.Driver;

	public float CurrentTime { get { return BubbleTextUtility.Instance.CurrentTime; }}
	public float MaxTime { get { return BubbleTextUtility.Instance.MaxTime; }}
	public string CurrentText { get { return BubbleTextUtility.Instance.CurrentText; }}
	public AudioClip CurrentClip{ get {return BubbleTextUtility.Instance.CurrentClip;}}
	public BubbleTextUtility.talkIcon CurrentIcon { get { return BubbleTextUtility.Instance.CurrentIcon; }}
	
	// Update is called once per frame
	void Update () {

		if (CurrentText != null && owner == CurrentIcon)
		{
			foreach (Transform t in transform)
			{
				t.gameObject.SetActive(true);
			}
			textSprite.text = CurrentText;

			if (CurrentTime <= timeBeforeFull)
			{
				transform.localScale = (new Vector3 (1f, 1f, 1f))*(Mathf.Pow(CurrentTime/timeBeforeFull, 3));
				if(!soundPlayed){
					soundPlayed = true;
					source.PlayOneShot(CurrentClip);
				}
			}
			else if (CurrentTime <= timeBeforeFull + boopTime)
			{
				if(soundPlayed){
					soundPlayed = false;
				}
				transform.localScale = (new Vector3 (1f, 1f, 1f))*(1 + Mathf.Sqrt(CurrentTime-timeBeforeFull));
			}
			else if (CurrentTime <= timeBeforeFull + boopTime*2)
			{
				transform.localScale = (new Vector3 (1f, 1f, 1f))*(1 + (boopTime/timeBeforeFull) - ((CurrentTime-boopTime)-timeBeforeFull));
			}
			
			if (CurrentTime > MaxTime-timeForFade)
			{
				transform.localScale = (new Vector3 (1f, 1f, 1f))*(1 - (CurrentTime - (MaxTime-timeForFade))/timeForFade);
			}
		}
		else 
		{
			foreach (Transform t in transform)
			{
				t.gameObject.SetActive(false);
			}
		}
	}
}
