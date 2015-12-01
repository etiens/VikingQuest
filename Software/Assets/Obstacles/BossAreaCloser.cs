using UnityEngine;
using System.Collections;

public class BossAreaCloser : MonoBehaviour
{
		[SerializeField] float closeSequenceTime = 5f;
		float closeSequenceTimer = 0f;
		bool sequenceStarted = false;
		[SerializeField] float submergeHeight = 80f;
		[SerializeField] BoxCollider detectionArea = null;
		Vector3 submergedPosition = Vector3.zero;
		Vector3 initialPosition = Vector3.zero;
		[SerializeField] GameObject ClosingWall = null;
		BoxCollider box = null;
		public Camera cinematicCamera;
		private bool cinematicStarted;
		private Animator cinematicAnimator;
		public AudioClip musicBossClip;
		private AudioSource musicBossSource;
		
		void Awake()
		{
			if(detectionArea == null)
				detectionArea = GetComponent<BoxCollider>() as BoxCollider;
			initialPosition = ClosingWall.transform.position;
			submergedPosition = initialPosition - Vector3.up*submergeHeight;
			ClosingWall.transform.position = submergedPosition;
			box = ClosingWall.GetComponentInChildren<BoxCollider>();
			if (box != null){
				box.enabled = false;
			}
			
			InitSounds ();
			
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(sequenceStarted)
			{
				closeSequenceTimer += Time.deltaTime;
				ClosingWall.transform.position = Vector3.Lerp (submergedPosition,initialPosition,closeSequenceTimer/closeSequenceTime);
				if(closeSequenceTimer >= closeSequenceTime)
					sequenceStarted = false;
			}
			
			if(cinematicStarted)
			{
				AnimatorStateInfo currentState = cinematicAnimator.GetCurrentAnimatorStateInfo(0);
				float playbackTime = currentState.normalizedTime % 1;
				if(playbackTime > 0.95)
				{
					GlobalScript.Instance.Camera.Camera.SetActive(true);
					GlobalScript.Instance.changeMusicDelayed(playbackTime * currentState.length);
					cinematicCamera.gameObject.SetActive(false);
					musicBossSource.Stop ();
					GlobalScript.Instance.Harpooner.GetCurrentStation().showTrajectoryLine();
					GlobalScript.Instance.Driver.StopControls = false;
					GlobalScript.Instance.Harpooner.StopControls = false;
				}
			}
		}
		
		void OnTriggerEnter(Collider collider)
		{
			if(collider.gameObject.tag == "boat")
			{
				if (box != null){
					box.enabled = true;
			}
				sequenceStarted = true;
				
				detectionArea.enabled = false;
				closeSequenceTimer = 0f;
				
				EngageBoss();
				
				if(cinematicCamera != null){
					cinematicStarted = true;
					GlobalScript.Instance.Camera.Camera.SetActive(false);
					cinematicCamera.gameObject.SetActive(true);
					musicBossSource.Play ();
					cinematicAnimator = cinematicCamera.GetComponent<Animator>();
					GlobalScript.Instance.Driver.StopControls = true;
					GlobalScript.Instance.Harpooner.StopControls = true;
					GlobalScript.Instance.Harpooner.GetCurrentStation().hideTrajectoryLine();
				}
			}
		}
	
	private void InitSounds(){
		musicBossSource = cinematicCamera.gameObject.AddComponent<AudioSource>();
		musicBossSource.clip = musicBossClip;
		musicBossSource.loop = true;
		musicBossSource.volume = 0.12f;
		musicBossSource.playOnAwake = false;
	}

		void EngageBoss()
		{
			Boss boss = FindObjectOfType<Boss>() as Boss;
			if(boss != null)
				boss.Engaged = true;
			else
				Debug.LogError("Couldn't find Boss in scene to engage");
		}
}

