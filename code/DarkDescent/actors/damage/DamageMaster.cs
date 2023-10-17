using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor.Damage;

public static class DamageMaster
{
	
	/// <summary>
	/// Proper nasty function. I should really try and figure out if there's a better way to handle this.
	/// </summary>
	/// <param name="actorComponent"></param>
	/// <param name="damageEventData"></param>
	public static void ApplyResistances( ActorComponent actorComponent, ref DamageEventData damageEventData )
	{
		var resist = float.PositiveInfinity;
        var stats = actorComponent.Stats;

        if (damageEventData.HasDamageType(DamageType.Physical))
        {
            resist = stats.Armor;
        }

        if (damageEventData.HasDamageType(DamageType.Magical) && stats.Warding < resist)
        {
	        resist = stats.Warding;
        }

        if (damageEventData.HasDamageType(DamageType.Fire) && stats.FireResistance < resist)
        {
	        resist = stats.FireResistance;
        }

        if (damageEventData.HasDamageType(DamageType.Frost) && stats.FrostResistance < resist)
        {
	        resist = stats.FrostResistance;
        }

        if (damageEventData.HasDamageType(DamageType.Electric) && stats.ElectricResistance < resist)
        {
	        resist = stats.ElectricResistance;
        }

        if (damageEventData.HasDamageType(DamageType.Poison) && stats.PoisonResistance < resist)
        {
	        resist = stats.PoisonResistance;
        }
        
        if (damageEventData.HasDamageType(DamageType.Necrotic) && stats.NecroticResistance < resist)
        {
	        resist = stats.NecroticResistance;
        }

        if (damageEventData.HasDamageType(DamageType.Arcane) && stats.ArcaneResistance < resist)
        {
	        resist = stats.ArcaneResistance;
        }

        if (damageEventData.HasDamageType(DamageType.Divine) && stats.DivineResistance < resist)
        {
	        resist = stats.DivineResistance;
        }

        if (damageEventData.HasDamageType(DamageType.Occult) && stats.OccultResistance < resist)
        {
	        resist = stats.OccultResistance;
        }

        //this is here cause if our resist is infinity, it just means it never got set.
        if ( float.IsPositiveInfinity( resist ) )
        {
	        resist = 0;
        }
        
        var resistMult = damageEventData.HasFlag(DamageFlags.IgnoreResistance)
            ? 1
            : GameBalanceResource.ActiveBalance.ResistanceScalingCurve.Evaluate(resist - damageEventData.ResistancePenetration);
        
        damageEventData.DamageResult = damageEventData.DamageOriginal - damageEventData.DamageOriginal*resistMult;
	}
}
