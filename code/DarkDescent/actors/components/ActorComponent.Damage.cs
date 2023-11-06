using System.Collections.Generic;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using DarkDescent.Actor.UI;
using DarkDescent.Components;
using DarkDescent.GameLog;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent : IDamageable
{
	[Property] 
	public bool CauseHitBounce { get; set; } = false;

	[Property]
	public bool ShowDamageNumbers { get; set; } = true;
	
	/// <summary>
	/// When we take damage, we assume it's going to be mostly unprocessed damage.
	/// </summary>
	/// <param name="damageEventData"></param>
	public void TakeDamage( DamageEventData damageEventData )
	{
		damageEventData.Target = this;
		
		//apply our resistances as we take damage
		ApplyResistances( ref damageEventData );
		
		var physics = GetComponent<PhysicsComponent>( true, true );
		if ( physics is not null && physics.GetBody() is not null )
		{
			ApplyKnockBack( physics.GetBody(), damageEventData );
		}
		
		damageEventData.CreateDamageEffects();
		
		if (ShowDamageNumbers)
			CreateDamageNumber(damageEventData);
		
		DamageHealth( damageEventData.DamageResult );

		GameLogSystem.DamageEventReceived( damageEventData );

		foreach ( var damageListener in GetComponents<IDamageTakenListener>(true, true) )
		{
			damageListener.OnDamageTaken( damageEventData, Health <= 0 );	
		}
		
		foreach ( var damageDealtListener in damageEventData.Originator.GetComponents<IDamageDealtListener>(true, true) )
		{
			damageDealtListener.OnDamageDealt( damageEventData, Health <= 0 );	
		}
		
		//i'm not dead yet!!
		if ( Health > 0 )
			return;

		if ( damageEventData.Originator is not null )
		{
			var exp = Game.Random.Float( 1, 100 );
			ExperienceNumberComponent.Create( exp, damageEventData );
			damageEventData.Originator.AddExperience( exp );
		}

		Alive = false;
		
		OnDeath(damageEventData);
		
		var modelPhysics = GetComponent<ModelPhysics>( true, true );
		if ( modelPhysics is not null && modelPhysics.PhysicsGroup is not null )
		{			
			PhysicsBody closestBody = null;
			var closestDistance = float.PositiveInfinity;
			foreach ( var body in modelPhysics.PhysicsGroup.Bodies )
			{
				var distance = body.Position.Distance( damageEventData.Position );
				if (  distance < closestDistance )
				{
					closestBody = body;
					closestDistance = distance;
				}
			}
			
			if (closestBody is not null)
				ApplyKnockBack( closestBody, damageEventData, 1f ); 
		}
	}

	private void ApplyKnockBack(PhysicsBody body, DamageEventData damageEventData, float multiplier = 1f)
	{
		if ( !body.IsValid() )
			return;

		var dir = damageEventData.Direction;
		var baseMult = 20000f;
		
		var baseForce = dir * body.Mass * baseMult;
		var knockbackForce = dir * damageEventData.KnockBackResult * baseMult * 5f;
		var additiveForce = dir * multiplier;
		body.ApplyForceAt( body.FindClosestPoint( damageEventData.Position ),
			baseForce + knockbackForce + additiveForce);
	}
	
	public void CreateDamageNumber(DamageEventData damageEventData)
	{
		var damageNumber = DamageNumberComponent.Create( damageEventData );
	}
	
	//TODO: Proper nasty function. I should really try and figure out if there's a better way to handle this.
	
	/// <summary>
	/// Apply our resistances to a damage event.
	/// </summary>
	/// <param name="actorComponent"></param>
	/// <param name="damageEventData"></param>
	public void ApplyResistances( ref DamageEventData damageEventData )
	{
		var resist = float.PositiveInfinity;
		var validOriginator = damageEventData.Originator is not null;
		
        if (damageEventData.HasDamageType(DamageType.Physical))
        {
            resist = Stats.Armor - (validOriginator ? damageEventData.Originator.Stats.ArmorPenetration : 0);;
        }

        var warding = Stats.Warding - (validOriginator ? damageEventData.Originator.Stats.WardingPenetration : 0);
        if (damageEventData.HasDamageType(DamageType.Magical) && warding < resist)
        {
	        resist = warding;
        }

        var fireResist = Stats.FireResistance - (validOriginator ? damageEventData.Originator.Stats.FirePenetration : 0);
        if (damageEventData.HasDamageType(DamageType.Fire) && fireResist < resist)
        {
	        resist = fireResist;
        }
        
        var frostResist = Stats.FrostResistance - (validOriginator ? damageEventData.Originator.Stats.FrostPenetration : 0);
        if (damageEventData.HasDamageType(DamageType.Frost) && frostResist < resist)
        {
	        resist = frostResist;
        }

        var electricResist = Stats.ElectricResistance - (validOriginator ? damageEventData.Originator.Stats.ElectricPenetration : 0);
        if (damageEventData.HasDamageType(DamageType.Electric) && electricResist < resist)
        {
	        resist = electricResist;
        }

        var poisonResist = Stats.PoisonResistance - (validOriginator ? damageEventData.Originator.Stats.PoisonPenetration : 0);
        if (damageEventData.HasDamageType(DamageType.Poison) && poisonResist < resist)
        {
	        resist = poisonResist;
        }
        
        var necroticResist = Stats.NecroticResistance - (validOriginator ? damageEventData.Originator.Stats.NecroticPenetration : 0);
        if (damageEventData.HasDamageType(DamageType.Necrotic) && necroticResist < resist)
        {
	        resist = necroticResist;
        }

        var arcaneResist = Stats.ArcaneResistance - (validOriginator ? damageEventData.Originator.Stats.ArcaneResistance : 0);
        if (damageEventData.HasDamageType(DamageType.Arcane) && Stats.ArcaneResistance < resist)
        {
	        resist = arcaneResist;
        }

        var divineResist = Stats.DivineResistance - (validOriginator ? damageEventData.Originator.Stats.DivineResistance : 0);
        if (damageEventData.HasDamageType(DamageType.Divine) && divineResist < resist)
        {
	        resist = divineResist;
        }

        var occultResist = Stats.OccultResistance - (validOriginator ? damageEventData.Originator.Stats.OccultResistance : 0);
        if (damageEventData.HasDamageType(DamageType.Occult) && occultResist < resist)
        {
	        resist = occultResist;
        }

        //this is here cause if our resist is infinity, it just means it never got set.
        if ( float.IsPositiveInfinity( resist ) )
        {
	        resist = 0;
        }
        
        var resistMult = damageEventData.HasFlag(DamageFlags.IgnoreResistance)
            ? 1
            : GameBalanceResource.ActiveBalance.ResistanceScalingCurve.Evaluate(resist);
        
        damageEventData.DamageResult = damageEventData.DamageOriginal - damageEventData.DamageOriginal*resistMult;
        damageEventData.KnockBackResult = damageEventData.KnockBackOriginal - Stats.KnockBackResistance;
	}
}
