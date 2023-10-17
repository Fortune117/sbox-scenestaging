using DarkDescent.Actor.UI;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	[Property, Category("Info Panel")] 
	public bool ShowInfoPanel { get; set; } = true;

	[Property, Range( 0, 600 ), Category("Info Panel")] 
	public float PanelDrawDistance { get; set; } = 300;

	[Property]
	private GameObject InfoPanelPrefab { get; set; }
	
	private ActorInfoPanel InfoPanel { get; set; }
	
	private void CreateInfoPanel()
	{
		if ( InfoPanelPrefab is null )
			return;
		
		var obj = SceneUtility.Instantiate( InfoPanelPrefab, Transform.Position, Transform.Rotation );
		obj.SetParent( GameObject );
		InfoPanel = obj.GetComponent<ActorInfoPanel>();
		InfoPanel.Actor = GameObject;
	}

	private void DestroyInfoPanel()
	{
		InfoPanel.GameObject.SetParent( null );
		InfoPanel.GameObject.Destroy();
	}
	
	private void UpdateInfoPanel()
	{
		if ( InfoPanel is null )
			return;
		
		var modelComponent = GetComponent<ModelComponent>( false, true );

		var height = 0f;
		if ( modelComponent is not null )
		{
			height = modelComponent.Bounds.Maxs.z;
		}
		
		var angles = Rotation.LookAt( -Camera.Rotation.Forward ).Angles();
		InfoPanel.Transform.Rotation = angles.WithPitch( 0 ).ToRotation();
		
		var tr = Physics.Trace.Ray( Transform.Position + Vector3.Up * 30f, Transform.Position + Vector3.Up * height )
			.Radius( 30f )
			.Run();
		
		InfoPanel.Transform.Position = Transform.Position + Vector3.Up * height;

		var distanceCheck = Camera.Position.Distance( InfoPanel.Transform.Position ) > PanelDrawDistance;
		
		InfoPanel.Panel.SetClass( "hidden", !ShowInfoPanel || distanceCheck );
	}
}
