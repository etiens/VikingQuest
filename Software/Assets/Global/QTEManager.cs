using UnityEngine;
using System.Collections;

public class QTEManager : MonoBehaviour{
	private bool inQTEMode = false;
	public bool InQTEMode {get{return inQTEMode;}}
	private VInput.Button qteButton = VInput.Button.A;
	public VInput.Button QTEButton {get{return qteButton;}}
	
	// QTE mode 2
	private float qteProgress = 0.5f;
	public float QTEProgress { get { return qteProgress; } }
	private float qteSpeed = 0.1f;
	private float qteHitImpulse = 0.1f;
	private float qteDifficultyProgression = 0.033f;
	
	public Texture2D bar;
	public Texture2D anchor;
	public Texture2D sink;
	public Texture2D recover;
	public Rect anchorPos;
	public Rect barPos;
	
	public void Start(){
		qteSpeed = 0.1f - qteDifficultyProgression;
	}

	public void EngageQTE(VInput.Button button, float maxTime, int requiredHit){
		inQTEMode = true;
		GlobalScript.Instance.Harpooner.GetCurrentStation().hideTrajectoryLine();
		qteProgress = 0.5f;
		qteSpeed += qteDifficultyProgression;
		
		GlobalScript.Instance.Driver.CurrentPlayBackLoop++;
		GlobalScript.Instance.Harpooner.CurrentPlayBackLoop++;
		GlobalScript.Instance.Driver.EngageQTEAnimation();
		GlobalScript.Instance.Harpooner.EngageQTEAnimation();

		// Make sure the UI is disabled in the QTE
		UIScript.Instance.DisableForQTE ();
	}

	public void TriggerQTE(){
		Utils.NetworkCommand(this, "ProcessQTE");
	}

	[RPC]
	private void ProcessQTE(){
	
		if(!inQTEMode) return;
		// Make sure the UI is disabled in the QTE
		UIScript.Instance.DisableForQTE ();

		if (!Network.isClient) {
			qteProgress += qteHitImpulse;
			if (qteProgress >= 1){
				// QTE was successful
				inQTEMode = false;
				Utils.NetworkCommand(this,"TurnOffQTE",new object[]{true});
			}
		}
	}

	public void Update(){
		if (!Network.isClient && inQTEMode) {
			/*qteTime += Time.deltaTime;

			if(qteTime >= qteMaxTime){
				// QTE was not successful
				Utils.NetworkCommand(this,"TurnOffQTE",new object[]{false});
			}*/
			qteProgress -= qteSpeed * Time.deltaTime;
			
			if(qteProgress <= 0){
				// QTE was not successful
				inQTEMode = false;
				Utils.NetworkCommand(this,"TurnOffQTE",new object[]{false});
			}
		}
	}
	
	public void OnGUI(){
		// This has been replaced in the HUD/UIScript and by objects UI Container in the editor.
		/*
		if(inQTEMode)
		{
			Rect barRealPos = new Rect(Screen.width/2 - barPos.width/2, 20, barPos.width, barPos.height);
			GUI.DrawTexture(barRealPos, bar);
			Rect anchorRealPos = new Rect(barRealPos.x + qteProgress * barRealPos.width- anchorPos.width/2, anchorPos.y, anchorPos.width, anchorPos.height);
			GUI.DrawTexture(anchorRealPos, anchor);
		}
		*/
	}

	public delegate void QTECompletedHandler(bool success);
	private QTECompletedHandler m_QTECompleted;
	public event QTECompletedHandler QTECompletedEvent{
		add
		{
			m_QTECompleted += value;
		}

		remove
		{
			m_QTECompleted -= value;
		}
	}
	
	[RPC]
	private void TurnOffQTE(bool success)
	{
		inQTEMode = false;
		OnQTECompleted(success);
	}

	public void OnQTECompleted(bool success)
	{
		
		if (m_QTECompleted != null) {
			m_QTECompleted(success);
		}
	}

	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		bool qteMode = false;
		float progress = 0f;
		if (stream.isWriting) {
			qteMode = inQTEMode;
			progress = qteProgress;
			stream.Serialize(ref qteMode);
			stream.Serialize(ref progress);
		} else {
			stream.Serialize(ref qteMode);
			stream.Serialize(ref progress);
			inQTEMode = qteMode;
			qteProgress = progress;
		}	
	}
}
