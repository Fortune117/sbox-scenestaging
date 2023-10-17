using Sandbox;
using System.Drawing;

namespace DarkDescent;

public partial class DarkDescentPlayerController : BaseComponent
{
	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
	[Property] public float CameraDistance { get; set; } = 200.0f;

	public Vector3 WishVelocity { get; private set; }

	[Property] GameObject Body { get; set; }
	[Property] GameObject Eye { get; set; }
	[Property] bool FirstPerson { get; set; }

	public Angles EyeAngles;
	public Vector3 EyePosition => Eye.Transform.Position;

	public Ray AimRay => new Ray( EyePosition, EyeAngles.Forward );
	
	private CharacterController CharacterController { get; set; }

	public override void OnStart()
	{
		CharacterController = GetComponent<CharacterController>();

		if ( Body.TryGetComponent<AnimatedModelComponent>( out var modelComponent ) )
		{
			//modelComponent.SetBodyGroup( "head", 1 );
		}
	}

	public override void Update()
	{
		// Eye input
		EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		EyeAngles.pitch = EyeAngles.pitch.Clamp( -89f, 89f );
		EyeAngles.roll = 0;
		EyeAngles = EyeAngles.Normal;

		// rotate body to look angles
		if ( Body is not null )
		{
			Body.Transform.Rotation = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();
		}

		// read inputs
		BuildWishVelocity();

		if ( CharacterController.IsOnGround && Input.Down( "Jump" ) )
		{
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;
			//if ( Duck.IsActive )
			//	flMul *= 0.8f;

			CharacterController.Punch( Vector3.Up * flMul * flGroundFactor );
		//	cc.IsOnGround = false;
		}

		if ( CharacterController.IsOnGround )
		{
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 );
			CharacterController.Accelerate( WishVelocity );
			CharacterController.ApplyFriction( 4.0f );
		}
		else
		{
			CharacterController.Velocity -= Gravity * Time.Delta * 0.5f;
			CharacterController.Accelerate( WishVelocity.ClampLength( 50 ) );
			CharacterController.ApplyFriction( 0.1f );
		}

		CharacterController.Move();

		if ( !CharacterController.IsOnGround )
		{
			CharacterController.Velocity -= Gravity * Time.Delta * 0.5f;
		}
		else
		{
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 );
		}
		
		UpdateAnimations();

		UpdateCamera();
	}

	private void UpdateCamera()
	{
		// Update camera position
		var camera = GameObject.GetComponent<CameraComponent>( true, true );
		if ( camera is not null )
		{
			var camPos = Eye.Transform.Position - EyeAngles.ToRotation().Forward * CameraDistance;

			if ( FirstPerson ) camPos = Eye.Transform.Position + EyeAngles.ToRotation().Forward * 8;

			if ( Body.TryGetComponent<AnimatedModelComponent>( out var modelComponent ) )
			{
				var pos = modelComponent.GetAttachment( "camera" );

				if ( pos is not null )
				{
					camera.Transform.Position = pos.Value.Position;
					camera.Transform.Rotation = pos.Value.Rotation;
				}
				else
				{
					camera.Transform.Position = camPos;
					camera.Transform.Rotation = EyeAngles.ToRotation();
				}
			}
			else
			{
				camera.Transform.Position = camPos;
				camera.Transform.Rotation = EyeAngles.ToRotation();
			}
		}
	}

	public void BuildWishVelocity()
	{
		var rot = Body.Transform.Rotation;

		WishVelocity = 0;

		if ( Input.Down( "Forward" ) ) WishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Down( "Run" ) ) WishVelocity *= 320.0f;
		else WishVelocity *= 150.0f;
	}
}
