using System;
using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	[ConVar.Replicated] public static bool dd_debug_stats { get; set; } = false; 
	
	/// <summary>
	/// We only want to network the computed stats of the actor, that way we dont have to do anything
	/// fancy or complicated on the client, and the stats only have to be calculated once.
	///
	/// These are our dynamically changing stats. Some stats will actually use a combination of these.
	/// </summary>
	private IDictionary<StatType, float> StatsDictionary { get; set; } = new Dictionary<StatType, float>(); 
	
	/// <summary>
	/// These are the real, internal stats of the player. They're not networked and are used to update the values
	/// that will be networked.
	/// </summary>
	private readonly Dictionary<StatType, StatValue> internalStatsDictionary = new();

	/// <summary>
	/// These are call backs that are run when a stat changes.
	/// Provides the old and new values of the stat.
	/// Only run serverside.
	/// </summary>
	private readonly Dictionary<StatType, Action<float, float>> statChangeCallbacks = new();

	public void AddStatModifier(StatModifier statModifier)
	{
		//apply this modifier to the internal StatValue
		var statValue = internalStatsDictionary[statModifier.StatType];

		var oldValue = statValue.Effective;
		
		statModifier.Apply(statValue);

		//set our networked stat to our effective stat value.
		StatsDictionary[statModifier.StatType] = statValue.Effective; 
		
		if (statChangeCallbacks.TryGetValue( statModifier.StatType, out var action ))
			action.Invoke(oldValue, statValue.Effective );
	}

	public void AddStatModifier( StatType statType, StatModifier.ModifierType modifierType, float value )
	{
		AddStatModifier( new StatModifier( statType, modifierType, value ) );
	}
	
	public void RemoveStatModifier( StatModifier statModifier )
	{
		var statValue = internalStatsDictionary[statModifier.StatType];
		
		var oldValue = statValue.Effective;
		
		statModifier.Remove(statValue);

		//set our networked stat to our effective stat value.
		StatsDictionary[statModifier.StatType] = statValue.Effective;
		
		if (statChangeCallbacks.TryGetValue( statModifier.StatType, out var action ))
			action.Invoke(oldValue, statValue.Effective );
	}

	public void RemoveStatModifier( StatType statType, StatModifier.ModifierType modifierType, float value )
	{
		RemoveStatModifier( new StatModifier(statType, modifierType, value) );
	}

	internal float GetStat( StatType type, float backup = 1 )
	{
		if ( !StatsDictionary.ContainsKey( type ) )
		{
			Log.Error( $"Tried to get unregistered stat: {type.ToString()}" );
			return backup;	
		}

		return StatsDictionary[type];
	}

	public void ResetStats()
	{
		foreach (var keyValuePair in internalStatsDictionary)
		{
			keyValuePair.Value.Reset();
			StatsDictionary[keyValuePair.Key] = keyValuePair.Value.Effective;
		}
	}

	/// <summary>
	/// Called when we first spawn the actor so all our stat values are filled out.
	/// Sets the internal stat value on server.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="value"></param>
	private void InitializeStat( StatType type, float value )
	{
		StatsDictionary[type] = value;
		internalStatsDictionary[type] = new StatValue( value );
	}

	private void SetBaseStat( StatType type, float value )
	{
		var oldValue = internalStatsDictionary[type].Effective;
		
		internalStatsDictionary[type].Base = value;
		StatsDictionary[type] = internalStatsDictionary[type].Effective;
		
		if (statChangeCallbacks.TryGetValue( type, out var action ))
			action.Invoke(oldValue, internalStatsDictionary[type].Effective );
	}

	private void AddStatCallback( StatType type, Action<float, float> action )
	{
		if ( !statChangeCallbacks.ContainsKey( type ) )
			statChangeCallbacks[type] = action;
		else
			statChangeCallbacks[type] += action;
	}

	private void RemoveStatCallback( StatType type, Action<float, float> action )
	{
		statChangeCallbacks[type] -= action;
	}

	/// <summary>
	/// This is called purely for hotloading purposes.
	/// Allows us to update our base stats when we change our StatsResource object.
	/// </summary>
	private void ReloadBaseStats()
	{
		SetBaseStat( StatType.Strength, BaseStats.Strength );
		SetBaseStat( StatType.Dexterity, BaseStats.Dexterity );
		SetBaseStat( StatType.Constitution, BaseStats.Constitution );
		SetBaseStat( StatType.Wisdom, BaseStats.Wisdom );
		SetBaseStat( StatType.Intelligence, BaseStats.Intelligence );
		SetBaseStat( StatType.Charisma, BaseStats.Charisma );
		
		//Health
		SetBaseStat( StatType.MaxHealth, BaseStats.MaxHealth );
		SetBaseStat( StatType.HealthRegen, BaseStats.HealthRegen );
		SetBaseStat( StatType.HealthRegenDelay, BaseStats.HealthRegenDelay );
		SetBaseStat( StatType.HealMultiplier, BaseStats.HealMultiplier );
		
		//Stamina
		SetBaseStat( StatType.MaxStamina, BaseStats.MaxStamina );
		SetBaseStat( StatType.StaminaRegen, BaseStats.StaminaRegen );
		SetBaseStat( StatType.StaminaCostMultiplier, BaseStats.StaminaCostMultiplier );
		SetBaseStat( StatType.StaminaRegenDelay, BaseStats.StaminaRegenDelay );
		
		//Actions
		SetBaseStat( StatType.ActionSpeed, BaseStats.ActionSpeed );
		SetBaseStat( StatType.SpellCastingSpeed, BaseStats.SpellCastingSpeed );
		
		//Movement
		SetBaseStat( StatType.MoveSpeedMultiplier, BaseStats.MoveSpeedMultiplier );
		SetBaseStat( StatType.JumpHeightMultiplier, BaseStats.JumpHeightMultiplier );
		
		//Combat
		SetBaseStat( StatType.PhysicalPower, BaseStats.PhysicalPower );
		SetBaseStat( StatType.SpellPower, BaseStats.SpellPower );
		
		SetBaseStat( StatType.FirePower, BaseStats.FirePower );
		SetBaseStat( StatType.FrostPower, BaseStats.FrostPower );
		SetBaseStat( StatType.ElectricPower, BaseStats.ElectricPower );
		SetBaseStat( StatType.PoisonPower, BaseStats.PoisonPower );
		SetBaseStat( StatType.NecroticPower, BaseStats.NecroticPower );
		
		SetBaseStat( StatType.ArcanePower, BaseStats.ArcanePower );
		SetBaseStat( StatType.DivinePower, BaseStats.DivinePower );
		SetBaseStat( StatType.OccultPower, BaseStats.OccultPower );
		
		SetBaseStat( StatType.ArmorPenetration, BaseStats.ArmorPenetration );
		SetBaseStat( StatType.WardingPenetration, BaseStats.WardingPenetration );
		SetBaseStat( StatType.CriticalMultiplier, BaseStats.CriticalMultiplier );
		SetBaseStat( StatType.KnockBack, BaseStats.KnockBack );
		
		//Defense
		SetBaseStat( StatType.Armor, BaseStats.Armor );
		SetBaseStat( StatType.Warding, BaseStats.Warding );
		SetBaseStat( StatType.CriticalNegation, BaseStats.CriticalNegation );
		SetBaseStat( StatType.KnockBackResistance, BaseStats.KnockBackResistance );
		
		SetBaseStat( StatType.FireResistance, BaseStats.FireResistance );
		SetBaseStat( StatType.FrostResistance, BaseStats.FrostResistance );
		SetBaseStat( StatType.ElectricResistance, BaseStats.ElectricResistance );
		SetBaseStat( StatType.PoisonResistance, BaseStats.PoisonResistance );
		SetBaseStat( StatType.NecroticResistance, BaseStats.NecroticResistance );
		
		SetBaseStat( StatType.ArcaneResistance, BaseStats.ArcaneResistance );
		SetBaseStat( StatType.DivineResistance, BaseStats.DivineResistance );
		SetBaseStat( StatType.OccultResistance, BaseStats.OccultResistance );
		
		//Misc
		SetBaseStat( StatType.CarryWeight, BaseStats.CarryWeight );
	}

	private void InitializeStats()
	{
		//Core stats
		InitializeStat( StatType.Strength, BaseStats.Strength );
		InitializeStat( StatType.Dexterity, BaseStats.Dexterity );
		
		InitializeStat( StatType.Constitution, BaseStats.Constitution );
		AddStatCallback( StatType.Constitution, OnConstitutionChanged );
		
		InitializeStat( StatType.Wisdom, BaseStats.Wisdom );
		InitializeStat( StatType.Intelligence, BaseStats.Intelligence );
		InitializeStat( StatType.Charisma, BaseStats.Charisma );
		
		//Health
		InitializeStat( StatType.MaxHealth, BaseStats.MaxHealth );
		AddStatCallback( StatType.MaxHealth, OnMaxHealthChanged );
		
		InitializeStat( StatType.HealthRegen, BaseStats.HealthRegen );
		InitializeStat( StatType.HealthRegenDelay, BaseStats.HealthRegenDelay );
		InitializeStat( StatType.HealMultiplier, BaseStats.HealMultiplier );
		
		//Stamina
		InitializeStat( StatType.MaxStamina, BaseStats.MaxStamina );
		AddStatCallback( StatType.MaxStamina, OnMaxStaminaChanged );
		
		InitializeStat( StatType.StaminaRegen, BaseStats.StaminaRegen );
		InitializeStat( StatType.StaminaCostMultiplier, BaseStats.StaminaCostMultiplier );
		InitializeStat( StatType.StaminaRegenDelay, BaseStats.StaminaRegenDelay );
		
		//Actions
		InitializeStat( StatType.ActionSpeed, BaseStats.ActionSpeed );
		InitializeStat( StatType.SpellCastingSpeed, BaseStats.SpellCastingSpeed );
		
		//Movement
		InitializeStat( StatType.MoveSpeedMultiplier, BaseStats.MoveSpeedMultiplier );
		InitializeStat( StatType.JumpHeightMultiplier, BaseStats.JumpHeightMultiplier );
		
		//Combat
		InitializeStat( StatType.PhysicalPower, BaseStats.PhysicalPower );
		InitializeStat( StatType.SpellPower, BaseStats.SpellPower );
		
		InitializeStat( StatType.FirePower, BaseStats.FirePower );
		InitializeStat( StatType.FrostPower, BaseStats.FrostPower );
		InitializeStat( StatType.ElectricPower, BaseStats.ElectricPower );
		InitializeStat( StatType.PoisonPower, BaseStats.PoisonPower );
		InitializeStat( StatType.NecroticPower, BaseStats.NecroticPower );
		
		InitializeStat( StatType.ArcanePower, BaseStats.ArcanePower );
		InitializeStat( StatType.DivinePower, BaseStats.DivinePower );
		InitializeStat( StatType.OccultPower, BaseStats.OccultPower );
		
		InitializeStat( StatType.ArmorPenetration, BaseStats.ArmorPenetration );
		InitializeStat( StatType.WardingPenetration, BaseStats.WardingPenetration );
		InitializeStat( StatType.CriticalMultiplier, BaseStats.CriticalMultiplier );
		InitializeStat( StatType.KnockBack, BaseStats.KnockBack );
		
		//Defense
		InitializeStat( StatType.Armor, BaseStats.Armor );
		InitializeStat( StatType.Warding, BaseStats.Warding );
		InitializeStat( StatType.CriticalNegation, BaseStats.CriticalNegation );
		InitializeStat( StatType.KnockBackResistance, BaseStats.KnockBackResistance );

		InitializeStat( StatType.FireResistance, BaseStats.FireResistance );
		InitializeStat( StatType.FrostResistance, BaseStats.FrostResistance );
		InitializeStat( StatType.ElectricResistance, BaseStats.ElectricResistance );
		InitializeStat( StatType.PoisonResistance, BaseStats.PoisonResistance );
		InitializeStat( StatType.NecroticResistance, BaseStats.NecroticResistance );
		
		InitializeStat( StatType.ArcaneResistance, BaseStats.ArcaneResistance );
		InitializeStat( StatType.DivineResistance, BaseStats.DivineResistance );
		InitializeStat( StatType.OccultResistance, BaseStats.OccultResistance );

		//Misc
		InitializeStat( StatType.CarryWeight, BaseStats.CarryWeight );

		Health = Stats.MaxHealth;
		Stamina = Stats.MaxStamina;
	}
	
	private void UpdateStats()
	{
		if ( dd_debug_stats && Game.IsServer )
		{
			var offset = 5;
			
			DebugOverlay.ScreenText( "------------CORE------------", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( $"Strength: {Stats.Strength}", offset++ );
			DebugOverlay.ScreenText( $"Dexterity: {Stats.Dexterity}", offset++ );
			DebugOverlay.ScreenText( $"Constitution: {Stats.Constitution}", offset++ );
			DebugOverlay.ScreenText( $"Wisdom: {Stats.Wisdom}", offset++ );
			DebugOverlay.ScreenText( $"Intelligence: {Stats.Intelligence}", offset++ );
			DebugOverlay.ScreenText( $"Charisma: {Stats.Charisma}", offset++ );
			
			offset++;
			
			
			DebugOverlay.ScreenText( $"Max Health: {Stats.MaxHealth}", offset++ );
			DebugOverlay.ScreenText( $"Health Regen: {Stats.HealthRegen:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Health Regen Delay: {Stats.HealthRegenDelay:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Heal Multiplier: {Stats.HealMultiplier:#0.0}", offset++ );

			offset++;
			
			DebugOverlay.ScreenText( $"Max Stamina: {Stats.MaxStamina}", offset++ );
			DebugOverlay.ScreenText( $"Stamina Regen: {Stats.StaminaRegen:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Stamina Regen Delay: {Stats.StaminaRegenDelay:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Stamina Cost Multiplier: {Stats.StaminaCostMultiplier:#0.0}", offset++ );

			offset++;
			
			DebugOverlay.ScreenText( $"Action Speed: {Stats.ActionSpeed:#0.00}", offset++ );
			DebugOverlay.ScreenText( $"Spell Casting Speed: {Stats.SpellCastingSpeed:#0.00}", offset++ );
			DebugOverlay.ScreenText( $"Move Speed Multiplier: {Stats.MoveSpeedMultiplier:#0.00}", offset++ );
			DebugOverlay.ScreenText( $"Jump Height Multiplier: {Stats.JumpHeightMultiplier:#0.00}", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( "------------COMBAT------------", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( $"Physical Power: {Stats.PhysicalPower:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Spell Power: {Stats.SpellPower:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Critical Multiplier: {Stats.CriticalMultiplier:#0.0}", offset++ );
			DebugOverlay.ScreenText( $"Knock Back: {Stats.KnockBack:#0.0}", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( "------------DEFENSE------------", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( $"Armor: {Stats.Armor}", offset++ );
			DebugOverlay.ScreenText( $"Warding: {Stats.Warding}", offset++ );
			DebugOverlay.ScreenText( $"Critical Negation: {Stats.Warding}", offset++ );
			DebugOverlay.ScreenText( $"Knock Back Resistance: {Stats.KnockBackResistance:#0.0}", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( $"Fire Resistance: {Stats.FireResistance}", offset++ );
			DebugOverlay.ScreenText( $"Frost Resistance: {Stats.FrostResistance}", offset++ );
			DebugOverlay.ScreenText( $"Electric Resistance: {Stats.ElectricResistance}", offset++ );
			DebugOverlay.ScreenText( $"Poison Resistance: {Stats.PoisonResistance}", offset++ );
			DebugOverlay.ScreenText( $"Necrotic Resistance: {Stats.NecroticResistance}", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( $"Arcane Resistance: {Stats.ArcaneResistance}", offset++ );
			DebugOverlay.ScreenText( $"Divine Resistance: {Stats.DivineResistance}", offset++ );
			DebugOverlay.ScreenText( $"Occult Resistance: {Stats.OccultResistance}", offset++ );
			
			offset++;
			
			DebugOverlay.ScreenText( $"Carry Weight: {Stats.CarryWeight:#0.0}", offset++ );
		}

		TickStats();
	}

	private void TickStats()
	{
		if (TimeSinceTookDamage > Stats.HealthRegenDelay && Stats.HealthRegen > 0)
			Heal( Stats.HealthRegen * Time.Delta );

		if (TimeSinceLastStaminaCost > Stats.StaminaRegenDelay)
			AddStamina( Stats.StaminaRegen * Time.Delta );
	}
}
