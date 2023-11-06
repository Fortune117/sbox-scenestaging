using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Actions
{
	public Actions() { }

	/// <summary>
	/// Multiplier for all our action speeds.
	/// Note that dexterity will also increase our action speed, but this lets you tweak it without
	/// modifying dex.
	/// </summary>
	[Range( 0, 5 )] 
	public float ActionSpeed { get; set; } = 1;

	/// <summary>
	/// Multiplier for our spell casting speed.
	/// </summary>
	[Range( 0, 5 )] 
	public float SpellCastingSpeed { get; set; } = 1;

	/// <summary>
	/// Multiplier for our move speed.
	/// Note that this is increased by our Dexterity.
	/// </summary>
	[Range(0, 3)] 
	public float MoveSpeedMultiplier { get; set; } = 1f;

	/// <summary>
	/// Multiplier for our jump height.
	/// Note that this is increased by our Strength.
	/// </summary>
	[Range(0, 3)] 
	public float JumpHeightMultiplier { get; set; } = 1f;
}
