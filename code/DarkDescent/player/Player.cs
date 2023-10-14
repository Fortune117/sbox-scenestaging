using Sandbox;
using System;
using System.Linq;
using DarkDescent.Actor;
using DarkDescent.Cameras;
using Camera = DarkDescent.Cameras.Camera;

namespace DarkDescent;

[Prefab]
public partial class Player : AnimatedEntity
{
	public static readonly Prefab Prefab = ResourceLibrary.Get<Prefab>( "data/prefabs/player.prefab" );
	
	
	public PawnController Controller { get; }

	/// <summary>
	/// The active weapon, or tool, or whatever else
	/// </summary>
	[Net, Predicted]
	public Entity ActiveChild { get; set; }
	
	
	
	public FirstPersonCamera FirstPersonCamera { get; }
	
	
	public ThirdPersonCamera ThirdPersonCamera { get; }
	
	
	public ActorComponent ActorComponent { get; }

	
	public ActiveItemsComponent ActiveItemsComponent { get; }
	
	public PhysicsPickupComponent PhysicsPickupComponent { get; }
	
	[ClientInput] public Vector3 InputDirection { get; private set; }
	[ClientInput] public Angles ViewAngles { get; private set; }
	
	public Camera ActiveCamera
	{
		get
		{
			return Components.GetAll<Camera>().FirstOrDefault( x => x.IsActiveCamera );
		}
	}
	
	public override Ray AimRay => new(Position + EyeOffset, ViewAngles.Forward);
	
	private Vector3 EyeOffset => Controller?.EyeLocalPosition ?? 0;
	
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();

		EnableDrawing = true;
		EnableHideInFirstPerson = false;
		EnableShadowInFirstPerson = true;
		
		Tags.Add(GameTags.Physics.Player);
		
		SurroundingBoundsMode = SurroundingBoundsType.Hitboxes;

		EnableLagCompensation = true;
	}
	
	public void Respawn()
	{
		Game.AssertServer();
		
		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;
		this.ClearWaterLevel();

		CreateHull();

		//GameManager.Current?.MoveToSpawnpoint( this );
		ResetInterpolation();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = false;
		EnableShadowInFirstPerson = true;
		
		SetBodyGroup( "head", 1 );
	}
	
	private void CreateHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );
		EnableHitboxes = true;
	}
	
	private TimeSince TimeSinceDied = 0;
	private TimeSince TimeSinceGroundEntityWasNull = 0;
	public override void Simulate( IClient client )
	{
		if ( LifeState == LifeState.Dead && Game.IsServer)
		{
			if ( TimeSinceDied > 3  )
			{
				Respawn();
			}
			return;
		}
		
		EnableSolidCollisions = !Controller.HasTag( "noclip" );

		var groundEntityWasValid = GroundEntity.IsValid();
		
		Controller.Update();
		SimulateAnimations();

		if (groundEntityWasValid && !GroundEntity.IsValid())
			TimeSinceGroundEntityWasNull = 0;
		
		
		if ( LifeState != LifeState.Alive )
			return;

		var wasHolding = PhysicsPickupComponent.HoldingItem;

		if ( wasHolding )
			return;
		
		ActiveItemsComponent.Simulate( client );

		SimulateTestDamage();
	}

	public override void FrameSimulate(IClient client)
	{
		ActiveCamera?.FrameSimulate();

		ActiveChild?.FrameSimulate(client);
		
		Controller?.FrameUpdate();

		if ( this.GetWaterLevel() > 0.9f )
		{
			Audio.SetEffect( "underwater", 1, velocity: 5.0f );
		}
		else
		{
			Audio.SetEffect( "underwater", 0, velocity: 1.0f );
		}
	}
	
	public override void BuildInput()
	{
		InputDirection = Input.AnalogMove;

		if ( Input.StopProcessing )
			return;
		
		var look = Input.AnalogLook;
		
		if ( ViewAngles.pitch > 90f || ViewAngles.pitch < -90f )
		{
			look = look.WithYaw( look.yaw * -1f );
		}

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89f, 89f );
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;

		ActiveChild?.BuildInput();

		Controller?.BuildInput();
	}
	
	[ConCmd.Server("toggle_view")]
	private static void ChangeView()
	{
		if (ConsoleSystem.Caller.Pawn is not Player player)
			return;

		if ( player.ActiveCamera is ThirdPersonCamera )
		{
			player.ThirdPersonCamera.IsActiveCamera = false;
			player.FirstPersonCamera.IsActiveCamera = true;
			
			player.SetBodyGroup( "head", 1 );
		}
		else
		{
			player.ThirdPersonCamera.IsActiveCamera = true;
			player.FirstPersonCamera.IsActiveCamera = false;
			
			player.SetBodyGroup( "head", 0 );
		}
	}
	
	[ConCmd.Server("noclip")]
	private static void DoPlayerNoclip()
	{
		if (ConsoleSystem.Caller.Pawn is not Player player)
			return;

		if ( player.Controller is NoclipController )
		{
			Log.Info( "Noclip Mode Off" );
			player.Components.Create<WalkController>();
		}
		else
		{
			Log.Info( "Noclip Mode On" );
			player.Components.Create<NoclipController>();
		}
	}
}
