using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Resistances
{
	public Resistances() { }

	/// <summary>
	/// Our base armor value.
	/// Increased by the equipment we're wearing.
	/// Gives us physical resistance.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float Armor { get; set; } = 0f;
	
	/// <summary>
	/// Our magic equivalent to armor.
	/// Gives us magical resistance.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float Warding { get; set; } = 0f;
	
	/// <summary>
	/// Flat fire resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float FireResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat fire resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float FrostResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat electric resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float ElectricResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat acid resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float PoisonResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat necrotic resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float NecroticResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat arcane resistance.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float ArcaneResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat divine resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float DivineResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat occult resistance score.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float OccultResistance { get; set; } = 0f;
	
	/// <summary>
	/// How much we reduce the critical multiplier when we take critical damage.
	/// </summary>
	[Range(-2f, 2f, 0.1f)]
	public float CriticalNegation { get; set; } = 0;
	
	/// <summary>
	/// How much we reduce knockback by when struck.
	/// Can be negative, leading to increased knockback.
	/// </summary>
	[Range(-300, 300, 1f)]
	public float KnockBackResistance { get; set; } = 0;
}
