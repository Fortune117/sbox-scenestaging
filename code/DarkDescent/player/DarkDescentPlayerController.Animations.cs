using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController 
{
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
		animHelper.AimAngle = EyeAngles.ToRotation();
		animHelper.DuckLevel = 1f; //(1 - Event.Controller.DuckFraction).Remap( 0, 0.35f );
		
		animHelper.WithLookAt( AimRay.Position + Body.Transform.Rotation.Forward * 200 );
		animHelper.WithVelocity( CharacterController.Velocity );
		animHelper.WithWishVelocity( WishVelocity );

		var pelvisAngle = 1 - EyeAngles.pitch.Remap( -89, 89, 0.01f, 0.99f );
		
		modelComponent.Set( "fPelvisAngle", pelvisAngle );
		modelComponent.Set( "bCrouching", false );
		modelComponent.Set( "fMoveSpeed", CharacterController.Velocity.Length / 150f );
		modelComponent.Set( "bAttack", Input.Pressed( "Attack1" ));
	}
}
