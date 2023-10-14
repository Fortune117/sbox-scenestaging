using Sandbox;

namespace DarkDescent.Cameras;

[Prefab]
public class ThirdPersonCamera : Camera
{
	[ConVar.Replicated]
	public static bool thirdperson_orbit { get; set; } = false;

	[ConVar.Replicated]
	public static bool thirdperson_collision { get; set; } = true;
	
	[Prefab, Range(30, 300)]
	private float OrbitDistance { get; set; } = 150;

	private Angles orbitAngles;

	private float orbitHeight = 0;

	protected override void Update()
	{
		if ( Game.LocalPawn is not AnimatedEntity pawn )
			return;

		Position = pawn.Position;
		Vector3 targetPos;

		var center = pawn.Position + Vector3.Up * 64;

		if ( thirdperson_orbit )
		{
			Position += Vector3.Up * ((pawn.CollisionBounds.Center.z * pawn.Scale) + orbitHeight);
			Rotation = Rotation.From( orbitAngles );

			targetPos = Position + Rotation.Backward * OrbitDistance;
		}
		else
		{
			center = Position + Vector3.Up * 64;

			var pos = center;
			var rot = Entity.ViewAngles.ToRotation() * Rotation.FromAxis( Vector3.Up, -16 );

			float distance = 100.0f * Entity.Scale;
			targetPos = pos + rot.Right * ((Entity.CollisionBounds.Mins.x + 64) * Entity.Scale);
			targetPos += rot.Forward * -distance;

			var tr = Trace.Ray( pos, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( Entity )
				.Radius( 8 )
				.Run();

			Position = tr.EndPosition;
		}

		if ( thirdperson_collision )
		{
			var tr = Trace.Ray( Position, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( pawn )
				.Radius( 8 )
				.Run();

			Position = tr.EndPosition;
		}
		else
		{
			Position = targetPos;
		}

		FieldOfView = 70;

		base.Update();
	}

	protected override void SetupCamera()
	{
		Sandbox.Camera.Position = Position;
		Sandbox.Camera.Rotation = Entity.ViewAngles.ToRotation();
		Sandbox.Camera.FirstPersonViewer = null;
		Sandbox.Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
		Sandbox.Camera.ZNear = 1f;
		Sandbox.Camera.ZFar = 200000f;
	}


	[GameEvent.Client.BuildInput]
	private void BuildInput()
	{
		if ( thirdperson_orbit && Input.Down( GameTags.Input.Walk ) )
		{
			if ( Input.Down( GameTags.Input.AttackPrimary ) )
			{
				OrbitDistance += Input.AnalogLook.pitch;
				OrbitDistance = OrbitDistance.Clamp( 0, 1000 );
			}
			else if ( Input.Down( GameTags.Input.AttackSecondary ) )
			{
				orbitHeight += Input.AnalogLook.pitch;
				orbitHeight = orbitHeight.Clamp( -1000, 1000 );
			}
			else
			{
				orbitAngles.yaw += Input.AnalogLook.yaw;
				orbitAngles.pitch += Input.AnalogLook.pitch;
				orbitAngles = orbitAngles.Normal;
				orbitAngles.pitch = orbitAngles.pitch.Clamp( -89, 89 );
			}

			Input.AnalogLook = Angles.Zero;

			Input.ClearActions();
			Input.StopProcessing = true;
		}
	}
}

