using System;
using Sandbox;

namespace DarkDescent.Cameras;

[Title( "View Bob" )]
[Category( "Camera" )]
[Icon( "trending_up" )]
public class ViewBob : BaseComponent, CameraComponent.ISceneCameraSetup
{
	[ConVar.Client]
	public bool viewbob_enabled { get; set; } = true;
	
	[Property]
	public float ViewBobSpeedThreshold { get; set; }
	
	[Property]
	public float ViewBobMagnitude { get; set; }
	
	[Property]
	public float ViewBobFrequency { get; set; }
	
	[Property]
	public float ViewBobRunningMultiplier { get; set; }
	
	protected virtual float GetHeadBobFraction()
	{
		if ( GameObject.TryGetComponent<CharacterController>( out var controller ) )
			return 1f;
		
		return (controller.Velocity.Length / ViewBobSpeedThreshold).Clamp(0, 1);
	}
	
	public Vector3 GetViewBobOffset()
	{
		if ( GameObject.TryGetComponent<CharacterController>( out var controller ) )
			return Vector3.Zero;

		if ( !GameObject.TryGetComponent<DarkDescentPlayerController>( out var playerController ) )
			return Vector3.Zero;
		
		if (!viewbob_enabled || !controller.IsOnGround)
			return Vector3.Zero;

		var frac = GetHeadBobFraction();
		var runningMult = 1f; //(IsSprinting ? ViewBobRunningMultiplier : 1); 

		var sinOffset = MathF.Sin(Time.Now * ViewBobFrequency * 2f * runningMult);
		var sinOffset2 = MathF.Sin(Time.Now * ViewBobFrequency * runningMult) / 2f;

		var z = Vector3.Up * sinOffset * ViewBobMagnitude * frac;
		var y = playerController.AimRotation.Left.WithZ(0).Normal;
		y *= sinOffset2 * ViewBobMagnitude * frac;

		return (z + y) * runningMult;
	}

	public void SetupCamera( CameraComponent camera, SceneCamera sceneCamera )
	{
		sceneCamera.Position += GetViewBobOffset();
	}
}
