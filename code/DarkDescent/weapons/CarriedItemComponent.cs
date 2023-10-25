using System.Linq;
using Sandbox;

namespace DarkDescent.Weapons;

public class CarriedItemComponent : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property]
	private bool FollowBoneMerge { get; set; }
	
	[Property]
	private ColliderBoxComponent HitBox { get; set; }
	
	private AnimatedModelComponent AnimatedModelComponent { get; set; }

	public override void OnEnabled()
	{
		base.OnEnabled();

		AnimatedModelComponent = GetComponent<AnimatedModelComponent>();
	}

	public override void OnStart()
	{
		AnimatedModelComponent = GetComponent<AnimatedModelComponent>();
	}

	public override void OnDisabled()
	{
		base.OnDisabled();

		AnimatedModelComponent = null;
	}

	public override void Update()
	{
		base.Update();

		if ( !FollowBoneMerge || AnimatedModelComponent?.SceneObject is null )
			return;
		
		var transform = AnimatedModelComponent.SceneObject.GetBoneWorldTransform( 0 );
		GameObject.Transform.World = transform;

		var body = HitBox.Shapes.FirstOrDefault()?.Body;

		if ( body is null || Scene.IsEditor)
			return;
		
		/*var tr = Physics.Trace.Ray(  )
			.UseHitboxes()
			.WithTag( "dummy" )
			.Run();*/
	}
}
