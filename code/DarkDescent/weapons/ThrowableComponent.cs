using DarkDescent.Actor;

namespace DarkDescent.Weapons;

public class ThrowableComponent : BaseComponent, BaseComponent.ICollisionListener
{
	protected CarriedItemComponent CarriedItemComponent { get; set; }
	
	protected ActorComponent Thrower { get; set; }
	
	public override void OnAwake()
	{
		CarriedItemComponent = GetComponent<CarriedItemComponent>(false);
	}

	public virtual void Throw(ActorComponent thrower, Vector3 direction)
	{
		Thrower = thrower;
		
		GameObject.SetParent( Scene );
		
		CarriedItemComponent.AnimatedModelComponent.BoneMergeTarget = null;
		
		CarriedItemComponent.ModelCollider.Enabled = true;
		
		CarriedItemComponent.PhysicsComponent.Enabled = true;
		CarriedItemComponent.PhysicsComponent.Velocity = direction * 1000f;
	}

	public virtual void OnThrow( ActorComponent thrower )
	{
		
	}

	protected virtual void OnThrowImpact( Collision collision ) { }

	public virtual void OnCollisionStart( Collision collision )
	{
		Enabled = false;
		CarriedItemComponent.PhysicsComponent.Gravity = true;
		
		OnThrowImpact(collision);
	}

	public virtual void OnCollisionUpdate( Collision collision )
	{
		
	}

	public virtual void OnCollisionStop( CollisionStop collision )
	{
	}
}
