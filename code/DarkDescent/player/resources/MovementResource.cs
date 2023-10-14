using Sandbox;

namespace DarkDescent;

[GameResource("Movement Info", "moveinfo", "Contains data about movement for a player.")]
public class MovementResource : GameResource
{
	[Category("Grounded Movement")]
	public float SprintSpeed { get; set; }
	
	[Range(0, 1), Category("Grounded Movement")]
	public float SprintForwardProportionality { get; set; }
	
	[Category("Grounded Movement")]
	public float RunSpeed { get; set; }
	
	[Category("Grounded Movement")]
	public float WalkSpeed { get; set; }
	
	/// <summary>
	/// How much our speed relies on moving forward.
	/// A value of 1 means we move at our appropriate speed in all directions.
	/// A value of 0.5 means we would move out half or appropriate speed if we move perpendicular to our forward vector.
	/// </summary>
	[Range(0, 1), Category("Grounded Movement")]
	public float SpeedForwardProportionality { get; set; }
	
	[Category("Grounded Movement")]
	public float Acceleration { get; set; }
	
	/// <summary>
	/// How much our acceleration relies on moving forward.
	/// A value of 1 means we accelerate at our appropriate speed in all directions.
	/// A value of 0.5 means we would accelerate out half or appropriate speed if we move perpendicular to our forward vector.
	/// </summary>
	[Range(0, 1), Category("Grounded Movement")]
	public float AccelerationForwardProportionality { get; set; }
	
	[Category("Grounded Movement")]
	public float GroundFriction { get; set; }
	
	[Category("Grounded Movement")]
	public float StopSpeed { get; set; }
	
	[Category("Grounded Movement")]
	public float GroundAngle { get; set; }
	
	[Category("Grounded Movement")]
	public float StepSize { get; set; }
	
	[Category("Grounded Movement")]
	public float MaxNonJumpVelocity { get; set; }
	
	
	[Category("Air Movement")]
	public float AirAcceleration { get; set; }
	
	[Category("Air Movement")]
	public float Gravity { get; set; }
	
	[Category("Air Movement")]
	public float AirControl { get; set; }
	
	[Category("Collisions")]
	public float EyeHeight { get; set; }
	
	[Category("Collisions")]
	public float BodyGirth { get; set; }
	
	[Category("Collisions")]
	public float BodyHeight { get; set; }
	
	[Category("Jumping")]
	public float JumpPower { get; set; }

	[Category( "Jumping" )]
	public float JumpCooldown { get; set; }
	
	[Category("Jumping")]
	public bool JumpWithDelay { get; set; }
	
	[ShowIf(nameof(JumpWithDelay), true), Category("Jumping")]
	public float JumpDelayIdle { get; set; }
	
	[ShowIf(nameof(JumpWithDelay), true), Category("Jumping")]
	public float JumpDelayMoving { get; set; }
	
	[Category("Jumping")]
	public Curve JumpOffsetCurve { get; set; }
	
	[Category("Jumping")]
	public Curve JumpLandingOffsetCurve { get; set; }
	
	[Category("Jumping")]
	public bool AutoJump { get; set; }
	
	[Category("Grounded Movement")]
	public float CrouchWalkSpeed { get; set; }
	
	[Category("Grounded Movement"), Range(0, 1)]
	public float CrouchHeightMultiplier { get; set; }
	
	[Category("Grounded Movement"), Range(0, 1)]
	public float CrouchMovingHeightOffset { get; set; }
	
	/// <summary>
	/// How quickly the player actually ducks when they start crouching.
	/// </summary>
	[Category("Grounded Movement")]
	public float CrouchEnterSpeed { get; set; }
}
