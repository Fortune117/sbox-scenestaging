using DarkDescent.Actor.UI;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	[Prefab, Net, Category("Info Panel")] 
	public bool ShowInfoPanel { get; set; } = true;

	[Prefab, Net, Range( 0, 600 ), Category("Info Panel")] 
	public float PanelDrawDistance { get; set; } = 300;

	private ActorInfoPanel InfoPanel { get; set; }
	
	private void CreateInfoPanel()
	{
		InfoPanel = new ActorInfoPanel(this);
	}

	private void DestroyInfoPanel()
	{
		InfoPanel?.Delete();
	}

	[GameEvent.Client.Frame]
	private void UpdateInfoPanel()
	{
		if ( !InfoPanel.IsValid() )
			return;
		
		var height = Entity.WorldSpaceBounds.Maxs.z;
		var angles = Rotation.LookAt( -Camera.Rotation.Forward ).Angles();
		InfoPanel.Rotation = angles.WithPitch( 0 ).ToRotation();
		
		var tr = Trace.Ray( Entity.Position + Vector3.Up * 30f, Entity.Position + Vector3.Up * height )
			.Radius( 30f )
			.StaticOnly()
			.Run();
		
		InfoPanel.Position = tr.EndPosition;

		var distanceCheck = Camera.Position.Distance( InfoPanel.Position ) > PanelDrawDistance;
		
		InfoPanel.SetClass( "hidden", !ShowInfoPanel || distanceCheck );
	}
}
