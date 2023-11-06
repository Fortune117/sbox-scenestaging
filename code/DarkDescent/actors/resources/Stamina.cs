using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Stamina
{
	public Stamina() { }

	/// <summary>
	/// Flat max stamina that this actor has.
	/// Note that this is added onto by Constitution.
	/// </summary>
	[Range(1, 100f, 1f)]
	public float MaxStamina { get; set; } = 30f;

	/// <summary>
	/// Flat stamina regen per second.
	/// Note that this is added onto by Dexterity.
	/// </summary>
	[Range(1, 15f)]
	public float StaminaRegen { get; set; } = 3f;

	/// <summary>
	/// Multiplier to our stamina costs.
	/// Note that this is lowered by Strength.
	/// </summary>
	[Range(0, 3)]
	public float StaminaCostMultiplier { get; set; } = 1f;

	/// <summary>
	/// How long in seconds after paying a stamina cost until we start regening.
	/// Note that this is decreased by Dexterity.
	/// </summary>
	[Range( 0, 5 )]
	public float StaminaRegenDelay { get; set; } = 1.5f;
}
