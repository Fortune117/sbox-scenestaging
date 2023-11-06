using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Health
{
	public Health() { }

	/// <summary>
	/// Flat max health for this actor.
	/// Note that this is added onto by constitution.
	/// </summary>
	[Range( 1, 100, 1f )] 
	public float MaxHealth { get; set; } = 30;

	/// <summary>
	/// Health regeneration per second.
	/// Be careful with this one, healing should be rare.
	/// </summary>
	[Range( 0, 10 )]
	public float HealthRegen { get; set; } = 0;

	/// <summary>
	/// How long after taking damage we wait before regening health.
	/// Reduced by Will.
	/// </summary>
	[Range( 0, 5 )]
	public float HealthRegenDelay { get; set; } = 1.5f;

	/// <summary>
	/// Multiplier for heals received.
	/// </summary>
	[Range( 0, 10 )]
	public float HealMultiplier { get; set; } = 0;
}
