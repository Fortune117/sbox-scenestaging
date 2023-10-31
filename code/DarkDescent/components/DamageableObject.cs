using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Components;

public class DamageableObject : BaseComponent, IDamageable
{
	[Property]
	public bool CauseHitBounce { get; set; }
	
	public void TakeDamage( DamageEventData data )
	{
		Log.Info( "hello!" );
	}
}
