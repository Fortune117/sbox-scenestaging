using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// Controls how our lil actor should think, act and feel about life in general.
/// </summary>
public class BehaviourComponent : BaseComponent
{
	protected TargetingComponent TargetingComponent { get; set; }
	protected ActorComponent ActorComponent { get; set; }

	protected TargetComponent Target => TargetingComponent.Target;

	public override void OnStart()
	{
		TargetingComponent = GetComponent<TargetingComponent>();
		ActorComponent = GetComponent<ActorComponent>();

		if ( ActorComponent is null )
		{
			Log.Error( $"No Actor Component on {GameObject.Name} for Behaviour Component!" );
		}
	}
}
