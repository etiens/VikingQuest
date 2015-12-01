using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Ice scripted corridor.
/// </summary>
public class IceScriptedCorridor : MonoBehaviour 
{
	// RNG pls
	static System.Random rng = new System.Random();

	// Ice "walls"
	public GameObject leftObjects;
	public GameObject rightObjects;

	// Ice spawners
	public float shardsCooldown;
	private float currentCooldown;
	public GameObject shardSpawnerContainer;
	public GameObject iceShardPrefab;

	// Wall movement
	public float timeBeforeLosing;
	private float initialTime;
	private float halfDistanceBetweenWall;
	private float speedWall;
	private Vector3 LeftWall;
	private Vector3 RightWall;
	public bool wallCrush;
	public bool repeatCrush;
	private float wallCrushCounter = 0f;
	public float wallCrushTimerMax = 10f;
	private float wallDecrushCounter = 0f;
	private float wallDecrushTimerMax;
	private GameObject  leftWall;
	private GameObject  rightWall;

	// State variables
	//[HideInInspector]
	public Collider enterZoneCollider;
	public IceScriptedCorridor previousIceCorridor;
	public enum CorridorState { notStarted, previous, current, finished }
	public CorridorState state;

	// Use this for initialization
	void Start () 
	{
		initialTime = Time.realtimeSinceStartup;
		currentCooldown = shardsCooldown;
		wallDecrushTimerMax = wallCrushTimerMax;

		if(wallCrush)
		{
			leftWall = leftObjects.transform.GetChild (0).gameObject;
			rightWall = rightObjects.transform.GetChild (0).gameObject;
			halfDistanceBetweenWall = System.Math.Abs( Vector3.Distance(leftWall.transform.position, rightWall.transform.position)/2);
			speedWall = halfDistanceBetweenWall / (timeBeforeLosing) ;
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// The current corridor spawns shards, moves walls and stuff
		if (state == CorridorState.current)
		{
			/// timer stuff to know when to end, ask marc
			if(!Network.isClient)
				SpawnShards(1f);
			if(wallCrush || repeatCrush)
				UpdateWallCrush();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "boat")
		{
			state = CorridorState.current;
			if (previousIceCorridor != null)
				{
					previousIceCorridor.state = CorridorState.finished;
				}

		}
	}
	
	void UpdateWallCrush()
	{

		if (wallCrushCounter < wallCrushTimerMax && wallCrush)
		{
			wallCrushCounter += Time.deltaTime;
			LeftWall = new Vector3 (leftWall.transform.position.x + speedWall*Time.deltaTime, leftWall.transform.position.y, leftWall.transform.position.z);
			RightWall = new Vector3 (rightWall.transform.position.x - speedWall*Time.deltaTime, rightWall.transform.position.y, rightWall.transform.position.z);
			// move left objects to the right
			leftWall.transform.position = LeftWall;
			
			// move right objects to the left
			rightWall.transform.position = RightWall;

			if(wallDecrushCounter > 0)
				wallDecrushCounter = 0;
		}
		else 
		{
			wallCrush = false;
			if(wallDecrushCounter < wallDecrushTimerMax)
			{
				wallDecrushCounter += Time.deltaTime;
				LeftWall = new Vector3 (leftWall.transform.position.x - speedWall*Time.deltaTime, leftWall.transform.position.y, leftWall.transform.position.z);
				RightWall = new Vector3 (rightWall.transform.position.x + speedWall*Time.deltaTime, rightWall.transform.position.y, rightWall.transform.position.z);
				// move left objects to the right
				leftWall.transform.position = LeftWall;
				
				// move right objects to the left
				rightWall.transform.position = RightWall;
			}
			else
			{
				wallCrushCounter = 0;
				wallCrush = true;
			}
			
		}
	}

	void SpawnShards(float slowMultiplier)
	{
		currentCooldown -= Time.deltaTime;
		if (currentCooldown <= 0)
		{
			currentCooldown += shardsCooldown*slowMultiplier;
			var randomSpawner = shardSpawnerContainer.transform.GetChild(rng.Next(shardSpawnerContainer.transform.childCount));
			randomSpawner.GetComponent<ShardsSpawner>().SpawnTimeFromNow(0.3f);
		}
	}
}
