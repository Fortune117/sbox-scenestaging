using DarkDescent.Actor.Damage;
using DarkDescent.Components;
using DarkDescent.GameLog.UI;
using Sandbox;

namespace DarkDescent.GameLog;

public partial class GameLogSystem
{
	[ClientRpc]
	public static void DamageEventReceived( DamageEventData damageEventData)
	{
		/*var localPlayer = DarkDescentGameManager.LocalPlayer;

		if ( localPlayer == damageEventData.OriginatorActor.Entity && damageEventData.TargetActor is not null)
		{
			var name = damageEventData.TargetActor.Entity.Name;

			if ( damageEventData.TargetActor.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
			{
				Log.Warning( "No object classifier found during damage event." );
				name = classifier.DisplayName;
			}
			
			var actionString = Language.GetPhrase( GameLogEvents.DamageEvents.Player.Strike );

			var damageString = $"{damageEventData.DamageResult:#0.}";
			
			actionString = string.Format( actionString, classifier.DisplayName, damageString );
		
			GameLogPanel.AddEntry( actionString );
		}*/
	}
}
