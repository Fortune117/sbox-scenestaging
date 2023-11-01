using System.Collections.Generic;

namespace DarkDescent.Actor.Damage;

public struct AttackEvent
{
	private HashSet<IDamageable> HitDamageables { get; }
	
	public List<HurtBoxComponent> HurtBoxes { get; }
	
	public GameObject Initiator { get; private set; }

	public AttackEvent()
	{
		HitDamageables = new();
		HurtBoxes = new();
	}

	public AttackHitEvent? CheckForHit()
	{
		foreach ( var hurtBox in HurtBoxes )
		{
			var tr = hurtBox.PerformTrace();
		
			if ( !tr.Hit )
				continue;
			
			var gameObject = tr.Body.GameObject;
			if ( gameObject is not GameObject hitGameObject )
				continue;

			var damageable = hitGameObject.GetComponentInParent<IDamageable>( true, true );
			if ( damageable is null ) //impacted the world?
			{
				continue;
			}

			if ( damageable.GameObject == Initiator )
				continue;

			if ( HitDamageables.Contains( damageable ) )
				continue;

			HitDamageables.Add( damageable );

			var attackHitEvent = new AttackHitEvent();
			attackHitEvent.HitDirection = hurtBox.DirectionMoment;
			attackHitEvent.Damageable = damageable;
			attackHitEvent.TraceResult = tr;

			return attackHitEvent;
		}

		return null;
	}

	public AttackEvent WithHurtBox( HurtBoxComponent hurtBoxComponent )
	{
		HurtBoxes.Add( hurtBoxComponent );
		return this;
	}

	public AttackEvent WithInitiator( GameObject initiator )
	{
		Initiator = initiator;
		return this;
	}
}
