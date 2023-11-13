using DarkDescent.UI;
using DarkDescent.Weapons;
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
	
	private void UpdateAnimations()
	{
		var animHelper = new CitizenSceneAnimationHelper( Body.SceneObject );
		
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
		
		Body.Set( "fPelvisAngle", pelvisAngle );
		Body.Set( "bCrouching", IsCrouching );
		Body.Set( "fMoveSpeed", CharacterController.Velocity.Length / 150f );
		Body.Set( "fActionSpeed", ActorComponent.Stats.ActionSpeed );
		
		Body.Set( "eHoldType", (int)CarriedItemComponent.HoldType);  
		Body.Set( "eHandedness", (int)CarriedItemComponent.Handedness);  
	}

	protected override void OnPreRender()
	{
		base.OnPreRender();

		if ( CarriedItemComponent.Handedness != Handedness.TwoHanded )
			return;
		
		Body.Set( "vLeftHandIKTarget", CarriedItemComponent.LeftHandIKTarget.Transform.Position );
	}
}
