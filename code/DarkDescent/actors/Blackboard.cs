using System.Collections.Generic;

namespace DarkDescent.Actor;

public static class Blackboard
{
	public static readonly List<ActorComponent> Actors = new();
	public static readonly List<TargetComponent> Targets = new();

	public static void Register( ActorComponent actorComponent )
	{
		Actors.Add( actorComponent );
	}

	public static void UnRegister( ActorComponent actorComponent )
	{
		Actors.Remove( actorComponent );
	}
	
	public static void Register( TargetComponent targetComponent )
	{
		Targets.Add( targetComponent );
	}

	public static void UnRegister( TargetComponent targetComponent )
	{
		Targets.Remove( targetComponent );
	}
}
