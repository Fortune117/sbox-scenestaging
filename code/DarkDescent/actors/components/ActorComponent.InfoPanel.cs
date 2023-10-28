using DarkDescent.Actor.UI;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	[Property, Category("Info Panel")] 
	public bool ShowInfoPanel { get; set; } = true;

	[Property, Range( 0, 600 ), Category("Info Panel")] 
	public float PanelDrawDistance { get; set; } = 300;
	
	[Property, Range( 0, 150 ), Category("Info Panel")] 
	public float InfoPanelOffset { get; set; } = 0;

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
		if ( InfoPanel is null )
			return;
		
		InfoPanel.GameObject.SetParent( null );
		InfoPanel.GameObject.Destroy();
	}
	
	private void UpdateInfoPanel()
	{
		if ( InfoPanel is null )
			return;
		
		var modelComponent = GetComponent<ModelComponent>( false, true );

		var height = InfoPanelOffset;
		if ( modelComponent is not null )
		{
			height += (modelComponent.Bounds.Maxs.z - Transform.Position.z) * 1.15f;
		}
		
		var angles = Rotation.LookAt( -Camera.Rotation.Forward ).Angles();
		InfoPanel.Transform.Rotation = angles.WithPitch( 0 ).ToRotation();
		
		var tr = Physics.Trace.Ray( Transform.Position + Vector3.Up * 30f, Transform.Position + Vector3.Up * height )
			.WithTag( "solid" )
			.Radius( 3f )
			.Run();
		
		InfoPanel.Transform.LocalPosition = Vector3.Up * height;

		var distanceCheck = Camera.Position.Distance( InfoPanel.Transform.Position ) > PanelDrawDistance;
		
		InfoPanel.Panel.SetClass( "hidden", !ShowInfoPanel || distanceCheck );
	}
}
