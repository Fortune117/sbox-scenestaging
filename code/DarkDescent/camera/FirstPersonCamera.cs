using System;
using Sandbox;

namespace DarkDescent.Cameras;

[Prefab]
public partial class FirstPersonCamera : Camera
{
	[Prefab, Net]
	public float ViewBobSpeedThreshold { get; set; }
	
	[Prefab, Net]
	public float ViewBobMagnitude { get; set; }
	
	[Prefab, Net]
	public float ViewBobFrequency { get; set; }
	
	[Prefab, Net]
	public float ViewBobRunningMultiplier { get; set; }
	
	[Prefab, Net]
	public MovementResource MovementResource { get; set; }
	
	private Vector3 lastPos;
	private Vector3 absolutePosition;

	protected override void OnActivate()
	{
		Position = Entity.AimRay.Position;
		Rotation = Entity.ViewAngles.ToRotation();

		lastPos = Position;
	}

	protected override void Update()
	{
		if (Entity is null)
			return;
		
		var eyePos = Entity.AimRay.Position;
		if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
		{
			absolutePosition = Vector3.Lerp( eyePos.WithZ( lastPos.z ), eyePos, 40.0f * Time.Delta );
		}
		else
		{
			absolutePosition = eyePos;
			Position = eyePos;
		}

		var pos = Entity.GetAttachment( "camera" );

		if ( pos is not null )
		{
			Position = pos.Value.Position;
			Rotation = pos.Value.Rotation;
		}
		else
		{
			Position = absolutePosition + GetViewBobOffset();
			Rotation = Entity.ViewAngles.ToRotation();
		}
		
		lastPos = absolutePosition;
		FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
		Sandbox.Camera.FirstPersonViewer = Entity;
		
		base.Update();
	}

	protected virtual float GetHeadBobFraction()
	{
		return (Entity.Velocity.Length / ViewBobSpeedThreshold).Clamp(0, 1);
	}
	
	public override Vector3 GetViewBobOffset()
	{
		if (!viewbob_enabled || Entity.GroundEntity == null)
			return Vector3.Zero;

		var frac = GetHeadBobFraction();
		var runningMult = (Entity.ActorComponent.IsSprinting ? ViewBobRunningMultiplier : 1); 

		var sinOffset = MathF.Sin(Time.Now * ViewBobFrequency * 2f * runningMult);
		var sinOffset2 = MathF.Sin(Time.Now * ViewBobFrequency * runningMult) / 2f;

		var z = Vector3.Up * sinOffset * ViewBobMagnitude * frac;
		var y = Entity.ViewAngles.ToRotation().Left.WithZ(0).Normal;
		y *= sinOffset2 * ViewBobMagnitude * frac;

		var jumpingOffset = Vector3.Zero;

		if (TimeUntilJump >= 0 && TotalJumpTime > 0.2f)
		{
			jumpingOffset += Vector3.Up * MovementResource.JumpOffsetCurve.Evaluate((TotalJumpTime - TimeUntilJump) / TotalJumpTime) * 12f;
		}

		if (TimeSinceLanded < JumpCooldown)
		{
			jumpingOffset += Vector3.Up * MovementResource.JumpLandingOffsetCurve.Evaluate(TimeSinceLanded/JumpCooldown) * 12f;
		}

		return (z + y) * runningMult + jumpingOffset;
	}

	protected override void SetupCamera()
	{
		Sandbox.Camera.Position = Position;
		Sandbox.Camera.Rotation = Rotation;
		Sandbox.Camera.FieldOfView = FieldOfView;
		Sandbox.Camera.ZNear = 1f;
		Sandbox.Camera.ZFar = 10000f;
	}
}

