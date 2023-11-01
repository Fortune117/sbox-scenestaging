using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// Anything with this component can be 'targeted' by actors with a targeting component.
/// Essentially designates an object as existing for actor npcs to interact with.
/// </summary>
public class TargetComponent : BaseComponent
{
	/// <summary>
	/// The factions this target is a part of.
	/// </summary>
	[Property]
	public Targeting.Faction Factions { get; set; }
	
	/// <summary>
	/// The primary point on this object that should be targeted.
	/// i.e. players center of mass
	/// </summary>
	[Property]
	public GameObject PrimaryTargetPoint { get; set; }
	
	/// <summary>
	/// A publicly accessible 'weakpoint' enemies can choose to target. i.e. the players head.
	/// </summary>
	[Property]
	public GameObject WeakPoint { get; set; }
	
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
			Log.Error( $"No Actor Component on {GameObject.Name} for Target Component!" );
		}
	}
}
