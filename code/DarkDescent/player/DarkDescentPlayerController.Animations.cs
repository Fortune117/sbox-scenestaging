using DarkDescent.UI;
using Sandbox;
using Sandbox.UI;

namespace DarkDescent;

public partial class DarkDescentPlayerController 
{
	private void HookupAnimEvents()
	{
		Body.SceneObject.OnGenericEvent += OnGenericAnimEvent;
	}

	private float comboDelay => 1f;

	private float windupFraction =  0.2857f;
	private float windupAndReleaseFraction = 0.571f;
	
	private float mainAttackSpeedScale;
	private float comboAttackSpeed;

	private bool isDoingCombo;
	private bool isAttacking;
	private int attackSide;
	private Vector2 lockedInputVector = Vector2.Zero;
	private const int inputVectorBufferSize = 5;
	private Vector2[] inputVectorBuffer = new Vector2[inputVectorBufferSize];
	private int count = 0;
	private TimeUntil TimeUntilNextAttack;
	private TimeUntil TimeUntilCanCombo;
	private TimeUntil TimeUntilComboInvalid;
	private TimeSince TimeSinceAttackStarted;
	private TimeSince TimeSinceComboStarted;

	private string mainAttack;
	private string comboAttack;
	private bool bufferedAttack;
	
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
			if ( TimeSinceAttackStarted < CarriedItemComponent.WindupTime )
			{
				mainAttackSpeedScale = 0.5f / CarriedItemComponent.WindupTime;
				modelComponent.Set( mainAttack, mainAttackSpeedScale );
			}
			else if ( TimeSinceAttackStarted < CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime )
			{
				mainAttackSpeedScale = 0.5f/CarriedItemComponent.ReleaseTime;
				modelComponent.Set( mainAttack,  mainAttackSpeedScale );
			}
			else
			{
				mainAttackSpeedScale = 1;
				modelComponent.Set( mainAttack,  mainAttackSpeedScale );
				isAttacking = false;
			}
		}

		if ( isDoingCombo )
		{
			if ( TimeSinceComboStarted < CarriedItemComponent.WindupTime )
			{
				comboAttackSpeed = 0.5f/CarriedItemComponent.WindupTime;
				modelComponent.Set( comboAttack, comboAttackSpeed );
			}
			else if ( TimeSinceComboStarted < CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime )
			{
				comboAttackSpeed = 0.5f / CarriedItemComponent.ReleaseTime;
				modelComponent.Set( comboAttack,  comboAttackSpeed );
			}
			else
			{
				comboAttackSpeed = 1f;
				modelComponent.Set( comboAttack, comboAttackSpeed  );
				isDoingCombo = false;
			}
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

			if ( isDoingCombo )
			{
				TimeSinceAttackStarted = TimeSinceComboStarted;
				TimeSinceComboStarted = 0;
				
				mainAttack = attackSide == 0 ? "fSpeedScaleAttackRight" : "fSpeedScaleAttackLeft";
				comboAttack = attackSide == 1 ? "fSpeedScaleAttackRight" : "fSpeedScaleAttackLeft";
			}
			else
			{
				TimeSinceComboStarted = 0;
				
				mainAttack = attackSide == 0 ? "fSpeedScaleAttackRight" : "fSpeedScaleAttackLeft";
				comboAttack = attackSide == 1 ? "fSpeedScaleAttackRight" : "fSpeedScaleAttackLeft";
			}
			
			Game.SetRandomSeed( count++ );
			modelComponent.Set( "fSwingBlend", -average.y );
			modelComponent.Set( "eAttackSide",  attackSide );
			modelComponent.Set( "bCombo", true );
			modelComponent.Set( "bAttack", true );
			
			isAttacking = true;
			isDoingCombo = true;
			
			TimeUntilNextAttack = (wait + 1f) / ActorComponent.Stats.ActionSpeed;
			TimeUntilCanCombo = wait / ActorComponent.Stats.ActionSpeed;
			TimeUntilComboInvalid = wait + CarriedItemComponent.RecoveryTime;

			Crosshair.SetAimPipVector( average.WithX( attackSide.Remap( 0, 1, -1, 1 ) ) );

			return;
		}

		if ( !Input.Down( "Attack1" ) || isAttacking || isDoingCombo )
			return;

		if ( !TimeUntilNextAttack && !bufferedAttack )
		{
			bufferedAttack = true;
			return;
		}
		
		attackSide = MathF.Sign( average.x ).Remap( -1, 1, 0, 1 );

		lockedInputVector = average;
		
		Game.SetRandomSeed( count++ );
		
		modelComponent.Set( "fSwingBlend", -average.y );
		modelComponent.Set( "eAttackSide",  attackSide );
		modelComponent.Set( "bAttack", true );
		
		mainAttack = attackSide == 1 ? "fSpeedScaleAttackRight" : "fSpeedScaleAttackLeft";
		
		TimeUntilNextAttack = (wait + 1f) / ActorComponent.Stats.ActionSpeed;
		TimeUntilCanCombo = wait / ActorComponent.Stats.ActionSpeed;
		TimeUntilComboInvalid = wait + CarriedItemComponent.RecoveryTime;
		TimeSinceAttackStarted = 0;
		mainAttackSpeedScale = 0;
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
