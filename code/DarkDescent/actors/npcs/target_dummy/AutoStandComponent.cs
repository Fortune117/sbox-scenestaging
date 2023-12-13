using System.Linq;
using Sandbox;

namespace DarkDescent.Actor;

public class AutoStandComponent : Component
{
	[Property, Range(0, 150)]
	private float SettleSpeed { get; set; }
	
	private TimeSince TimeSinceBeenOnGround { get; set; }


	protected override void OnFixedUpdate()
	{
		if ( !GameObject.Components.TryGet<Collider>( out var physicsComponent ) )
			return;
		
		if ( !IsOnGround() )
		{
			TimeSinceBeenOnGround = 0;
			return;
		}

		if ( TimeSinceBeenOnGround < 0.1f )
			return;

		var body = physicsComponent.KeyframeBody;
		
		body.AngularDamping = 15f;
;
		var mult = GameObject.Transform.Rotation.Up.Dot( Vector3.Up );

		if ( mult.AlmostEqual( 1 ) )
			return;
		
		mult = mult.Remap( 0, 1, 0.5f, 2f );
		
		body.AngularVelocity += Vector3.Up.Cross( Transform.Rotation.Up ) * -SettleSpeed * mult * Time.Delta;
	}

	private bool IsOnGround()
	{
		var tr = Scene.Trace.Ray( Transform.Position, Transform.Position + Vector3.Down * 23f )
			.WithoutTags( "dummy" )
			.Run();
		
		/*Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.IgnoreDepth = true;
		Gizmo.Draw.Line( tr.StartPosition, tr.EndPosition );*/

		return tr.Hit && tr.Normal.Dot( Vector3.Up ).AlmostEqual( 1, 0.2f );
	}
}
