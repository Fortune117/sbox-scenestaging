using Sandbox;

namespace DarkDescent.Components;

/// <summary>
/// The point of this component is to 'classify' things about an object in a way that is human readable.
/// For instance, when talking about an object in the event log, this can give us the much needed context for it.
/// </summary>
[Prefab]
public partial class ObjectClassifierComponent : EntityComponent, ISingletonComponent
{
	/// <summary>
	/// The name of this object. Will be used in the event log.
	/// </summary>
	[Prefab, Net]
	private string ObjectName { get; set; }
	
	/// <summary>
	/// The word used to describe how this object is 'broken'.
	/// i.e. a crate might be "broken.", a goblin might be "killed" and undead might be "destroyed."
	/// </summary>
	[Prefab, Net]
	private string BreakVerb { get; set; }

	public string DisplayName => Language.GetPhrase( ObjectName );
}
