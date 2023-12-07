using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.GameLog;
using Sandbox;

namespace DarkDescent.Components;

public partial class PhysicsInteractableComponent : Component, IDamageable
{
	[Property]
	public bool CanPickUp { get; set; }
	
	/// <summary>
	/// How much strength is required to pick up this object.
	/// </summary>
	[Property, Range( 0, 100 )]
	public float StrengthThreshold { get; set; } = 10;

	/// <summary>
	/// How much strength below the threshold can we be and still move the object?
	/// </summary>
	[Property,  Range( 0, 100 )]
	public float StrengthLeeway { get; set; } = 5;
	
	[Property]
	public bool CauseHitBounce { get; set; }

	[Property] 
	public bool PlayHitSound { get; set; } = false;
	
	public void TakeDamage( DamageEventData damageEventData )
	{
		damageEventData.CreateDamageEffects();
		
		var physics = Components.GetInDescendantsOrSelf<PhysicsComponent>();
		if ( physics is not null && physics.GetBody() is not null )
		{
			ApplyKnockBack( physics.GetBody(), damageEventData );
		}
	}

	private void ApplyKnockBack( PhysicsBody body, DamageEventData damageEventData )
	{
		if ( !body.IsValid() )
			return;

		if ( damageEventData.Originator is null )
			return;

		var strengthDif = damageEventData.Originator.Stats.Strength - StrengthThreshold + StrengthLeeway;

		if ( strengthDif <= 0 )
			return;

		var frac = strengthDif.Remap( 0, 100, 0.2f, 0.75f );

		body.ApplyForceAt( body.FindClosestPoint( damageEventData.Position ),
			damageEventData.Direction * frac * body.Mass * 150000 );
	}
}
