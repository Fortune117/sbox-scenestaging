namespace DarkDescent.Actor;

public class Stats
{
	private ActorComponent ActorComponent { get; set; }
	
	public Stats( ActorComponent actorComponent )
	{
		ActorComponent = actorComponent;
	}

	private float GetPrimaryAttribute()
	{
		switch ( ActorComponent.PrimaryAttribute )
		{
			case Attributes.Strength:
				return ActorComponent.GetStat( StatType.Strength );
			case Attributes.Dexterity:
				return ActorComponent.GetStat( StatType.Dexterity );
			case Attributes.Constitution:
				return ActorComponent.GetStat( StatType.Constitution );
			case Attributes.Wisdom:
				return ActorComponent.GetStat( StatType.Wisdom );
			case Attributes.Intelligence:
				return ActorComponent.GetStat( StatType.Intelligence );
			case Attributes.Charisma:
				return ActorComponent.GetStat( StatType.Charisma );
		}

		Log.Error( $"Somehow don't have a primary attribute assigned to Actor {ActorComponent.GameObject}" );
		return 0f;
	}

	private float GetSpellCastingAttribute()
	{
		switch ( ActorComponent.SpellCastingAttribute )
		{
			case Attributes.Strength:
				return ActorComponent.GetStat( StatType.Strength );
			case Attributes.Dexterity:
				return ActorComponent.GetStat( StatType.Dexterity );
			case Attributes.Constitution:
				return ActorComponent.GetStat( StatType.Constitution );
			case Attributes.Wisdom:
				return ActorComponent.GetStat( StatType.Wisdom );
			case Attributes.Intelligence:
				return ActorComponent.GetStat( StatType.Intelligence );
			case Attributes.Charisma:
				return ActorComponent.GetStat( StatType.Charisma );
		}
		
		Log.Error( $"Somehow don't have a spell casting attribute assigned to Actor {ActorComponent.GameObject}" );
		return 0f;
	}

	public float GetMaxHealthForConScore( float con )
	{
		return ActorComponent.GetStat( StatType.MaxHealth ) +
				                 GameBalanceResource.ActiveBalance.ConstitutionToMaxHealthCurve.Evaluate( con );
	}
	
	public float GetMaxHealthForMaxHealthBonus( float bonus )
	{
		return bonus + GameBalanceResource.ActiveBalance.ConstitutionToMaxHealthCurve.Evaluate( Constitution );
	}

	public float GetMaxStaminaForConScore( float con )
	{
		return ActorComponent.GetStat( StatType.MaxStamina ) +
		       GameBalanceResource.ActiveBalance.ConstitutionToMaxStaminaCurve.Evaluate( con );
	}

	public float GetMaxStaminaForMaxStaminaBonus( float bonus )
	{
		return bonus + GameBalanceResource.ActiveBalance.ConstitutionToMaxStaminaCurve.Evaluate( Constitution );
	}
	
	#region Core
	//This is where our public facing stat values live. Not entirely sure this is the best way to handle it but it feels very neat.
	public float Strength => ActorComponent.GetStat( StatType.Strength );
	public float Dexterity => ActorComponent.GetStat( StatType.Dexterity );
	public float Constitution => ActorComponent.GetStat( StatType.Constitution );
	public float Wisdom => ActorComponent.GetStat( StatType.Wisdom );
	public float Intelligence  => ActorComponent.GetStat( StatType.Intelligence );
	public float Charisma  => ActorComponent.GetStat( StatType.Charisma );

	public float PrimaryAttribute => GetPrimaryAttribute();
	public float SpellCastingAttribute => GetSpellCastingAttribute();
	
	#endregion

	#region Health
	public float MaxHealth
	{
		get
		{
			return ActorComponent.GetStat( StatType.MaxHealth ) +
			       GameBalanceResource.ActiveBalance.ConstitutionToMaxHealthCurve.Evaluate( Constitution );
		}
	}

	public float HealthRegen
	{
		get
		{
			return ActorComponent.GetStat( StatType.HealthRegen ) +
			       GameBalanceResource.ActiveBalance.ConstitutionToHealthRegenCurve.Evaluate( Constitution );
		}
	}

	public float HealthRegenDelay
	{
		get
		{
			return ActorComponent.GetStat( StatType.HealthRegenDelay ) +
			       GameBalanceResource.ActiveBalance.ConstitutionToHealthRegenDelayCurve.Evaluate( Constitution );
		}
	}
	
	public float HealMultiplier
	{
		get
		{
			return ActorComponent.GetStat( StatType.HealMultiplier ) +
			       GameBalanceResource.ActiveBalance.ConstitutionToHealMultiplierCurve.Evaluate( Constitution );
		}
	}
	#endregion

	#region Stamina

	public float MaxStamina
	{
		get
		{
			return ActorComponent.GetStat( StatType.MaxStamina ) +
			       GameBalanceResource.ActiveBalance.ConstitutionToMaxStaminaCurve.Evaluate( Constitution );
		}
	}

	public float StaminaRegen
	{
		get
		{
			return ActorComponent.GetStat( StatType.StaminaRegen ) +
			       GameBalanceResource.ActiveBalance.DexterityToStaminaRegenCurve.Evaluate( Dexterity );
		}
	}

	public float StaminaCostMultiplier
	{
		get
		{
			return ActorComponent.GetStat( StatType.StaminaCostMultiplier ) +
			       GameBalanceResource.ActiveBalance.StrengthToStaminaCostMultiplierCurve.Evaluate( Strength );
		}
	}

	public float StaminaRegenDelay
	{
		get
		{
			return ActorComponent.GetStat( StatType.StaminaRegenDelay ) +
			       GameBalanceResource.ActiveBalance.DexterityToStaminaRegenDelayCurve.Evaluate( Dexterity );
		}
	}

	#endregion

	#region Actions

	public float ActionSpeed
	{
		get
		{
			return ActorComponent.GetStat( StatType.ActionSpeed ) +
			       GameBalanceResource.ActiveBalance.DexterityToActionSpeedCurve.Evaluate( Dexterity );
		}
	}
	
	public float SpellCastingSpeed
	{
		get
		{
			return ActorComponent.GetStat( StatType.SpellCastingSpeed ) +
			       GameBalanceResource.ActiveBalance.SpellCastingAttributeToSpellCastingSpeedCurve.Evaluate( SpellCastingAttribute );
		}
	}
	
	#endregion

	#region Movement
	public float MoveSpeedMultiplier
	{
		get
		{
			return ActorComponent.GetStat( StatType.MoveSpeedMultiplier ) +
			       GameBalanceResource.ActiveBalance.DexterityToMoveSpeedCurve.Evaluate( Dexterity );
		}
	}

	public float JumpHeightMultiplier
	{
		get
		{
			return ActorComponent.GetStat( StatType.JumpHeightMultiplier ) +
			       GameBalanceResource.ActiveBalance.StrengthToJumpHeightCurve.Evaluate( Strength );
		}
	}
	#endregion

	#region Combat
	
	public float PhysicalPower
	{
		get
		{
			return ActorComponent.GetStat( StatType.PhysicalPower ) +
			       GameBalanceResource.ActiveBalance.PrimaryAttributeToPhysicalPowerCurve.Evaluate( PrimaryAttribute );
		}
	}
	
	public float SpellPower
	{
		get
		{
			return ActorComponent.GetStat( StatType.SpellPower ) +
			       GameBalanceResource.ActiveBalance.SpellCastingAttributeToSpellPowerCurve.Evaluate(
				       SpellCastingAttribute );
		}
	}
	
	public float FirePower => ActorComponent.GetStat( StatType.FirePower );
	public float FrostPower => ActorComponent.GetStat( StatType.FrostPower );
	public float ElectricPower => ActorComponent.GetStat( StatType.ElectricPower );
	public float PoisonPower => ActorComponent.GetStat( StatType.PoisonPower );
	public float NecroticPower => ActorComponent.GetStat( StatType.NecroticPower );
	
	public float ArcanePower => ActorComponent.GetStat( StatType.ArcanePower );
	public float DivinePower => ActorComponent.GetStat( StatType.DivinePower );
	public float OccultPower => ActorComponent.GetStat( StatType.OccultPower );

	public float ArmorPenetration => ActorComponent.GetStat( StatType.ArmorPenetration );
	public float WardingPenetration => ActorComponent.GetStat( StatType.WardingPenetration );
	public float FirePenetration => ActorComponent.GetStat( StatType.FirePenetration );
	public float FrostPenetration => ActorComponent.GetStat( StatType.FrostPenetration );
	public float ElectricPenetration => ActorComponent.GetStat( StatType.ElectricPenetration );
	public float PoisonPenetration => ActorComponent.GetStat( StatType.PoisonPenetration );
	public float NecroticPenetration => ActorComponent.GetStat( StatType.NecroticPenetration );
	
	public float ArcanePenetration => ActorComponent.GetStat( StatType.ArcanePenetration );
	public float DivinePenetration => ActorComponent.GetStat( StatType.DivinePenetration );
	public float OccultPenetration => ActorComponent.GetStat( StatType.OccultPenetration );

	public float CriticalMultiplier => ActorComponent.GetStat( StatType.CriticalMultiplier );

	public float KnockBack
	{
		get
		{
			return ActorComponent.GetStat( StatType.KnockBack ) + GameBalanceResource.ActiveBalance.StrengthToKnockBackCurve.Evaluate(
				Strength );
		}
	}
	
	#endregion

	#region Defense

	public float Armor => ActorComponent.GetStat( StatType.Armor );
	public float Warding => ActorComponent.GetStat( StatType.Warding );
	public float CriticalNegation => ActorComponent.GetStat( StatType.CriticalNegation );
	public float KnockBackResistance => ActorComponent.GetStat( StatType.KnockBackResistance );
	
	public float FireResistance => ActorComponent.GetStat( StatType.FireResistance );
	public float FrostResistance => ActorComponent.GetStat( StatType.FrostResistance );
	public float ElectricResistance => ActorComponent.GetStat( StatType.ElectricResistance );
	public float PoisonResistance => ActorComponent.GetStat( StatType.PoisonResistance );
	public float NecroticResistance => ActorComponent.GetStat( StatType.NecroticResistance );

	public float ArcaneResistance => ActorComponent.GetStat( StatType.ArcaneResistance );
	public float DivineResistance => ActorComponent.GetStat( StatType.DivineResistance );
	public float OccultResistance => ActorComponent.GetStat( StatType.OccultResistance );

	#endregion


	#region Misc

	public float CarryWeight
	{
		get
		{
			return ActorComponent.GetStat( StatType.CarryWeight ) +
			       GameBalanceResource.ActiveBalance.StrengthToCarryWeightCurve.Evaluate( Strength );
		}
	}
	
	public float Luck
	{
		get
		{
			return ActorComponent.GetStat( StatType.Luck );
		}
	}

	#endregion

	//i really tried my best to avoid something like this but frankly, it is awfully convenient.
	//perhaps if i hate myself enough i'll rewrite this mess
	public float GetStatForStatType( StatType type )
	{
		return type switch
		{
			StatType.Strength => Strength,
			StatType.Dexterity => Dexterity,
			StatType.Constitution => Constitution,
			StatType.Wisdom => Wisdom,
			StatType.Intelligence => Intelligence,
			StatType.Charisma => Charisma,
			
			StatType.MaxHealth => MaxHealth,
			StatType.HealthRegen => HealthRegen,
			StatType.HealMultiplier => HealMultiplier,
			StatType.HealthRegenDelay => HealthRegenDelay,
			
			StatType.MaxStamina => MaxStamina,
			StatType.StaminaRegen => StaminaRegen,
			StatType.StaminaCostMultiplier => StaminaCostMultiplier,
			StatType.StaminaRegenDelay => StaminaRegenDelay,
			
			StatType.ActionSpeed => ActionSpeed,
			StatType.SpellCastingSpeed => SpellCastingSpeed,
			
			StatType.MoveSpeedMultiplier => MoveSpeedMultiplier,
			StatType.JumpHeightMultiplier => JumpHeightMultiplier,
			
			StatType.PhysicalPower => PhysicalPower,
			StatType.SpellPower => SpellPower,
			
			StatType.FirePower => FirePower,
			StatType.FrostPower => FrostPower,
			StatType.ElectricPower => ElectricPower,
			StatType.PoisonPower => PoisonPower,
			StatType.NecroticPower => NecroticPower,
			
			StatType.ArcanePower => ArcanePower,
			StatType.DivinePower => DivinePower,
			StatType.OccultPower => OccultPower,
			
			StatType.ArmorPenetration => ArcanePenetration,
			StatType.WardingPenetration => WardingPenetration,
			
			StatType.FirePenetration => FirePenetration,
			StatType.FrostPenetration => FrostPenetration,
			StatType.ElectricPenetration => ElectricPenetration,
			StatType.PoisonPenetration => PoisonPenetration,
			StatType.NecroticPenetration => NecroticPenetration,
			StatType.ArcanePenetration => ArcanePenetration,
			StatType.DivinePenetration => DivinePenetration,
			StatType.OccultPenetration => OccultPenetration,
			
			StatType.CriticalMultiplier => CriticalMultiplier,
			StatType.KnockBack => KnockBack,
			
			StatType.Armor => Armor,
			StatType.Warding => Warding,
			StatType.CriticalNegation => CriticalNegation,
			StatType.KnockBackResistance => KnockBackResistance,
			
			StatType.FireResistance => FireResistance,
			StatType.FrostResistance => FrostResistance,
			StatType.ElectricResistance => ElectricResistance,
			StatType.PoisonResistance => PoisonResistance,
			StatType.NecroticResistance => NecroticResistance,
			StatType.ArcaneResistance => ArcaneResistance,
			StatType.DivineResistance => DivineResistance,
			StatType.OccultResistance => OccultResistance,
			
			StatType.CarryWeight => CarryWeight,
			StatType.Luck => Luck,
			
			_ => throw new ArgumentOutOfRangeException( nameof(type), type, null )
		};
	}
	
}
