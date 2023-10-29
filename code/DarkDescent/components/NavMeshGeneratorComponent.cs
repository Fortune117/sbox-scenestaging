using System.Threading.Tasks;
using Sandbox;

namespace DarkDescent;

public class NavMeshGeneratorComponent : BaseComponent
{
	private NavigationMesh mesh;
	private TimeSince TimeSinceStart;

	public override void DrawGizmos()
	{
		base.DrawGizmos();
		
		if (mesh is not null)
			Gizmo.Draw.LineNavigationMesh( mesh );
	}

	public override void OnStart()
	{
	}

	public override void Update()
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
