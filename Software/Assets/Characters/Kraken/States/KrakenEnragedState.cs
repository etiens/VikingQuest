using System;

public class KrakenEnragedState:KrakenAttackState
{
	
	public KrakenEnragedState (Boss boss) : base(boss)
	{
		waveAttackPercent = 30;
		directAttackPercent = 35;
		throwComboPercent = 35;
		standandCooldown = 2.5f;
	}
	
	public override void Setup ()
	{
		base.Setup();
		foreach(Tentacle t in kraken.LiveTentacles)
		{
			t.MovePosition(kraken.TentacleAnglePosition(t.AngleRelativeToKraken,kraken.CurrentTentacleRadius),3f);
		}
	}
}
