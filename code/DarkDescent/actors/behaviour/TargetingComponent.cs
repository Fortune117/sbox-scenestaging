namespace DarkDescent.Actor;

public class TargetingComponent : BaseComponent
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
