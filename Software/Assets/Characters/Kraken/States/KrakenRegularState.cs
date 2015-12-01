using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KrakenRegularState:KrakenAttackState
{
	
	public KrakenRegularState (Boss boss) : base(boss)
	{
		waveAttackPercent = 15;
		directAttackPercent = 60;
		throwComboPercent = 25;
		standandCooldown = 5f;
	}
	
	public override void Setup ()
	{
		base.Setup ();
		//Debug.Log("Passing in Kraken Regular Setup");
	}
}
