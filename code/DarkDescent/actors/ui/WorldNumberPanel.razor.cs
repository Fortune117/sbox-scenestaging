using Sandbox;
using Sandbox.Razor;

namespace DarkDescent.Actor.UI;

public partial class WorldNumberPanel
{
	protected Vector3 Velocity { get; set; }

	public WorldNumberPanel()
	{
		var size = 500f;
		PanelBounds = new Rect { Width = size, Height = size, Position = new Vector2( -size/2f, -size/2f )};
		WorldScale = 1.5f;
	}

	protected virtual void CalculateVelocity() { }
	
	[GameEvent.Client.Frame]
	private void OnFrame()
	{
		var angles = Rotation.LookAt( -Camera.Rotation.Forward ).Angles();
		Rotation = angles.WithPitch( 0 ).ToRotation();

		var tr = Trace.Ray( Position, Position + Velocity * Time.Delta * 20f )
			.Radius( 2f )
			.StaticOnly()
			.Run();

		if ( tr.Hit )
		{
			Velocity = Vector3.Reflect( Velocity, tr.Normal );
		}

		Position += Velocity * Time.Delta;
		Velocity = Velocity.LerpTo( 0, Time.Delta * 8f );
	}
	
	private TimeSince TimeSinceCreated = 0;
	public override void Tick()
	{
		if ( TimeSinceCreated > 0 )
			Delete();
	}
}
