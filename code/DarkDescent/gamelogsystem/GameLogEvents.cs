namespace DarkDescent.GameLog;

public class GameLogEvents
{
	public static class Player
	{
		public const string You = "eventlog.player.you";
	}

	public static class DamageEvents
	{
		public static class Player
		{
			public const string Strike = "You strike the {0} for {1} damage.";
		}
	}
	
	public static class Interactions
	{
		public static class Pickup
		{
			public const string Success  = "interact.pickup.success";
			public const string Drop = "interact.pickup.drop";
			public const string TooHeavy = "interact.pickup.tooheavy";
			public const string Struggle = "interact.pickup.struggle";
			public const string Throw = "interact.pickup.throw";
			public const string LoseGrip = "interact.pickup.losegrip";
			public const string StrengthFail = "interact.pickup.strengthfail";
		}
	}
}
