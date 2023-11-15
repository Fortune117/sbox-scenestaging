using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Weapons;

public class ThrowableWeaponComponent : ThrowableComponent
{
	private CarriedWeaponComponent WeaponComponent { get; set; }
	
	[Property, Range(0, 1)]
	private float GravityMultiplier { get; set; } = 0.5f;

	public override void OnAwake()
	{
		base.OnAwake();

		WeaponComponent = GetComponent<CarriedWeaponComponent>();
	}

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
		GameObject.Transform.Position = collision.Contact.Point;

		var dir = collision.Other.Shape.Body.FindClosestPoint(collision.Contact.Point) -
		          GameObject.Transform.Position;

		GameObject.Transform.Rotation = Rotation.LookAt(dir.Normal);

		ApplyThrownDamage( damageable, collision );
	}

	private void ApplyThrownDamage( IDamageable target, Collision collision )
	{
		var knockback = Thrower.Stats.KnockBack;

		var damage = WeaponComponent.GetDamage( Thrower );
		
		var damageEvent = new DamageEventData()
			.WithOriginator( Thrower )
			.WithTarget( target )
			.WithSurface( collision.Other.Shape.Surface)
			.WithPosition( collision.Contact.Point )
			.WithDirection( -collision.Contact.Speed.Normal )
			.WithKnockBack( knockback )
			.WithDamage( damage )
			.WithType( WeaponComponent.GetDamageType() )
			.AsCritical( false );
		
		target.TakeDamage( damageEvent );
	}
}
