using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Cameras;
using DarkDescent.UI;
using Sandbox;

namespace DarkDescent.Weapons;

public partial class CarriedWeaponComponent
{
	private const int inputVectorBufferSize = 5;

	public DarkDescentPlayerController PlayerController { get; set; }
	
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

	public override bool HasPriority => isAttacking;

	public override void UpdateForPlayer()
	{
		AttackUpdate();
		
		if (isAttacking)
			AttackHitUpdate();
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
			
			PlayerController.Body.Set( "fHitStopSpeedScale", HitStopSpeedScale );
			
			if (SwordTrail.ParticleSystem is not null)
				SwordTrail.ParticleSystem.PlaybackSpeed = HitStopSpeedScale;
		}
		
		TimeSinceAttackStarted += Time.Delta * mult;
		TimeSinceComboStarted += Time.Delta * mult;
		TimeUntilNextAttack -= Time.Delta * mult;
		TimeUntilCanCombo -= Time.Delta * mult;
		TimeUntilComboInvalid -= Time.Delta * mult;
		
		//don't think at all if our attack was stopped (i.e. interrupted)
		if ( attackStopped && TimeSinceAttackStopped < RecoveryTime )
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

		var actionSpeed = PlayerController.ActorComponent.Stats.ActionSpeed; 
		
		var windupTime = WindupTime;
		windupTime /= actionSpeed;

		var windUpAndRelease = WindupTime + ReleaseTime;
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
		
		PlayerController.BlockerComponent.SetActive( isBlocking );
		PlayerController.Body.Set( "bBlocking", isBlocking );

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
		if ( !hitboxesActive )
			return;
		
		var hit = attackEvent.CheckForHit();

		if ( hit is null )
		{
			StopScrapeEffect();
			return;
		}

		var hitEvent = hit.Value;
		
		var knockback = PlayerController.ActorComponent.Stats.KnockBack;

		var damage = GetDamage( PlayerController.ActorComponent );
		
		var damageEvent = new DamageEventData()
			.WithOriginator( PlayerController.ActorComponent )
			.WithTarget( hitEvent.Damageable )
			.UsingTraceResult( hitEvent.TraceResult )
			.WithDirection( GetImpactDirection() )
			.WithKnockBack( knockback )
			.WithDamage( damage )
			.WithType( GetDamageType() )
			.AsCritical( false );
		
		if ( hitEvent.HitWorld ) //impacted the world?
		{
			if ( hitEvent.TraceResult.Fraction < BounceFraction )
			{
				StopScrapeEffect();
				BounceAttack(hitEvent.TraceResult);
			}
			else
			{
				PlayScrapeEffect( hitEvent.TraceResult );
				
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
			Sound.FromWorld( ImpactSound.ResourceName, hitEvent.TraceResult.HitPosition );
		
		if ( hitEvent.Damageable.CauseHitBounce )
		{
			BounceAttack(hitEvent.TraceResult);
			return;
		}
	}

	private void DoHitStop()
	{
		TimeSinceLastHit = 0;
		HitStopSpeedScale = 0.05f;

		if ( !PlayerController.Camera.TryGetComponent<CameraShake>( out var cameraShake ) )
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
		PlayerController.Body.Set( "fHitStopSpeedScale", 0f );
		PlayerController.Body.Set( "bAttackStopped", true );
		DeactivateAttack();
	}
	
	private void BeginAttack( Vector2 inputVector, bool isCombo = false )
	{
		var actionSpeed = PlayerController.ActorComponent.Stats.ActionSpeed;
		var windUpAndRelease = WindupTime + ReleaseTime;
		windUpAndRelease /= actionSpeed;
		
		var windupSpeedScale = 0.5f / WindupTime;
		windupSpeedScale *= actionSpeed;

		var releaseSpeedScale = 0.5f / ReleaseTime;
		releaseSpeedScale *= actionSpeed;
		
		PlayerController.Body.Set( "fWindupSpeedScale", windupSpeedScale );
		PlayerController.Body.Set( "fReleaseSpeedScale",  releaseSpeedScale );
		PlayerController.Body.Set( "fRecoverySpeedScale",  1 );
		
		if ( isCombo )
		{
			isDoingCombo = true;
			TimeSinceComboStarted = 0;
			
			attackSide++;
			attackSide %= 2;
			
			inputVector = inputVector.WithX( attackSide.Remap( 0, 1, -1, 1 ) );
			
			PlayerController.Body.Set( "fSwingBlend", -inputVector.y );
			PlayerController.Body.Set( "eAttackSide",  attackSide );
			
			PlayerController.Body.Set( "bCombo", true );
			
			//make sure hitboxes are turned off when we start our combo
			DeactivateAttack();
		}
		else
		{
			attackSide = MathF.Sign( inputVector.x ).Remap( -1, 1, 0, 1 );
			
			TimeSinceAttackStarted = 0;
			
			PlayerController.Body.Set( "fSwingBlend", -inputVector.y );
			PlayerController.Body.Set( "eAttackSide",  attackSide );
			
			PlayerController.Body.Set( "bAttack", true );
		}
		
		HitStopSpeedScale = 1;
		TimeUntilNextAttack = windUpAndRelease + RecoveryTime / PlayerController.ActorComponent.Stats.ActionSpeed ;
		TimeUntilCanCombo = windUpAndRelease;
		TimeUntilComboInvalid = windUpAndRelease + (RecoveryTime/2f)/ PlayerController.ActorComponent.Stats.ActionSpeed;

		isAttacking = true;
		attackStopped = false;
		bufferedAttack = false;
		
		Crosshair.SetAimPipVector( inputVector );
	}

	private void ActivateAttack()
	{
		attackEvent = new AttackEvent()
			.WithInitiator( PlayerController.GameObject )
			.WithHurtBox( HurtBox );
		
		hitboxesActive = true;

		SwordTrail.StartTrail();

		var sound = Sound.FromWorld( SwingSound.ResourceName, PlayerController.AimRay.Position );
		sound.SetPitch( PlayerController.ActorComponent.Stats.ActionSpeed.Remap( 0, 2, 0.5f, 1.5f ) );
	}

	private void DeactivateAttack()
	{
		hitboxesActive = false;
		
		SwordTrail.StopTrail();
	}
	
	private void InterruptAttack()
	{
		if ( !isAttacking )
			return;
		
		isAttacking = false;
		attackStopped = true;
		TimeSinceAttackStopped = 0;
		PlayerController.Body.Set( "bInterruptAttack", true );
		DeactivateAttack();
	}

	public override Vector2 UpdateInputForPlayer( Vector2 input )
	{
		if ( isAttacking )
		{
			input = new Vector2( input.x.Clamp( -TurnCapX * Time.Delta, TurnCapX * Time.Delta ),
				input.y.Clamp( -TurnCapY * Time.Delta, TurnCapY * Time.Delta ) );
		}

		return input;
	}
}
