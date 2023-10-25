using Sandbox;

namespace DarkDescent.Actor.Damage;

public class HurtBoxComponent : BaseComponent
{
	[Property]
	private Vector3 Center1 { get; set; }
	
	[Property]
	private Vector3 Center2 { get; set; }
	
	[Property]
	private float Radius { get; set; }

	public override void DrawGizmos()
	{
		if ( !Scene.IsEditor )
			return;
		
		using var scope = Gizmo.Scope( $"{GetHashCode()}" );
		
		Gizmo.Transform = Gizmo.Transform.WithScale( 1 );
		Gizmo.Draw.Color = Color.Green;

		Gizmo.Draw.LineCapsule( new Capsule( Center1, Center2, Radius ) );
	}

	public override void Update()
	{
		base.Update();

		if ( Scene.IsEditor )
			return;

		var tr = PerformTrace();

		Gizmo.Transform = Transform.World;
		Gizmo.Draw.Color = tr.Hit ? Color.Red : Color.Green;

		Gizmo.Draw.LineCapsule( new Capsule( Center1, Center2, Radius ) );
	}

	public PhysicsTraceResult PerformTrace()
	{
		var tr = Physics.Trace.Ray( Transform.World.TransformVector( Center1 ), Transform.World.TransformVector( Center2 ) )
			.Radius( Radius )
			.WithTag( "dummy" )
			.Run();

		return tr;
	}
}
