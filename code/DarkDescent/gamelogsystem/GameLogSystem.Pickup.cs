using DarkDescent.Components;
using DarkDescent.GameLog.UI;
using Sandbox;

namespace DarkDescent.GameLog;

public static partial class GameLogSystem
{
		
	public static void PlayerPickupObject( PhysicsInteractableComponent target, bool struggle )
	{
		var actionString = struggle
			? Language.GetPhrase( GameLogEvents.Interactions.Pickup.Struggle )
			: Language.GetPhrase( GameLogEvents.Interactions.Pickup.Success );

		var name = target.GameObject.Name;

		var classifier = target.GetComponent<ObjectClassifierComponent>();
		if ( classifier is not null)
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );

		GameLogPanel.AddEntry( actionString );
	}

	public static void PlayerDropObject( PhysicsInteractableComponent target )
	{
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.Drop );

		var name = target.GameObject.Name;

		var classifier = target.GetComponent<ObjectClassifierComponent>();
		if ( classifier is not null)
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}
	
	public static void PlayerThrowObject( PhysicsInteractableComponent target )
	{
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.Throw );

		var name = target.GameObject.Name;

		var classifier = target.GetComponent<ObjectClassifierComponent>();
		if ( classifier is not null)
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}

	public static void PlayerFailToPickup( PhysicsInteractableComponent target )
	{
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.TooHeavy );

		var name = target.GameObject.Name;

		var classifier = target.GetComponent<ObjectClassifierComponent>();
		if ( classifier is not null)
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}


	public static void PlayerStrengthFailPickup( GameObject gameObject )
	{
		var target = gameObject.GetComponent<PhysicsInteractableComponent>();
		if ( target is null )
			return;
		
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.StrengthFail );

		var name = target.GameObject.Name;

		var classifier = gameObject.GetComponent<ObjectClassifierComponent>();
		if ( classifier is not null)
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}
	

	public static void PlayerLoseGrip( GameObject gameObject )
	{
		var target = gameObject.GetComponent<PhysicsInteractableComponent>();
		if ( target is null )
			return;
		
		var actionString = Language.GetPhrase( GameLogEvents.Interactions.Pickup.LoseGrip );

		var name = target.GameObject.Name;

		var classifier = gameObject.GetComponent<ObjectClassifierComponent>();
		if ( classifier is not null)
		{
			name = classifier.DisplayName;
		}

		actionString = string.Format( actionString, name );
		
		GameLogPanel.AddEntry( actionString );
	}
}
