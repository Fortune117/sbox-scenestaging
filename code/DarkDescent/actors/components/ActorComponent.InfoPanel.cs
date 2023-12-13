using DarkDescent.Actor.UI;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	[Property, ToggleGroup("Info Panel")] 
	private bool ShowInfoPanel { get; set; } = true;

	[Property, Range( 0, 600 ), ToggleGroup("Info Panel")] 
	private float PanelDrawDistance { get; set; } = 300;
	
	[Property, Range( 0, 150 ), ToggleGroup("Info Panel")] 
	private float InfoPanelOffset { get; set; } = 0;
	
	/// <summary>
	/// Will use this as the info panels parent if set, otherwise uses GameObject.
	/// </summary>
	[Property, ToggleGroup("Info Panel")]
	private GameObject InfoPanelParent { get; set; }

	[Property, ToggleGroup("Info Panel")]
	private GameObject InfoPanelPrefab { get; set; }
	
	private ActorInfoPanel InfoPanel { get; set; }
	
	private void CreateInfoPanel()
	{
		if ( InfoPanelPrefab is null )
			return;
		
		var obj = SceneUtility.Instantiate( InfoPanelPrefab, Transform.Position, Transform.Rotation );
		obj.SetParent( InfoPanelParent ?? Scene );
		InfoPanel = obj.Components.Get<ActorInfoPanel>();
		InfoPanel.Actor = this;
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
		
		var height = InfoPanelOffset;
		
		var angles = Rotation.LookAt( -Camera.Rotation.Forward ).Angles();
		InfoPanel.Transform.Rotation = angles.WithPitch( 0 ).ToRotation();
		
		var tr = Scene.Trace.Ray( Transform.Position + Vector3.Up * 30f, Transform.Position + Vector3.Up * height )
			.WithTag( "solid" )
			.Radius( 3f )
			.Run();

		InfoPanel.Transform.Position = InfoPanel.Transform.Position.LerpTo(Transform.Position + Vector3.Up * height, Time.Delta * 30f);

		var distanceCheck = Camera.Position.Distance( InfoPanel.Transform.Position ) > PanelDrawDistance;
		
		InfoPanel.Panel.SetClass( "hidden", !ShowInfoPanel || distanceCheck || !Alive );
	}
}
