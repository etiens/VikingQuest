public class StateIds
{
	public const int Waiting = 0;		//Kraken is waiting for player to enter zone
	public const int Idle = 1;			//Kraken on cooldown
	public const int Stunned = 2;		//Kraken is stunned after X amount of impact blows
	public const int Hurt = 3;			//Kraken just lost a tentacle or lost an eye
	public const int Regular = 4;		//Kraken is in initial phase
	public const int Enraged = 5;		//Kraken lost both eyes, wiggling non stop
	public const int Dead = 6;			//Kraken kun is ded
	public const int Attacking = 7;		//Kraken is attacking encapsulates Regular and Enraged
	
	public static string Name(int id)
	{
		string name = string.Empty;
		switch(id)
		{
		case Waiting:
			name = "Waiting";
			break;
		
		case Idle:
			name = "Idle";
			break;
			
		case Stunned:
			name = "Stunned";
			break;
			
		case Hurt:
			name = "Hurt";
			break;
		
		case Attacking:
			name = "Attacking";
			break;
			
		case Regular:
			name = "Regular";
			break;
			
		case Enraged:
			name = "Enraged";
			break;
			
		case Dead:
			name = "Dead";
			break;
		}
		return name;
	}
}
