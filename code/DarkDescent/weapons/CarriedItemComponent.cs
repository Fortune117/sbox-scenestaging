﻿using System.Collections.Generic;
using System.Linq;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Weapons;

public partial class CarriedItemComponent : BaseComponent, BaseComponent.ExecuteInEditor
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
	public AnimatedModelComponent AnimatedModelComponent { get; set; }
	public ThrowableComponent ThrowableComponent { get; set; }

	public bool CanThrow => ThrowableComponent is not null;

	public virtual bool HasPriority { get; set; } = false;
	
	[Property]
	protected bool FollowBoneMerge { get; set; }

	public override void OnAwake()
	{
		ThrowableComponent = GetComponent<ThrowableComponent>(false);
		AnimatedModelComponent = GetComponent<AnimatedModelComponent>(false);
		ModelCollider = GetComponent<ModelCollider>(false);
		PhysicsComponent = GetComponent<PhysicsComponent>(false);
	}

	protected override void OnPreRender()
	{
		if ( !FollowBoneMerge || AnimatedModelComponent?.SceneObject is null )
			return;
		
		var transform = AnimatedModelComponent.SceneObject.GetBoneWorldTransform( 0 );
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
		ModelCollider.Enabled = false;
		PhysicsComponent.Enabled = false;
	}

	public virtual void UpdateForPlayer() { }

	public virtual Vector2 UpdateInputForPlayer( Vector2 input )
	{
		return input;
	}
}
