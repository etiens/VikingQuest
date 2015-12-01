using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KrakenAttackState:IState
{
	protected float attackingTimer = 0f;
	protected bool Attacking {get{return attackingTimer > 0f;}}
	protected System.Random random;
	protected Kraken kraken;
	
	protected int waveAttackPercent = 25;
	protected int directAttackPercent = 50;
	protected int throwComboPercent = 25;
	protected float standandCooldown = 5f;
	
	public KrakenAttackState (Boss boss) : base(StateIds.Attacking, boss)
	{
		NextStateIds.Add(StateIds.Idle);
		NextStateIds.Add(StateIds.Stunned);
		NextStateIds.Add (StateIds.Hurt);
		random = new System.Random();
		kraken = boss as Kraken;
	}
	
	public override int ChangeState (float deltaTime)
	{
		if(boss.Hurt)
			return StateIds.Hurt;
		
		if(boss.StunHP <= 0)
			return StateIds.Stunned;
		
		if(Attacking)
			return StateId;
		
		if(boss.Recovering)
			return StateIds.Idle;
		
		return StateId;
	}
	
	public override void ProcessState (float deltaTime)
	{
		if (Attacking)
			attackingTimer -= deltaTime;
		else {
			int rand = random.Next(100);
			if(rand < waveAttackPercent){
				WaveAttack(standandCooldown);
				kraken.PlayAngrySound();
			}
			else if(rand < waveAttackPercent + directAttackPercent){
				DirectAttack(standandCooldown);
				kraken.PlayAngrySound();
			}
			else if(rand < waveAttackPercent + directAttackPercent + throwComboPercent){
				ThrowIceBergCombo(standandCooldown);
				kraken.PlayAngrySound();
			}
			
		}
	}
	
	public override void Setup ()
	{
		attackingTimer = 0f;
		kraken.StopAnimations();
		//Debug.Log("Passing in Attacking Setup");
	}
	
	protected void DirectAttack(float cooldown)
	{
		attackingTimer = boss.Attack1();
		boss.SignalRecovering(cooldown);
	}
	
	protected void ThrowIceBergCombo(float cooldown)
	{
		attackingTimer = boss.Attack5();
		boss.SignalRecovering(cooldown);
	}
	
	protected void WaveAttack(float cooldown)
	{
		
		if(random.Next()%2 == 0)
			attackingTimer = boss.Attack3();
		else
			attackingTimer = boss.Attack4();
		
		boss.SignalRecovering(cooldown);
	}
	
	public override void SerializeNetwork (UnityEngine.BitStream stream)
	{
		float _attackingTimer = attackingTimer;
		if(stream.isWriting)
		{
			stream.Serialize(ref _attackingTimer);
		}
		else{
			stream.Serialize(ref _attackingTimer);
			attackingTimer = _attackingTimer;
		}
	}
}

