using DarkDescent.Actor;
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

		var actor = tr.Body.GameObject;
		if ( actor is not GameObject hitGameObject )
			return;

		var hitActor = hitGameObject.GetComponentInParent<ActorComponent>( true, true );
		if ( hitActor is null )
			return;

		foreach ( var tag in tr.Tags )
		{
			Log.Info( tag );
		}

		var damage = new DamageInfo()
			.WithPosition( tr.HitPosition + tr.Normal * 5f )
			.WithForce( tr.Direction )
			.WithDamage( 15f )
			.WithTag( GameTags.Damage.Physical );

		hitActor.TakeDamage( damage );
	}
}
