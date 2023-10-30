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
	
	private void UpdateAnimations()
	{
		if ( !Body.TryGetComponent<AnimatedModelComponent>( out var modelComponent ) )
			return;
		
		var animHelper = new CitizenSceneAnimationHelper( modelComponent.SceneObject );

		Scene.TimeScale = 1f;
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

		if (Input.MouseDelta.Length > 0.1f)
			inputVectorBuffer = inputVectorBuffer.Prepend( Input.MouseDelta ).Take( inputVectorBufferSize ).ToArray();
		
		var average = Vector2.Zero;
		foreach ( var inputVector in inputVectorBuffer )
		{
			average += inputVector;
		}
		average /= inputVectorBuffer.Length;
		average = average.Normal;
		
		if ( isAttacking )
		{
			if ( TimeSinceAttackStarted < CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime )
			{
				if (!hitboxesActive)
					OnAttackStart();
			}
			else if (hitboxesActive && !isDoingCombo)
			{
				OnAttackEnd();
			}
		}

		if ( isDoingCombo )
		{ 
			if ( TimeSinceComboStarted < CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime )
			{
				if (!hitboxesActive)
					OnAttackStart();
			}
			else if (hitboxesActive)
			{
				Log.Info( "test" );
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
		
		var wait = CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime;
		
		if ( !TimeUntilNextAttack && TimeUntilCanCombo && !TimeUntilComboInvalid && Input.Down( "Attack1" ) )
		{
			attackSide++;
			attackSide %= 2;

			lockedInputVector = -lockedInputVector;
			
			Game.SetRandomSeed( count++ );
			modelComponent.Set( "fSwingBlend", -average.y );
			modelComponent.Set( "eAttackSide",  attackSide );
			modelComponent.Set( "bCombo", true );
			
			isAttacking = true;
			isDoingCombo = true;

			TimeSinceComboStarted = 0;
			TimeUntilNextAttack = (wait + CarriedItemComponent.RecoveryTime) / ActorComponent.Stats.ActionSpeed;
			TimeUntilCanCombo = wait / ActorComponent.Stats.ActionSpeed;
			TimeUntilComboInvalid = wait + CarriedItemComponent.RecoveryTime/2f;

			Crosshair.SetAimPipVector( average.WithX( attackSide.Remap( 0, 1, -1, 1 ) ) );

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
		
		modelComponent.Set( "fWindupSpeedScale", 0.5f / CarriedItemComponent.WindupTime );
		modelComponent.Set( "fReleaseSpeedScale",  0.5f / CarriedItemComponent.ReleaseTime );
		modelComponent.Set( "fRecoverySpeedScale",  1 );
		
		TimeUntilNextAttack = (wait + CarriedItemComponent.RecoveryTime) / ActorComponent.Stats.ActionSpeed;
		TimeUntilCanCombo = wait / ActorComponent.Stats.ActionSpeed;
		TimeUntilComboInvalid = wait + CarriedItemComponent.RecoveryTime/2f;
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
