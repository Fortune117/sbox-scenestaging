using System.Collections.Generic;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.UI;
using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	private static class PlayerEvents
	{
		public const string AttackStartEvent = "AttackStartEvent";
		public const string AttackEndEvent = "AttackEndEvent";
	}
	
	private enum HoldTypes
	{
		Fists,
		TwoHandedSword
	}
	
	[Property]
	private CarriedWeaponComponent CarriedItemComponent { get; set; }
	
	private bool isDoingCombo;
	private bool isAttacking;
	private int attackSide;
	
	private bool bufferedAttack;
	private bool attackStopped;
	
	private Vector2[] inputVectorBuffer = new Vector2[inputVectorBufferSize];

	private TimeUntil TimeUntilNextAttack;
	private TimeUntil TimeUntilCanCombo;
	private TimeUntil TimeUntilComboInvalid;
	private TimeSince TimeSinceAttackStarted;
	private TimeSince TimeSinceComboStarted;

	private float HitStopSpeedScale = 1f;
	private TimeSince TimeSinceLastHit;
	private TimeSince TimeSinceAttackStopped;
	
	private void OnGenericAnimEvent( SceneModel.GenericEvent genericEvent )
	{
		/*switch ( genericEvent.Type )
		{
			case PlayerEvents.AttackStartEvent:
				OnAttackStart();
				break;
			case PlayerEvents.AttackEndEvent:
				OnAttackEnd();
				break;
		}*/
	}

	public void OnBlock()
	{
		Body.Set( "bBlockImpact", true );
	}

	private bool hitboxesActive;
	private readonly HashSet<IDamageable> hitDamageables = new();

	private bool isBlocking;

	private void AttackUpdate()
	{
		if ( attackStopped && TimeSinceAttackStopped < CarriedItemComponent.RecoveryTime )
			return;
		
		if ( TimeSinceLastHit > 0.09f && !attackStopped )
			HitStopSpeedScale = 1f;
		
		if (!attackStopped)
			Body.Set( "fHitStopSpeedScale", HitStopSpeedScale );
		
		if (Input.MouseDelta.Length > 0.1f)
			inputVectorBuffer = inputVectorBuffer.Prepend( Input.MouseDelta ).Take( inputVectorBufferSize ).ToArray();
		
		var average = Vector2.Zero;
		foreach ( var inputVector in inputVectorBuffer )
		{
			average += inputVector;
		}
		average /= inputVectorBuffer.Length;
		average = average.Normal;

		var actionSpeed = ActorComponent.Stats.ActionSpeed; 
		
		var windupTime = CarriedItemComponent.WindupTime;
		windupTime /= actionSpeed;

		var windUpAndRelease = CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime;
		windUpAndRelease /= actionSpeed;
		
		if ( isAttacking && !isDoingCombo )
		{
			if ( TimeSinceAttackStarted > windupTime && TimeSinceAttackStarted < windUpAndRelease )
			{
				if (!hitboxesActive)
					ActivateHitBoxes();
			}
			else if (hitboxesActive)
			{
				DeactivateHitBoxes();
			}

			if ( TimeSinceAttackStarted > windUpAndRelease + 0.05f )
			{
				isAttacking = false;
			}
		}

		if ( isDoingCombo )
		{ 
			if ( TimeSinceComboStarted > windupTime && TimeSinceComboStarted < windUpAndRelease )
			{
				if (!hitboxesActive)
					ActivateHitBoxes();
			}
			else if (hitboxesActive)
			{
				DeactivateHitBoxes();
			}

			if ( TimeSinceComboStarted > windUpAndRelease + 0.05f )
			{
				isAttacking = false;
				isDoingCombo = false;
			}
		}

		if ( !isAttacking )
		{
			Crosshair.SetAimPipVector( average );

			isBlocking = Input.Down( "Attack2" );
		}
		else
		{
			isBlocking = false;
		}
		
		CarriedItemComponent.SetBlockStatus( isBlocking );
		Body.Set( "bBlocking", isBlocking );
		
		if ( !attackStopped && !TimeUntilNextAttack && TimeUntilCanCombo && !TimeUntilComboInvalid && Input.Down( "Attack1" ) )
		{
			BeginAttack( average, true );

			return;
		}

		if ( !Input.Down( "Attack1" ) && !bufferedAttack  )
			return;

		if ( !TimeUntilNextAttack )
		{
			if ( TimeUntilComboInvalid )
				bufferedAttack = true;
			
			return;
		}
		
		BeginAttack( average, false );
	}
	
	private void AttackHitUpdate()
	{
		var tr = CarriedItemComponent.GetWeaponTrace();

		if ( tr.Hit )
		{
			Gizmo.Transform = Scene.Transform.World;
			Gizmo.Draw.Color = Color.Red.WithAlpha( 0.5f );
			Gizmo.Draw.SolidSphere( tr.EndPosition, 4 );
		}
		
		if ( !hitboxesActive )
			return;
		
		if ( !tr.Hit )
			return;

		var gameObject = tr.Body.GameObject;
		if ( gameObject is not GameObject hitGameObject )
			return;

		var damageable = hitGameObject.GetComponentInParent<IDamageable>( true, true );
		if ( damageable is null ) //impacted the world?
		{
			if (tr.Fraction < CarriedItemComponent.BounceFraction)
				BounceAttack();
			return;
		}

		if ( damageable == ActorComponent )
			return;

		if ( hitDamageables.Contains( damageable ) )
			return;

		hitDamageables.Add( damageable );

		DoHitStop();
		
		var knockback = ActorComponent is not null ? ActorComponent.Stats.KnockBack : 0;

		var damage = new DamageEventData()
			.WithOriginator( ActorComponent )
			.WithPosition( tr.HitPosition + tr.Normal * 5f )
			.WithDirection( CarriedItemComponent.GetImpactDirection() )
			.WithKnockBack( knockback )
			.WithDamage( 1f )
			//.WithTags( tr.Shape.Tags ) //commented out cause we can't get tags like this :<
			.WithType( DamageType.Physical )
			.AsCritical( false );

		damageable.TakeDamage( damage );
		
		if ( damageable.CauseHitBounce )
		{
			BounceAttack();
			return;
		}
	}

	private void DoHitStop()
	{
		TimeSinceLastHit = 0;
		HitStopSpeedScale = 0.05f;
	}
	
	/// <summary>
	/// Our attack was stopped short, cancel the attack and play a lil bounce animation.
	/// </summary>
	private void BounceAttack()
	{
		isAttacking = false;
		attackStopped = true;
		TimeSinceAttackStopped = 0;
		Body.Set( "fHitStopSpeedScale", 0f );
		Body.Set( "bAttackStopped", true );
	}
	
	private void BeginAttack( Vector2 inputVector, bool isCombo = false )
	{
		var actionSpeed = ActorComponent.Stats.ActionSpeed;
		var windUpAndRelease = CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime;
		windUpAndRelease /= actionSpeed;
		
		var windupSpeedScale = 0.5f / CarriedItemComponent.WindupTime;
		windupSpeedScale *= actionSpeed;

		var releaseSpeedScale = 0.5f / CarriedItemComponent.ReleaseTime;
		releaseSpeedScale *= actionSpeed;
		
		Body.Set( "fWindupSpeedScale", windupSpeedScale );
		Body.Set( "fReleaseSpeedScale",  releaseSpeedScale );
		Body.Set( "fRecoverySpeedScale",  1 );
		
		if ( isCombo )
		{
			isDoingCombo = true;
			TimeSinceComboStarted = 0;
			
			attackSide++;
			attackSide %= 2;
			
			inputVector = inputVector.WithX( attackSide.Remap( 0, 1, -1, 1 ) );
			
			Body.Set( "fSwingBlend", -inputVector.y );
			Body.Set( "eAttackSide",  attackSide );
			
			Body.Set( "bCombo", true );
			
			//make sure hitboxes are turned off when we start our combo
			DeactivateHitBoxes();
		}
		else
		{
			attackSide = MathF.Sign( inputVector.x ).Remap( -1, 1, 0, 1 );
			
			TimeSinceAttackStarted = 0;
			
			Body.Set( "fSwingBlend", -inputVector.y );
			Body.Set( "eAttackSide",  attackSide );
			
			Body.Set( "bAttack", true );
		}
		
		HitStopSpeedScale = 1;
		TimeUntilNextAttack = windUpAndRelease + CarriedItemComponent.RecoveryTime / ActorComponent.Stats.ActionSpeed ;
		TimeUntilCanCombo = windUpAndRelease;
		TimeUntilComboInvalid = windUpAndRelease + (CarriedItemComponent.RecoveryTime/2f)/ ActorComponent.Stats.ActionSpeed;

		isAttacking = true;
		attackStopped = false;
		bufferedAttack = false;
		
		Crosshair.SetAimPipVector( inputVector );
	}

	private void ActivateHitBoxes()
	{
		hitDamageables.Clear();
		hitboxesActive = true;
	}

	private void DeactivateHitBoxes()
	{
		hitboxesActive = false;
	}
}
