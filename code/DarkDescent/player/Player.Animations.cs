using Sandbox;

namespace DarkDescent;

public partial class Player
{
	private TimeSince TimeSinceFootShuffle = 0;
	private void SimulateAnimations()
	{
		var animHelper = new CitizenAnimationHelper( this );

		bool sitting = Controller.HasTag( "sitting" );
		bool noclip = Controller.HasTag( "noclip" ) && !sitting;
		
		animHelper.IsClimbing = Controller.HasTag( "climbing" );
		animHelper.IsGrounded = GroundEntity != null || noclip;
		animHelper.IsSitting = sitting;
		animHelper.IsSwimming = this.GetWaterLevel() > 0.5f && !sitting;
		animHelper.IsNoclipping = noclip;
		animHelper.AimAngle = ViewAngles.ToRotation();
		animHelper.DuckLevel = (1 - Controller.DuckFraction).Remap( 0, 0.35f );
		
		Rotation = Rotation.LookAt( ViewAngles.Forward.WithZ( 0 ));
		
		animHelper.WithLookAt( AimRay.Position + Rotation.Forward * 200 );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( Controller.WishVelocity );

		var pelvisAngle = 1 - ViewAngles.pitch.Remap( -89, 89, 0.01f, 0.99f );
		
		SetAnimParameter( "fPelvisAngle", pelvisAngle );
		SetAnimParameter( "bCrouching", Controller.IsDucking );
		SetAnimParameter( "fMoveSpeed", Velocity.Length / ( Controller.IsDucking ? 70f : 150f) );
		SetAnimParameter( "bAttack", Input.Pressed( GameTags.Input.AttackPrimary ));
	}
}
