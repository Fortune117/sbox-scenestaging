using System;
using Sandbox;

namespace DarkDescent;

public partial class WalkController
{
    public virtual void DuckSimulate()
    {
    	if (Input.Down(GameTags.Input.Run) || Input.Pressed(GameTags.Input.Jump))
    		TryUnDuck();
    	else if (Input.Pressed(GameTags.Input.Duck))
    		ToggleDuck();

    	if ( IsDucking )
    	{
    		SetTag( "ducked" );
		    DuckFraction = DuckFraction.LerpTo(MovementResource.CrouchHeightMultiplier, Time.Delta * MovementResource.CrouchEnterSpeed);
            
            var offset = Entity.Velocity.Length.LerpInverse(0, MovementResource.CrouchWalkSpeed) * MovementResource.CrouchMovingHeightOffset;
            EyeLocalPosition *= DuckFraction + offset;
    	}
    	else
    	{
	        var offset = Entity.Velocity.Length.LerpInverse(0, MovementResource.CrouchWalkSpeed) * MovementResource.CrouchMovingHeightOffset;
	        DuckFraction = DuckFraction.LerpTo(1, Time.Delta * MovementResource.CrouchEnterSpeed);
            EyeLocalPosition *= (DuckFraction + offset).Clamp(0, 1);
    	}
    }

    protected virtual void ToggleDuck()
    {
    	if (IsDucking)
    		TryUnDuck();
    	else
    		TryDuck();
    }

    protected virtual void TryDuck()
    {
    	if (IsDucking)
    		return;
    	
    	//Player.PlayMovementNoise(0.5f);
    	
        IsDucking = true;
    }

    protected virtual void TryUnDuck()
    {
    	var pm = TraceBBox( Position, Position, originalMins, originalMaxs );
    	if ( pm.StartedSolid ) return;

    	if (!IsDucking)
    		return;
    	
    	//Player.PlayMovementNoise(0.5f);
    	
        IsDucking = false;
    }

    // Uck, saving off the bbox kind of sucks
    // and we should probably be changing the bbox size in PreTick
    Vector3 originalMins;
    Vector3 originalMaxs;

    public virtual void UpdateDuckBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
    {
    	originalMins = mins;
    	originalMaxs = maxs;

    	if ( IsDucking )
    		maxs = maxs.WithZ( 36 * scale );
    }
    
    // Could we do this in a generic callback too?
    public virtual float GetDuckWishSpeed()
    {
    	if ( !IsDucking ) 
		    return -1;
	    
	    var moveSpeedMult = Entity.ActorComponent.Stats.MoveSpeedMultiplier;
	    
    	return MovementResource.CrouchWalkSpeed * moveSpeedMult;
    }
}
