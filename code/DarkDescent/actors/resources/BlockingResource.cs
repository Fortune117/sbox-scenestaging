using DarkDescent.Actor.StatSystem;
using Sandbox;

namespace DarkDescent.Actor;

[GameResource("Block Stats", "block", "Contains data used for block calculations.")]
public class BlockingResource : GameResource
{
	public Resistances Resistances { get; set; }
}
