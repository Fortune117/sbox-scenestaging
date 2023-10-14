using System;
using Sandbox;
using DarkDescent.Cameras;

namespace DarkDescent;

public partial class WalkController
{
    private bool CanJump()
    {
        if (GroundEntity == null)
            return false;
        
        if (TimeSinceLanded < MovementResource.JumpCooldown)
            return false;

        if (!Entity.ActorComponent.CanAffordJump())
            return false;

        return true;
    }
    
    public virtual void CheckJumpButton()
    {
        // If we are in the water most of the way...
        if (Swimming)
        {
            // swimming, not jumping
            ClearGroundEntity();

            Velocity = Velocity.WithZ(100);
            return;
        }

        if (jumpQueued)
            return;
        
        //Can't attempt a new jump while crouching
        if (IsDucking)
            return;

        if (!CanJump())
            return;
        
        if (MovementResource.JumpWithDelay)
			QueueJump();
        else
	        PerformJump();
    }

    private void QueueJump()
    {
        jumpQueued = true;
        TimeUntilJump = Velocity.Length < 60f ? MovementResource.JumpDelayIdle : MovementResource.JumpDelayMoving;
        Entity.ActiveCamera?.JumpQueued(TimeUntilJump);
        Entity.ActorComponent.OnJumpQueued();
    }

    private void PerformJump()
    {
       Entity.ActorComponent.OnJump();

        ClearGroundEntity();

        Velocity = Velocity.WithZ( Velocity.z +
                                   MovementResource.JumpPower * Entity.ActorComponent.Stats.JumpHeightMultiplier );

        Velocity -= new Vector3(0, 0, MovementResource.Gravity * 0.5f) * Time.Delta;
        
        AddEvent("jump");
    }
    
    private TimeSince TimeSinceLanded { get; set; }
    private TimeUntil TimeUntilJump { get; set; }
    private bool jumpQueued { get; set; }
    
    private void JumpSimulate()
    {
        if (!jumpQueued)
            return;

        if (TimeUntilJump > 0)
            return;
        
        if (CanJump())
            PerformJump();
        
        jumpQueued = false;
    }
    
    private Vector3 oldVelocity;
    private Entity oldGroundEntity;
    protected virtual void CheckFalling()
    {
        if (oldGroundEntity != GroundEntity)
        {
            if (GroundEntity != null)
                OnHitGround(oldVelocity);
        }

        oldVelocity = Velocity;
        oldGroundEntity = GroundEntity;
    }

    protected void OnHitGround(Vector3 fallVelocity)
    {
        TimeSinceLanded = 0;
        Entity.ActiveCamera?.SnapToEyePosition();

        var speed = MathF.Abs(fallVelocity.z);
        var frac = speed.LerpInverse(0f, 500f);
        
        //Player.PlayMovementNoise(frac*3f);
        Entity.TryFootstepSound(0, frac*50f);
        Entity.TryFootstepSound(1, frac*50f);
        
        if (speed > 100f)
	        Entity.ActiveCamera?.JumpLanded(MovementResource.JumpCooldown);
        
        //Player.DoFallDamage(speed);
    } 
}
