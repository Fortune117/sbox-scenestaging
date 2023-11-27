using Sandbox;

namespace DarkDescent.Actor.StatSystem;

[GameResource("Stat Modifier", "statmod", "contains some stat modifiers that can be applied to an actor.")]
public class StatModifierResource : GameResource
{
	public StatModifier StatModifier { get; set; }
}
