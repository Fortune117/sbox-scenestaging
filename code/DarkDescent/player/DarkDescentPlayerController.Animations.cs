﻿using DarkDescent.UI;
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
	private TimeSince TimeSinceAttackStarted;
	private TimeSince TimeSinceComboStarted;

	private string mainAttack;
	private string comboAttack;
	
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
				if (!hitboxesActive)
					OnAttackStart();
				
				mainAttackSpeedScale = 0.5f/CarriedItemComponent.ReleaseTime;
				modelComponent.Set( mainAttack,  mainAttackSpeedScale );
			}
			else
			{
				if (hitboxesActive)
					OnAttackEnd();
				
				mainAttackSpeedScale = 1;
				modelComponent.Set( mainAttack,  mainAttackSpeedScale );
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
				if (!hitboxesActive)
					OnAttackStart();

				comboAttackSpeed = 0.5f / CarriedItemComponent.ReleaseTime;
				modelComponent.Set( comboAttack,  comboAttackSpeed );
			}
			else
			{
				if (hitboxesActive)
					OnAttackEnd();
			
				comboAttackSpeed = 1f;
				modelComponent.Set( comboAttack, comboAttackSpeed  );
			}
		}
		
		if ( TimeUntilNextAttack )
		{
			isDoingCombo = false;
			isAttacking = false;
		}

		if ( !isAttacking )
		{
			Crosshair.SetAimPipVector( average );
		}
		
		var wait = CarriedItemComponent.WindupTime + CarriedItemComponent.ReleaseTime;
		
		if ( !TimeUntilNextAttack && TimeUntilCanCombo && Input.Down( "Attack1" ) )
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
			
			isAttacking = true;
			isDoingCombo = true;
			
			TimeUntilNextAttack = (wait + 1f) / ActorComponent.Stats.ActionSpeed;
			TimeUntilCanCombo = wait / ActorComponent.Stats.ActionSpeed;

			Crosshair.SetAimPipVector( average.WithX( attackSide.Remap( 0, 1, -1, 1 ) ) );

			return;
		}
		
		if ( !TimeUntilNextAttack || !Input.Down( "Attack1" ) )
			return;
		
		attackSide = MathF.Sign( average.x ).Remap( -1, 1, 0, 1 );

		lockedInputVector = average;
		
		Game.SetRandomSeed( count++ );
		
		modelComponent.Set( "fSwingBlend", -average.y );
		modelComponent.Set( "eAttackSide",  attackSide );
		modelComponent.Set( "bAttack", true );
		
		mainAttack = attackSide == 1 ? "fSpeedScaleAttackRight" : "fSpeedScaleAttackLeft";
		
		TimeUntilNextAttack = (wait + 1f) / ActorComponent.Stats.ActionSpeed;
		TimeUntilCanCombo = wait / ActorComponent.Stats.ActionSpeed;
		TimeSinceAttackStarted = 0;
		mainAttackSpeedScale = 0;

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
