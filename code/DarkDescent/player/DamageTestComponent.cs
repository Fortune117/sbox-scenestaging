using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent;

public class DamageTestComponent : BaseComponent
{
	public override void Update()
	{
		if ( !Input.Pressed( GameTags.Input.AttackPrimary ) )
			return;

		if ( !TryGetComponent<DarkDescentPlayerController>( out var playerController ) )
			return;

		var tr = Physics.Trace.Ray( playerController.AimRay, 200 )
			.UseHitboxes()
			.Radius( 3f )
			.Run();

		if ( !tr.Hit )
			return;

		var gameObject = tr.Body.GameObject;
		if ( gameObject is not GameObject hitGameObject )
			return;

		var hitActor = hitGameObject.GetComponentInParent<ActorComponent>( true, true );
		if ( hitActor is null )
			return;

		foreach ( var tag in tr.Tags )
		{
			Log.Info( tag );
		}

		var actor = GetComponent<ActorComponent>();
		var knockback = actor is not null ? actor.Stats.KnockBack : 0;

		var damage = new DamageEventData()
			.WithOriginator( actor )
			.WithTarget( hitActor )
			.WithPosition( tr.HitPosition + tr.Normal * 5f )
			.WithDirection( tr.Direction )
			.WithKnockBack( knockback )
			.WithDamage( 15f )
			.WithType( DamageType.Physical )
			.AsCritical( false );

		hitActor.TakeDamage( damage );
	}
}
