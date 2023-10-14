using DarkDescent.Components;
using DarkDescent.GameLog.UI;
using Sandbox;

namespace DarkDescent.GameLog;

public static partial class GameLogSystem
{
		
	public static void PlayerPickupObject( PickupTargetComponent target, bool struggle )
	{
		var actionString = struggle
			? Language.GetPhrase( GameLogEvents.Interactions.Pickup.Struggle )
			: Language.GetPhrase( GameLogEvents.Interactions.Pickup.Success );

		var name = target.Name;

		if ( target.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );

		GameLogPanel.AddEntry( actionString );
	}

	public static void PlayerDropObject( PickupTargetComponent target )
	{
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.Drop );

		var name = target.Name;

		if ( target.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}
	
	public static void PlayerThrowObject( PickupTargetComponent target )
	{
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.Throw );

		var name = target.Name;

		if ( target.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}

	public static void PlayerFailToPickup( PickupTargetComponent target )
	{
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.TooHeavy );

		var name = target.Name;

		if ( target.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}

	[ClientRpc]
	public static void PlayerStrengthFailPickup( Entity entity )
	{
		if ( !entity.Components.TryGet<PickupTargetComponent>( out var target ) )
			return;
		
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.StrengthFail );

		var name = target.Name;

		if ( target.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}
	
	[ClientRpc]
	public static void PlayerLoseGrip( Entity entity )
	{
		if ( !entity.Components.TryGet<PickupTargetComponent>( out var target ) )
			return;
		
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.LoseGrip );

		var name = target.Name;

		if ( target.Entity.Components.TryGet<ObjectClassifierComponent>( out var classifier ) )
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}
}
