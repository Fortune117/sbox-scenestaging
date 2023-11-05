using System.Collections.Generic;

namespace DarkDescent.Actor;

public static class Blackboard
{
	public static IReadOnlyCollection<ActorComponent> Actors => actors;
	public static IReadOnlyCollection<TargetComponent> Targets => targets;
	
	private static readonly List<ActorComponent> actors = new();
	private static readonly List<TargetComponent> targets = new();

	public static void Register( ActorComponent actorComponent )
	{
		actors.Add( actorComponent );
	}

	public static void UnRegister( ActorComponent actorComponent )
	{
		actors.Remove( actorComponent );
	}
	
	public static void Register( TargetComponent targetComponent )
	{
		targets.Add( targetComponent );
	}

	public static void UnRegister( TargetComponent targetComponent )
	{
		targets.Remove( targetComponent );
	}
}
