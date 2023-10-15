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

		physicsComponent.GetBody().AngularDamping = 5f;

		var mult = GameObject.Transform.Rotation.Up.Dot( Vector3.Up );

		if ( mult.AlmostEqual( 1 ) )
			return;
		
		mult = mult.Remap( 0, 1, 0.25f, 3f );
		
		physicsComponent.GetBody().AngularVelocity += Vector3.Up.Cross( Transform.Rotation.Up ) * -SettleSpeed * mult * Time.Delta;
	}

	private bool IsOnGround()
	{
		var tr = Physics.Trace.Ray( Transform.Position, Transform.Position + Vector3.Down * 17f )
			.Run();

		return tr.Hit && tr.Normal.Dot( Vector3.Up ).AlmostEqual( 1, 0.2f );
	}
}
