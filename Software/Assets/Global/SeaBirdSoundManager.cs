using UnityEngine;
using System.Collections;

public class SeaBirdSoundManager : MonoBehaviour {

	public AudioClip seabirdSound1;

	private AudioSource audioSource;

	public Vector2 seabirdSound1TimeRange;
	private float seabirdSound1Timer;


	// Use this for initialization
	void Awake () {
		this.audioSource = GetComponent<AudioSource>();
		seabirdSound1Timer = Random.Range(seabirdSound1TimeRange.x, seabirdSound1TimeRange.y);
	}
	
	// Update is called once per frame
	void Update () {
		seabirdSound1Timer -= Time.deltaTime;
		if(seabirdSound1Timer <= 0){
			this.audioSource.PlayOneShot(seabirdSound1);
			seabirdSound1Timer = Random.Range(seabirdSound1TimeRange.x, seabirdSound1TimeRange.y);
			seabirdSound1Timer += seabirdSound1.length;
		}
	}

	public enum SeabirdSound{
		SeabirdSound1
	}
}
