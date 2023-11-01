using Sandbox;
using System.Drawing;
using DarkDescent.Actor;

namespace DarkDescent;

public partial class DarkDescentPlayerController : BaseComponent
{
	[Property, Range( 0, 300 )] 
	public float WalkSpeed { get; set; } = 75f;

	[Property, Range( 0, 300 )] 
	public float RunSpeed { get; set; } = 150f;
	
	[Property] 
	public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
	
	[Property] 
	public float CameraDistance { get; set; } = 200.0f;
	
	[Property] 
	private AnimatedModelComponent Body { get; set; }
	
	[Property] 
	private GameObject Eye { get; set; }

	[Property]
	private CharacterController CharacterController { get; set; }
	
	[Property]
	private ActorComponent ActorComponent { get; set; }
	
	[Property]
	private GameObject RightIKTarget { get; set; }
	
	[Property]
	private GameObject LeftIKTarget { get; set; }

	private bool IsCrouching { get; set; }
	
	private Vector3 WishVelocity { get; set; }
	
	private Angles internalEyeAngles;

	public Rotation AimRotation => Eye.Transform.Rotation;

	public Ray AimRay => new Ray( Eye.Transform.Position, Eye.Transform.Rotation.Forward );
	
	public override void OnStart()
	{
		HookupAnimEvents();
	}

	public override void Update()
	{
		// Eye input

		var input = Input.MouseDelta * 0.1f;

		if ( isAttacking )
		{
			input = new Vector2( input.x.Clamp( -CarriedItemComponent.TurnCapX * Time.Delta, CarriedItemComponent.TurnCapX * Time.Delta ),
				input.y.Clamp( -CarriedItemComponent.TurnCapY * Time.Delta, CarriedItemComponent.TurnCapY * Time.Delta ) );
		}
		
		internalEyeAngles.pitch += input.y;
		internalEyeAngles.yaw -= input.x;
		internalEyeAngles.pitch = internalEyeAngles.pitch.Clamp( -89f, 89f );
		internalEyeAngles.roll = 0;
		internalEyeAngles = internalEyeAngles.Normal;

		// rotate body to look angles
		if ( Body is not null )
		{
			Body.Transform.Rotation = new Angles( 0, internalEyeAngles.yaw, 0 ).ToRotation();
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
		
		IsCrouching =  Input.Down( "Duck" );

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
		
		AttackUpdate();
		
		if (isAttacking)
			AttackHitUpdate();
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

		if ( Input.Down( "Run" ) ) WishVelocity *= RunSpeed;
		else WishVelocity *= WalkSpeed;
	}
}
