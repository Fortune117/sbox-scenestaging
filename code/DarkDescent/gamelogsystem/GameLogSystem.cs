namespace DarkDescent.GameLog;

public static partial class GameLogSystem
{
	public static void PlayerPerformAction(Player player, string action)
	{
		Log.Info( $"{player} {action}" );
	}
}
