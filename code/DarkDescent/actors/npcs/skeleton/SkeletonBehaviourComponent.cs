using Sandbox;

namespace DarkDescent.Actor;

public class SkeletonBehaviourComponent : BehaviourComponent
{
	[Property]
	private AnimatedModelComponent Body { get; set; }
	
	public override void Update()
	{
		base.Update();
	}
}
