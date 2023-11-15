using DarkDescent.Actor;

namespace DarkDescent.Weapons;

public class ThrowableComponent : BaseComponent, BaseComponent.ICollisionListener
{
	protected CarriedItemComponent CarriedItemComponent { get; set; }
	protected AnimatedModelComponent AnimatedModelComponent { get; set; }
	protected ModelCollider ModelCollider { get; set; }
	protected PhysicsComponent PhysicsComponent { get; set; }

	protected ActorComponent Thrower { get; set; }
	
	public override void OnAwake()
	{
		CarriedItemComponent = GetComponent<CarriedItemComponent>(false);
		AnimatedModelComponent = GetComponent<AnimatedModelComponent>(false);
		ModelCollider = GetComponent<ModelCollider>(false);
		PhysicsComponent = GetComponent<PhysicsComponent>(false);
	}

	public virtual void Throw(ActorComponent thrower, Vector3 direction)
	{
		Thrower = thrower;
		
		GameObject.SetParent( Scene );
		
		AnimatedModelComponent.BoneMergeTarget = null;
		
		ModelCollider.Enabled = true;
		
		PhysicsComponent.Enabled = true;
		PhysicsComponent.Velocity = direction * 1000f;
	}

	public virtual void OnThrow( ActorComponent thrower )
	{
		
	}

	protected virtual void OnThrowImpact( Collision collision ) { }

	public virtual void OnCollisionStart( Collision collision )
	{
		Enabled = false;
		PhysicsComponent.Gravity = true;
		
		OnThrowImpact(collision);
	}

	public virtual void OnCollisionUpdate( Collision collision )
	{
		
	}

	public virtual void OnCollisionStop( CollisionStop collision )
	{
	}
}
