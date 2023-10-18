using System.Linq;
using Sandbox;

namespace DarkDescent.Actor;

public class AutoStandComponent : BaseComponent
{
	[Property, Range(0, 50)]
	private float SettleSpeed { get; set; }
	
	private TimeSince TimeSinceBeenOnGround { get; set; }


	public override void Update()
	{
		if ( !GameObject.TryGetComponent<PhysicsComponent>( out var physicsComponent ) )
			return;
		
		if ( !IsOnGround() )
		{
			TimeSinceBeenOnGround = 0;
			return;
		}

		if ( TimeSinceBeenOnGround < 0.1f )
			return;

		var body = physicsComponent.GetBody();
		
		body.AngularDamping = 5f;
;
		var mult = GameObject.Transform.Rotation.Up.Dot( Vector3.Up );

		if ( mult.AlmostEqual( 1 ) )
			return;
		
		mult = mult.Remap( 0, 1, 0.25f, 3f );
		
		body.AngularVelocity += Vector3.Up.Cross( Transform.Rotation.Up ) * -SettleSpeed * mult * Time.Delta;
	}

	private bool IsOnGround()
	{
		var tr = Physics.Trace.Ray( Transform.Position, Transform.Position + Vector3.Down * 23f )
			.WithoutTags( "dummy" )
			.Run();
		
		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.IgnoreDepth = true;
		Gizmo.Draw.Line( tr.StartPosition, tr.EndPosition );

		return tr.Hit && tr.Normal.Dot( Vector3.Up ).AlmostEqual( 1, 0.2f );
	}
}
