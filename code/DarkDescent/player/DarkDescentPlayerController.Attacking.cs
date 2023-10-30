using System.Collections.Generic;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	private enum HoldTypes
	{
		Fists,
		TwoHandedSword
	}
	
	[Property]
	private CarriedWeaponComponent CarriedItemComponent { get; set; }
	
	private static class PlayerEvents
	{
		public const string AttackStartEvent = "AttackStartEvent";
		public const string AttackEndEvent = "AttackEndEvent";
	}
	
	private void OnGenericAnimEvent( SceneModel.GenericEvent genericEvent )
	{
		switch ( genericEvent.Type )
		{
			case PlayerEvents.AttackStartEvent:
				OnAttackStart();
				break;
			case PlayerEvents.AttackEndEvent:
				OnAttackEnd();
				break;
		}
	}

	private bool hitboxesActive;
	private readonly HashSet<ActorComponent> HitActors = new();
	
	private void AttackUpdate()
	{
		var tr = CarriedItemComponent.GetWeaponTrace();

		if ( tr.Hit )
		{
			Gizmo.Transform = Scene.Transform.World;
			Gizmo.Draw.Color = Color.Red.WithAlpha( 0.5f );
			Gizmo.Draw.SolidSphere( tr.EndPosition, 4 );
		}
		
		if ( !hitboxesActive )
			return;
		
		if ( !tr.Hit )
			return;

		var gameObject = tr.Body.GameObject;
		if ( gameObject is not GameObject hitGameObject )
			return;

		var hitActor = hitGameObject.GetComponentInParent<ActorComponent>( true, true );
		if ( hitActor is null )
			return;

		if ( HitActors.Contains( hitActor ) )
			return;

		HitActors.Add( hitActor );
		
		var knockback = ActorComponent is not null ? ActorComponent.Stats.KnockBack : 0;

		var damage = new DamageEventData()
			.WithOriginator( ActorComponent )
			.WithTarget( hitActor )
			.WithPosition( tr.HitPosition + tr.Normal * 5f )
			.WithDirection( CarriedItemComponent.GetImpactDirection() )
			.WithKnockBack( knockback )
			.WithDamage( 1f )
			//.WithTags( tr.Shape.Tags ) //commented out cause we can't get tags like this :<
			.WithType( DamageType.Physical )
			.AsCritical( false );

		hitActor.TakeDamage( damage );
		
		
	}

	private void OnAttackStart()
	{
		HitActors.Clear();
		hitboxesActive = true;
	}

	private void OnAttackEnd()
	{
		hitboxesActive = false;
	}
}
