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

		CarriedItemComponent.PhysicsComponent.AngularVelocity = -direction.Cross( Vector3.Up ) * 20f;
		CarriedItemComponent.PhysicsComponent.Gravity = false;
		WeaponComponent.Interactable = false;
	}

	public override void Update()
	{
		base.Update();

		if ( CarriedItemComponent.PhysicsComponent.GetBody() is null )
			return;
		
		CarriedItemComponent.PhysicsComponent.Velocity -= Vector3.Down * Scene.PhysicsWorld.Gravity * GravityMultiplier * Time.Delta;
	}

	protected override void OnThrowImpact( Collision collision )
	{
		WeaponComponent.Interactable = true;

		var damageable = collision.Other.GameObject.GetComponentInParent<IDamageable>( true, true );
		if ( damageable is null )
			return;
		
		//WeaponComponent.PhysicsComponent.Enabled = false;
		//WeaponComponent.ModelCollider.IsTrigger = true;
		
		//GameObject.SetParent( damageable.GameObject );
		//GameObject.Transform.Position = collision.Contact.Point;

		var dir = collision.Other.Shape.Body.FindClosestPoint(collision.Contact.Point) -
		          GameObject.Transform.Position;

		//GameObject.Transform.Rotation = Rotation.LookAt(dir.Normal);

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
