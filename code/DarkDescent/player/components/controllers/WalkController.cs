using System;
using Sandbox;

namespace DarkDescent;

[Prefab]
public partial class WalkController : PawnController
{
	[Prefab, Net]
	public MovementResource MovementResource { get; set; }

	private readonly Unstuck Unstuck;
	private float SurfaceFriction;

    public WalkController()
    {
        Unstuck = new Unstuck(this);
    }
    
    /// <summary>
    /// This is temporary, get the hull size for the player's collision
    /// </summary>
    public override BBox GetHull()
    {
        var girth = MovementResource.BodyGirth * 0.5f;
        var mins = new Vector3(-girth, -girth, 0);
        var maxs = new Vector3(+girth, +girth, MovementResource.BodyHeight);

        return new BBox(mins, maxs);
    }

    private Vector3 mins;
    private Vector3 maxs;

    public virtual void SetBBox(Vector3 mins, Vector3 maxs)
    {
        if (this.mins == mins && this.maxs == maxs)
            return;

        this.mins = mins;
        this.maxs = maxs;
    }

    /// <summary>
    /// Update the size of the bbox. We should really trigger some shit if this changes.
    /// </summary>
    public virtual void UpdateBBox()
    {
        var girth = MovementResource.BodyGirth * 0.5f;

        var mins = new Vector3(-girth, -girth, 0) * Entity.Scale;
        var maxs = new Vector3(+girth, +girth, MovementResource.BodyHeight) * Entity.Scale;

        UpdateDuckBBox(ref mins, ref maxs, Entity.Scale);

        SetBBox(mins, maxs);
    }

    public override void FrameSimulate()
    {
        base.FrameSimulate();
        
        EyeRotation = Entity.ViewAngles.ToRotation();
    }

    public override void Simulate()
    {
        EyeLocalPosition = Vector3.Up * (MovementResource.EyeHeight * Entity.Scale);
        EyeLocalPosition += TraceOffset;
        
        UpdateBBox();
        
        // If we're a bot, spin us around 180 degrees.
        if ( Entity.Client.IsBot )
            EyeRotation = Entity.ViewAngles.WithYaw( Entity.ViewAngles.yaw + 180f ).ToRotation();
        else
            EyeRotation = Entity.ViewAngles.ToRotation();
        
        if (Unstuck.TestAndFix())
            return;

        CheckLadder();
        Swimming = Entity.GetWaterLevel() > 0.6f;
        
        CheckFalling();
        
        if (!Swimming && !IsTouchingLadder)
        {
            Velocity -= new Vector3(0, 0, MovementResource.Gravity * 0.5f) * Time.Delta;
            Velocity += new Vector3(0, 0, BaseVelocity.z) * Time.Delta;

            BaseVelocity = BaseVelocity.WithZ(0);
        }

        if (MovementResource.AutoJump ? Input.Down(GameTags.Input.Jump) : Input.Pressed(GameTags.Input.Jump))
        {
            CheckJumpButton();
        }
        
        DuckSimulate();
        JumpSimulate();

        // Friction is handled before we add in any base velocity. That way, if we are on a conveyor,
        // we don't slow when standing still, relative to the conveyor.
        bool bStartOnGround = GroundEntity != null;
        if (bStartOnGround)
        {
            Velocity = Velocity.WithZ(0);

            if (GroundEntity != null)
            {
                ApplyFriction(MovementResource.GroundFriction * SurfaceFriction);
            }
        }

        if ( !Entity.ActorComponent.IsSprinting && Entity.ActorComponent.CanStartRun && Input.Down( GameTags.Input.Run ) )
        {
	        if (!Input.Down( GameTags.Input.Backward ) && Input.Down( GameTags.Input.Forward ))
		        Entity.ActorComponent.IsSprinting = true;
        }
        
        if ( Entity.ActorComponent.IsSprinting )
        {
	        if (GroundEntity is null)
		        Entity.ActorComponent.IsSprinting = false;
		        
	        else if ( !Input.Down( GameTags.Input.Run ) || !Entity.ActorComponent.CanRun )
		        Entity.ActorComponent.IsSprinting = false;

	        else if ( Input.Down( GameTags.Input.Backward ) || !Input.Down( GameTags.Input.Forward ) )
		        Entity.ActorComponent.IsSprinting = false;
        }

        //
        // Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
        //
        WishVelocity = new Vector3( Entity.InputDirection.x.Clamp( -1f, 1f ), Entity.InputDirection.y.Clamp( -1f, 1f ), 0 );;
        WishVelocity *= Entity.ViewAngles.WithPitch( 0 ).ToRotation();

        if (!Swimming && !IsTouchingLadder)
        {
            WishVelocity = WishVelocity.WithZ(0);
        }

        WishVelocity = WishVelocity.Normal * WishVelocity.Length.Clamp(0, 1);
        WishVelocity *= GetWishSpeed();
        
        var forwardProp = Entity.InputDirection.x;

        if ( forwardProp < -0.1f && !IsDucking )
	        WishVelocity *= MathF.Abs( forwardProp ) * MovementResource.SpeedForwardProportionality;

        var bStayOnGround = false;
        if (Swimming)
        {
            ApplyFriction(1);
            WaterMove();
        }
        else if (IsTouchingLadder)
        {
            SetTag("climbing");
            LadderMove();
        }
        else if (GroundEntity != null)
        {
            bStayOnGround = true;
            WalkMove();
        }
        else
        {
            AirMove();
        }

        CategorizePosition(bStayOnGround);
        
        if (!Swimming && !IsTouchingLadder)
        {
            Velocity -= new Vector3(0, 0, MovementResource.Gravity * 0.5f) * Time.Delta;
        }
        
        if (GroundEntity != null)
        {
            Velocity = Velocity.WithZ(0);
        }

        if (Debug)
        {
            DebugOverlay.Box(Position + TraceOffset, mins, maxs, Color.Red);
            DebugOverlay.Box(Position, mins, maxs, Color.Blue);

            var lineOffset = 0;
            if (Game.IsServer) lineOffset = 10;

            DebugOverlay.ScreenText($"        Position: {Position}", lineOffset + 0);
            DebugOverlay.ScreenText($"        Velocity: {Velocity}", lineOffset + 1);
            DebugOverlay.ScreenText($"    BaseVelocity: {BaseVelocity}", lineOffset + 2);
            DebugOverlay.ScreenText($"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]", lineOffset + 3);
            DebugOverlay.ScreenText($" SurfaceFriction: {SurfaceFriction}", lineOffset + 4);
            DebugOverlay.ScreenText($"    WishVelocity: {WishVelocity}", lineOffset + 5);
        }
    }

    public virtual float GetWishSpeed()
    {
        var ws = GetDuckWishSpeed();
        if (ws >= 0) return ws;

        var moveSpeedMult = Entity.ActorComponent.Stats.MoveSpeedMultiplier;
        
        if (Entity.ActorComponent.IsSprinting)
        {
	        return MovementResource.SprintSpeed * moveSpeedMult;
        }
        
        if (Input.Down(GameTags.Input.Walk)) return MovementResource.WalkSpeed * moveSpeedMult;

        return MovementResource.RunSpeed * moveSpeedMult;
    }

    public virtual void WalkMove()
    {
        var wishdir = WishVelocity.Normal;
        var wishspeed = WishVelocity.Length;
        var accel = MovementResource.Acceleration;

        var forwardDir = Entity.ViewAngles.WithPitch(0).ToRotation().Forward;
        var frac = ( 1 - wishdir.Dot(forwardDir).Clamp(0, 1));
        
        if (wishspeed > MovementResource.RunSpeed && Input.Down(GameTags.Input.Run))
        {
            var sprintProp = 1 - (1 - MovementResource.SprintForwardProportionality) * frac;
            wishspeed *= sprintProp; //Limit sprinting if not going forward
        }
        
        var accelProp = 1 - (1 - MovementResource.AccelerationForwardProportionality) * frac;
        accel *= accelProp;
        
        WishVelocity = WishVelocity.WithZ(0);
        WishVelocity = WishVelocity.Normal * wishspeed;

        Velocity = Velocity.WithZ(0);
        Accelerate(wishdir, wishspeed, 0, accel);
        Velocity = Velocity.WithZ(0);
        Velocity += BaseVelocity;

        try
        {
            if (Velocity.Length < 1.0f)
            {
                Velocity = Vector3.Zero;
                return;
            }

            // first try just moving to the destination
            var dest = (Position + Velocity * Time.Delta).WithZ(Position.z);

            var pm = TraceBBox(Position, dest);

            if (pm.Fraction.AlmostEqual( 1f ))
            {
                Position = pm.EndPosition;
                StayOnGround();
                return;
            }

            StepMove();
        }
        finally
        {

            // Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
            Velocity -= BaseVelocity;
        }

        StayOnGround();
    }

    public virtual void StepMove()
    {
        MoveHelper mover = new MoveHelper(Position, Velocity);
        mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Entity);
        mover.MaxStandableAngle = MovementResource.GroundAngle;

        mover.TryMoveWithStep(Time.Delta, MovementResource.StepSize);

        Position = mover.Position;
        Velocity = mover.Velocity;
    }

    public virtual void Move()
    {
        MoveHelper mover = new MoveHelper(Position, Velocity);
        mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Entity);
        mover.MaxStandableAngle = MovementResource.GroundAngle;

        mover.TryMove(Time.Delta);

        Position = mover.Position;
        Velocity = mover.Velocity;
    }

    /// <summary>
    /// Add our wish direction and speed onto our velocity
    /// </summary>
    public virtual void Accelerate(Vector3 wishdir, float wishspeed, float speedLimit, float acceleration)
    {
        if (speedLimit > 0 && wishspeed > speedLimit)
            wishspeed = speedLimit;

        // See if we are changing direction a bit
        var currentspeed = Velocity.Dot(wishdir);

        // Reduce wishspeed by the amount of veer.
        var addspeed = wishspeed - currentspeed;

        // If not going to add any speed, done.
        if (addspeed <= 0)
            return;

        // Determine amount of acceleration.
        var accelspeed = acceleration * Time.Delta * wishspeed * SurfaceFriction;

        // Cap at addspeed
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        Velocity += wishdir * accelspeed;
    }

    /// <summary>
    /// Remove ground friction from velocity
    /// </summary>
    public virtual void ApplyFriction(float frictionAmount = 1.0f)
    {
        // Calculate speed
        var speed = Velocity.Length;
        if (speed < 0.1f) return;

        // Bleed off some speed, but if we have less than the bleed
        // threshold, bleed the threshold amount.
        var control = (speed < MovementResource.StopSpeed) ? MovementResource.StopSpeed : speed;

        // Add the amount to the drop amount.
        var drop = control * Time.Delta * frictionAmount;

        // scale the velocity
        float newspeed = speed - drop;
        if (newspeed < 0) 
	        newspeed = 0;

        if ( newspeed.AlmostEqual( speed ) )
	        return;
        
        newspeed /= speed;
        Velocity *= newspeed;
    }

    public virtual void AirMove()
    {
        var wishdir = WishVelocity.Normal;
        var wishspeed = WishVelocity.Length;

        Accelerate(wishdir, wishspeed, MovementResource.AirControl, MovementResource.AirAcceleration);

        Velocity += BaseVelocity;

        Move();

        Velocity -= BaseVelocity;
    }

    public virtual void WaterMove()
    {
        var wishdir = WishVelocity.Normal;
        var wishspeed = WishVelocity.Length;

        wishspeed *= 0.8f;

        Accelerate(wishdir, wishspeed, 100, MovementResource.Acceleration);

        Velocity += BaseVelocity;

        Move();

        Velocity -= BaseVelocity;
    }

    bool IsTouchingLadder = false;
    Vector3 LadderNormal;

    public virtual void CheckLadder()
    {
        var wishvel = new Vector3( Entity.InputDirection.x.Clamp( -1f, 1f ), Entity.InputDirection.y.Clamp( -1f, 1f ), 0);
        wishvel *= Entity.ViewAngles.WithPitch(0).ToRotation();
        wishvel = wishvel.Normal;

        if (IsTouchingLadder)
        {
            if (Input.Pressed(GameTags.Input.Jump))
            {
                Velocity = LadderNormal * 100.0f;
                IsTouchingLadder = false;

                return;

            }
            
            if (GroundEntity != null && LadderNormal.Dot(wishvel) > 0)
            {
                IsTouchingLadder = false;

                return;
            }
        }

        const float ladderDistance = 1.0f;
        var start = Position;
        Vector3 end = start + (IsTouchingLadder ? (LadderNormal * -1.0f) : wishvel) * ladderDistance;

        var pm = Trace.Ray(start, end)
                    .Size(mins, maxs)
                    .WithTag("ladder")
                    .Ignore(Entity)
                    .Run();

        IsTouchingLadder = false;

        if (pm.Hit)
        {
            IsTouchingLadder = true;
            LadderNormal = pm.Normal;
        }
    }

    public virtual void LadderMove()
    {
        var velocity = WishVelocity;
        float normalDot = velocity.Dot(LadderNormal);
        var cross = LadderNormal * normalDot;
        Velocity = (velocity - cross) + (-normalDot * LadderNormal.Cross(Vector3.Up.Cross(LadderNormal).Normal));

        Move();
    }
    
    public virtual void CategorizePosition(bool bStayOnGround)
    {
        SurfaceFriction = 1.0f;

        // Doing this before we move may introduce a potential latency in water detection, but
        // doing it after can get us stuck on the bottom in water if the amount we move up
        // is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
        // this several times per frame, so we really need to avoid sticking to the bottom of
        // water on each call, and the converse case will correct itself if called twice.
        //CheckWater();

        var point = Position - Vector3.Up * 2;
        var vBumpOrigin = Position;

        //
        //  Shooting up really fast.  Definitely not on ground trimmed until ladder shit
        //
        var bMovingUpRapidly = Velocity.z > MovementResource.MaxNonJumpVelocity;

        var bMoveToEndPos = false;

        if (GroundEntity != null) // and not underwater
        {
            bMoveToEndPos = true;
            point.z -= MovementResource.StepSize;
        }
        else if (bStayOnGround)
        {
            bMoveToEndPos = true;
            point.z -= MovementResource.StepSize;
        }

        if (bMovingUpRapidly || Swimming) // or ladder and moving up
        {
            ClearGroundEntity();
            return;
        }

        var pm = TraceBBox(vBumpOrigin, point, 4.0f);

        if (pm.Entity == null || Vector3.GetAngle(Vector3.Up, pm.Normal) > MovementResource.GroundAngle)
        {
            ClearGroundEntity();
            bMoveToEndPos = false;

            if (Velocity.z > 0)
                SurfaceFriction = 0.25f;
        }
        else
        {
            UpdateGroundEntity(pm);
        }

        if (bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f)
        {
            Position = pm.EndPosition;
        }
    }

    /// <summary>
    /// We have a new ground entity
    /// </summary>
    public virtual void UpdateGroundEntity(TraceResult tr)
    {
        GroundNormal = tr.Normal;

        // VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
        // A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
        // This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
        SurfaceFriction = (tr.Surface.Friction * 1.25f).Clamp( 0, 1f );
        
        GroundEntity = tr.Entity;

        if (GroundEntity != null)
        {
            BaseVelocity = GroundEntity.Velocity;
        }
    }

    /// <summary>
    /// We're no longer on the ground, remove it
    /// </summary>
    public virtual void ClearGroundEntity()
    {
        if (GroundEntity is null) 
	        return;

        GroundEntity = null;
        GroundNormal = Vector3.Up;
        SurfaceFriction = 1.0f;
    }

    /// <summary>
    /// Traces the current bbox and returns the result.
    /// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
    /// position. This is good when tracing down because you won't be tracing through the ceiling above.
    /// </summary>
    public override TraceResult TraceBBox(Vector3 start, Vector3 end, float liftFeet = 0.0f)
    {
        return TraceBBox(start, end, mins, maxs, liftFeet);
    }

    /// <summary>
    /// Try to keep a walking player on the ground when running down slopes etc
    /// </summary>
    public virtual void StayOnGround()
    {
        var start = Position + Vector3.Up * 2;
        var end = Position + Vector3.Down * MovementResource.StepSize;

        // See how far up we can go without getting stuck
        var trace = TraceBBox(Position, start);
        start = trace.EndPosition;

        // Now trace down from a known safe position
        trace = TraceBBox(start, end);

        if (trace.Fraction <= 0) return;
        if (trace.Fraction >= 1) return;
        if (trace.StartedSolid) return;
        if (Vector3.GetAngle(Vector3.Up, trace.Normal) > MovementResource.GroundAngle) return;

        // This is incredibly hacky. The real problem is that trace returning that strange value we can't network over.
        // float flDelta = fabs( mv->GetAbsOrigin().z - trace.m_vEndPos.z );
        // if ( flDelta > 0.5f * DIST_EPSILON )

        Position = trace.EndPosition;
    }
}

