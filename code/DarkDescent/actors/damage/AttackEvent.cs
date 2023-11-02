using System.Collections.Generic;

namespace DarkDescent.Actor.Damage;

public struct AttackEvent
{
	private HashSet<IDamageable> HitDamageables { get; }
	
	public List<HurtBoxComponent> HurtBoxes { get; }
	
	public GameObject Initiator { get; private set; }
	
	public bool Blocked { get; private set; }

	public AttackEvent()
	{
		HitDamageables = new();
		HurtBoxes = new();
	}

	public AttackHitEvent? CheckForHit()
	{
		if ( Blocked )
			return null;
		
		var attackHitEvent = new AttackHitEvent();
		
		foreach ( var hurtBox in HurtBoxes )
		{
			var tr = hurtBox.PerformTrace();
		
			if ( !tr.Hit )
				continue;
			
			var gameObject = tr.Body.GameObject;
			if ( gameObject is not GameObject hitGameObject )
				continue;

			if ( hitGameObject.TryGetComponent<AttackBlockerComponent>( out var blockerComponent ) )
			{
				Log.Info( "BLOCKED!" );
				Blocked = true;
				
				attackHitEvent.HitDirection = hurtBox.DirectionMoment;
				attackHitEvent.Blocker = blockerComponent;
				attackHitEvent.WasBlocked = true;
				attackHitEvent.TraceResult = tr;
				
				blockerComponent.BlockedHit(attackHitEvent);
				
				return attackHitEvent;
			}
			
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
