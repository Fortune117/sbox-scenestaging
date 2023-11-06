using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor.Damage;

/// <summary>
/// Temporary resource while I figure out where the fuck to put this logic.
/// </summary>
[GameResource("Damage", "damage", "Contains data about damage that can scale off a player.")]
public class DamageResource : GameResource
{
	public List<Die> Dice { get; set; }
	public float Flat { get; set; }
	public DamageFactors Factors { get; set; }
	public DamageType DamageTypes { get; set; }

	public float Calculate( Stats stats )
	{
		return Dice.Sum( x => x.Roll() ) + Factors.Calculate( stats ) + Flat;
	}
}

public struct DamageFactors
{
	public DamageFactors() { }

	[Range( -1, 2 )]
	public float PhysicalPower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float SpellPower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float FirePower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float FrostPower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float ElectricPower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float PoisonPower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float NecroticPower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float ArcanePower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float DivinePower { get; set; } = 0;
	
	[Range( -1, 2 )]
	public float OccultPower { get; set; } = 0;

	public float Calculate( Stats stats )
	{
		return PhysicalPower * stats.PhysicalPower
		       + SpellPower * stats.SpellPower
		       + FirePower * stats.FirePower
		       + FrostPower * stats.FrostPower
		       + ElectricPower * stats.ElectricPower
		       + PoisonPower * stats.PoisonPower
		       + NecroticPower * stats.NecroticPower
		       + ArcanePower * stats.ArcanePower
		       + DivinePower * stats.DivinePower
		       + OccultPower * stats.OccultPower;
	}
}
public struct Die
{
	public int Count { get; set; }
	public DieType Type { get; set; }

	public int Roll()
	{
		var total = 0;
		for ( var i = 0; i < Count; i++ )
		{
			total += Game.Random.Next(1, (int)Type);
		}

		return total;
	}
}

public enum DieType
{
	D4 = 4,
	D6 = 6,
	D8 = 8,
	D10 = 10,
	D12 = 12,
	D20 = 20,
}
