using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	private bool isThrowing;
	private bool isReleasingThrow;
	private float timeSinceThrowReleased;

	private void CheckForThrow()
	{
		if ( isThrowing )
			return;

		if ( Input.Down( "reload" ) )
		{
			isThrowing = true;
			isReleasingThrow = false;
			
			Body.Set( "bStartThrow", true );
			Body.Set( "bHoldThrow", true );
		}
	}
	
	private void ThrowUpdate()
	{
		if ( Input.Released( "reload" ) )
		{
			Body.Set( "bHoldThrow", false );
			Body.Set( "bReleaseThrow", true );

			isReleasingThrow = true;
			timeSinceThrowReleased = 0;
		}

		if ( isReleasingThrow )
		{
			timeSinceThrowReleased += Time.Delta * ActorComponent.Stats.ActionSpeed;

			if ( timeSinceThrowReleased > 0.17f )
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
