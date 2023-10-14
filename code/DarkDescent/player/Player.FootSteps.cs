using Sandbox;

namespace DarkDescent;

public partial class Player
{
	private TimeSince timeSinceLastFootstep = 0;

	public virtual float FootstepVolume()
	{
		var speed = Velocity.WithZ(0).Length;

		var value = speed.LerpInverse( 0.0f, 150.0f, false ) * 15f;

		if (speed <= 120)
			value /= 2f;

		return value;
	}
	
	/// <summary>
	/// A foostep has arrived!
	/// </summary>
	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( LifeState != LifeState.Alive )
			return;

		if ( !Game.IsClient )
			return;

		if ( timeSinceLastFootstep < 0.2f )
			return;

		volume *= FootstepVolume();

		timeSinceLastFootstep = 0;

		if ( !TryFootstepSound(foot, volume) ) 
			return;

		//PlayMovementNoise(volume * 0.025f);
		
		//ActiveWeapon?.OnFootStep();
	}

	public bool TryFootstepSound(int foot = 0, float volume = 1)
	{
		var tr = Trace.Ray( Position, Position + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) 
			return false;

		tr.Surface.DoFootstep( this, tr, foot, volume );

		return true;
	}
}
