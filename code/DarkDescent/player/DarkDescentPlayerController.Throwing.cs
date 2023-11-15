using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	private bool isThrowing;
	private bool isReleasingThrow;
	private float timeSinceThrowReleased;

	private TimeSince timeSinceThrowStarted;

	private void CheckForThrow()
	{
		if ( isThrowing )
			return;

		if ( Input.Down( "reload" ) )
		{
			isThrowing = true;
			isReleasingThrow = false;
			
			Body.Set( "bIsThrowing", true );

			timeSinceThrowStarted = 0;
		}
	}
	
	private void ThrowUpdate()
	{
		if ( Input.Released( "reload" ) )
		{
			Body.Set( "bIsThrowing", false );

			isReleasingThrow = true;
			timeSinceThrowReleased = 0;
		}

		if ( isReleasingThrow )
		{
			if (timeSinceThrowStarted > 0.33 )
				timeSinceThrowReleased += Time.Delta * ActorComponent.Stats.ActionSpeed;

			if ( timeSinceThrowReleased > 0.16f )
			{
				isThrowing = false;
				ThrowItem();
			}
		}
	}

	private void ThrowItem()
	{
		if ( !CarriedItemComponent.CanThrow )
			return;
		
		CarriedItemComponent.Throw( ActorComponent, Eye.Transform.Rotation.Forward );

		CarriedItemComponent = null;
	}
}
