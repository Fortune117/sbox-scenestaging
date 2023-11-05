using Sandbox;

namespace DarkDescent.Actor.Damage;

public class HurtBoxComponent : BaseComponent
{
	public Vector3 DirectionMoment
	{
		get
		{
			return HitDirection.Transform.Rotation.Forward;
		}
	}

	public Capsule Capsule => new Capsule( Transform.World.TransformVector( Center1 ),
		Transform.World.TransformVector( Center2 ), Radius );
		
	[Property]
	private GameObject HitDirection { get; set; }
	
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

		Gizmo.Transform = Scene.Transform.World;
		Gizmo.Draw.Color = Color.Green;

		Gizmo.Draw.LineCapsule( new Capsule( Transform.World.TransformVector(Center1), Transform.World.TransformVector(Center2), Radius ) );
	}
	
	public override void Update()
	{
		base.Update();

		if ( Scene.IsEditor )
			return;

		var tr = PerformTrace();

		Gizmo.Transform = Scene.Transform.World;
		Gizmo.Draw.Color = Color.Green;

		Gizmo.Draw.LineCapsule( new Capsule( Transform.World.TransformVector(Center1), Transform.World.TransformVector(Center2), Radius ) );
	}

	public PhysicsTraceResult PerformTrace()
	{
		var tr = Physics.Trace.Ray( Transform.World.TransformVector( Center1 ), Transform.World.TransformVector( Center2 ) )
			.WithoutTags( "nohit" )
			.Radius( Radius )
			.Run();

		return tr;
	}
}
