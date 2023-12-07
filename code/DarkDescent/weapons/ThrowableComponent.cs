using DarkDescent.Actor;

namespace DarkDescent.Items;

public class ThrowableComponent : Component, Component.ICollisionListener
{
	protected ItemComponent ItemComponent { get; set; }
	
	protected ActorComponent Thrower { get; set; }
	
	protected bool IsBeingThrown { get; set; }
	
	protected override void OnAwake()
	{
		ItemComponent = Components.Get<ItemComponent>(true);
	}

	public virtual void Throw(ActorComponent thrower, Vector3 direction)
	{
		Thrower = thrower;
		IsBeingThrown = true;
		
		GameObject.SetParent( Scene );
		
		ItemComponent.AnimatedModelComponent.BoneMergeTarget = null;
		
		ItemComponent.ModelCollider.Enabled = true;
		
		ItemComponent.PhysicsComponent.Enabled = true;
		ItemComponent.PhysicsComponent.Velocity = direction * 1000f;
	}

	public virtual void OnThrow( ActorComponent thrower )
	{
		
	}

	protected virtual void OnThrowImpact( Collision collision ) { }

	public virtual void OnCollisionStart( Collision collision )
	{
		Enabled = false;
		ItemComponent.PhysicsComponent.Gravity = true;
		IsBeingThrown = false;
		
		OnThrowImpact(collision);
	}

	public virtual void OnCollisionUpdate( Collision collision )
	{
		
	}

	public virtual void OnCollisionStop( CollisionStop collision )
	{
	}
}
