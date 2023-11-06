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
	public List<DamageFactors> Factors { get; set; }
	
	public float Flat { get; set; }
	public DamageType DamageTypes { get; set; }

	public float Calculate( Stats stats )
	{
		return Dice.Sum( x => x.Roll() ) + Factors.Sum( x => x.Calculate( stats )) + Flat;
	}
}

public struct DamageFactors
{
	public DamageFactors() { }

	public StatType Stat { get; set; }
	public float Factor { get; set; }

	public float Calculate( Stats stats )
	{
		return stats.GetStatForStatType( Stat ) * Factor;
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
