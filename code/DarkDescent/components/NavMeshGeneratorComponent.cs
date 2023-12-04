using System.Threading.Tasks;
using Sandbox;

namespace DarkDescent;

public class NavMeshGeneratorComponent : BaseComponent
{
	private NavigationMesh mesh;
	private TimeSince TimeSinceStart;

	protected override void DrawGizmos()
	{
		base.DrawGizmos();
		
		if (mesh is not null)
			Gizmo.Draw.LineNavigationMesh( mesh );
	}

	protected override void OnStart()
	{
	}

	protected override void OnUpdate()
	{
		if (TimeSinceStart > 2 && mesh is null)
			GenerateMesh();
	}

	private void GenerateMesh()
	{
		mesh = new NavigationMesh();
		mesh.Generate( Scene.PhysicsWorld );
		
		Log.Info( Scene.PhysicsWorld );

		var path = new NavigationPath( mesh );
		
		Log.Info( mesh.Nodes.Count );
	}
}
