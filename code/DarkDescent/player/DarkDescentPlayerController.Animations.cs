using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController 
{
	private void HookupAnimEvents()
	{
		Body.SceneObject.OnGenericEvent += OnGenericAnimEvent;
	}

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

		inputVectorBuffer = inputVectorBuffer.Prepend( Input.MouseDelta ).Take( inputVectorBufferSize ).ToArray();
		
		var average = Vector2.Zero;
		foreach ( var inputVector in inputVectorBuffer )
		{
			average += inputVector;
		}
		average /= inputVectorBuffer.Length;
		average = average.Normal;
		
		if ( !TimeUntilNextAttack || !Input.Down( "Attack1" ) )
			return;
		
		var side = MathF.Sign( average.x ) + 1;
		
		Game.SetRandomSeed( count++ );
		modelComponent.Set( "fSwingBlend", -average.y );
		modelComponent.Set( "eAttackSide",  side );
		modelComponent.Set( "bAttack", true );
		
		TimeUntilNextAttack = 1.75f / ActorComponent.Stats.ActionSpeed;
	}

	protected override void OnPreRender()
	{
		base.OnPreRender();
		if ( !Body.TryGetComponent<AnimatedModelComponent>( out var modelComponent ) )
			return;
		
		modelComponent.Set( "vLeftHandIKTarget", LeftIKTarget.Transform.Position );
	}
}
