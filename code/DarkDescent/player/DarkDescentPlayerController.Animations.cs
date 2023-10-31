using DarkDescent.UI;
using Sandbox;
using Sandbox.UI;

namespace DarkDescent;

public partial class DarkDescentPlayerController 
{
	private const int inputVectorBufferSize = 5;
	
	private void HookupAnimEvents()
	{
		Body.SceneObject.OnGenericEvent += OnGenericAnimEvent;
	}
	
	private bool isDoingCombo;
	private bool isAttacking;
	private int attackSide;
	private int count;
	
	private bool bufferedAttack;
	
	private Vector2 lockedInputVector = Vector2.Zero;
	private Vector2[] inputVectorBuffer = new Vector2[inputVectorBufferSize];

	private TimeUntil TimeUntilNextAttack;
	private TimeUntil TimeUntilCanCombo;
	private TimeUntil TimeUntilComboInvalid;
	private TimeSince TimeSinceAttackStarted;
	private TimeSince TimeSinceComboStarted;

	private float HitStopSpeedScale = 1f;
	private TimeSince TimeSinceLastHit;
	
	private void UpdateAnimations()
	{
		if ( !Body.TryGetComponent<AnimatedModelComponent>( out var modelComponent ) )
			return;
		
		var animHelper = new CitizenSceneAnimationHelper( modelComponent.SceneObject );
		
		animHelper.IsClimbing = false;
		animHelper.IsGrounded = CharacterController.IsOnGround;
		animHelper.IsSitting = false;
		animHelper.IsSwimming = false;
		animHelper.IsNoclipping = false;
		animHelper.AimAngle = internalEyeAngles.ToRotation();
		animHelper.DuckLevel = 1f; //(1 - Event.Controller.DuckFraction).Remap( 0, 0.35f );
		
		animHelper.WithLookAt( AimRay.Position + Body.Transform.Rotation.Forward * 200 );
		animHelper.WithVelocity( CharacterController.Velocity );
		animHelper.WithWishVelocity( WishVelocity );

		var pelvisAngle = 1 - internalEyeAngles.pitch.Remap( -89, 89, 0.01f, 0.99f );
		
		modelComponent.Set( "fPelvisAngle", pelvisAngle );
		modelComponent.Set( "bCrouching", IsCrouching );
		modelComponent.Set( "fMoveSpeed", CharacterController.Velocity.Length / 150f );
		modelComponent.Set( "fActionSpeed", ActorComponent.Stats.ActionSpeed );
		modelComponent.Set( "vLeftHandIKTarget", LeftIKTarget.Transform.Position );

		if (TimeSinceLastHit > 0.02f)
			HitStopSpeedScale = HitStopSpeedScale.Approach( 1f, 3f * Time.Delta );
		
		modelComponent.Set( "fHitStopSpeedScale", HitStopSpeedScale );

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
					OnAttackStart();
			}
			else if (hitboxesActive)
			{
				OnAttackEnd();
			}
		}

		if ( isDoingCombo )
		{ 
			if ( TimeSinceComboStarted > windupTime && TimeSinceComboStarted < windUpAndRelease )
			{
				if (!hitboxesActive)
					OnAttackStart();
			}
			else if (hitboxesActive)
			{
				OnAttackEnd();
			}
		}

		if ( TimeUntilNextAttack )
		{
			isAttacking = false;
		}

		if ( TimeUntilComboInvalid )
		{
			isDoingCombo = false;
		}

		if ( !isAttacking )
		{
			Crosshair.SetAimPipVector( average );
		}
		
		if ( !TimeUntilNextAttack && TimeUntilCanCombo && !TimeUntilComboInvalid && Input.Down( "Attack1" ) )
		{
			attackSide++;
			attackSide %= 2;

			lockedInputVector = -lockedInputVector;
			
			Game.SetRandomSeed( count++ );
			modelComponent.Set( "fSwingBlend", -average.y );
			modelComponent.Set( "eAttackSide",  attackSide );
			modelComponent.Set( "bCombo", true );
			
			HitStopSpeedScale = 1;
			
			isAttacking = true;
			isDoingCombo = true;

			TimeSinceComboStarted = 0;
			TimeUntilNextAttack = windUpAndRelease + CarriedItemComponent.RecoveryTime / ActorComponent.Stats.ActionSpeed ;
			TimeUntilCanCombo = windUpAndRelease;
			TimeUntilComboInvalid = windUpAndRelease + (CarriedItemComponent.RecoveryTime/2f)/ ActorComponent.Stats.ActionSpeed;

			Crosshair.SetAimPipVector( average.WithX( attackSide.Remap( 0, 1, -1, 1 ) ) );
			
			//combo started, make sure our hitboxes turn off
			OnAttackEnd();

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
		
		attackSide = MathF.Sign( average.x ).Remap( -1, 1, 0, 1 );

		lockedInputVector = average;
		
		Game.SetRandomSeed( count++ );
		
		modelComponent.Set( "fSwingBlend", -average.y );
		modelComponent.Set( "eAttackSide",  attackSide );
		modelComponent.Set( "bAttack", true );

		var windupSpeedScale = 0.5f / CarriedItemComponent.WindupTime;
		windupSpeedScale *= actionSpeed;

		var releaseSpeedScale = 0.5f / CarriedItemComponent.ReleaseTime;
		releaseSpeedScale *= actionSpeed;
		
		modelComponent.Set( "fWindupSpeedScale", windupSpeedScale );
		modelComponent.Set( "fReleaseSpeedScale",  releaseSpeedScale );
		modelComponent.Set( "fRecoverySpeedScale",  1 );

		HitStopSpeedScale = 1;
		TimeUntilNextAttack = windUpAndRelease + CarriedItemComponent.RecoveryTime / ActorComponent.Stats.ActionSpeed ;
		TimeUntilCanCombo = windUpAndRelease;
		TimeUntilComboInvalid = windUpAndRelease + (CarriedItemComponent.RecoveryTime/2f)/ ActorComponent.Stats.ActionSpeed;
		TimeSinceAttackStarted = 0;
		
		bufferedAttack = false;
		isAttacking = true;
	}

	protected override void OnPreRender()
	{
		base.OnPreRender();
		if ( !Body.TryGetComponent<AnimatedModelComponent>( out var modelComponent ) )
			return;
		
		modelComponent.Set( "vLeftHandIKTarget", LeftIKTarget.Transform.Position );
	}
}
