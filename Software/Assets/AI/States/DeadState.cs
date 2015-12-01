using System;
using UnityEngine;

public class DeadState:IState
{

	protected float waitTime = 5f;
	protected float waitTimer = 0f;
	protected bool waiting = true;
	
	protected bool falling = false;
	protected float fallingTimer = 0f;
	protected float fallingTime = 10f;
	
	protected Vector3 initialPosition = Vector3.zero;
	protected Vector3 destinationPosition = Vector3.zero;
	
	public DeadState (Boss boss) : base(StateIds.Dead, boss)
	{
		
	}
	
	public override int ChangeState (float deltaTime)
	{
		return StateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		//play death animation
		if(waiting)
		{
			waitTimer += deltaTime;
			if(waitTimer >= waitTime)
			{
				waiting = false;
				falling = true;
				fallingTimer = 0f;
				initialPosition  = boss.transform.position;
				destinationPosition = boss.transform.position - Vector3.up*120f;
				boss.StopHurtSound();
				boss.PlayVictorySound();
			}
		}
		else if(falling)
		{
			fallingTimer += deltaTime;
			boss.transform.position = Vector3.Lerp(initialPosition,destinationPosition,fallingTimer/fallingTime);
			
			if(fallingTimer > fallingTime)
			{
				falling = false;
				Utils.NetworkCommand(GlobalScript.Instance,"BigBigWinner");
			}
		}
	}
	
	public override void Setup ()
	{
		//start death animaiton
		waiting = true;
		waitTimer = 0f;
		boss.DeathSetup();
	}
	
	public override void SerializeNetwork (UnityEngine.BitStream stream)
	{
		
	}
}
