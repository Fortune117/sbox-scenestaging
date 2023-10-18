using System;
using Sandbox;

namespace DarkDescent.Actor;

[GameResource("Stats", "stats", "Contains data about an actors base stats.")]
public class StatsResource : GameResource
{
	[Range( 1, 100, 1f ), Category("Core")] 
	public float Strength { get; set; } = 10;
	
	[Range( 1, 100, 1f ), Category("Core")] 
	public float Dexterity { get; set; } = 10;
	
	[Range( 1, 100, 1f ), Category("Core")] 
	public float Constitution { get; set; } = 10;
	
	[Range( 1, 100, 1f ), Category("Core")] 
	public float Wisdom { get; set; } = 10;
	
	[Range( 1, 100, 1f ), Category("Core")] 
	public float Intelligence { get; set; } = 10;
	
	[Range( 1, 100, 1f ), Category("Core")] 
	public float Charisma { get; set; } = 10;
	
	/// <summary>
	/// Flat max health for this actor.
	/// Note that this is added onto by constitution.
	/// </summary>
	[Range( 1, 100, 1f ), Category("Health")] 
	public float MaxHealth { get; set; } = 30;

	/// <summary>
	/// Health regeneration per second.
	/// Be careful with this one, healing should be rare.
	/// </summary>
	[Range( 0, 5 ), Category("Health")]
	public float HealthRegen { get; set; } = 0;

	/// <summary>
	/// How long after taking damage we wait before regening health.
	/// Reduced by Will.
	/// </summary>
	[Range( 0, 5 ), Category("Health")]
	public float HealthRegenDelay { get; set; } = 1.5f;

	/// <summary>
	/// Multiplier for heals received.
	/// </summary>
	[Range( 0, 5 ), Category("Health")]
	public float HealMultiplier { get; set; } = 0;

	/// <summary>
	/// Flat max stamina that this actor has.
	/// Note that this is added onto by Constitution.
	/// </summary>
	[Range(1, 100f, 1f), Category("Stamina")]
	public float MaxStamina { get; set; } = 30f;

	/// <summary>
	/// Flat stamina regen per second.
	/// Note that this is added onto by Dexterity.
	/// </summary>
	[Category("Stamina")]
	public float StaminaRegen { get; set; } = 3f;

	/// <summary>
	/// Multiplier to our stamina costs.
	/// Note that this is lowered by Strength.
	/// </summary>
	[Category("Stamina")]
	public float StaminaCostMultiplier { get; set; } = 1f;

	/// <summary>
	/// How long in seconds after paying a stamina cost until we start regening.
	/// Note that this is decreased by Dexterity.
	/// </summary>
	[Range( 0, 5 ), Category("Stamina")]
	public float StaminaRegenDelay { get; set; } = 1.5f;
	
	/// <summary>
	/// Multiplier for all our action speeds.
	/// Note that dexterity will also increase our action speed, but this lets you tweak it without
	/// modifying dex.
	/// </summary>
	[Range( 0, 5 ), Category("Actions")] 
	public float ActionSpeed { get; set; } = 1;

	/// <summary>
	/// Multiplier for our spell casting speed.
	/// </summary>
	[Range( 0, 5 ), Category("Actions")] 
	public float SpellCastingSpeed { get; set; } = 1;

	/// <summary>
	/// Multiplier for our move speed.
	/// Note that this is increased by our Dexterity.
	/// </summary>
	[Category("Movement"), Range(0, 3)] 
	public float MoveSpeedMultiplier { get; set; } = 1f;

	/// <summary>
	/// Multiplier for our jump height.
	/// Note that this is increased by our Strength.
	/// </summary>
	[Category("Movement"), Range(0, 3)] 
	public float JumpHeightMultiplier { get; set; } = 1f;
	
	/// <summary>
	/// Our base physical power, improves the effectiveness of our physical attacks.
	/// Increased by our primary attribute (usually Str/Dex).
	/// </summary>
	[Range( 0, 300f, 1f ), Category("Combat")]
	public float PhysicalPower { get; set; } = 0f;
	
	/// <summary>
	/// Our base spell power, improves the effectiveness of our spells.
	/// Increased by our spell casting attribute (usually Will/Cha/Int).
	/// </summary>
	[Range( 0, 300f, 1f ), Category("Combat")]
	public float SpellPower { get; set; } = 0f;

	/// <summary>
	/// How much armor we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f ), Category( "Combat" )]
	public float ArmorPenetration { get; set; } = 0;

	/// <summary>
	/// How much warding we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f ), Category( "Combat" )]
	public float WardingPenetration { get; set; } = 0;

	/// <summary>
	/// How much our damage is multiplied when we crit.
	/// </summary>
	[Range( 0, 5, 1f ), Category( "Combat" )]
	public float CriticalMultiplier { get; set; } = 1.5f;
	
	/// <summary>
	/// How much knockback we apply on each hit.
	/// </summary>
	[Range(0, 300, 1f), Category( "Combat" )]
	public float KnockBack { get; set; } = 0;
	
	/// <summary>
	/// Our base armor value.
	/// Increased by the equipment we're wearing.
	/// Gives us physical resistance.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float Armor { get; set; } = 0f;
	
	/// <summary>
	/// Our magic equivalent to armor.
	/// Gives us magical resistance.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float Warding { get; set; } = 0f;

	/// <summary>
	/// How much we reduce the critical multiplier when we take critical damage.
	/// </summary>
	[Range(-300, 300, 1f), Category( "Defense" )]
	public float CriticalNegation { get; set; } = 0;
	
	/// <summary>
	/// How much we reduce the critical multiplier when we take critical damage.
	/// </summary>
	[Range(-300, 300, 1f), Category( "Defense" )]
	public float KnockBackResistance { get; set; } = 0;
	
	/// <summary>
	/// Flat fire resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float FireResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat fire resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float FrostResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat electric resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float ElectricResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat acid resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float PoisonResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat necrotic resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float NecroticResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat arcane resistance.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float ArcaneResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat divine resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float DivineResistance { get; set; } = 0f;
	
	/// <summary>
	/// Flat occult resistance score.
	/// </summary>
	[Range(-300, 300, 1f), Category("Defense")]
	public float OccultResistance { get; set; } = 0f;
	
	/// <summary>
	/// How much weight we can carry.
	/// Increased by our Strength score.
	/// </summary>
	[Category("Misc"), Range(0, 400)]
	public float CarryWeight { get; set; }

	public Action OnReload;

	protected override void PostReload()
	{
		base.PostReload();
		OnReload?.Invoke();
	}
}
