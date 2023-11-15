using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Weapons;

public class ThrowableWeaponComponent : ThrowableComponent
{
	[Property, Range(0, 1)]
	private float GravityMultiplier { get; set; } = 0.5f;
	
	public override void Throw( ActorComponent thrower, Vector3 direction )
	{
		base.Throw( thrower, direction );

		PhysicsComponent.AngularVelocity = -direction.Cross( Vector3.Up ) * 20f;
		PhysicsComponent.Gravity = false;
	}

	public override void Update()
	{
		base.Update();

		if ( PhysicsComponent.GetBody() is null )
			return;
		
		PhysicsComponent.Velocity -= Vector3.Down * Scene.PhysicsWorld.Gravity * GravityMultiplier * Time.Delta;
	}

	protected override void OnThrowImpact( Collision collision )
	{
		if ( !collision.Other.GameObject.TryGetComponent<IDamageable>( out var damageable, true, true ) )
			return;

		PhysicsComponent.Enabled = false;
		ModelCollider.Enabled = false;
		GameObject.SetParent( damageable.GameObject );
	}
}
