using DarkDescent;
using DarkDescent.Actor;
using DarkDescent.Components;
using DarkDescent.GameLog;
using DarkDescent.UI;
using Sandbox;

namespace DarkDescent.Components;

public partial class PhysicsInteractableComponent : IInteractable
{
	
	public IInteractable.InteractionType InteractType { get; set; } = IInteractable.InteractionType.Pickup;
	
	private bool HoldingItem { get; set; }
	
	[Property, Range( 0, 5 )] 
	private float StaminaDrainBase { get; set; } = 1;

	[Property, Range( 0, 5 )] 
	private float StaminaDrainPerMissingStrength { get; set; } = 1;
	
	[Property, Range( 0, 15 )] 
	private float StaminaThrowCost { get; set; } = 5;

	[Property, Range( 0, 100 )] 
	private float ThrowForcePerStrength { get; set; } = 10f;

	[Property, Range( 0, 300 )] 
	private float ThrowForceBase { get; set; } = 300f;
	
	private PhysicsBody HeldBody { get; set; }
	private Vector3 HeldPos { get; set; }
	private Rotation HeldRot { get; set; }
	private Vector3 HoldPos { get; set; }
	private Rotation HoldRot { get; set; }
	
	private DarkDescentPlayerController Carrier { get; set; }
	
	private float GetEffectiveStrength(ActorComponent actorComponent)
	{
		return actorComponent.Stats.Strength - StrengthThreshold;
	}

	private bool CanCarry( ActorComponent actorComponent )
	{
		return actorComponent.Stats.Strength > StrengthThreshold - StrengthLeeway;
	}

	bool IInteractable.CanInteract( DarkDescentPlayerController playerController )
	{
		return CanPickUp && CanCarry(playerController.ActorComponent);
	}
	
	void IInteractable.InteractionUpdate()
	{
		if ( !HoldingItem )
			return;
			
		PickupMove( Carrier.AimRay.Position, Carrier.AimRay.Forward, Carrier.AimRotation );
			
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

	void IInteractable.Interact( DarkDescentPlayerController playerController, SceneTraceResult tr )
	{
		var passedStrengthTest = CanCarry( playerController.ActorComponent );
				
		if ( !passedStrengthTest )
		{
			GameLogSystem.PlayerFailToPickup( this );
			return;
		}
		
		Carrier = playerController;
		Carrier.ActiveInteractable = this;
		
		var attachPos = tr.Body.FindClosestPoint( playerController.AimRay.Position );

		var holdDistance = playerController.HoldRange + attachPos.Distance( tr.Body.MassCenter );
		PickupStart( tr.Body, tr.StartPosition + tr.Direction * holdDistance, Carrier.AimRotation );
	}
	
	private void PickupStart(PhysicsBody body, Vector3 grabPos, Rotation grabRot )
	{
		//PickupEnd();

		HeldBody = body;
		HeldPos = HeldBody.LocalMassCenter;
		HeldRot = grabRot.Inverse * HeldBody.Rotation;

		HoldPos = HeldBody.Position;
		HoldRot = HeldBody.Rotation;

		HeldBody.Sleeping = false;
		HeldBody.AutoSleep = false;

		HoldingItem = true;
		
		GameLogSystem.PlayerPickupObject( this, GetEffectiveStrength( Carrier.ActorComponent ) < 0 );
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
		
		if (useLog)
			GameLogSystem.PlayerDropObject( this );
		
		HoldingItem = false;

		Carrier.ActiveInteractable = null;
		Carrier = null;
	}
	
	private void PickupMove( Vector3 startPos, Vector3 dir, Rotation rot )
	{
		if ( !HeldBody.IsValid() )
			return;
		
		var attachPos = HeldBody.FindClosestPoint( startPos );
		var holdDistance = Carrier.HoldRange + attachPos.Distance( HeldBody.MassCenter );

		HoldPos = startPos - HeldPos * HeldBody.Rotation + dir * holdDistance;
		HoldRot = rot * HeldRot;

		var effectiveStrength = GetEffectiveStrength( Carrier.ActorComponent );

		var staminaCost = StaminaDrainBase +
		                  (effectiveStrength < 0 ? MathF.Abs( effectiveStrength ) * StaminaDrainPerMissingStrength : 0);
		staminaCost *= Time.Delta;
		
		if ( !Carrier.ActorComponent.CanAffordStaminaCost( staminaCost ) )
		{
			GameLogSystem.PlayerStrengthFailPickup(GameObject);
			PickupEnd();
		}
		else
		{
			Carrier.ActorComponent.PayStamina( staminaCost );
		}
		
		if ( !CanCarry( Carrier.ActorComponent ) )
		{
			GameLogSystem.PlayerStrengthFailPickup(GameObject);
			PickupEnd();
		}
		
		if ( attachPos.Distance( Carrier.AimRay.Position ) > Carrier.InteractRange )
		{
			GameLogSystem.PlayerLoseGrip(GameObject);
			PickupEnd();
		}
	}
	
	private void Throw()
	{
		var effectiveStrength = GetEffectiveStrength( Carrier.ActorComponent );
		
		if ( effectiveStrength < 0 || !Carrier.ActorComponent.CanAffordStaminaCost( StaminaThrowCost ))
		{
			PickupEnd();
			return;
		}
		
		Carrier.ActorComponent.PayStamina( StaminaThrowCost );

		//always add a little bit if we can actually throw something
		var force = ThrowForceBase + effectiveStrength * ThrowForcePerStrength;
		if ( HeldBody.PhysicsGroup?.BodyCount > 1 )
		{
			// Don't throw ragdolls as hard
			HeldBody.PhysicsGroup.ApplyImpulse( Carrier.AimRay.Forward * (force * 0.5f), true );
			HeldBody.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * force, true );
		}
		else
		{
			HeldBody.ApplyImpulse( Carrier.AimRay.Forward * (HeldBody.Mass * force) );
			HeldBody.ApplyAngularImpulse( Vector3.Random * (HeldBody.Mass * force) );
		}

		Sound.Play( "interact.throw", Carrier.AimRay.Position );
		GameLogSystem.PlayerThrowObject( this );
		
		PickupEnd(false, false);
	}
	
	public void OnPrePhysicsStep()
	{
		if ( !HeldBody.IsValid() )
			return;
		
		var effectiveStrength = GetEffectiveStrength( Carrier.ActorComponent );

		var smoothTime = 0.05f;

		if ( effectiveStrength < 0 )
		{
			var fraction = MathF.Abs( effectiveStrength ) / StrengthLeeway;
			
			smoothTime = fraction.Remap( 0, 1, 0.05f, 1.5f );
		}

		var velocity = HeldBody.Velocity;
		Vector3.SmoothDamp( HeldBody.Position, HoldPos, ref velocity, smoothTime, Time.Delta );
		HeldBody.Velocity = velocity;

		var angularVelocity = HeldBody.AngularVelocity;
		GameObject.Transform.Rotation = Rotation.SmoothDamp( HeldBody.Rotation, HoldRot, ref angularVelocity, smoothTime, Time.Delta );
		HeldBody.AngularVelocity = angularVelocity;

		if ( HoldPos.Distance( HeldBody.Position ) > 60f )
		{
			GameLogSystem.PlayerLoseGrip(GameObject);
			PickupEnd();
		}
	}
}
