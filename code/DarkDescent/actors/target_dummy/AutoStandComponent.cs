using Sandbox;

namespace DarkDescent.Actor;

[Prefab]
public class AutoStandComponent : EntityComponent<ModelEntity>
{
	[Prefab, Range(0, 50)]
	private float SettleSpeed { get; set; }
	
	private TimeSince TimeSinceBeenOnGround { get; set; }
	
	
	[GameEvent.Physics.PreStep]
	private void OnTick()
	{
		if ( Game.IsClient )
			return;

		if ( !Entity.IsValid() || Entity.PhysicsGroup is null )
			return;
		
		if ( !IsOnGround() )
		{
			TimeSinceBeenOnGround = 0;
			return;
		}


		if ( TimeSinceBeenOnGround < 0.1f )
			return;

		Entity.PhysicsGroup.AngularDamping = 5f;

		var mult = Entity.Rotation.Up.Dot( Vector3.Up );

		if ( mult.AlmostEqual( 1 ) )
			return;
		
		mult = mult.Remap( 0, 1, 0.25f, 3f );
		
		Entity.PhysicsGroup.AddAngularVelocity(Vector3.Up.Cross( Entity.Rotation.Up ) * -SettleSpeed * mult * Time.Delta);
	}

	private bool IsOnGround()
	{
		var tr = Trace.Ray( Entity.Position, Entity.Position + Vector3.Down * 17f )
			.Ignore( Entity )
			.Run();

		return tr.Hit && tr.Normal.Dot( Vector3.Up ).AlmostEqual( 1, 0.2f );
	}
}
