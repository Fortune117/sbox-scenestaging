using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Items;

public class EnchantTest : BaseComponent, IDamageDealtListener
{
	private IDamageInflictor Inflictor { get; set; }
	
	[Property]
	private GameObject TargetStatusEffectPrefab { get; set; }
	
	[Property]
	private GameObject SelfStatusEffectPrefab { get; set; }
	
	public override void OnAwake()
	{
		Inflictor = GetComponentInParent<IDamageInflictor>( false, true );
	}

	public void OnDamageDealt( DamageEventData damageEventData, bool isLethal )
	{
		ApplyTargetEffects( damageEventData, isLethal );
	}

	private void ApplyTargetEffects(DamageEventData damageEventData, bool isLethal)
	{
		if ( TargetStatusEffectPrefab is null )
			return;
		
		if ( damageEventData.Inflictor != Inflictor )
			return;

		if ( !damageEventData.Target.GameObject.TryGetComponent<ActorComponent>( out var actorComponent ) )
			return;
		
		actorComponent.StatusEffectsManager.AddStatusEffect( TargetStatusEffectPrefab, damageEventData.Originator );
	}
	
	private void ApplySelfEffects(DamageEventData damageEventData, bool isLethal)
	{
		if ( SelfStatusEffectPrefab is null )
			return;
		
		if ( damageEventData.Inflictor != Inflictor )
			return;
		
		damageEventData.Originator.StatusEffectsManager.AddStatusEffect( SelfStatusEffectPrefab, damageEventData.Originator );
	}
}
