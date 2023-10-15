using DarkDescent.Actor;

namespace DarkDescent.GameLog;

public static partial class GameLogSystem
{
	public static void PlayerPerformAction(ActorComponent player, string action)
	{
		Log.Info( $"{player} {action}" );
	}
}
