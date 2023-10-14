using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor.Damage;

public static class DamageMaster
{
	public static DamageEventData GenerateDamageEventData(ActorComponent targetActor, DamageInfo damageInfo)
	{
		var damageEventData = new DamageEventData();
		var typeAndFlags = ConvertTagsToTypeAndFlags( damageInfo.Tags );

		damageEventData = damageEventData.WithTarget( targetActor.GameObject );
		//damageEventData = damageEventData.WithOriginator( damageInfo.Damage );
		
		damageEventData.DamageOriginal = damageInfo.Damage;
		
		damageEventData.Position = damageInfo.Position;
		damageEventData.Direction = damageInfo.Force.Normal;
		
		if (damageEventData.OriginatorActor is not null)
			damageEventData.KnockBack = damageEventData.OriginatorActor.Stats.KnockBack;
		
		damageEventData.ResistancePenetration = 0; //TODO: calculate this somehow
		
		damageEventData.DamageTypes = typeAndFlags.damageType;
		damageEventData.DamageFlags = typeAndFlags.damageFlags;
		
		damageEventData.IsCritical = false;
		
		ApplyResistances( targetActor, ref damageEventData );
		
		return damageEventData;
	}

	/// <summary>
	/// Proper nasty function. I should really try and figure out if there's a better way to handle this.
	/// </summary>
	/// <param name="actorComponent"></param>
	/// <param name="damageEventData"></param>
	private static void ApplyResistances( ActorComponent actorComponent, ref DamageEventData damageEventData )
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

	private static (DamageType damageType, DamageFlags damageFlags) ConvertTagsToTypeAndFlags( IEnumerable<string> tags )
	{
		var damageTypes = DamageType.None;
		var damageFlags = DamageFlags.None;

		//not great :/
		foreach ( var tag in tags )
		{
			switch ( tag )
			{
				case GameTags.Damage.Physical:
					damageTypes |= DamageType.Physical;
					break;
				case GameTags.Damage.Magical:
					damageTypes |= DamageType.Magical;
					break;
				case GameTags.Damage.Fire:
					damageTypes |= DamageType.Fire;
					break;
				case GameTags.Damage.Frost:
					damageTypes |= DamageType.Frost;
					break;
				case GameTags.Damage.Electric:
					damageTypes |= DamageType.Electric;
					break;
				case GameTags.Damage.Poison:
					damageTypes |= DamageType.Poison;
					break;
				case GameTags.Damage.Necrotic:
					damageTypes |= DamageType.Necrotic;
					break;
				case GameTags.Damage.Arcane:
					damageTypes |= DamageType.Arcane;
					break;
				case GameTags.Damage.Divine:
					damageTypes |= DamageType.Divine;
					break;
				case GameTags.Damage.Occult:
					damageTypes |= DamageType.Occult;
					break;
				case GameTags.Damage.Absolute:
					damageTypes |= DamageType.Absolute;
					break;
				case GameTags.Damage.Chaotic:
					damageTypes |= DamageType.Chaotic;
					break;
				
				//DAMAGE FLAGS
				
				case GameTags.Damage.Flags.IgnoreResistance:
					damageFlags |= DamageFlags.IgnoreResistance; 
					break;
			}
		}

		return (damageTypes, damageFlags);
	}
}
