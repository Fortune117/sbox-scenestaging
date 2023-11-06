using DarkDescent.Actor.StatSystem;
using Sandbox;

namespace DarkDescent.Actor;

[GameResource("Block Stats", "block", "Contains data used for block calculations.")]
public class BlockingResource : GameResource
{
	[Property] 
	public SoundEvent BlockSound { get; set; }
	
	[Property]
	public Sandbox.ParticleSystem  BlockEffect { get; set; }
	
	[Property]
	public SoundEvent ParrySound { get; set; }
	
	[Property]
	public Sandbox.ParticleSystem ParryEffect { get; set; }
	
	/// <summary>
	/// What percentage of physical damage we are negating.
	/// </summary>
	[Range(-1, 1)]
	public float PhysicalNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of magical damage we are negating.
	/// </summary>
	[Range(-1, 1)]
	public float MagicalNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of fire damage we are negating.
	/// </summary>
	[Range(-1, 1)]
	public float FireNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of frost damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float FrostNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of electric damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float ElectricNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of poison damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float PoisonNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of necrotic damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float NecroticNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of arcane damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float ArcaneNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of divine damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float DivineNegation { get; set; } = 0f;
	
	/// <summary>
	/// What percentage of occult damage are we negating.
	/// </summary>
	[Range(-1, 1)]
	public float OccultNegation { get; set; } = 0f;
	
	/// <summary>
	/// How much we reduce the critical multiplier when we take critical damage.
	/// </summary>
	[Range(-1, 1)]
	public float CriticalNegation { get; set; } = 0;
	
	/// <summary>
	/// What percentage of knockback are we negating.
	/// </summary>
	[Range(-2, 1)]
	public float KnockBackNegation { get; set; } = 0;
}
