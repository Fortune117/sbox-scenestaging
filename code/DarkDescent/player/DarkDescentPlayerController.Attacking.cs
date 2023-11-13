using System.Collections.Generic;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using DarkDescent.Cameras;
using DarkDescent.UI;
using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController : IDamageTakenListener
{
	private static class PlayerEvents
	{
		public const string AttackStartEvent = "AttackStartEvent";
		public const string AttackEndEvent = "AttackEndEvent";
	}
	
	[Property]
	private CarriedWeaponComponent CarriedItemComponent { get; set; }
	
	[Property]
	private AttackBlockerComponent BlockerComponent { get; set; }
	
	private bool isDoingCombo;
	private bool isAttacking;
	private int attackSide;
	
	private bool bufferedAttack;
	private bool attackStopped;
	
	private Vector2[] inputVectorBuffer = new Vector2[inputVectorBufferSize];

	private float TimeUntilNextAttack;
	private float TimeUntilCanCombo;
	private float TimeUntilComboInvalid;
	private float TimeSinceAttackStarted;
	private float TimeSinceComboStarted;

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

	public void OnBlock(DamageEventData damageEvent, bool isParry)
	{
		Body.Set( "bBlockImpact", true );
		
		foreach ( var blockListener in GetComponents<IBlockListener>(true, true) )
		{
			blockListener.OnBlock( damageEvent, isParry );	
		}
	}

	private bool hitboxesActive;
	private bool isBlocking;

	private void AttackUpdate()
	{
		var mult = 1f;

		if ( !attackStopped )
		{
			if ( TimeSinceLastHit > 0.09f )
				HitStopSpeedScale = 1f;
			
			mult = HitStopSpeedScale;
			
			Body.Set( "fHitStopSpeedScale", HitStopSpeedScale );
			
			if (CarriedItemComponent.SwordTrail.ParticleSystem is not null)
				CarriedItemComponent.SwordTrail.ParticleSystem.PlaybackSpeed = HitStopSpeedScale;
		}
		
		TimeSinceAttackStarted += Time.Delta * mult;
		TimeSinceComboStarted += Time.Delta * mult;
		TimeUntilNextAttack -= Time.Delta * mult;
		TimeUntilCanCombo -= Time.Delta * mult;
		TimeUntilComboInvalid -= Time.Delta * mult;
		
		//don't think at all if our attack was stopped (i.e. interrupted)
		if ( attackStopped && TimeSinceAttackStopped < CarriedItemComponent.RecoveryTime )
			return;
		
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
					ActivateAttack();
			}
			else if (hitboxesActive)
			{
				DeactivateAttack();
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
					ActivateAttack();
			}
			else if (hitboxesActive)
			{
				DeactivateAttack();
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
		
		BlockerComponent.SetActive( isBlocking );
		Body.Set( "bBlocking", isBlocking );

		var canAttack = TimeUntilNextAttack <= 0;
		var canCombo = TimeUntilCanCombo <= 0;
		var comboInvalid = TimeUntilComboInvalid <= 0;
		
		if ( !attackStopped && !canAttack && canCombo && !comboInvalid && Input.Down( "Attack1" ) )
		{
			BeginAttack( average, true );

			return;
		}

		if ( !Input.Down( "Attack1" ) && !bufferedAttack  )
			return;

		if ( !canAttack )
		{
			if ( comboInvalid )
				bufferedAttack = true;
			
			return;
		}
		
		BeginAttack( average, false );
	}


	private TimeSince lastScrapeEvent;
	private AttackEvent attackEvent;
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
		
		var hit = attackEvent.CheckForHit();

		if ( hit is null )
		{
			CarriedItemComponent.StopScrapeEffect();
			return;
		}

		var hitEvent = hit.Value;
		
		var knockback = ActorComponent.Stats.KnockBack;

		var damage = CarriedItemComponent.GetDamage( ActorComponent );
		
		var damageEvent = new DamageEventData()
			.WithOriginator( ActorComponent )
			.WithTarget( hitEvent.Damageable )
			.UsingTraceResult( hitEvent.TraceResult )
			.WithDirection( CarriedItemComponent.GetImpactDirection() )
			.WithKnockBack( knockback )
			.WithDamage( damage )
			.WithType( CarriedItemComponent.GetDamageType() )
			.AsCritical( false );
		
		if ( hitEvent.HitWorld ) //impacted the world?
		{
			if ( hitEvent.TraceResult.Fraction < CarriedItemComponent.BounceFraction )
			{
				CarriedItemComponent.StopScrapeEffect();
				BounceAttack(hitEvent.TraceResult);
			}
			else
			{
				CarriedItemComponent.PlayScrapeEffect( hitEvent.TraceResult );
				
				if ( lastScrapeEvent > 0.02f )
				{
					damageEvent.CreateScrapeEffect();
					lastScrapeEvent = 0; 
				}
			}
			
			return;
		}
		
		if ( hitEvent.WasBlocked )
		{
			damageEvent.WasBlocked = true;
			damageEvent = hitEvent.Blocker.BlockedHit( damageEvent );
		}
		
		DoHitStop();
		
		if ( damageEvent.DamageResult > 0 )
			hitEvent.Damageable.TakeDamage( damageEvent );

		if (hitEvent.Damageable.PlayHitSound)
			Sound.FromWorld( CarriedItemComponent.ImpactSound.ResourceName, hitEvent.TraceResult.HitPosition );
		
		if ( hitEvent.Damageable.CauseHitBounce )
		{
			BounceAttack(tr);
			return;
		}
	}

	private void DoHitStop()
	{
		TimeSinceLastHit = 0;
		HitStopSpeedScale = 0.05f;

		if ( !Camera.TryGetComponent<CameraShake>( out var cameraShake ) )
			return;
		
		cameraShake.AddShake( 4f, 2f, -1f, 0.5f );
	}
	
	/// <summary>
	/// Our attack was stopped short, cancel the attack and play a lil bounce animation.
	/// </summary>
	private void BounceAttack(PhysicsTraceResult traceResult)
	{
		isAttacking = false;
		attackStopped = true;
		TimeSinceAttackStopped = 0;
		Body.Set( "fHitStopSpeedScale", 0f );
		Body.Set( "bAttackStopped", true );
		DeactivateAttack();
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
			DeactivateAttack();
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

	private void ActivateAttack()
	{
		attackEvent = new AttackEvent()
			.WithInitiator( GameObject )
			.WithHurtBox( GetComponent<HurtBoxComponent>( true, true ) );
		
		hitboxesActive = true;

		CarriedItemComponent.SwordTrail.StartTrail();

		var sound = Sound.FromWorld( CarriedItemComponent.SwingSound.ResourceName, Eye.Transform.Position );
		sound.SetPitch( ActorComponent.Stats.ActionSpeed.Remap( 0, 2, 0.5f, 1.5f ) );
	}

	private void DeactivateAttack()
	{
		hitboxesActive = false;
		
		CarriedItemComponent.SwordTrail.StopTrail();
	}

	private void InterruptAttack()
	{
		if ( !isAttacking )
			return;
		
		isAttacking = false;
		attackStopped = true;
		TimeSinceAttackStopped = 0;
		Body.Set( "bInterruptAttack", true );
		DeactivateAttack();
	}

	public void OnDamageTaken( DamageEventData damageEventData, bool isLethal )
	{
		if ( isLethal )
			return;
		
		//InterruptAttack(); 

		ApplyKnockBack( damageEventData );
	}

	private void ApplyKnockBack( DamageEventData damageEventData )
	{
		var knockback = damageEventData.Direction * damageEventData.KnockBackResult;
		CharacterController.Punch( knockback );
	}
}
