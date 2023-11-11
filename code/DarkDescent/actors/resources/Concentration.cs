using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Concentration
{
	public Concentration() { }

	/// <summary>
	/// Flat max concentration for this actor.
	/// Note that this is increased by the actors spell casting attribute.
	/// </summary>
	[Range( 1, 100, 1f )] 
	public float MaxConcentration { get; set; } = 30;

	/// <summary>
	/// Concentration regeneration per second.
	/// </summary>
	[Range( 0, 10 )]
	public float ConcentrationRegen { get; set; } = 0;

	/// <summary>
	/// How long after taking concentration damage we wait before regenerating concentration.
	/// Reduced by Intelligence.
	/// </summary>
	[Range( 0, 5 )]
	public float ConcentrationRegenDelay { get; set; } = 2.5f;
	
	/// <summary>
	/// How long after taking concentration damage we wait before regenerating concentration.
	/// Reduced by Will.
	/// </summary>
	[Range( 0, 2 )]
	public float ConcentrationCostMultiplier { get; set; } = 1f;
}
