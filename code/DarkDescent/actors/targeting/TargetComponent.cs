using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// Anything with this component can be 'targeted' by actors with a targeting component.
/// Essentially designates an object as existing for actor npcs to interact with.
/// </summary>
public class TargetComponent : Component
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

	protected override void OnEnabled()
	{
		Blackboard.Register( this );
	}

	protected override void OnDisabled()
	{
		Blackboard.UnRegister( this );
	}

	protected override void OnDestroy()
	{
		Blackboard.UnRegister( this );
	}
}
