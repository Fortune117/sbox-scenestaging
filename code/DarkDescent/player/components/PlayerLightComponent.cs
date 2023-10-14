using Sandbox;

namespace DarkDescent;

[Prefab]
public partial class PlayerLightComponent : EntityComponent<Player>
{
	[Prefab, Net, Range( 0, 10 )] 
	private float Brightness { get; set; } = 1f;
	
	[Prefab,  Net]
	private Color Color { get; set; }
	
	[Prefab, Net, Range( 1, 2048 )] 
	private float Range { get; set; } = 1024;

	[Prefab, Net, Range( 0, 1 )] 
	private float LinerAttenuation { get; set; } = 1f;

	[Prefab, Net, Range( 0, 1 )] 
	private float QuadraticAttenuation { get; set; } = 0f;

	[Prefab, Net, Range( 0, 1 )] 
	private float FogStrength { get; set; } = 1f;

	[Prefab, Net, Range( 0, 180 )] 
	private float OuterConeAngle { get; set; } = 120;

	[Prefab, Net, Range( 0, 180 )] 
	private float InnerConeAngle { get; set; } = 60;

	[Prefab,  Net] 
	private bool DynamicShadows { get; set; } = false;
	
	private SpotLightEntity spotLight;

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Game.IsClient )
		{
			CreateClientLight();
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		
		spotLight?.Delete();
	}
	
	[Event.Hotload]
	private void CreateClientLight()
	{
		spotLight?.Delete();
		spotLight = CreateLight();
		spotLight.Enabled = false;
		spotLight.EnableViewmodelRendering = true;
	}

	private SpotLightEntity CreateLight()
	{
		var light = new SpotLightEntity
		{
			Enabled = false,
			DynamicShadows = DynamicShadows,
			Range = Range,
			LinearAttenuation = LinerAttenuation,
			QuadraticAttenuation = QuadraticAttenuation,
			Brightness = Brightness,
			Color = Color,
			InnerConeAngle = InnerConeAngle,
			OuterConeAngle = OuterConeAngle,
			FogStrength = FogStrength,
			Owner = Entity,
		};

		return light;
	}
	
	[GameEvent.Client.Frame]
	private void UpdateViewLight()
	{
		if (!spotLight.IsValid())
			return;
		
		if ( Entity.Client == Game.LocalClient )
			spotLight.Enabled = true;
		else
			spotLight.Enabled = false;
		
		spotLight.Position = Camera.Position + Camera.Rotation.Up * 3f + Camera.Rotation.Forward * 5f;
		spotLight.Rotation = Camera.Rotation;
	}
}
