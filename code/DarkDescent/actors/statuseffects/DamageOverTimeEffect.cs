using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class DamageOverTimeEffect : StatusEffectComponent
{
	[Property, ToggleGroup("Damage")]
	public DamageResource DamageResource { get; set; }
	
	protected override void Tick( float delta )
	{
		var damageEvent = new DamageEventData()
			.WithOriginator( ManagerComponent.ActorComponent )
			.WithTarget( ManagerComponent.ActorComponent )
			.WithPosition( ManagerComponent.ActorComponent.Transform.Position )
			.WithDirection( Vector3.Zero )
			.WithKnockBack( 0 )
			.WithDamage( 2 * TimeSinceLastTick )
			.WithType( DamageType.Fire )
			.WithFlags( DamageFlags.NoFlinch )
			.AsCritical( false );
		
		ManagerComponent.ActorComponent.TakeDamage( damageEvent );
	}
}
