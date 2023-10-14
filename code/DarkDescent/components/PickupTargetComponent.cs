using Sandbox;

namespace DarkDescent.Components;

[Prefab]
public partial class PickupTargetComponent : EntityComponent<ModelEntity>
{
	/// <summary>
	/// How much strength is required to pick up this object.
	/// </summary>
	[Prefab, Net, Range( 0, 100 )]
	public float StrengthThreshold { get; set; } = 10;

	/// <summary>
	/// How much strength below the threshold can we be and still move the object?
	/// </summary>
	[Prefab, Net, Range( 0, 100 )]
	public float StrengthLeeway { get; set; } = 5;
}
