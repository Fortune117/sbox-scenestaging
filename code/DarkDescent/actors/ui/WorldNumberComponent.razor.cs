using Sandbox;
using Sandbox.Razor;

namespace DarkDescent.Actor.UI;

public partial class WorldNumberComponent
{
	protected Vector3 Velocity { get; set; }
	
	protected virtual void CalculateVelocity() { }
	
	private TimeSince TimeSinceCreated = 0;

	protected override void OnUpdate()
	{
		var angles = Rotation.LookAt( -Camera.Main.Rotation.Forward ).Angles();
		Transform.Rotation = angles.WithPitch( 0 ).ToRotation();

		var tr = Physics.Trace.Ray( Transform.Position, Transform.Position + Velocity * Time.Delta * 50f )
			.Radius( 2f )
			.Run();

		if ( tr.Hit )
		{
			Velocity = Vector3.Reflect( Velocity, tr.Normal );
		}

		Transform.Position += Velocity * Time.Delta;
		Velocity = Velocity.LerpTo( 0, Time.Delta * 8f );
		
		if ( TimeSinceCreated > 1 )
			GameObject.Destroy();
	}
}
