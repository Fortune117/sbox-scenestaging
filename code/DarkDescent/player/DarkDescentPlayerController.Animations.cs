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

	private bool isAttacking;
	private Vector2 lockedInputVector = Vector2.Zero;
	private const int inputVectorBufferSize = 5;
	private Vector2[] inputVectorBuffer = new Vector2[inputVectorBufferSize];
	private int count = 0;
	private TimeUntil TimeUntilNextAttack;
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

		if ( TimeUntilNextAttack )
			isAttacking = false;

		if ( !isAttacking )
		{
			var angle = average.Degrees - 90;
			angle = angle.NormalizeDegrees();
			
			Log.Info( angle );
			
			var transform = new PanelTransform();
			transform.AddRotation( new Vector3( 0, 0, angle ) );
			transform.AddTranslate( Length.Percent( -50 ), Length.Percent( -50 ) );

			Crosshair.Instance.AimPip.Style.TransformOriginX = Length.Percent( -100 );
			Crosshair.Instance.AimPip.Style.TransformOriginY = Length.Percent( -100 );
			Crosshair.Instance.AimPip.Style.Transform = transform;
		}
		
		if ( !TimeUntilNextAttack || !Input.Down( "Attack1" ) )
			return;
		
		var side = MathF.Sign( average.x ) + 1;

		lockedInputVector = average;
		
		Game.SetRandomSeed( count++ );
		modelComponent.Set( "fSwingBlend", -average.y );
		modelComponent.Set( "eAttackSide",  side );
		modelComponent.Set( "bAttack", true );
		
		TimeUntilNextAttack = 1.75f / ActorComponent.Stats.ActionSpeed;

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
