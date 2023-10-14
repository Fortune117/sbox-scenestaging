using System;
using DarkDescent.Components;
using DarkDescent.GameLog;
using DarkDescent.UI;
using Sandbox;

namespace DarkDescent;

[Prefab]
public partial class PhysicsPickupComponent : EntityComponent<Player>
{
	public bool HoldingItem { get; set; }
	
	[Prefab, Net, Predicted, Range(10, 300)]
	private float PickupRange { get; set; }
	
	[Prefab, Net, Predicted, Range(10, 300)]
	private float HoldRange { get; set; }

	[Prefab, Net, Range( 0, 5 )] 
	private float StaminaDrainBase { get; set; } = 1;

	[Prefab, Net, Range( 0, 5 )] 
	private float StaminaDrainPerMissingStrength { get; set; } = 1;
	
	[Prefab, Net, Range( 0, 15 )] 
	private float StaminaThrowCost { get; set; } = 5;
	
	[Prefab, Net, Range(0, 100)]
	private float ThrowForcePerStrength { get; set; }
	
	[Prefab, Net, Range(0, 300)]
	private float ThrowForceBase { get; set; }
	
	[Net, Predicted]
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

			return Entity.ActorComponent.Stats.Strength - PickupTarget.StrengthThreshold;
		}
	}

	private bool CanCarryTarget( PickupTargetComponent targetComponent )
	{
		return 	Entity.ActorComponent.Stats.Strength > targetComponent.StrengthThreshold - targetComponent.StrengthLeeway;
	}
	
	/// <summary>
	/// Return true when we're holding an item.
	/// </summary>
	/// <returns></returns>
	public void Simulate()
	{
		using ( Prediction.Off() )
		{
			var wasHolding = HoldingItem;
			if ( !HoldingItem )
			{
				TryPickup();
			}

			if ( !HoldingItem )
				return;
			
			if (Game.IsClient)
				Crosshair.Instance?.SetClass( "interact", false );
			
			PickupMove( Entity.AimRay.Position, Entity.AimRay.Forward, Entity.ViewAngles.ToRotation() );

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
		}
	}

	private bool TryPickup()
	{
		if (Game.IsClient)
			Crosshair.Instance?.SetClass( "interact", false );
		
		var tr = Trace.Ray( Entity.AimRay, PickupRange )
			.Radius( 3 )
			.Ignore( Entity )
			.DynamicOnly()
			.Run();

		if ( !tr.Hit )
			return false;

		if ( !tr.Entity.Components.TryGet<PickupTargetComponent>( out var pickupTarget ) )
			return false;
		
		var passedStrengthTest = Entity.ActorComponent.Stats.Strength >
		                         pickupTarget.StrengthThreshold - pickupTarget.StrengthLeeway;

		if ( Game.IsClient )
		{
			Crosshair.Instance?.SetClass( "interact", true );
			Crosshair.InteractPossible = passedStrengthTest;
		}
			
		if ( !Input.Pressed( GameTags.Input.Interact ) )
			return false;
		
		if ( pickupTarget.Entity.Tags.Has( "grabbed" ) )
			return false;

		if ( !passedStrengthTest )
		{
			if (Game.IsClient)
				GameLogSystem.PlayerFailToPickup( pickupTarget );
			
			return false;
		}
		
		var attachPos = tr.Body.FindClosestPoint( Entity.AimRay.Position );

		var holdDistance = HoldRange + attachPos.Distance( tr.Body.MassCenter );
		PickupStart( pickupTarget, tr.Body, tr.StartPosition + tr.Direction * holdDistance, Entity.ViewAngles.ToRotation() );
		
		return true;
	}
	
	[GameEvent.Physics.PreStep]
	public void OnPrePhysicsStep()
	{
		if ( !Game.IsServer )
			return;

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
		Entity.Rotation.SmoothDamp( HeldBody.Rotation, HoldRot, ref angularVelocity, smoothTime, Time.Delta );
		HeldBody.AngularVelocity = angularVelocity;

		if ( HoldPos.Distance( HeldBody.Position ) > 60f )
		{
			if ( Game.IsServer )
			{
				GameLogSystem.PlayerLoseGrip( To.Single(Entity), PickupTarget.Entity);
			}
			PickupEnd();
		}
	}

	private void PickupStart( PickupTargetComponent target, PhysicsBody body, Vector3 grabPos, Rotation grabRot )
	{
		if ( !body.IsValid() )
			return;

		if ( body.PhysicsGroup == null )
			return;

		PickupEnd();

		HeldBody = body;
		HeldPos = HeldBody.LocalMassCenter;
		HeldRot = grabRot.Inverse * HeldBody.Rotation;

		HoldPos = HeldBody.Position;
		HoldRot = HeldBody.Rotation;

		HeldBody.Sleeping = false;
		HeldBody.AutoSleep = false;

		PickupTarget = target;
		PickupTarget.Entity.Tags.Add( "grabbed" );

		HoldingItem = true;
		
		if ( Game.IsClient )
		{
			Crosshair.Instance.SetClass( "interact", false );
			GameLogSystem.PlayerPickupObject( target, EffectiveStrength < 0 );
		}
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

		if ( PickupTarget is not null && PickupTarget.Entity.IsValid() )
		{
			PickupTarget.Entity.Tags.Remove( "grabbed" );
			
			if (Game.IsClient && useLog)
				GameLogSystem.PlayerDropObject( PickupTarget );
		}

		PickupTarget = null;
		HoldingItem = false;
	}

	private void PickupMove( Vector3 startPos, Vector3 dir, Rotation rot )
	{
		if ( !HeldBody.IsValid() )
			return;

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
		
		if ( !Entity.ActorComponent.CanAffordStaminaCost( staminaCost ) )
		{
			if ( Game.IsClient )
			{
				GameLogSystem.PlayerStrengthFailPickup( To.Single(Entity), PickupTarget.Entity );
			}
			PickupEnd();
		}
		else
		{
			Entity.ActorComponent.PayStamina( staminaCost );
		}
		
		if ( !CanCarryTarget( PickupTarget ) )
		{
			if ( Game.IsServer )
			{
				GameLogSystem.PlayerStrengthFailPickup( To.Single(Entity), PickupTarget.Entity );
			}
			PickupEnd();
		}
		
		if ( attachPos.Distance( Entity.AimRay.Position ) > PickupRange )
		{
			if ( Game.IsServer )
			{
				GameLogSystem.PlayerLoseGrip( To.Single(Entity), PickupTarget.Entity );
			}
			PickupEnd();
		}
	}

	private void Throw()
	{
		if ( EffectiveStrength < 0 || !Entity.ActorComponent.CanAffordStaminaCost( StaminaThrowCost ))
		{
			PickupEnd();
			return;
		}
		
		Entity.ActorComponent.PayStamina( StaminaThrowCost );

		//always add a little bit if we can actually throw something
		var force = ThrowForceBase + EffectiveStrength * ThrowForcePerStrength;
		if ( HeldBody.PhysicsGroup.BodyCount > 1 )
		{
			// Don't throw ragdolls as hard
			HeldBody.PhysicsGroup.ApplyImpulse( Entity.AimRay.Forward * (force * 0.5f), true );
			HeldBody.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * force, true );
		}
		else
		{
			HeldBody.ApplyImpulse( Entity.AimRay.Forward * (HeldBody.Mass * force) );
			HeldBody.ApplyAngularImpulse( Vector3.Random * (HeldBody.Mass * force) );
		}

		using ( Prediction.Off() )
		{
			if (Game.IsServer)
				Sound.FromWorld( "interact.throw", Entity.AimRay.Position );
		}
			
		
		if (Game.IsClient)
			GameLogSystem.PlayerThrowObject( PickupTarget );
		
		PickupEnd(false, false);
	}
}
