using Sandbox;

namespace DarkDescent.Actor.Damage;

public class HurtBoxComponent : Component
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

	protected override void DrawGizmos()
	{
		if ( !Scene.IsEditor )
			return;

		Gizmo.Transform = Scene.Transform.World;
		Gizmo.Draw.Color = Color.Green;

		Gizmo.Draw.LineCapsule( new Capsule( Transform.World.TransformVector(Center1), Transform.World.TransformVector(Center2), Radius ) );
	}
	
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Scene.IsEditor )
			return;

		var tr = PerformTrace();

		/*
		Gizmo.Transform = Scene.Transform.World;
		Gizmo.Draw.Color = Color.Green;

		Gizmo.Draw.LineCapsule( new Capsule( Transform.World.TransformVector(Center1), Transform.World.TransformVector(Center2), Radius ) );
		*/
	}

	public SceneTraceResult PerformTrace()
	{
		var tr = Scene.Trace.Ray( Transform.World.TransformVector( Center1 ), Transform.World.TransformVector( Center2 ) )
			.WithoutTags( "nohit" )
			.UseHitboxes()
			.Radius( Radius )
			.Run();

		if ( tr.Hit )
			Gizmo.Draw.Color = Color.Red;
		else
			Gizmo.Draw.Color = Color.White;
		
		Gizmo.Draw.LineCapsule( new Capsule(tr.StartPosition, tr.EndPosition, Radius ) );

		return tr;
	}
}
