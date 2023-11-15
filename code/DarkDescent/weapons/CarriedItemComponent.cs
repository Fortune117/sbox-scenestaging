using System.Collections.Generic;
using System.Linq;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Weapons;

public class CarriedItemComponent : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property]
	protected bool FollowBoneMerge { get; set; }
	
	[Property]
	public GameObject RightHandIKTarget { get; set; }
	
	[Property]
	public GameObject LeftHandIKTarget { get; set; }

	protected AnimatedModelComponent AnimatedModelComponent { get; set; }
	
	protected ThrowableComponent ThrowableComponent { get; set; }

	public bool CanThrow => ThrowableComponent is not null;
	
	public override void OnEnabled()
	{
		base.OnEnabled();

		AnimatedModelComponent = GetComponent<AnimatedModelComponent>();
	}

	public override void OnStart()
	{
		ThrowableComponent = GetComponent<ThrowableComponent>(false);
		AnimatedModelComponent = GetComponent<AnimatedModelComponent>();
	}

	public override void OnDisabled()
	{
		base.OnDisabled();
	}

	public override void Update()
	{
		base.Update();
		
		if ( !FollowBoneMerge || AnimatedModelComponent?.SceneObject is null )
			return;
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
}
