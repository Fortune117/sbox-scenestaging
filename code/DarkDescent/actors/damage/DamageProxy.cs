using Sandbox;

namespace DarkDescent.Actor.Damage;

public class DamageProxy : Component, IDamageable
{
	[Property]
	public ActorComponent DamageableProxy { get; set; }


	public bool CauseHitBounce { get; set; }
	public bool PlayHitSound { get; set; }

	public void TakeDamage( DamageEventData data )
	{
		DamageableProxy?.TakeDamage( data );
	}
}
