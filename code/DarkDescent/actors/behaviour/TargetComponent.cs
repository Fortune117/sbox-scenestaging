namespace DarkDescent.Actor;

/// <summary>
/// Anything with this component can be 'targeted' by actors with a targeting component.
/// Essentially designates an object as existing for actor npcs to interact with.
/// </summary>
public class TargetComponent : BaseComponent
{
	private ActorComponent ActorComponent { get; set; }

	public override void OnEnabled()
	{
		Blackboard.Register( this );
	}

	public override void OnDisabled()
	{
		Blackboard.UnRegister( this );
	}

	public override void OnDestroy()
	{
		Blackboard.UnRegister( this );
	}
	
	public override void OnStart()
	{
		ActorComponent = GetComponent<ActorComponent>();

		if ( ActorComponent is null )
		{
			Log.Error( $"No Actor Component on {GameObject.Name} for Behaviour Component!" );
		}
	}
}
