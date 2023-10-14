using DarkDescent.Actor;
using Sandbox;

namespace DarkDescent;

public partial class Player
{
	private void SimulateTestDamage()
	{
		if ( !Game.IsServer )
			return;
		
		if ( !Input.Pressed( GameTags.Input.AttackPrimary ) )
			return;

		var tr = Physics.Trace.Ray( AimRay, 200 )
			.UseHitboxes()
			.Radius( 3f )
			.Run();

		/*if ( !tr.Hit )
			return;

		var actor = tr.Body.GameObject;
		if ( ! )
			return;

		foreach ( var tag in tr.Tags )
		{
			Log.Info( tag );
		}
		
		var damage = new DamageInfo()
			.WithPosition( tr.HitPosition + tr.Normal * 5f )
			.WithForce( tr.Direction )
			.UsingTraceResult( tr )
			.WithAttacker( this )
			.WithDamage( 15f )
			.WithTag( GameTags.Damage.Physical );

		actorComponent.TakeDamage( damage ); */
	}
}
