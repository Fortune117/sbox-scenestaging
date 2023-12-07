using Sandbox;
using System.Drawing;
using DarkDescent.Actor;
using DarkDescent.UI;
using DarkDescent.Items;

namespace DarkDescent;

public partial class DarkDescentPlayerController : Component
{
	[Property, Range( 0, 300 ), ToggleGroup("Movement")] 
	public float WalkSpeed { get; set; } = 75f;

	[Property, Range( 0, 300 ), ToggleGroup("Movement")] 
	public float RunSpeed { get; set; } = 150f;
	
	[Property, ToggleGroup("Movement")] 
	public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
	
	[Property]
	public ActorComponent ActorComponent { get; set; }
	
	[Property, ToggleGroup("Movement")]
	private CharacterController CharacterController { get; set; }
	
	[Property, ToggleGroup("Body2")]
	public SkinnedModelRenderer Body { get; set; }
	
	[Property, ToggleGroup("Body2")] 
	private GameObject Eye { get; set; }
	
	[Property, ToggleGroup("Camera2")] 
	public CameraComponent Camera { get; set; }
	
	private bool IsCrouching { get; set; }
	
	private Vector3 WishVelocity { get; set; }
	
	public Rotation AimRotation => Eye.Transform.Rotation;

	public Ray AimRay => new Ray( Eye.Transform.Position, Eye.Transform.Rotation.Forward );
	
	private Angles internalEyeAngles;

	protected override void OnStart()
	{
		BlockerComponent.OnBlock += OnBlock;
		
		EquipItem( RightHandItem );
		EquipItem( LeftHandItem );
	}

	protected override void OnUpdate()
	{
		// Eye input

		var input = Input.MouseDelta * 0.1f;

		if ( RightHandItem is not null )
		{
			input = RightHandItem.UpdateInputForPlayer( input );
		}

		if ( LeftHandItem is not null )
		{
			input = LeftHandItem.UpdateInputForPlayer( input );
		}
		
		internalEyeAngles.pitch += input.y;
		internalEyeAngles.yaw -= input.x;
		internalEyeAngles.pitch = internalEyeAngles.pitch.Clamp( -89f, 89f );
		internalEyeAngles.roll = 0;
		internalEyeAngles = internalEyeAngles.Normal;

		// rotate body to look angles
		Body.Transform.Rotation = new Angles( 0, internalEyeAngles.yaw, 0 ).ToRotation();

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
			CharacterController.ApplyFriction( 3.0f );
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

		Crosshair.SetAimPipVisible( RightHandItem is not null );

		InteractableUpdate();

		if ( ActiveInteractable is not null || TimeSinceInteraction < 0.5f )
			return;
		
		if ( RightHandItem is null )
			return;

		if (!RightHandItem.HasPriority)
			CheckForThrow();

		if ( isThrowing )
		{
			ThrowUpdate();
			return;
		}
		
		RightHandItem?.UpdateForPlayer();
		LeftHandItem?.UpdateForPlayer();
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
