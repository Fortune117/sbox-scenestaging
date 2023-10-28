using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// Controls how our lil actor should think, act and feel about life in general.
/// </summary>
public class BehaviourComponent : BaseComponent
{
	private ActorComponent ActorComponent { get; set; }

	public override void OnStart()
	{
		ActorComponent = GetComponent<ActorComponent>();

		if ( ActorComponent is null )
		{
			Log.Error( $"No Actor Component on {GameObject.Name} for Behaviour Component!" );
		}
	}
}
