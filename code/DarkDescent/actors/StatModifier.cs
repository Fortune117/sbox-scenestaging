using DarkDescent.Actor;

namespace DarkDescent;

public struct StatModifier
{
	public enum ModifierType
	{
		Addition,
		AdditiveMultiply
	}

	public StatType StatType { get; set; }
	public ModifierType Type { get; set; }
	public float Value { get; set; }

	public StatModifier( StatType statType, ModifierType modifierType, float value )
	{
		StatType = statType;
		Type = modifierType;
		Value = value;
	}

	public void Apply(StatValue statValue)
	{
		switch (Type)
		{
			case ModifierType.Addition:
				statValue.Modifier += Value;
				break;
			case ModifierType.AdditiveMultiply:
				statValue.Modifier += statValue.Base * Value;
				break;
		}
	}

	public void Remove( StatValue statValue )
	{
		switch (Type)
		{
			case ModifierType.Addition:
				statValue.Modifier -= Value;
				break;
			case ModifierType.AdditiveMultiply:
				statValue.Modifier -= statValue.Base * Value;
				break;
		}
	}
}
