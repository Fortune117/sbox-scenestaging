using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class DamageOverTimeEffect : StatusEffectComponent
{
	/// <summary>
	/// Damage that will be dealt per second. Accounts for tick rate, etc.
	/// </summary>
	[Property, ToggleGroup("Damage")]
	public DamageResource DamageResource { get; set; }
	
	protected override void Tick( float delta )
	{
		var body = ManagerComponent.ActorComponent.Body;
		
		var damageEvent = DamageResource.GenerateDamageEvent(Originator.Stats)
			.WithOriginator( Originator )
			.WithTarget( ManagerComponent.ActorComponent )
			.WithPosition( body.Bounds.Center )
			.WithDirection( Vector3.Zero )
			.WithKnockBack( 0 )
			.AsCritical( false );

		damageEvent.DamageOriginal *= TimeSinceLastTick;
		damageEvent.DamageResult *= TimeSinceLastTick;
		
		ManagerComponent.ActorComponent.TakeDamage( damageEvent );
	}
}
