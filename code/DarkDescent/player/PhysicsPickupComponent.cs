using System;
using DarkDescent.Actor;
using DarkDescent.Components;
using DarkDescent.GameLog;
using DarkDescent.UI;
using Sandbox;

namespace DarkDescent;

public partial class PhysicsPickupComponent : BaseComponent
{
	public bool HoldingItem { get; set; }
	
	[Property, Range(10, 300)]
	private float PickupRange { get; set; }
	
	[Property, Range(10, 300)]
	private float HoldRange { get; set; }

	[Property, Range( 0, 5 )] 
	private float StaminaDrainBase { get; set; } = 1;

	[Property, Range( 0, 5 )] 
	private float StaminaDrainPerMissingStrength { get; set; } = 1;
	
	[Property, Range( 0, 15 )] 
	private float StaminaThrowCost { get; set; } = 5;
	
	[Property, Range(0, 100)]
	private float ThrowForcePerStrength { get; set; }
	
	[Property, Range(0, 300)]
	private float ThrowForceBase { get; set; }
	
	private PickupTargetComponent PickupTarget { get; set; }
	
	private PhysicsBody HeldBody { get; set; }
	private Vector3 HeldPos { get; set; }
	private Rotation HeldRot { get; set; }
	private Vector3 HoldPos { get; set; }
	private Rotation HoldRot { get; set; }
	
	private float EffectiveStrength
	{
		get
		{
			if ( PickupTarget is null )
				return 0;

			return GameObject.GetComponent<ActorComponent>().Stats.Strength - PickupTarget.StrengthThreshold;
		}
	}

	private bool CanCarryTarget( PickupTargetComponent targetComponent )
	{
		return GameObject.GetComponent<ActorComponent>().Stats.Strength > targetComponent.StrengthThreshold - targetComponent.StrengthLeeway;
	}

	/// <summary>
	/// Return true when we're holding an item.
	/// </summary>
	/// <returns></returns>
	public override void Update()
	{
		var wasHolding = HoldingItem;
		if ( !HoldingItem )
		{
			TryPickup();
		}

		if ( !HoldingItem )
			return;
		
		Crosshair.Instance?.SetClass( "interact", false );

		var player = GameObject.GetComponent<DarkDescentPlayerController>();
			
		PickupMove( player.AimRay.Position, player.AimRay.Forward, player.AimRotation );

		if ( !wasHolding )
			return;
			
		if ( HoldingItem && Input.Pressed( GameTags.Input.AttackPrimary ) )
		{
			Throw();
			return;
		}

		if ( HoldingItem )
		{
			if (Input.Pressed( GameTags.Input.AttackSecondary ) || Input.Pressed( GameTags.Input.Interact ))
				PickupEnd();
		}

		OnPrePhysicsStep();
	}

	private bool TryPickup()
	{
		Crosshair.Instance?.SetClass( "interact", false );
		
		var player = GameObject.GetComponent<DarkDescentPlayerController>();
		
		var tr = Physics.Trace.Ray( player.AimRay, PickupRange )
			.Radius( 3 )
			.Run();

		if ( !tr.Hit )
			return false;
		
		if ( tr.Body?.GameObject is not GameObject gameObject )
			return false;
		
		if ( !gameObject.TryGetComponent<PickupTargetComponent>(out var pickupTarget ))
			return false;
		
		var passedStrengthTest = GameObject.GetComponent<ActorComponent>().Stats.Strength >
		                         pickupTarget.StrengthThreshold - pickupTarget.StrengthLeeway;


		Crosshair.Instance?.SetClass( "interact", true );
		Crosshair.InteractPossible = passedStrengthTest;
		
		if ( !Input.Pressed( "use" ) )
			return false;
		
		/*if ( pickupTarget.Entity.Tags.Has( "grabbed" ) )
			return false;*/

		if ( !passedStrengthTest )
		{
			GameLogSystem.PlayerFailToPickup( pickupTarget );
			return false;
		}
		
		var attachPos = tr.Body.FindClosestPoint( player.AimRay.Position );

		var holdDistance = HoldRange + attachPos.Distance( tr.Body.MassCenter );
		PickupStart( pickupTarget, tr.Body, tr.StartPosition + tr.Direction * holdDistance, player.AimRotation );
		
		return true;
	}

	public void OnPrePhysicsStep()
	{
		if ( !HeldBody.IsValid() )
			return;

		var smoothTime = 0.05f;

		if ( EffectiveStrength < 0 )
		{
			var fraction = MathF.Abs( EffectiveStrength ) / PickupTarget.StrengthLeeway;
			
			smoothTime = fraction.Remap( 0, 1, 0.05f, 1.5f );
		}

		var velocity = HeldBody.Velocity;
		Vector3.SmoothDamp( HeldBody.Position, HoldPos, ref velocity, smoothTime, Time.Delta );
		HeldBody.Velocity = velocity;

		var angularVelocity = HeldBody.AngularVelocity;
		GameObject.Transform.Rotation.SmoothDamp( HeldBody.Rotation, HoldRot, ref angularVelocity, smoothTime, Time.Delta );
		HeldBody.AngularVelocity = angularVelocity;

		if ( HoldPos.Distance( HeldBody.Position ) > 60f )
		{
			GameLogSystem.PlayerLoseGrip(PickupTarget.GameObject);
			PickupEnd();
		}
	}

	private void PickupStart( PickupTargetComponent target, PhysicsBody body, Vector3 grabPos, Rotation grabRot )
	{
		PickupEnd();

		HeldBody = body;
		HeldPos = HeldBody.LocalMassCenter;
		HeldRot = grabRot.Inverse * HeldBody.Rotation;

		HoldPos = HeldBody.Position;
		HoldRot = HeldBody.Rotation;

		HeldBody.Sleeping = false;
		HeldBody.AutoSleep = false;

		PickupTarget = target;
		//PickupTarget.Entity.Tags.Add( "grabbed" );

		HoldingItem = true;
		
		Crosshair.Instance.SetClass( "interact", false );
		GameLogSystem.PlayerPickupObject( target, EffectiveStrength < 0 );
	}

	private void PickupEnd(bool dampenForces = true, bool useLog = true)
	{
		if ( HeldBody.IsValid() )
		{
			HeldBody.AutoSleep = true;

			if ( dampenForces )
			{
				HeldBody.Velocity = HeldBody.Velocity.LerpTo( Vector3.Zero, 0.8f );
				HeldBody.AngularVelocity = HeldBody.AngularVelocity.LerpTo( Vector3.Zero, 0.8f );
			}
		}
		
		HeldBody = null;
		HeldRot = Rotation.Identity;

		if ( PickupTarget is not null && PickupTarget.GameObject is not null )
		{
			//PickupTarget.GameObject.Tags.Remove( "grabbed" );
			
			if (useLog)
				GameLogSystem.PlayerDropObject( PickupTarget );
		}

		PickupTarget = null;
		HoldingItem = false;
	}

	private void PickupMove( Vector3 startPos, Vector3 dir, Rotation rot )
	{
		if ( !HeldBody.IsValid() )
			return;
		
		var player = GameObject.GetComponent<DarkDescentPlayerController>();

		var attachPos = HeldBody.FindClosestPoint( startPos );
		var holdDistance = HoldRange + attachPos.Distance( HeldBody.MassCenter );

		HoldPos = startPos - HeldPos * HeldBody.Rotation + dir * holdDistance;
		HoldRot = rot * HeldRot;

		if ( PickupTarget is null )
		{
			PickupEnd();
			return;
		}

		var staminaCost = StaminaDrainBase +
		                  (EffectiveStrength < 0 ? MathF.Abs( EffectiveStrength ) * StaminaDrainPerMissingStrength : 0);
		staminaCost *= Time.Delta;
		
		if ( !GameObject.GetComponent<ActorComponent>().CanAffordStaminaCost( staminaCost ) )
		{
			GameLogSystem.PlayerStrengthFailPickup(PickupTarget.GameObject);
			PickupEnd();
		}
		else
		{
			GameObject.GetComponent<ActorComponent>().PayStamina( staminaCost );
		}
		
		if ( !CanCarryTarget( PickupTarget ) )
		{
			GameLogSystem.PlayerStrengthFailPickup(PickupTarget.GameObject);
			PickupEnd();
		}
		
		if ( attachPos.Distance( player.AimRay.Position ) > PickupRange )
		{
			GameLogSystem.PlayerLoseGrip(PickupTarget.GameObject);
			PickupEnd();
		}
	}

	private void Throw()
	{
		if ( EffectiveStrength < 0 || !GameObject.GetComponent<ActorComponent>().CanAffordStaminaCost( StaminaThrowCost ))
		{
			PickupEnd();
			return;
		}

		var player = GameObject.GetComponent<DarkDescentPlayerController>();
		GameObject.GetComponent<ActorComponent>().PayStamina( StaminaThrowCost );

		//always add a little bit if we can actually throw something
		var force = ThrowForceBase + EffectiveStrength * ThrowForcePerStrength;
		if ( HeldBody.PhysicsGroup?.BodyCount > 1 )
		{
			// Don't throw ragdolls as hard
			HeldBody.PhysicsGroup.ApplyImpulse( player.AimRay.Forward * (force * 0.5f), true );
			HeldBody.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * force, true );
		}
		else
		{
			HeldBody.ApplyImpulse( player.AimRay.Forward * (HeldBody.Mass * force) );
			HeldBody.ApplyAngularImpulse( Vector3.Random * (HeldBody.Mass * force) );
		}

		Sound.FromWorld( "interact.throw", player.AimRay.Position );
		GameLogSystem.PlayerThrowObject( PickupTarget );
		
		PickupEnd(false, false);
	}
}
