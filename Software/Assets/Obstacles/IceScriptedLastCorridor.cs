using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IceScriptedLastCorridor : MonoBehaviour {


	// RNG pls
	static System.Random rng = new System.Random();
	
	// Ice "walls"
	public GameObject leftObjects;
	public GameObject rightObjects;
	
	// Wall movement
	public float timeBeforeLosing;
	private float initialTime;
	private float halfDistanceBetweenWall;
	private float speedWall;
	private Vector3 LeftWall;
	private Vector3 RightWall;
	private bool wallMoving = false;
	private float leftWallMovingCounter = 0f;
	public float leftWallMovingTimerMax = 10f;
	private float rightWallMovingCounter = 0f;
	public float rightWallMovingTimerMax = 15f;
	private float firstWallCounter = 0f;
	public float firstWallTimerMax = 10f;
	private GameObject  leftWall;
	private GameObject  rightWall;
	
	// State variables
	public Collider enterZoneCollider;
	public enum CorridorState { notStarted, previous, current, finished }
	public CorridorState state;
	
	// Use this for initialization
	void Start () 
	{
		initialTime = Time.realtimeSinceStartup;
		
		leftWall = leftObjects.transform.GetChild (0).gameObject;
		rightWall = rightObjects.transform.GetChild (0).gameObject;
		halfDistanceBetweenWall = System.Math.Abs( Vector3.Distance(leftWall.transform.position, rightWall.transform.position)/2);
		speedWall = halfDistanceBetweenWall / (timeBeforeLosing) ;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.networkView.isMine)
		{
			
			// The current corridor spawns shards, moves walls and stuff
			if (state == CorridorState.current)
			{
				firstWallCounter += Time.deltaTime;
				if(firstWallCounter < firstWallTimerMax  && !wallMoving)
				{
					RightWall = new Vector3 (rightWall.transform.position.x + speedWall*Time.deltaTime, rightWall.transform.position.y, rightWall.transform.position.z);
					rightWall.transform.position = RightWall;
				}
				else
				{
					wallMoving = true;
				}

				if(wallMoving)
				{
					leftWallMovingCounter += Time.deltaTime;
					rightWallMovingCounter += Time.deltaTime;
					if (leftWallMovingCounter < leftWallMovingTimerMax)
					{
						LeftWall = new Vector3 (leftWall.transform.position.x, leftWall.transform.position.y, leftWall.transform.position.z + speedWall*Time.deltaTime);

						// move left objects to the right
						leftWall.transform.position = LeftWall;

					}
					else
					{
						// do nothing brah
					}
					if (rightWallMovingCounter < rightWallMovingTimerMax)
					{
						RightWall = new Vector3 (rightWall.transform.position.x, rightWall.transform.position.y, rightWall.transform.position.z + speedWall*Time.deltaTime);
						// move right objects to the left
						rightWall.transform.position = RightWall;
					}
					else
					{
						wallMoving = false;
						state = CorridorState.finished;
					}
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "boat")
		{
			state = CorridorState.current;

		}
	}

}

