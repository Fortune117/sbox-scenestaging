using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Components;

public class DamageableObject : BaseComponent, IDamageable
{
	[Property]
	public bool CauseHitBounce { get; set; }
	
	public void TakeDamage( DamageEventData damageEventData )
	{
		var physics = GetComponent<PhysicsComponent>( true, true );
		if ( physics is not null && physics.GetBody() is not null )
		{
			ApplyKnockBack( physics.GetBody(), damageEventData );
		}
	}
	
	private void ApplyKnockBack(PhysicsBody body, DamageEventData damageEventData)
	{
		if ( !body.IsValid() )
			return;
		
		body.ApplyForceAt( body.FindClosestPoint( damageEventData.Position ),
			damageEventData.Direction * damageEventData.KnockBackResult * body.Mass * 300 );
	}
}
