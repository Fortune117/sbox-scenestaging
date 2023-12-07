using System.Collections.Generic;
using System.Linq;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Items;

public partial class ItemComponent : Component, Component.ExecuteInEditor
{
	public DarkDescentPlayerController PlayerController { get; set; }
	
	[Property]
	public GameObject RightHandIKTarget { get; set; }
	
	[Property]
	public GameObject LeftHandIKTarget { get; set; }
	
	[Property]
	public HoldType HoldType { get; set; }
	
	[Property]
	public Handedness Handedness { get; set; }
	
	public ModelCollider ModelCollider { get; set; }
	public PhysicsComponent PhysicsComponent { get; set; }
	public SkinnedModelRenderer AnimatedModelComponent { get; set; }
	public ThrowableComponent ThrowableComponent { get; set; }

	public bool CanThrow => ThrowableComponent is not null;

	public virtual bool HasPriority { get; set; } = false;
	
	[Property]
	protected bool FollowBoneMerge { get; set; }

	protected override void OnAwake()
	{
		ThrowableComponent = Components.Get<ThrowableComponent>(true);
		AnimatedModelComponent = Components.Get<SkinnedModelRenderer>(true);
		ModelCollider = Components.Get<ModelCollider>(true);
		PhysicsComponent = Components.Get<PhysicsComponent>(true);
	}

	protected override void OnPreRender()
	{
		if ( !FollowBoneMerge || AnimatedModelComponent?.SceneObject is null )
			return;
		
		var transform = AnimatedModelComponent.SceneModel.GetBoneWorldTransform( 0 );
		GameObject.Transform.Position = transform.Position;
		GameObject.Transform.Rotation = transform.Rotation;
		
		base.OnPreRender();
	}

	public void Throw(ActorComponent thrower, Vector3 direction)
	{
		ThrowableComponent.Enabled = true;
		ThrowableComponent.Throw( thrower, direction );
	}

	public virtual void OnEquipped()
	{
		if (ModelCollider is not null)
			ModelCollider.Enabled = false;
		PhysicsComponent.Enabled = false;
	}

	public virtual void UpdateForPlayer() { }

	public virtual Vector2 UpdateInputForPlayer( Vector2 input )
	{
		return input;
	}
}

public enum HoldType
{
	None = 0,
	Sword = 1,
}

public enum Handedness
{
	None = 0,
	Right = 1,
	Left = 2,
	Both
}
